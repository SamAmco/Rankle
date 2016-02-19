using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameFinal.Menu_s
{
    class FormElement
    {
        #region variables
        protected SpriteFont spriteFont;
        protected Vector2 position;

        protected bool active = true;
        protected float scale;

        protected Vector2 targetPos;
        protected bool reachedTarget = false;
        protected float movementFactor = 10;

        protected bool destroyOnTarget = false;
        protected Rectangle mouseRect;
        protected MouseState prevMouseState;
        #endregion

        public FormElement(SpriteFont spriteFont, Vector2 position, Vector2 targetPos)
        {
            this.spriteFont = spriteFont;
            this.position = position;
            this.targetPos = targetPos;
            mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            prevMouseState = Mouse.GetState();
        }

        public virtual bool Update(GameTime gameTime)
        {
            if (!reachedTarget)
            {
                position += (targetPos - position) / movementFactor;

                if (Math.Abs(targetPos.X - position.X) <= 1 && Math.Abs(targetPos.Y - position.Y) <= 1)
                {
                    position = targetPos;
                    reachedTarget = true;
                    if (destroyOnTarget)
                        return true;
                }
            }
            mouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            prevMouseState = Mouse.GetState();
            return false;
        }

        public void setNewTarget(Vector2 target)
        {
            reachedTarget = false;
            targetPos = target;
        }

        public void setDestroyTarget(Vector2 target)
        {
            reachedTarget = false;
            destroyOnTarget = true;
            targetPos = target;
        }

        public Vector2 getPos()
        {
            return position;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
