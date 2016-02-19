using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.DemoBaseXNA;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using GameFinal.Control;
using FarseerPhysics.Dynamics.Contacts;
using GameFinal.Misc;

namespace GameFinal.Objects
{
    class Missile
    {
        #region Variables
        public Body missileBody;
        public Fixture missileFixture;
        Vector2 missileOrigin;
        Texture2D missileTex;
        bool destroy = false;
        int characterIndex;
        float scale = 0.2f;
        float startRotation;
        int exhaustTimer = 0;
        ExplosionGenerator expGen;
        float accelerationForce = 0.1f;
        Audio audio;
        Random rnd;
        InGame parentGame;
        #endregion

        public Missile(Texture2D missileTex, Vector2 characterPosition, Vector2 Offset,
            float rotation, float speed, InGame parentGame, int characterIndex, ExplosionGenerator expGen, Audio audio)
        {
            this.missileTex = missileTex;
            this.missileOrigin = new Vector2((missileTex.Width * scale) / 2, (missileTex.Height * scale) / 2);
            this.characterIndex = characterIndex;
            this.expGen = expGen;
            this.audio = audio;
            this.rnd = new Random();
            this.parentGame = parentGame;

            Vector2 Pos = StaticHelpers.RotateVector(Offset, rotation);
            Pos += ConvertUnits.ToDisplayUnits(characterPosition);

            Vector2 velocity = StaticHelpers.VectorFromRotation(rotation) * speed;

            missileFixture = FixtureFactory.CreateRectangle(parentGame.world,
                ConvertUnits.ToSimUnits(missileTex.Width * scale),
                ConvertUnits.ToSimUnits(missileTex.Height * scale),
                15);

            /*Changeable Variables...*/

            missileFixture.Friction = 0.1f;
            missileFixture.Restitution = 0.1f;

            missileBody = missileFixture.Body;
            missileBody.BodyType = BodyType.Dynamic;
            missileBody.IsBullet = true;
            missileBody.affectPlayer = 3 + (100 * characterIndex);
            missileBody.Position = ConvertUnits.ToSimUnits(Pos);
            missileBody.Rotation = rotation;
            missileBody.LinearVelocity = velocity;
            velocity.Normalize();
            this.startRotation = rotation;
            

            missileFixture.OnCollision += new OnCollisionEventHandler(this.On_Collision);
        }

        public bool Update(GameTime gameTime)
        {
            Vector2 testV = missileBody.LinearVelocity;
            testV.Normalize();
            if (Math.Abs(startRotation - missileBody.Rotation) > 0.1f)
            {
                destroy = true;
            }
            if (destroy)
            {
                expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(missileBody.Position), 1);
                audio.playSound("minesHit",
                    StaticHelpers.getVolume(missileBody.Position, parentGame.getMainCharacterPos()),
                    -0.02f * rnd.Next(0, 10),
                    StaticHelpers.getPan(missileBody.Position, parentGame.getMainCharacterPos()));
                missileBody.Dispose();
            }

            exhaustTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (exhaustTimer >= 10)
            {
                exhaustTimer = 0;
                expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(missileBody.Position), 2, 0.4f);
                missileBody.ApplyLinearImpulse(StaticHelpers.VectorFromRotation(startRotation) * accelerationForce);
            }

            return destroy;
        }

        public bool On_Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body.IsStatic)
            {
                destroy = true;
            }
            return true;
        }

        public void Dispose()
        {
            expGen.CreateExplosion(ConvertUnits.ToDisplayUnits(missileBody.Position), 1);
            audio.playSound("minesHit",
                    StaticHelpers.getVolume(missileBody.Position, parentGame.getMainCharacterPos()),
                    -0.02f * rnd.Next(0, 10),
                    StaticHelpers.getPan(missileBody.Position, parentGame.getMainCharacterPos()));
            missileBody.Dispose();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!destroy)
            {
                Vector2 position = ConvertUnits.ToDisplayUnits(missileBody.WorldCenter) + StaticHelpers.RotateVector(-missileOrigin, missileBody.Rotation);
                spriteBatch.Draw(missileTex,
                    position,
                    null,
                    Color.White,//new Color(255,255,255,0.8f),
                    missileBody.Rotation,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.13f);
            }
        }
    }
}
