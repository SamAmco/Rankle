using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.DemoBaseXNA;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using GameFinal.Weapons;
using GameFinal.Misc;

namespace GameFinal
{
    class MainCharacter: GameCharacter
    {
        #region Variables

        KeyboardState keyState;
        Camera2D cam;

        List<int> fireCallList = new List<int>();
        List<Vector2> minePlaceList = new List<Vector2>();
        public Vector2 serverTargetPos = new Vector2();
        bool isKeyDown = false;


        MouseState prevMouseState;
        float aimFor = 0;
        Texture2D sights;
        bool drawSights = true;


        KeyboardState prevKeyState;
        GamePadState prevGPState;
        bool typing = false;
        #endregion

        public MainCharacter(Texture2D[] tankTexs, Vector2 Position,
            InGame parentGame, bool server, Camera2D cam, Texture2D[] gameInterfaceTexs, SpriteFont spriteFont,
            int[] weapon, int characterIndex, int tankSkin, Audio audio)
            : base(10, 100, 100, 100, 100, 1f, spriteFont, characterIndex, tankTexs, parentGame, Position, weapon, server, tankSkin, audio)
        {
            this.cam = cam;
            this.sights = tankTexs[16];

            serverTargetPos = Position;

            rifle = new Rifle(100, 20, 0, this, new Vector2(0, -((tankTex.Height * 0.1f) / 2) - 2),
                parentGame, bulletTex, tankTexs[15], false, characterIndex, tankFixtures, eRifle, audio);
            invisibility = new Invisibility(eInvisibility, true, audio, parentGame, this);
            mines = new Mines(tankTexs[14], eMines, this, parentGame, tankFixtures, characterIndex, false, 200, audio);
            missiles = new Missiles(1000, 0, 500, this, new Vector2(0, -((tankTex.Height * 0.1f) / 2) - 12), parentGame,
                tankTexs[15], false, characterIndex, eMissiles, audio);
            prevMouseState = Mouse.GetState();
            prevKeyState = Keyboard.GetState();
            prevGPState = GamePad.GetState(PlayerIndex.One);
            
        }

        public bool Update(GameTime gameTime, Camera2D cam, OtherCharacter[] otherCharacters)
        {
            keyState = Keyboard.GetState();


            Vector2 positionDifference = serverTargetPos - getPos();
            if (positionDifference.Length() > ConvertUnits.ToSimUnits(150))
            {
                tankBody.Position = serverTargetPos;
            }
            else if (isKeyDown && positionDifference.Length() > ConvertUnits.ToSimUnits(80))
            {
                tankBody.ApplyLinearImpulse(positionDifference.Length() * (positionDifference * 0.08f));
            }
            else if (!isKeyDown && positionDifference.Length() > ConvertUnits.ToSimUnits(5))
            {
                float ratio = positionDifference.Length();
                Vector2 positionDifference2 = positionDifference / 1.0f;
                positionDifference2.Normalize();
                tankBody.ApplyLinearImpulse(ratio * (positionDifference2 * 0.1f));
                //tankBody.Position += ConvertUnits.ToSimUnits(-positionDifference * 0.1f * ratio);
            }
            isKeyDown = false;

            if (Keyboard.GetState().IsKeyDown(Keys.W) ||
                Keyboard.GetState().IsKeyDown(Keys.A) ||
                Keyboard.GetState().IsKeyDown(Keys.S) ||
                Keyboard.GetState().IsKeyDown(Keys.D) ||
                prevMouseState.X != Mouse.GetState().X ||
                prevMouseState.Y != Mouse.GetState().Y)
                keyMouseInput();
            else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left != Vector2.Zero ||
                GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                gamePadInput();
            else
                drawExhaust = false;


            float diff = StaticHelpers.WrapAngle(aimFor - tankBody.Rotation);
            if (Math.Abs(diff) > MathHelper.ToRadians(5))
            {
                if (diff > 0)
                    tankBody.ApplyAngularImpulse(ConvertUnits.ToSimUnits(1));
                else
                    tankBody.ApplyAngularImpulse(ConvertUnits.ToSimUnits(-1));
            }
            else if (Math.Abs(diff) > MathHelper.ToRadians(0.5f))
            {
                if (diff > 0)
                    tankBody.AngularVelocity = 0.6f;
                else
                    tankBody.AngularVelocity = -0.6f;
            }
            else
                tankBody.AngularVelocity = 0;


            if ((Keyboard.GetState().IsKeyDown(Keys.F) && !prevKeyState.IsKeyDown(Keys.F) ||
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Y) && !prevGPState.IsButtonDown(Buttons.Y)) && !typing)
                drawSights = !drawSights;

            prevMouseState = Mouse.GetState();
            prevKeyState = Keyboard.GetState();
            prevGPState = GamePad.GetState(PlayerIndex.One);
            return base.Update(gameTime, otherCharacters, this);
        }

        public void setTyping(bool val)
        {
            this.typing = val;
        }
        public bool getTyping()
        {
            return typing;
        }

