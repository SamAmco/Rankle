using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameFinal.Objects;

namespace GameFinal.Control
{
    class ExplosionGenerator
    {
        Texture2D[] explosionTextures;
        List<Explosion> explosionList;

        public ExplosionGenerator(Texture2D[] explosionTextures)
        {
            this.explosionTextures = explosionTextures;
            explosionList = new List<Explosion>();
        }

        public void CreateExplosion(Vector2 position, int number)
        {
            explosionList.Add(new Explosion(explosionTextures, position, 50, 1, number));
        }
        public void CreateExplosion(Vector2 position, int number, float scale)
        {
            explosionList.Add(new Explosion(explosionTextures, position, 50, scale, number));
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < explosionList.Count; i++ )
            {
                if (explosionList[i].Update(gameTime))
                    explosionList.Remove(explosionList[i]);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Explosion e in explosionList)
            {
                e.Draw(spriteBatch);
            }
        }
    }
}
