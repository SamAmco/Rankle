using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameFinal.Menu_s
{
    class Label : FormElement
    {
        String text;
        Color c;

        public Label(SpriteFont spriteFont, Vector2 position, Vector2 targetPos,
            String text, float scale, Color c)
            : base(spriteFont, position, targetPos)
        {
            this.text = text;
            this.scale = scale;
            this.c = c;
        }

        public override bool Update(GameTime gameTime)
        {
            return base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = position - ((spriteFont.MeasureString(text) * StaticHelpers.fontScale) / 2);
            spriteBatch.DrawString(spriteFont,
                text,
                drawPosition,
                c, 
                0,
                Vector2.Zero,
                scale * StaticHelpers.fontScale,
                SpriteEffects.None,
                0.19f);
            base.Draw(spriteBatch);
        }
    }
}
