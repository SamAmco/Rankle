using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameFinal.Display
{
    class KeepScore
    {
        Score[] scores;
        Rectangle clientBounds;
        Texture2D red;
        SpriteFont font;

        int padding = 10;
        int margin = 20;
        float titleScale = 0.8f;
        float scoreScale = 1;
        int lineSpacing = 20;

        int namePos = 0;
        int killsPos = 0;
        int deathsPos = 0;
        int kDPos = 0;

        public KeepScore(Texture2D red, SpriteFont font, Rectangle clientBounds)
        {
            scores = new Score[8];
            this.clientBounds = clientBounds;
            this.red = red;
            this.font = font;

            namePos = margin + padding;
            killsPos = (int)(((clientBounds.Width - (2 * (margin + padding))) / 8) * 4) + namePos;
            deathsPos = killsPos + (int)(((clientBounds.Width - (2 * (margin + padding))) / 8) * 1.33f);
            kDPos = deathsPos + (int)(((clientBounds.Width - (2 * (margin + padding))) / 8) * 1.33f);
        }

        public void AddPlayer(int index, string name)
        {
            scores[index] = new Score(name);
        }

        public void Death(int index)
        {
            scores[index].Death();
        }

        public void Kill(int index)
        {
            scores[index].Kill();
        }

        public float getKD(int i)
        {
            return scores[i].getKD();
        }

        public bool samePlayer(string name, int index)
        {
            if (scores[index] == null)
                return false;
            else if (scores[index].getName() == name)
                return true;
            else return false;
        }

        public Score[] getScores()
        {
            return scores;
        }

        public void setScores(Score[] scores)
        {
            this.scores = scores;
        }

        public void removePlayer(int index)
        {
            scores[index] = null;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camPos)
        {
            spriteBatch.Draw(red,
                new Rectangle(margin, margin, clientBounds.Width - (margin * 2), clientBounds.Height - (margin * 2)),
                null,
                Color.Black * 0.7f,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0.05f);

            spriteBatch.DrawString(font, "Name", new Vector2(namePos, margin + padding), Color.White, 0, Vector2.Zero, titleScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
            spriteBatch.DrawString(font, "Kills", new Vector2(killsPos, margin + padding), Color.White, 0, Vector2.Zero, titleScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
            spriteBatch.DrawString(font, "Deaths", new Vector2(deathsPos, margin + padding), Color.White, 0, Vector2.Zero, titleScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
            spriteBatch.DrawString(font, "K/D", new Vector2(kDPos, margin + padding), Color.White, 0, Vector2.Zero, titleScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);

            Score[] temp = new Score[8];
            Array.Copy(scores, temp, 8);
            //Console.Write(temp[0].getName());
            Array.Sort(temp);
            //Console.WriteLine("     " + temp[0].getName());

            int x = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (temp[i] != null)
                {
                    spriteBatch.DrawString(font, temp[i].getName(), new Vector2(namePos, margin + padding + (lineSpacing * (x + 2))),
                        Color.White, 0, Vector2.Zero, scoreScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
                    spriteBatch.DrawString(font, temp[i].getKills().ToString(), new Vector2(killsPos, margin + padding + (lineSpacing * (x + 2))),
                        Color.White, 0, Vector2.Zero, scoreScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
                    spriteBatch.DrawString(font, temp[i].getDeaths().ToString(), new Vector2(deathsPos, margin + padding + (lineSpacing * (x + 2))),
                        Color.White, 0, Vector2.Zero, scoreScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
                    spriteBatch.DrawString(font, temp[i].getKD().ToString(), new Vector2(kDPos, margin + padding + (lineSpacing * (x + 2))),
                        Color.White, 0, Vector2.Zero, scoreScale * StaticHelpers.fontScale, SpriteEffects.None, 0.04f);
                    x++;
                }
            }
        }
    }
}
