using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameFinal.Misc;

namespace GameFinal.Menu_s
{
    class WhiteWhipe
    {
        #region variables
        Texture2D white;
        Texture2D whipe;
        Rectangle clientBounds;
        Rectangle whiteDrawRect;
        int whipeY;
        int speed;
        bool whipeThrough;
        int timer = 5000;

        public delegate void ScreenBlankListener(object o, EventArgs e);
        public event ScreenBlankListener on_Blank;

        bool callBlank = false;
        Audio audio;
        bool playedSwishDown = false;
        #endregion

        public WhiteWhipe(Texture2D[] whipeTexs, Rectangle clientBounds, int speed, bool whipeThrough, Audio audio)
        {
            this.white = whipeTexs[0];
            this.whipe = whipeTexs[1];
            this.clientBounds = clientBounds;
            this.speed = speed;
            this.whiteDrawRect = new Rectangle(0, 0, clientBounds.Width, 0);
            this.whipeY = -whipe.Height;
            this.whipeThrough = whipeThrough;
            this.audio = audio;
        }

        public bool Update(GameTime gameTime)
        {
            if (!playedSwishDown)
            {
                audio.playSound("swishDown", 0.7f, 0.3f, 0);
                playedSwishDown = true;
            }
            if (callBlank)
            {
                callBlank = false;
                on_Blank(this, new EventArgs());
            }

            if (timer < 5000)
            {
                if (timer > 0)
                    timer -= gameTime.ElapsedGameTime.Milliseconds;
                else if (!whipeThrough)
                {
                    whipeThrough = true;
                    audio.playSound("swishUp", 0.7f, 0.3f, 0);
                }
            }

            if (whipeY >= clientBounds.Height)
            {
                if (whipeThrough)
                {
                    if (whipeY < (clientBounds.Height * 2) + whipe.Height)
                        whipeY += speed;
                    else
                        return true;
                }
                else
                    whipeY = clientBounds.Height;
            }
            else if (whipeY < clientBounds.Height)
            {
                whipeY += speed;
                if (whipeY >= clientBounds.Height)
                {
                    callBlank = true;
                }
            }
            
            return false;
        }

        //public bool ScreenBlank()
        //{
        //    if (whipeY >= clientBounds.Height)
        //        return true;
        //    else return false;
        //}

        public void WhipeThrough()
        {
            if (timer == 5000)
                timer = 800;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //if (whipeY < clientBounds.Height)
            if (whipeY > clientBounds.Height && !whipeThrough)
                whipeY = clientBounds.Height;

                spriteBatch.Draw(white,
                    new Rectangle(0, whipeY - clientBounds.Height, clientBounds.Width, clientBounds.Height),
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.1f);

                spriteBatch.Draw(whipe,
                    new Rectangle(0, whipeY, clientBounds.Width, whipe.Height),
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.1f);
                spriteBatch.Draw(whipe,
                    new Rectangle(clientBounds.Width, whipeY - clientBounds.Height - (whipe.Height / 2), clientBounds.Width, whipe.Height),
                    null,
                    Color.White,
                    (float)Math.PI,
                    new Vector2(whipe.Width / 2, whipe.Height / 2),
                    SpriteEffects.None,
                    0.1f);
        }
    }
}
