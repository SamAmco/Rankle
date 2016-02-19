using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameFinal.Objects
{
    class SpriteSheet
    {
        Texture2D tex;
        Point startFrame;
        Point currentFrame;
        Point numOfFrames;
        Point frameSize;
        float timeMax;
        float timeCount = 0;
        bool bounce = false;
        bool back = false;

        public SpriteSheet(Texture2D tex, Point startFrame, Point frameSize, float fPS, Point numOfFrames)
        {
            this.tex = tex;
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.frameSize = frameSize;
            this.numOfFrames = numOfFrames;
            timeMax = (1000f / fPS);
        }
        public SpriteSheet(Texture2D tex, Point startFrame, Point frameSize, float fPS, Point numOfFrames, bool bounce)
        {
            this.tex = tex;
            this.startFrame = startFrame;
            currentFrame = startFrame;
            this.frameSize = frameSize;
            this.numOfFrames = numOfFrames;
            timeMax = (1000f / fPS);
            this.bounce = bounce;
        }

        public bool Update(GameTime gameTime)
        {
            if (!bounce)
            {
                timeCount += gameTime.ElapsedGameTime.Milliseconds;
                if (timeCount >= timeMax)
                {
                    if (currentFrame.X >= numOfFrames.X - 1)
                    {
                        currentFrame.X = 0;
                        if (currentFrame.Y >= numOfFrames.Y - 1)
                            currentFrame.Y = 0;
                        else
                            currentFrame.Y += 1;
                    }
                    else
                        currentFrame.X += 1;
                    timeCount = 0;
                }
                if (currentFrame == new Point(numOfFrames.X - 1, numOfFrames.Y - 1))
                    return true;
                else return false;
            }
            else
            {
                if (!back)
                {
                    timeCount += gameTime.ElapsedGameTime.Milliseconds;
                    if (timeCount >= timeMax)
                    {
                        if (currentFrame.X >= numOfFrames.X - 1)
                        {
                            currentFrame.X = 0;
                            if (currentFrame.Y >= numOfFrames.Y - 1)
                                currentFrame.Y = 0;
                            else
                                currentFrame.Y += 1;
                        }
                        else
                            currentFrame.X += 1;
                        timeCount = 0;
                    }
                    if (currentFrame == new Point(numOfFrames.X - 1, numOfFrames.Y - 1))
                        back = true;
                }
                else
                {
                    timeCount += gameTime.ElapsedGameTime.Milliseconds;
                    if (timeCount >= timeMax)
                    {
                        if (currentFrame.X <= 0)
                        {
                            currentFrame.X = numOfFrames.X - 1;
                            if (currentFrame.Y <= 0)
                                currentFrame.Y = 0;
                            else
                                currentFrame.Y -= 1;
                        }
                        else
                            currentFrame.X -= 1;
                        timeCount = 0;
                    }
                    if (currentFrame == Point.Zero)
                        back = false;
                }
                return false;
            }
        }

        public Texture2D getTex()
        {
            return tex;
        }
        public Rectangle getDrawRect()
        {
            return new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);
        }
        public Vector2 getOrigin()
        {
            return new Vector2((float)frameSize.X / 2, (float)frameSize.Y / 2);
        }
    }
}
