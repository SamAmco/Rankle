using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameFinal
{
    class DamageIndicator
    {
        Vector2 position;
        Vector2 direction = new Vector2(0, -1);
        int alpha = 300;
        SpriteFont spriteFont;
        string damage;
        int r = 0;
        int g = 0;
        int b = 0;

        public DamageIndicator(Vector2 position, SpriteFont spriteFont, float damage, int characterIndex)
        {
            this.spriteFont = spriteFont;
            this.position = position;
            if (damage < 0)
                this.damage = "+" + (-Math.Round(damage, 1)).ToString();
            else
                this.damage = Math.Round(damage, 1).ToString();

            switch (characterIndex)
            {
                case 0 :
                    r = 237;
                    g = 28;
                    b = 36;
                    break;
                case  1 :
                    r = 11;
                    g = 102;
                    b = 255;
                    break;
                case 2 :
                    r = 11;
                    g = 255;
                    b = 29;
                    break;
                case 3 :
                    r = 255;
                    g = 157;
                    b = 11;
                    break;
                case 4 :
                    r = 132;
                    g = 132;
                    b = 132;
                    break;
                case 5 :
                    r = 106;
                    g = 0;
                    b = 21;
                    break;
                case 6 :
                    r = 64;
                    g = 128;
                    b = 128;
                    break;
                case 7 :
                    r = 20;
                    g = 20;
                    b = 20;
                    break;
            }
        }

        public bool Draw(SpriteBatch spriteBatch)
        {
            if(alpha > 0)
                alpha -= 5;
            if (alpha <= 50)
                return true;

            position += direction;

            spriteBatch.DrawString(spriteFont,
                damage,
                position,
                new Color(r, g, b, alpha),
                0,
                Vector2.Zero,
                2 * StaticHelpers.fontScale,
                SpriteEffects.None,
                0.1f);

            return false;
        }
    }
}
