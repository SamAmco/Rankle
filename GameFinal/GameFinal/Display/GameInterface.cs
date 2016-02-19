using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using System.Collections.Generic;
using System;
using FarseerPhysics.DemoBaseXNA;
using GameFinal.Display;
using GameFinal.Misc;

namespace GameFinal
{
    class GameInterface
    {
        #region variables
        Texture2D emptyButtonTex;
        Texture2D EmptyBar;
        Texture2D EnergyCentre;
        Texture2D EnergyEnd;
        Texture2D HealthCentre;
        Texture2D HealthEnd;
        Texture2D[] buffTexs;
        Rectangle clientBounds;
        //Camera2D cam;
        Body characterBody;
        MainCharacter mainCharacter;
        PowerSelectButton[] powerSelectButtons;
        MouseState previousMouseState;
        GamePadState prevGPState;
        KeyboardState previousKeyState;
        Texture2D[] weaponTexs;
        SpriteFont spriteFont;
        Audio audio;

        float barScale = 0.5f;
        float buffScale;

        string flashMessage = "";
        int flashTimer = 0;
        int flashTimerMax = 3000;

        List<int> buffs;
        bool drawIntercom = true;
        #endregion

        public GameInterface(Texture2D[] gameInterfaceTexs, Rectangle clientBounds,// Camera2D cam,
            Body characterBody, MainCharacter mainCharacter, Texture2D[] weaponTexs, SpriteFont spriteFont, Audio audio)
        {
            this.EmptyBar = gameInterfaceTexs[0];
            this.EnergyCentre = gameInterfaceTexs[1];
            this.EnergyEnd = gameInterfaceTexs[2];
            this.HealthCentre = gameInterfaceTexs[3];
            this.HealthEnd = gameInterfaceTexs[4];
            this.emptyButtonTex = gameInterfaceTexs[5];
            this.audio = audio;


            buffTexs = new Texture2D[8];
            for (int i = 6; i < gameInterfaceTexs.Length; i++)
            {
                buffTexs[i - 6] = gameInterfaceTexs[i];
            }

                this.clientBounds = clientBounds;
            //this.cam = cam;
            this.characterBody = characterBody;
            this.mainCharacter = mainCharacter;
            this.weaponTexs = weaponTexs;
            
            previousMouseState = Mouse.GetState();
            prevGPState = GamePad.GetState(PlayerIndex.One);
            powerSelectButtons = new PowerSelectButton[5];
            this.spriteFont = spriteFont;

            buffScale = ((float)clientBounds.Width / 40f) / (float)emptyButtonTex.Width;
            barScale = 0.5f * ((float)clientBounds.Width / 1600f);

            buffs = new List<int>();
            //buffs.Add(1);
            //buffs.Add(2);
            //buffs.Add(3);
            //buffs.Add(4);
            //buffs.Add(5);
            //buffs.Add(6);
            //buffs.Add(7);
            //buffs.Add(8);
        }

