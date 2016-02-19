using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using GameFinal.Objects;
using GameFinal.Misc;

namespace GameFinal
{
    class Rifle
    {
        #region Variables
        List<Bullet> bulletList = new List<Bullet>();
        List<Missile> missileList = new List<Missile>();
        int gunSpeed; 
        int bulletSpeed;
        int bulletTimer;
        InGame parentGame;
        Vector2 offset;
        Texture2D bulletTex;
        Texture2D missileTex;
        GameCharacter shooter;
        float requiredEnergy = 0;
        bool canQue = false;
        bool fired = false;
        int que = 0;
        float lastRot = 0;
        int characterIndex;
        List<Fixture> tankFixtures;
        Audio audio;
        bool super_Gun = false;
        #endregion

        public Rifle(int gunSpeed, int bulletSpeed, int bulletTimer, GameCharacter shooter,
            Vector2 offset, InGame parentGame, Texture2D bulletTex, Texture2D missileTex, bool canQue,
            int characterIndex, List<Fixture> tankFixtures, float requiredEnergy, Audio audio)
        {
            this.gunSpeed = gunSpeed;
            this.bulletSpeed = bulletSpeed;
            this.bulletTimer = bulletTimer;
            this.parentGame = parentGame;
            this.offset = offset;
            this.bulletTex = bulletTex;
            this.shooter = shooter;
            this.canQue = canQue;
            this.characterIndex = characterIndex;
            this.tankFixtures = tankFixtures;
            this.requiredEnergy = requiredEnergy;
            this.missileTex = missileTex;
            this.audio = audio;
        }

        public bool FireWeapon(GameTime gameTime, float energy)
        {
            if (bulletTimer >= gunSpeed && energy >= requiredEnergy)
            {
                createBullets(shooter.getRotation());
                bulletTimer = 0;
                return true;
            }
            else
            {
                if (bulletTimer >= gunSpeed && !(energy >= requiredEnergy))
                {
                    audio.playSound("click",
                        StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                        0,
                        StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                    bulletTimer = 0;
                }
                if (canQue)
                {
                    que += 1;
                    if (fired)
                    {
                        fired = false;
                        return true;
                    }
                    else return false;
                }
                return false;
            }
        }

        public bool FireWeapon(GameTime gameTime, float energy, float rotation)
        {
            if (bulletTimer >= gunSpeed && energy >= requiredEnergy)
            {
                createBullets(rotation);
                bulletTimer = 0;
                lastRot = 0;
                return true;
            }
            else 
            {
                if (bulletTimer >= gunSpeed && !(energy >= requiredEnergy))
                {
                    audio.playSound("click",
                        StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                        0,
                        StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                    bulletTimer = 0;
                }
                if (canQue)
                {
                    que += 1;
                    lastRot = rotation;
                    if (fired)
                    {
                        fired = false;
                        return true;
                    }
                    else return false;
                }
                return false;
            }
        }

        private void createBullets(float rotation)
        {
            if (super_Gun)
            {
                Missile m1 = new Missile(missileTex,
                    shooter.getPos(),
                    offset + new Vector2(-10, -10),
                    rotation,
                    shooter.getVelocity().Length(),
                    parentGame,
                    characterIndex,
                    parentGame.getExplosionGenerator(),
                    audio);
                Missile m2 = new Missile(missileTex,
                   shooter.getPos(),
                   offset + new Vector2(10, -10),
                   rotation,
                   shooter.getVelocity().Length(),
                   parentGame,
                   characterIndex,
                   parentGame.getExplosionGenerator(),
                   audio);
                audio.playSound("missiles",
                    StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                    0,
                    StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                missileList.Add(m1);
                missileList.Add(m2);
            }
            else
            {
                Bullet b1 = new Bullet(bulletTex,
                   shooter.getPos(),
                   offset + new Vector2(-10, 0),
                   rotation,
                   bulletSpeed,
                   parentGame,
                   characterIndex,
                   tankFixtures,
                   audio);
                Bullet b2 = new Bullet(bulletTex,
                   shooter.getPos(),
                   offset + new Vector2(10, 0),
                   rotation,
                   bulletSpeed,
                   parentGame,
                   characterIndex,
                   tankFixtures,
                   audio);

                bulletList.Add(b1);
                bulletList.Add(b2);
                //if (StaticHelpers.debugMode && !(shooter.getPos().X == parentGame.getMainCharacterPos().X 
                //    && shooter.getPos().Y == parentGame.getMainCharacterPos().Y))
                audio.playSound("rifle", StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()) * 0.2f,
                    -0.5f + (0.025f * (characterIndex - 6)),
                    StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
            }

        }

        public void UpdateBullets(GameTime gameTime)
        {
            if (que > 0)
            {
                if (que > 2)
                    que = 2;
                if (lastRot == 0)
                    FireWeapon(gameTime, shooter.energy);
                else FireWeapon(gameTime, shooter.energy, lastRot);
                que -= 1;
                fired = true;
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].Update(gameTime))
                {
                    bulletList.Remove(bulletList[i]);
                }
            }
            for (int i = 0; i < missileList.Count; i++)
            {
                if (missileList[i].Update(gameTime))
                {
                    missileList.Remove(missileList[i]);
                }
            }
            if (bulletTimer < gunSpeed)
                bulletTimer += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Bullet b in bulletList)
            {
                b.Draw(spriteBatch);
            }
            foreach (Missile m in missileList)
            {
                m.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            foreach (Bullet b in bulletList)
            {
                b.Dispose();
            }
        }

        public void superGun()
        {
            if (!super_Gun)
            {
                gunSpeed *= 4;
                super_Gun = true;
            }
        }
        public void StopSuperGun()
        {
            if (super_Gun)
            {
                gunSpeed /= 4;
                super_Gun = false;
            }
        }

        public bool getSuperGun()
        {
            return super_Gun;
        }
    }
}
