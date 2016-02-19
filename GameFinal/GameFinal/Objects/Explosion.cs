using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameFinal.Objects
{
    class Explosion
    {
        SpriteSheet spriteSheet;
        float scale;
        Vector2 origin;
        Vector2 position;
        float rotation;

        public Explosion(Texture2D[] textures, Vector2 position, float fPS, float scale, int index)
        {
            this.scale = scale;
            this.position = position;
            Texture2D texture = textures[index];
            Random rnd = new Random();
            rotation = (float)rnd.NextDouble() * (float)Math.PI;
            switch (index)
            {
                case 0:
                    spriteSheet = new SpriteSheet(texture, new Point(0, 0), new Point(330, 330), fPS, new Point(texture.Width / 330, texture.Height / 330));
                    origin = new Vector2(165, 165);
                    break;
                case 1:
                    spriteSheet = new SpriteSheet(texture, new Point(0, 0), new Point(282, 282), fPS, new Point(texture.Width / 282, texture.Height / 282));
                    origin = new Vector2(141, 141);
                    break;
                case 2:
                    spriteSheet = new SpriteSheet(texture, new Point(0, 0), new Point(102, 102), fPS, new Point(texture.Width / 102, texture.Height / 102));
                    origin = new Vector2(51, 51);
                    break;
                case 3:
                    spriteSheet = new SpriteSheet(texture, new Point(0, 0), new Point(210, 210), fPS, new Point(texture.Width / 210, texture.Height / 210));
                    origin = new Vector2(105, 105);
                    break;
            }
        }

        public bool Update(GameTime gameTime)
        {
            return spriteSheet.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet.getTex(),
                position,
                spriteSheet.getDrawRect(),
                Color.White,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0.08f);
        }
    }
}