        public void Update(GameTime gameTime)
        {
            CheckMouse(gameTime);
            CheckKeyboard(gameTime);
            previousMouseState = Mouse.GetState();
            prevGPState = GamePad.GetState(PlayerIndex.One);

            if (flashTimer <= 0)
                flashMessage = "";
            else flashTimer -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public void FlashMessage(string flashMessage)
        {
            this.flashMessage = flashMessage;
            this.flashTimer = flashTimerMax;
        }

        public void FlashMessage(string flashMessage, int i)
        {
            this.flashMessage = flashMessage;
            this.flashTimer = flashTimerMax;

            switch (i)
            {
                case 1:
                    buffs.Add(1);
                    break;
                case 2:
                    buffs.Add(2);
                    break;
                case 3:
                    buffs.Add(3);
                    break;
                case 4:
                    buffs.Add(4);
                    break;
                case 5:
                    buffs.Add(5);
                    break;
                case 6:
                    buffs.Add(6);
                    break;
                case 7:
                    buffs.Add(7);
                    break;
                case 8:
                    buffs.Add(8);
                    break;

            }
        }

        private void CheckKeyboard(GameTime gameTime)
        {
            if (((Keyboard.GetState().IsKeyDown(Keys.I) && previousKeyState.IsKeyUp(Keys.I))
                || (GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed && prevGPState.Buttons.B == ButtonState.Released))
                && !mainCharacter.getTyping())
                drawIntercom = !drawIntercom;

            if ((Keyboard.GetState().IsKeyDown(Keys.D1) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadUp)) && !mainCharacter.getTyping())
            {
                if (!(previousKeyState.IsKeyDown(Keys.D1) || prevGPState.IsButtonDown(Buttons.DPadUp)))
                    audio.playSound("selectWeapon");
                mainCharacter.currentWeapon = 0;
                powerSelectButtons[0] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(0), 500, weaponTexs[mainCharacter.weapon[mainCharacter.currentWeapon]]));
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.D2) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadRight)) && !mainCharacter.getTyping())
            {
                if (!(previousKeyState.IsKeyDown(Keys.D2) || prevGPState.IsButtonDown(Buttons.DPadRight)))
                    audio.playSound("selectWeapon");
                mainCharacter.currentWeapon = 1;
                powerSelectButtons[1] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(90)), 500, weaponTexs[mainCharacter.weapon[mainCharacter.currentWeapon]]));
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.D3) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadDown)) && !mainCharacter.getTyping())
            {
                if (!(previousKeyState.IsKeyDown(Keys.D3) || prevGPState.IsButtonDown(Buttons.DPadDown)))
                    audio.playSound("selectWeapon");
                mainCharacter.currentWeapon = 2;
                powerSelectButtons[2] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(180)), 500, weaponTexs[mainCharacter.weapon[mainCharacter.currentWeapon]]));
            }
            else if ((Keyboard.GetState().IsKeyDown(Keys.D4) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.DPadLeft)) && !mainCharacter.getTyping())
            {
                if (!(previousKeyState.IsKeyDown(Keys.D4) || prevGPState.IsButtonDown(Buttons.DPadLeft)))
                    audio.playSound("selectWeapon");
                mainCharacter.currentWeapon = 3;
                powerSelectButtons[3] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(270)), 500, weaponTexs[mainCharacter.weapon[mainCharacter.currentWeapon]]));
            }
            previousKeyState = Keyboard.GetState();
        }

        private void CheckMouse(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightTrigger))
            {
                mainCharacter.FireWeapon(gameTime);
            }

            if ((Mouse.GetState().RightButton != ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
                || (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder) && !prevGPState.IsButtonDown(Buttons.RightShoulder)))
            {
                int buttonClicked = ButtonClicked();
                if (powerSelectButtons[0] == null)
                {
                    powerSelectButtons = new PowerSelectButton[5];

                    powerSelectButtons[0] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(0), 5000, weaponTexs[mainCharacter.weapon[0]]));
                    powerSelectButtons[1] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(90)), 5000, weaponTexs[mainCharacter.weapon[1]]));
                    powerSelectButtons[2] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(180)), 5000, weaponTexs[mainCharacter.weapon[2]]));
                    powerSelectButtons[3] = (new PowerSelectButton(emptyButtonTex, clientBounds,
                        StaticHelpers.VectorFromRotation(MathHelper.ToRadians(270)), 5000, weaponTexs[mainCharacter.weapon[3]]));
                }
                else if (buttonClicked < 4)
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        if (buttonClicked == i)
                        {
                            mainCharacter.currentWeapon = i;
                            powerSelectButtons = new PowerSelectButton[4];
                        }
                    }
                }
                else if (powerSelectButtons[0] != null)
                {
                    powerSelectButtons = new PowerSelectButton[4];
                }
            }
        }

        private int ButtonClicked()
        {
            for (int i = 0; i < 4; i++)
            {
                if (powerSelectButtons[i] != null)
                {
                    if (powerSelectButtons[i].GetRectangle().Intersects(new Rectangle((int)Mouse.GetState().X, (int)Mouse.GetState().Y, 1, 1)))
                    {
                        return i;
                    }
                }
            }
            return 5;
        }

        private bool intersectsSight(Rectangle r, float rotation, float scale)
        {
            Vector2 start = ConvertUnits.ToDisplayUnits(mainCharacter.getPos());
            Vector2 end = start + StaticHelpers.RotateVector(new Vector2(0, -(1000 * scale)), rotation);
            Vector2 diff = end - start;
            for (int i = 0; i < 10; i++)
            {
                Vector2 p = start + ((diff / 10) * i);
                Rectangle r2 = new Rectangle((int)p.X - 3, (int)p.Y - 3, 6, 6);
                if(r.Intersects(r2))
                    return true;
            }
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float health, float defence,
            float maxHealth, float energy, float maxEnergy, OtherCharacter[] otherCharacters, KeepScore keepScore, List<string> intercomList)
        {
            if (drawIntercom)
            {
                int il = intercomList.Count;
                foreach (string s in intercomList)
                {
                    float scale = 0.24f;
                    float height = spriteFont.MeasureString(s).Y * scale;
                    spriteBatch.DrawString(spriteFont,
                        s,
                        new Vector2(30, clientBounds.Height - 80) + (new Vector2(0, -height) * il),
                        Color.Red * 0.7f,
                        0,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        0.06f);
                    il--;
                }
            }

            DrawBar(new Vector2(clientBounds.Width - (EmptyBar.Width * barScale) - (100 * barScale * 2), 90f * barScale * 2),// + cam.TLCorner,
                spriteBatch,
                health,
                maxHealth,
                true,
                barScale);
            DrawBar(new Vector2(clientBounds.Width - (EmptyBar.Width * barScale) - (70 * barScale * 2), 120f * barScale * 2),// + cam.TLCorner,
                spriteBatch,
                energy,
                maxEnergy,
                false,
                barScale);
            
            for (int i = 0; i < buffs.Count; i++)
            {
                Vector2 p = new Vector2(clientBounds.Width - (emptyButtonTex.Width * buffScale * (i + 1)) - (90 * buffScale * 2), 5 * buffScale * 2);
                spriteBatch.Draw(emptyButtonTex,
                    p,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    buffScale,
                    SpriteEffects.None,
                    0.0065f);
                spriteBatch.Draw(buffTexs[buffs[i] - 1], 
                    p,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    buffScale,
                    SpriteEffects.None,
                    0.006f);
            }

            string def2 = defence.ToString();
            if (def2.Length > 4)
                def2 = def2.Substring(0, 4);

            spriteBatch.DrawString(spriteFont,
                            def2,
                            new Vector2(clientBounds.Width - (80 * barScale * 2), 85f * barScale * 2),
                            Color.Red,
                            0,
                            Vector2.Zero,
                            barScale * 2 * StaticHelpers.fontScale,
                            SpriteEffects.None,
                            0.006f);

            if (flashMessage != "")
            {
                float scale = 5 * ((float)flashTimer / (float)flashTimerMax);
                Vector2 offset = new Vector2((spriteFont.MeasureString(flashMessage).X * StaticHelpers.fontScale) / 2,
                    (spriteFont.MeasureString(flashMessage).Y * StaticHelpers.fontScale) / 2) * scale;
                spriteBatch.DrawString(spriteFont,
                    flashMessage,
                    (new Vector2(clientBounds.Width / 2, clientBounds.Height / 2)),//cam.TLCorner + (new Vector2(clientBounds.Width / 2, clientBounds.Height / 2)),
                    Color.Red * ((float)flashTimer / (float)flashTimerMax),
                    0,
                    offset / (scale * StaticHelpers.fontScale),
                    scale * StaticHelpers.fontScale,
                    SpriteEffects.None,
                    0.06f);
            }

            foreach (OtherCharacter o in otherCharacters)
            {
                if (o != null)
                {
                    if (intersectsSight(o.getDisplayAABB(), mainCharacter.getRotation(), 0.5f))
                    {
                        string def = (keepScore.getKD(o.characterIndex) / 10).ToString();
                        if (def.Length > 4)
                            def = def.Substring(0, 4);

                        Vector2 nameSize = spriteFont.MeasureString(o.playerName + "    +" + def) * StaticHelpers.fontScale;
                        spriteBatch.DrawString(spriteFont,
                            o.playerName + "    +" + def,
                            new Vector2((clientBounds.Width / 2), clientBounds.Height - ((EmptyBar.Height * 0.4f) * 2)) + new Vector2(-(nameSize.X / 2), -(nameSize.Y * 1.2f)),
                            Color.Black,
                            0,
                            Vector2.Zero,
                            StaticHelpers.fontScale,
                            SpriteEffects.None,
                            0.06f);
                        DrawBar(new Vector2((clientBounds.Width / 2) - ((EmptyBar.Width * 0.4f) / 2), clientBounds.Height - ((EmptyBar.Height * 0.4f) * 2)),// + cam.TLCorner,
                            spriteBatch,
                            o.health,
                            o.maxHealth,
                            true,
                            0.4f);
                    }
                }
            }

            #region DrawPowerSelectButtons
            for (int p = 0; p < 4; p++)
            {
                if (powerSelectButtons[p] != null)
                {
                    if (powerSelectButtons[p].Draw(spriteBatch, gameTime))
                        powerSelectButtons[p] = null;
                }
            }
            #endregion
        }

        private void DrawBar(Vector2 position, SpriteBatch spriteBatch, float amount, float maxAmount, bool isHealth, float scale)
        {
            spriteBatch.Draw(EmptyBar,
                position,
                null,
                Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.055f);
            if (isHealth)
            {
                if (amount > 0)
                {
                    float healthWidth = (amount / maxAmount) * ((EmptyBar.Width * scale) - (2 * (HealthEnd.Width * scale)));
                    float maxHealthWidth = ((EmptyBar.Width * scale) - (2 * (HealthEnd.Width * scale)));
                    spriteBatch.Draw(HealthCentre,
                        new Rectangle((int)(position.X + (HealthEnd.Width * scale) + (maxHealthWidth - healthWidth) - 1), (int)position.Y, (int)healthWidth + 2, (int)(HealthCentre.Height * scale)),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.06f);
                    spriteBatch.Draw(HealthEnd,
                        new Rectangle((int)(position.X + (EmptyBar.Width * scale) - (HealthEnd.Width * scale)), (int)(position.Y), (int)(HealthEnd.Width * scale) + 1, (int)(HealthEnd.Height * scale)),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.06f);
                    spriteBatch.Draw(HealthEnd,
                        new Rectangle((int)(position.X + ((HealthEnd.Width * scale) / 2) + (maxHealthWidth - healthWidth)), (int)(position.Y + ((HealthEnd.Height * scale) / 2)), (int)(HealthEnd.Width * scale) + 1, (int)(HealthEnd.Height * scale)),
                        null,
                        Color.White,
                        MathHelper.ToRadians(180),
                        new Vector2((HealthEnd.Width) / 2, (HealthEnd.Height) / 2),
                        SpriteEffects.None,
                        0.06f);
                }
            }
            else
            {
                if (amount > 0)
                {
                    float healthWidth = (amount / maxAmount) * ((EmptyBar.Width * scale) - (2 * (HealthEnd.Width * scale)));
                    float maxHealthWidth = ((EmptyBar.Width * scale) - (2 * (HealthEnd.Width * scale)));
                    spriteBatch.Draw(EnergyCentre,
                        new Rectangle((int)(position.X + (HealthEnd.Width * scale) + (maxHealthWidth - healthWidth) - 1), (int)position.Y, (int)healthWidth + 2, (int)(HealthCentre.Height * scale)),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.06f);
                    spriteBatch.Draw(EnergyEnd,
                        new Rectangle((int)(position.X + (EmptyBar.Width * scale) - (HealthEnd.Width * scale)), (int)(position.Y), (int)(HealthEnd.Width * scale) + 1, (int)(HealthEnd.Height * scale)),
                        null,
                        Color.White,
                        0,
                        Vector2.Zero,
                        SpriteEffects.None,
                        0.06f);
                    spriteBatch.Draw(EnergyEnd,
                        new Rectangle((int)(position.X + ((HealthEnd.Width * scale) / 2) + (maxHealthWidth - healthWidth)), (int)(position.Y + ((HealthEnd.Height * scale) / 2)), (int)(HealthEnd.Width * scale) + 1, (int)(HealthEnd.Height * scale)),
                        null,
                        Color.White,
                        MathHelper.ToRadians(180),
                        new Vector2((HealthEnd.Width) / 2, (HealthEnd.Height) / 2),
                        SpriteEffects.None,
                        0.06f);
                }
            }
        }
    }
}
