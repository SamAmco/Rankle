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
    class TankSelector : FormElement
    {
        Texture2D[] tankTexs;
        int selectedIndex;

        int padding = 10;

        float xTransition = 0;

        Vector2 preferredSize;
        Audio audio;

        public TankSelector(Texture2D[] tankTexs, SpriteFont spriteFont, Vector2 position, Vector2 targetPos, int selectedIndex, float scale, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.tankTexs = tankTexs;
            this.selectedIndex = selectedIndex;
            preferredSize = Vector2.Zero;
            this.scale = scale;
            this.audio = audio;
            xTransition = selectedIndex;
            prevMouseState = Mouse.GetState();
        }

        public TankSelector(Texture2D[] tankTexs, SpriteFont spriteFont, Vector2 position, Vector2 targetPos, int selectedIndex, Vector2 preferredSize, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.tankTexs = tankTexs;
            this.selectedIndex = selectedIndex;
            this.preferredSize = preferredSize;
            this.audio = audio;
            scale = 0.15f;
            xTransition = selectedIndex;
            prevMouseState = Mouse.GetState();
        }

        public override bool Update(GameTime gameTime)
        {
            Rectangle r;
            if (preferredSize != Vector2.Zero)
            {
                r = new Rectangle((int)(position.X - padding - (preferredSize.X * 1.5f)),
                    (int)(position.Y - preferredSize.Y / 2),
                    (int)((preferredSize.X * 3) + (2 * padding)),
                    (int)preferredSize.Y);
            }
            else
            {
               r  = new Rectangle((int)(position.X - padding - ((tankTexs[0].Width * scale) * 1.5f)),
                    (int)(position.Y - (tankTexs[0].Height * scale) / 2),
                    (int)((((tankTexs[0].Width) * scale) * 3) + (2 * padding)),
                    (int)(tankTexs[0].Height * scale));
            }

            if (xTransition != selectedIndex)
            {
                xTransition += (selectedIndex - xTransition) / 10;
                if (xTransition <= 0.01f)
                    xTransition = selectedIndex;
            }

            if (mouseRect.Intersects(r))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
                {
                    if (selectedIndex < tankTexs.Length - 1 && Mouse.GetState().X > position.X)
                    {
                        audio.playSound("click", 1, -0.15f, 0);
                        selectedIndex++;
                    }
                    else if (selectedIndex > 0 && Mouse.GetState().X < position.X)
                    {
                        audio.playSound("click", 1, -0.15f, 0);
                        selectedIndex--;
                    }
                }
            }
            return base.Update(gameTime);
        }

        public int getSelectedIndex()
        {
            return selectedIndex;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < tankTexs.Length; i++)
            {
                if (!(Math.Abs(i - selectedIndex) > 2))
                {
                    float xOffset;
                    Vector2 texPos;
                    float opacity;
                    if (preferredSize != Vector2.Zero)
                    {
                        xOffset = ((preferredSize.X + padding) * (i - xTransition));
                        texPos = new Vector2(position.X - (preferredSize.X / 2) + xOffset, position.Y - (preferredSize.Y / 2));
                        opacity = (((preferredSize.X + padding) * 2) / Math.Abs(xOffset / scale));
                    }
                    else
                    {
                        xOffset = (((tankTexs[i].Width * scale) + padding) * (i - xTransition));
                        texPos = new Vector2(position.X - ((tankTexs[i].Width * scale) / 2) + xOffset, position.Y - ((tankTexs[i].Height * scale) / 2));
                        opacity = 1f - (Math.Abs(xOffset) / 150f);//(((tankTexs[i].Width * scale) + padding) / Math.Abs(xOffset / scale));
                    }

                    if (preferredSize != Vector2.Zero)
                    {
                        if (opacity > 0.2f)
                        {
                            spriteBatch.Draw(tankTexs[i],
                                new Rectangle((int)texPos.X, (int)texPos.Y, (int)preferredSize.X, (int)preferredSize.Y),
                                null,
                                Color.White * opacity,
                                0,
                                Vector2.Zero,
                                SpriteEffects.None,
                                0.3f);
                        }
                    }
                    else if (opacity > 0.2f)
                    {
                        spriteBatch.Draw(tankTexs[i],
                            texPos,
                            null,
                            Color.White * opacity,
                            0,
                            Vector2.Zero,
                            scale,
                            SpriteEffects.None,
                            0.3f);
                    }
                }
            }
            base.Draw(spriteBatch);
        }
    }
}