        private void gamePadInput()
        {
            Vector2 engineDirection = flipY(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left);
            float engineOutput = enginePower;
            if (!(Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation) - StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection)))
                < MathHelper.ToRadians(45) || Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation + 1) -
                StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection) + 1)) < MathHelper.ToRadians(45)))
            {
                engineOutput /= 2;
                drawExhaust = false;
            }
            else
                drawExhaust = true;

            tankBody.ApplyLinearImpulse(ConvertUnits.ToSimUnits(engineDirection * engineOutput));

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Length() > 0.5f)
                aimFor = StaticHelpers.WrapAngle(StaticHelpers.RotationFromVector(flipY(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right)));
        }

        private Vector2 flipY(Vector2 v)
        {
            return new Vector2(v.X, -v.Y);
        }

        private void keyMouseInput()
        {
            if (!typing)
            {
                Vector2 engineDirection = Vector2.Zero;
                float engineOutput = enginePower;

                if (keyState.IsKeyDown(Keys.A))
                {
                    engineDirection.X -= 1;
                    isKeyDown = true;
                }
                if (keyState.IsKeyDown(Keys.W))
                {
                    engineDirection.Y -= 1;
                    isKeyDown = true;
                }
                if (keyState.IsKeyDown(Keys.D))
                {
                    engineDirection.X += 1;
                    isKeyDown = true;
                }
                if (keyState.IsKeyDown(Keys.S))
                {
                    engineDirection.Y += 1;
                    isKeyDown = true;
                }
                bool twoButtons = false;
                if (Math.Abs(engineDirection.X) + Math.Abs(engineDirection.Y) > 1)
                {
                    engineDirection.X *= 0.71f;
                    engineDirection.Y *= 0.71f;
                    twoButtons = true;
                }

                if (twoButtons)
                {
                    if (!(Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation) - StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection)))
                    < MathHelper.ToRadians(45) || Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation + 1) -
                    StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection) + 1)) < MathHelper.ToRadians(45)))
                    {
                        engineOutput /= 2;
                        drawExhaust = false;
                    }
                    else
                        drawExhaust = true;
                }
                else if (!(Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation) - StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection)))
                    < MathHelper.ToRadians(20) || Math.Abs(StaticHelpers.WrapToFullCircle(tankBody.Rotation + 1) -
                    StaticHelpers.WrapToFullCircle(StaticHelpers.RotationFromVector(engineDirection) + 1)) < MathHelper.ToRadians(20)))
                {
                    engineOutput /= 2;
                    drawExhaust = false;
                }
                else
                    drawExhaust = true;

                tankBody.ApplyLinearImpulse(ConvertUnits.ToSimUnits(engineDirection * engineOutput));
            }

            aimFor = StaticHelpers.WrapAngle((StaticHelpers.TurnToFace(ConvertUnits.ToDisplayUnits(getPos()),
                    new Vector2((Mouse.GetState().X / cam.Zoom) + cam.TLCorner.X, (Mouse.GetState().Y / cam.Zoom) + cam.TLCorner.Y)) +
                    MathHelper.PiOver2));// - tankBody.Rotation);
        }

        private void addFireCall(int weapon)
        {
            fireCallList.Add(weapon + (10 * (int)(Math.Round(StaticHelpers.WrapToFullCircle(tankBody.Rotation), 5) * 100000)));
        }

        public void FireWeapon(GameTime gameTime)
        {
            switch (weapon[currentWeapon])
            {
                case 0:
                    break;
                case 1:
                    {
                        if (parentGame.world.TestPointAll(getPos() + 
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(0,
                            -((tankTex.Height * 0.1f) / 2) - 12)), tankBody.Rotation)).Count == 0)
                        {
                            if (rifle.FireWeapon(gameTime, energy))
                            {
                                base.TakeEnergy(currentWeapon);
                                addFireCall(currentWeapon);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        if (invisibility.FireWeapon(false, energy))
                        {
                            base.TakeEnergy(currentWeapon);
                            addFireCall(currentWeapon);
                        }
                        break;
                    }
                case 3:
                    {
                        if (parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(0, 50)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(0, 70)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(0, 90)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(10, 50)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(10, 70)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(10, 90)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(-10, 50)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(-10, 70)), tankBody.Rotation)).Count == 0 &&
                            parentGame.world.TestPointAll(getPos() +
                            StaticHelpers.RotateVector(ConvertUnits.ToSimUnits(new Vector2(-10, 90)), tankBody.Rotation)).Count == 0)
                        {
                            if (mines.FireWeapon(gameTime, energy))
                            {
                                base.TakeEnergy(currentWeapon);
                                //addFireCall(mines.getPlaceList());
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        if (missiles.FireWeapon(gameTime, energy))
                        {
                            base.TakeEnergy(currentWeapon);
                            addFireCall(currentWeapon);
                        }
                        break;
                    }
            }
        }

        protected override void bashPlayer(Fixture fixtureA, Fixture fixtureB)
        {
            //Console.WriteLine(fixtureA.Body.LinearVelocity.Length());
            float vol = (fixtureA.Body.LinearVelocity + fixtureB.Body.LinearVelocity).Length() / 13.6f;
            if (vol > 1)
                vol = 1;
            
            audio.playSound("bashOther",
                vol * StaticHelpers.getVolume(tankBody.Position, parentGame.getMainCharacterPos()),
                vol * 0.3f,
                StaticHelpers.getPan(tankBody.Position, parentGame.getMainCharacterPos()));
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(bulletTex, ConvertUnits.ToDisplayUnits(getPos()), Color.White);
            //spriteBatch.Draw(bulletTex, ConvertUnits.ToDisplayUnits(serverTargetPos), Color.White);


            if (drawSights)
                spriteBatch.Draw(sights,
                    ConvertUnits.ToDisplayUnits(this.getPos()),
                    null,
                    Color.White * 0.8f,
                    this.getRotation(),
                    new Vector2(sights.Width / 2, sights.Height),
                    0.5f,
                    SpriteEffects.None,
                    0.111f);

            base.Draw(spriteBatch);
        }

        public List<int> GetFireCallList()
        {
            List<int> temp = new List<int>();
            foreach (int i in fireCallList)
            {
                temp.Add(i);
            }
            fireCallList.Clear();
            return temp;
        }
        public List<Vector2> GetMinePlaceList()
        {
            return mines.getPlaceList();
        }
    }
}
