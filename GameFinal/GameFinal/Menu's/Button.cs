using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameFinal.Misc;

namespace GameFinal.Menu_s
{
    class Button : FormElement
    {
        #region variables
        Texture2D leftEnd;
        Texture2D middle;
        Texture2D rightEnd;
        String text;
        Vector2 stringSize;
        Audio audio;

        public delegate void click_Listener(object o, EventArgs e);
        public event click_Listener on_Click;
        #endregion

        public Button(Texture2D[] buttonTexs, SpriteFont spriteFont, Vector2 position, String text, Vector2 targetPos, float elementScale, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            leftEnd = buttonTexs[0];
            middle = buttonTexs[1];
            rightEnd = buttonTexs[2];

            //elementScale *= StaticHelpers.fontScale;
            stringSize = spriteFont.MeasureString(text) * StaticHelpers.fontScale;
            scale = (stringSize.Y / leftEnd.Height) * elementScale;
            stringSize *= scale;
            

            this.spriteFont = spriteFont;
            this.position = position;
            this.text = text;
            this.targetPos = targetPos;
            this.audio = audio;
        }

        public override bool Update(GameTime gameTime)
        {
            if (true)
            {
                Rectangle area = new Rectangle((int)(position.X - (stringSize.X / 2) - (leftEnd.Width * scale)),
                    (int)(position.Y - (leftEnd.Height * scale * 0.5f)),
                    (int)(stringSize.X + (leftEnd.Width * scale * 2)),
                    (int)(leftEnd.Height * scale));

                if (mouseRect.Intersects(area))
                {
                    active = true;
                    if (prevMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
                    {
                        audio.playSound("click");
                        on_Click(this, new EventArgs());
                    }
                }
                else
                    active = false;
            }
            return base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color c = Color.Red;
            if (active)
                c = Color.White;

            float endScale = stringSize.Y / leftEnd.Height;
            spriteBatch.Draw(leftEnd,
                new Vector2(position.X - (stringSize.X / 2) - (leftEnd.Width * endScale), position.Y - (leftEnd.Height * endScale * 0.5f)),
                null,
                Color.White,
                0,
                Vector2.Zero,
                endScale,
                SpriteEffects.None,
                0.2f);
            spriteBatch.Draw(rightEnd,
                new Vector2(position.X + (stringSize.X / 2), position.Y - (rightEnd.Height * endScale * 0.5f)),
                null,
                Color.White,
                0,
                Vector2.Zero,
                endScale,
                SpriteEffects.None,
                0.2f);
            spriteBatch.Draw(middle,
                new Rectangle((int)(position.X - (stringSize.X / 2)), (int)(position.Y - (middle.Height * endScale * 0.5f)), (int)stringSize.X + 1, (int)stringSize.Y + 1),
                null,
                Color.White,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0.2f);
            spriteBatch.DrawString(spriteFont,
                text,
                new Vector2(position.X - (stringSize.X / 2), position.Y - (stringSize.Y / 2)),
                c,
                0,
                Vector2.Zero,
                scale * StaticHelpers.fontScale,
                SpriteEffects.None,
                0.21f);

            base.Draw(spriteBatch);
        }
    }
}
