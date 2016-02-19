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
    class OtherCharacter : GameCharacter
    {
        #region Variables
        public Vector2 targetPos = new Vector2();
        public Vector2 targetVelocity = new Vector2();
        public float targetRotation;
        public string playerName;
        #endregion

        public OtherCharacter(Texture2D[] tankTexs, Vector2 Position,
            InGame parentGame, bool server, Texture2D[] gameInterfaceTexs, SpriteFont spriteFont,
            int[] weapon, int characterIndex, int tankSkin, string playerName, Audio audio)
            : base(10, 100, 100, 100, 100, 1f, spriteFont, characterIndex, tankTexs, parentGame, Position, weapon, server, tankSkin, audio)
        {

            rifle = new Rifle(100, 20, 0, this, new Vector2(0, -((tankTex.Height * 0.1f) / 2) - 2),
                parentGame, bulletTex, tankTexs[15], true, characterIndex, tankFixtures, eRifle, audio);
            invisibility = new Invisibility(eInvisibility, false, audio, parentGame, this);
            mines = new Mines(tankTexs[14], eMines, this, parentGame, tankFixtures, characterIndex, true, 200, audio);
            missiles = new Missiles(1000, 2, 500, this, new Vector2(0, -((tankTex.Height * 0.1f) / 2) - 12), parentGame,
                tankTexs[15], true, characterIndex, eMissiles, audio);

            targetPos = Position;
            this.playerName = playerName;
        }

        public bool Update(GameTime gameTime, Camera2D cam, OtherCharacter[] otherCharacters, MainCharacter m)
        {

            Vector2 positionDifference = targetPos - getPos();
            if (positionDifference.Length() > ConvertUnits.ToSimUnits(100))
            {
                if (!server)
                {
                    tankBody.Position = targetPos;
                }
            }
            else if (positionDifference.Length() > 0)
            {
                float ratio = positionDifference.Length();
                tankBody.Position += positionDifference * 0.1f * ratio;
            }
            tankBody.LinearVelocity = targetVelocity;
            
            if (tankBody.LinearVelocity.Length() > 3.5f)
                drawExhaust = true;
            else
                drawExhaust = false;

            //float rotationDifference = targetRotation - tankBody.Rotation;
            //rotationDifference = StaticHelpers.WrapAngle(rotationDifference);
            //if (rotationDifference != 0f)
            //{
            //    tankBody.Rotation += rotationDifference * 0.2f;
            //    tankBody.Rotation = StaticHelpers.WrapAngle(tankBody.Rotation);
            //}

            double diff = StaticHelpers.WrapAngle(targetRotation - tankBody.Rotation);
            if (Math.Abs(diff) > MathHelper.ToRadians(5))
            {
                if (diff > 0)
                    tankBody.ApplyAngularImpulse(ConvertUnits.ToSimUnits(1));
                else
                    tankBody.ApplyAngularImpulse(ConvertUnits.ToSimUnits(-1));
            }
            else
            {
                tankBody.AngularVelocity = 0;
            }
            return base.Update(gameTime, otherCharacters, m);
        }

        public void FireWeapon(GameTime gameTime, int call)
        {
            int weaponIndex = call % 10;
            float rotation = StaticHelpers.WrapAngle((float)(call / 10) / 100000);

            bool takeEnergy = false;
            switch (weapon[weaponIndex])
            {
                case 0:
                    break;
                case 1:
                    if (rifle.FireWeapon(gameTime, energy, rotation))
                        takeEnergy = true;
                    break;
                case 2:
                    if (invisibility.FireWeapon(true, energy))
                        takeEnergy = true;
                    break;
                //case 3:
                //    if (mines.FireWeapon(gameTime, energy, rotation))
                //        takeEnergy = true;
                //    break;
                case 4:
                    if (missiles.FireWeapon(gameTime, energy, rotation))
                        takeEnergy = true;
                    break;
            }
            if (takeEnergy)
                base.TakeEnergy(weaponIndex);
        }

        public void PlaceMine(Vector2 v)
        {
            mines.PlaceMine(v);
            base.TakeEnergy(2);
        }

        protected override void bashPlayer(Fixture fixtureA, Fixture fixtureB)
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            //spriteBatch.Draw(bulletTex, ConvertUnits.ToDisplayUnits(targetPos), Color.White);
            base.Draw(spriteBatch);
        }
    }
}