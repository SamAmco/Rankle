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
    class CheckBox : FormElement
    {
        Texture2D[] texs;
        bool check;
        Audio audio;

        public CheckBox(SpriteFont spriteFont, Vector2 position, Vector2 targetPos, Texture2D[] texs, float scale, bool check, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.texs = texs;
            this.scale = scale;
            this.check = check;
            this.audio = audio;
        }

        public override bool Update(GameTime gameTime)
        {
            Rectangle r = new Rectangle((int)(position.X - ((texs[0].Width * scale) / 2)),
                (int)(position.Y - ((texs[0].Height * scale) / 2)),
                (int)(texs[0].Width * scale),
                (int)(texs[0].Height * scale));

            if (mouseRect.Intersects(r))
            {
                if (prevMouseState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    audio.playSound("click", 1, 0.1f, 0);
                    check = !check;
                }
            }

            return base.Update(gameTime);
        }

        public bool getChecked()
        {
            return check;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 v = new Vector2(position.X - ((texs[0].Width * scale) / 2), position.Y - ((texs[0].Height * scale) / 2));
            if (check)
            {
                spriteBatch.Draw(texs[1],
                    v,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.19f);
            }
            else
            {
                spriteBatch.Draw(texs[0],
                    v,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.19f);
            }
            base.Draw(spriteBatch);
        }
    }
}
