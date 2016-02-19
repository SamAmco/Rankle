using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameFinal.Misc;
using Microsoft.Xna.Framework.Input;

namespace GameFinal.Menu_s
{
    class Slider : FormElement
    {
        float value;
        Texture2D sliderTex;
        Texture2D barTex;
        Audio audio;
        bool audioSlider;
        bool holdDown = false;
        
        public Slider(Texture2D barTex, Texture2D sliderTex, SpriteFont spriteFont, Vector2 position,
            Vector2 targetPos, float value, float scale, bool audioSlider, Audio audio)
            : base(spriteFont, position, targetPos)
        {
            this.value = value;
            this.scale = scale;
            this.sliderTex = sliderTex;
            this.barTex = barTex;
            this.audio = audio;
            this.audioSlider = audioSlider;
        }

        public override bool Update(GameTime gameTime)
        {
            if (audioSlider)
            {
                if (mouseRect.Intersects(new Rectangle((int)(position.X - ((barTex.Width * scale) / 2)),
                    (int)(position.Y - ((barTex.Height * scale) / 2)),
                    (int)(barTex.Width * scale),
                    (int)(barTex.Height * scale))) || holdDown)
                {
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        holdDown = true;
                        value = (Mouse.GetState().X - (position.X - ((barTex.Width * scale) / 2) + (64 * scale)))
                            / ((barTex.Width * scale) - (128 * scale));
                        if (value > 1)
                            value = 1;
                        else if (value < 0)
                            value = 0;
                    }
                    else if (prevMouseState.LeftButton == ButtonState.Pressed)
                    {
                        holdDown = false;
                        audio.setEffectVolume(value);
                        audio.playSound("click");
                    }
                }
            }
            return base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPos = position + new Vector2(-(barTex.Width * scale) / 2, -(barTex.Height * scale) / 2);
            spriteBatch.Draw(barTex,
                drawPos,
                null,
                Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.21f);
            spriteBatch.Draw(sliderTex,
                new Vector2(drawPos.X + (value * ((barTex.Width - 128) * scale)) + (scale * 64) - ((sliderTex.Width * scale) / 2), drawPos.Y),
                null,
                Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.2f);

            base.Draw(spriteBatch);
        }
    }
}
