using System;using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameFinal.Misc;

namespace GameFinal.Menu_s
{
    class TextBox : FormElement
    {
        #region variables
        String text;
        Color c;
        Texture2D black;
        int index = 0;
        int maxSize;
        bool onlyNumbers;
        bool canBeNull;
        KeyboardState prevKeyState;

        bool clear = false;
        int clearTimer = 0;

        int cursorTimerMax = 500;
        int cursorTimer = 500;
        bool showCursor = true;

        TextBox nextBox;
        Rectangle rec;
        Audio audio;
        #endregion

        public TextBox(SpriteFont spriteFont, Vector2 position, Vector2 targetPos,
            Texture2D black, String text, float scale, Color c, int maxSize, bool onlyNumbers, bool canBeNull, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.text = text;
            this.c = c;
            this.black = black;
            this.maxSize = maxSize;
            this.scale = scale;
            this.onlyNumbers = onlyNumbers;
            this.canBeNull = canBeNull;
            this.nextBox = null;
            this.audio = audio;
            index = text.ToCharArray().Length;
            prevKeyState = Keyboard.GetState();
        }

        public TextBox(SpriteFont spriteFont, Vector2 position, Vector2 targetPos,
            Texture2D black, String text, float scale, Color c, int maxSize, bool onlyNumbers,
            bool canBeNull, TextBox nextBox, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.text = text;
            this.c = c;
            this.black = black;
            this.maxSize = maxSize;
            this.scale = scale;
            this.onlyNumbers = onlyNumbers;
            this.canBeNull = canBeNull;
            this.nextBox = nextBox;
            this.active = false;
            this.audio = audio;
            index = text.ToCharArray().Length;
            prevKeyState = Keyboard.GetState();
        }

        public override bool Update(GameTime gameTime)
        {
            rec = new Rectangle((int)position.X - 5,
                    (int)(position.Y - (((spriteFont.MeasureString("A").Y * StaticHelpers.fontScale) * scale) / 2)) - 5,
                    (int)((spriteFont.MeasureString(text).X * StaticHelpers.fontScale) * scale) + 10,
                    (int)((spriteFont.MeasureString("A").Y * StaticHelpers.fontScale) * scale) + 10);
            if (!active)
            {
                if (rec.Intersects(mouseRect))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        audio.playSound("click", 1, -0.1f, 0);
                        active = true;
                        showCursor = true;
                        cursorTimer = cursorTimerMax;
                    }
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Tab) && !prevKeyState.IsKeyDown(Keys.Tab))
                {
                    audio.playSound("click", 1, -0.1f, 0);
                    this.active = false;
                    if (nextBox != null)
                        nextBox.setActive(true);
                }

                cursorTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (cursorTimer < 0)
                {
                    showCursor = !showCursor;
                    cursorTimer = cursorTimerMax;
                }
                if (!rec.Intersects(mouseRect))
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        active = false;
                }

                Keys[] k1 = Keyboard.GetState().GetPressedKeys();
                Keys[] k2 = prevKeyState.GetPressedKeys();

                if (compareKeys(k1, k2))
                {
                    clearTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (clearTimer > 400)
                        clear = true;
                }
                else
                {
                    clearTimer = 0;
                    clear = false;
                }



                if (Keyboard.GetState().IsKeyDown(Keys.Left) && (!prevKeyState.IsKeyDown(Keys.Left) || clear) && index > 0)
                    index--;
                else if (Keyboard.GetState().IsKeyDown(Keys.Right) && (!prevKeyState.IsKeyDown(Keys.Right) || clear) && index < text.Length)
                    index++;
                else if (Keyboard.GetState().IsKeyDown(Keys.Back) && (!prevKeyState.IsKeyDown(Keys.Back) || clear) && index > 0)
                {
                    String first = text.Substring(0, index - 1);
                    String second = text.Substring(index);
                    text = first + second;
                    index--;
                }
                


                foreach (Keys ke1 in k1)
                {
                    bool wasDown = false;
                    if (!clear)
                    {
                        foreach (Keys ke2 in k2)
                        {
                            if (ke1.Equals(ke2))
                                wasDown = true;
                        }
                    }
                    if (!wasDown)
                    {
                        String s = StaticHelpers.keyToLetter(ke1, !onlyNumbers, (Keyboard.GetState().IsKeyDown(Keys.LeftShift) 
                            || Keyboard.GetState().IsKeyDown(Keys.RightShift)));
                        if (text.Length < maxSize)
                        {
                            if (prevKeyState.GetPressedKeys().Length == 0)//if ((Keyboard.GetState().IsKeyDown(Keys.Back) && (!prevKeyState.IsKeyDown(Keys.Back)) || !Keyboard.GetState().IsKeyDown(Keys.Back)))
                                audio.playSound("click", 1, 0.2f, 0);
                            text = text.Insert(index, s);
                            index += s.Length;
                        }
                    }
                }
            }
            prevKeyState = Keyboard.GetState();
            return base.Update(gameTime);
        }

        private bool compareKeys(Keys[] k1, Keys[] k2)
        {
            bool same = true;
            foreach (Keys ke1 in k1)
            {
                bool found = false;
                foreach (Keys ke2 in k2)
                {
                    if (ke1.Equals(ke2))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    same = false;
                    break;
                }
            }
            return same;
        }

        public void setActive(bool active)
        {
            this.active = active;
            prevKeyState = Keyboard.GetState();
        }

        public String getText()
        {
            if (!canBeNull && text == "")
                return "no name";
            else
                return text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont,
                text,
                new Vector2(position.X, position.Y - (((spriteFont.MeasureString(text).Y * StaticHelpers.fontScale) * scale) / 2)),
                c,
                0,
                Vector2.Zero,
                scale * StaticHelpers.fontScale,
                SpriteEffects.None,
                0.2f);
            spriteBatch.Draw(black,
                rec,
                null,
                Color.White * 0.4f,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0.21f);
            if (active && showCursor)
            {
                Rectangle r = new Rectangle((int)(position.X + 3 + ((spriteFont.MeasureString(text.Substring(0, index)).X * StaticHelpers.fontScale) * scale)),
                    (int)(position.Y - (((spriteFont.MeasureString("A").Y * StaticHelpers.fontScale) * scale) / 2)),
                    2,
                    (int)(((spriteFont.MeasureString("A").Y * StaticHelpers.fontScale) * scale)));
                spriteBatch.Draw(black,
                    r,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.199f);
            }
            base.Draw(spriteBatch);
        }
    }
}
