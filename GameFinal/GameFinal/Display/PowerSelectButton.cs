using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFinal
{
    class PowerSelectButton
    {
        Rectangle clientBounds;
        Vector2 screenPosition;
        int incrementCount = 0;
        Vector2 direction;
        Texture2D buttonTex;
        Texture2D weaponTex;
        int alpha = 100;
        int timer;
        float scale;

        public PowerSelectButton(Texture2D buttonTex, Rectangle clientBounds, Vector2 direction, int timer,
            Texture2D weaponTex)
        {
            this.clientBounds = clientBounds;
            this.direction = direction;
            this.buttonTex = buttonTex;
            this.screenPosition = new Vector2(clientBounds.Width / 2, clientBounds.Height / 2);
            this.timer = timer;
            this.weaponTex = weaponTex;

            this.scale = 2 * ((float)clientBounds.Width / 1600f);
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)screenPosition.X - (int)(20 * scale), (int)screenPosition.Y - (int)(20 * scale), (int)(40 * scale), (int)(40 * scale));
        }

        public bool Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(alpha < 255)
                alpha += 5;

            if (incrementCount < 10)
            {
                screenPosition += direction * 11;
                incrementCount++;
            }
            timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (timer <= 0)
                return true;

            spriteBatch.Draw(buttonTex,
                new Rectangle((int)(screenPosition.X - 20), (int)(screenPosition.Y - 20), (int)(40 * scale), (int)(40 * scale)),
                null,
                new Color(255, 255, 255, alpha),
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0.1f);

            spriteBatch.Draw(weaponTex,
                new Rectangle((int)(screenPosition.X - 20), (int)(screenPosition.Y - 20), (int)(40 * scale), (int)(40 * scale)),
                null,
                new Color(255, 255, 255, alpha),
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);

            return false;
        }
    }
}
