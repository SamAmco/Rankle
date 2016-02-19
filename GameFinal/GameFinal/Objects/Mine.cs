using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameFinal.Control;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.DemoBaseXNA;
using FarseerPhysics.Dynamics.Contacts;
using GameFinal.Misc;

namespace GameFinal.Objects
{
    class Mine
    {
        SpriteSheet mineSheet;
        ExplosionGenerator expGen;
        public Body mineBody;
        public Fixture mineFixture;
        Vector2 mineOrigin;
        bool destroy = false;
        int characterIndex;
        float scale = 0.5f;
        Random rnd;
        Audio audio;
        InGame parentGame;

        public Mine(Texture2D mineTex, Vector2 pos, Vector2 vel, ExplosionGenerator expGen, int characterIndex, InGame parentGame, Audio audio)
        {
            mineSheet = new SpriteSheet(mineTex, Point.Zero, new Point(100, 100), 60, new Point(5, 2), true);
            this.mineOrigin = new Vector2(((mineTex.Width / 5) * scale) / 2, ((mineTex.Height / 2) * scale) / 2);
            this.characterIndex = characterIndex;
            this.expGen = expGen;
            this.audio = audio;
            this.parentGame = parentGame;
            rnd = new Random();

            mineFixture = FixtureFactory.CreateCircle(parentGame.world,
                ConvertUnits.ToSimUnits(((mineTex.Width / 5) * scale) / 2),
                15);

            /*Changeable Variables...*/

            mineFixture.Friction = 0.1f;
            mineFixture.Restitution = 0.1f;

            mineBody = mineFixture.Body;
            mineBody.BodyType = BodyType.Dynamic;
            mineBody.mine = true;
            //mineBody.affectPlayer = 2 + (100 * characterIndex);
            mineBody.Position = ConvertUnits.ToSimUnits(pos);
            mineBody.Rotation = (float)rnd.NextDouble() * (float)Math.PI;
            mineBody.AngularVelocity = rnd.Next(-2, 3);
            mineBody.LinearVelocity = vel;
            mineFixture.OnCollision += new OnCollisionEventHandler(this.On_Collision);
        }

        public bool Update(GameTime gameTime, OtherCharacter[] otherCharacters, MainCharacter m)
        {
            int col = 60;
            foreach (OtherCharacter o in otherCharacters)
            {
                if (o != null)
                {
                    if (o.characterIndex != this.characterIndex)
                    {
                        Rectangle r = new Rectangle((int)ConvertUnits.ToDisplayUnits(o.getPos().X) - col / 2, (int)ConvertUnits.ToDisplayUnits(o.getPos().Y) - col / 2, col, col);
                        if (r.Intersects(new Rectangle((int)ConvertUnits.ToDisplayUnits(mineBody.Position.X) - col / 2,
                            (int)ConvertUnits.ToDisplayUnits(mineBody.Position.Y) - col / 2, col, col)))
                        {
                            o.MineHit(characterIndex);
                            destroy = true;
                        }
                    }
                }
            }
            if (m != null)
            {
                if (m.characterIndex != this.characterIndex)
                {
                    Rectangle r = new Rectangle((int)ConvertUnits.ToDisplayUnits(m.getPos().X) - col / 2, (int)ConvertUnits.ToDisplayUnits(m.getPos().Y) - col / 2, col, col);
                    if (r.Intersects(new Rectangle((int)ConvertUnits.ToDisplayUnits(mineBody.Position.X) - col / 2,
                        (int)ConvertUnits.ToDisplayUnits(mineBody.Position.Y) - col / 2, col, col)))
                    {
                        m.MineHit(characterIndex);
                        destroy = true;
                    }
                }
            }

            mineSheet.Update(gameTime);
            if (destroy)
            {
                expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(mineBody.Position), 1);
                playAudio();
                mineBody.Dispose();
                return true;
            }
            else return false;
        }

        public bool On_Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body.tankIndex != characterIndex && !fixtureB.Body.IsBullet && !fixtureB.Body.mine)
            {
                //destroy = true;
                return true;
            }
            else return false;
        }

        public void Dispose()
        {
            expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(mineBody.Position), 1);
            playAudio();
            mineBody.Dispose();
        }

        private void playAudio()
        {
            audio.playSound("minesHit",
                StaticHelpers.getVolume(mineBody.Position, parentGame.getMainCharacterPos()),
                0.02f * rnd.Next(0, 10),
                StaticHelpers.getPan(mineBody.Position, parentGame.getMainCharacterPos()));
        }

        public Vector2 getPos()
        {
            return mineBody.Position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mineSheet.getTex(),
                ConvertUnits.ToDisplayUnits(mineBody.Position) - StaticHelpers.RotateVector(mineOrigin, mineBody.Rotation),
                mineSheet.getDrawRect(),
                Color.White,
                mineBody.Rotation,
                Vector2.Zero,//mineOrigin,
                scale,
                SpriteEffects.None,
                0.13f);
        }
    }
}
