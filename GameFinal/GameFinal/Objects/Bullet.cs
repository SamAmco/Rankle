using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DemoBaseXNA;
using System;
using FarseerPhysics.Dynamics.Contacts;
using System.Collections.Generic;
using GameFinal.Control;
using GameFinal.Misc;

namespace GameFinal
{
    class Bullet
    {
        #region Variables
        public Body bulletBody;
        public Fixture bulletFixture;
        Vector2 bulletOrigin;
        Texture2D bulletTex;
        bool destroy = false;
        int characterIndex;
        List<Fixture> tankFixtures;
        float scale = 0.1f;
        Vector2 startVelocity;
        float startRotation;
        ExplosionGenerator expGen;
        Audio audio;
        InGame parentGame;
        Random rnd;
        #endregion

        public Bullet(Texture2D bulletTex, Vector2 characterPosition, Vector2 Offset,
            float rotation, int speed, InGame parentGame, int characterIndex, List<Fixture> tankFixtures, Audio audio)
        {
            this.bulletTex = bulletTex;
            this.bulletOrigin = new Vector2((bulletTex.Width * scale) / 2, (bulletTex.Height * scale) / 2);
            this.characterIndex = characterIndex;
            this.tankFixtures = tankFixtures;
            this.expGen = parentGame.getExplosionGenerator();
            this.audio = audio;
            this.parentGame = parentGame;

            rnd = new Random();

            Vector2 Pos = StaticHelpers.RotateVector(Offset, rotation);
            Pos += ConvertUnits.ToDisplayUnits(characterPosition);

            Vector2 velocity = StaticHelpers.VectorFromRotation(rotation) * speed;

            bulletFixture = FixtureFactory.CreateRectangle(parentGame.world,
                ConvertUnits.ToSimUnits(bulletTex.Width * scale),
                ConvertUnits.ToSimUnits(bulletTex.Height * scale),
                0.01f);

            /*Changeable Variables...*/

            bulletFixture.Friction = 0.1f;
            bulletFixture.Restitution = 0.1f;

            bulletBody = bulletFixture.Body;
            bulletBody.BodyType = BodyType.Dynamic;
            bulletBody.IsBullet = true;
            bulletBody.affectPlayer = 1 + (100 * characterIndex);
            bulletBody.Position = ConvertUnits.ToSimUnits(Pos);
            bulletBody.Rotation = rotation;
            this.startVelocity = velocity;
            this.startRotation = rotation;
            bulletBody.LinearVelocity = velocity;

            bulletFixture.OnCollision += new OnCollisionEventHandler(this.On_Collision);
        }

        public bool Update(GameTime gameTime)
        {
            if (Math.Abs(bulletBody.LinearVelocity.Length() - startVelocity.Length()) > 0.01f || Math.Abs(startRotation - bulletBody.Rotation) > 0.01f)
            {
                destroy = true;
                expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(bulletBody.Position)
                    + StaticHelpers.RotateVector(new Vector2(0, -(bulletTex.Height * scale) / 2), startRotation), 3, 0.4f);
            }
            if (destroy)
                bulletBody.Dispose();

            return destroy;
        }

        public bool On_Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body.IsStatic)
            {
                audio.playSound("bulletWall", StaticHelpers.getVolume(bulletBody.Position, parentGame.getMainCharacterPos()),
                    ((float)(rnd.Next(0, 9)) / 10) - 0.4f,
                    StaticHelpers.getPan(bulletBody.Position, parentGame.getMainCharacterPos()));
            }
            else if (fixtureB.Body.tankIndex != characterIndex && !fixtureB.Body.IsBullet)
            {
                audio.playSound("bulletPlayer", StaticHelpers.getVolume(bulletBody.Position, parentGame.getMainCharacterPos()) * 0.65f,
                        ((float)(rnd.Next(0, 9)) / 10) - 0.4f,//((float)(rnd.Next(0, 9)) / 10) - 0.4f,
                        StaticHelpers.getPan(bulletBody.Position, parentGame.getMainCharacterPos()));
            }
            return true;
        }

        public void Dispose()
        {
            expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(bulletBody.Position)
                + StaticHelpers.RotateVector(new Vector2(0, -(bulletTex.Height * scale) / 2), bulletBody.Rotation), 3, 0.4f);
            bulletBody.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!(Math.Abs(bulletBody.LinearVelocity.Length() - startVelocity.Length()) > 0.01f || Math.Abs(startRotation - bulletBody.Rotation) > 0.01f) && !destroy)
            {
                Vector2 position = ConvertUnits.ToDisplayUnits(bulletBody.WorldCenter) + StaticHelpers.RotateVector(-bulletOrigin, bulletBody.Rotation);
                spriteBatch.Draw(bulletTex,
                    position,
                    null,
                    Color.White,//new Color(255,255,255,0.8f),
                    bulletBody.Rotation,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.13f);
            }
        }
    }
}
