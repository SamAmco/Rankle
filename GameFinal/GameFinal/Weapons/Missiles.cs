using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameFinal.Objects;
using GameFinal.Misc;

namespace GameFinal.Weapons
{
    class Missiles
    {
        #region variables 
        List<Missile> missileList = new List<Missile>();
        int gunSpeed; 
        int missileSpeed;
        int missileTimer;
        InGame parentGame;
        Vector2 offset;
        Texture2D missileTex;
        GameCharacter shooter;
        float requiredEnergy = 0;
        bool canQue = false;
        bool fired = false;
        int que = 0;
        int characterIndex;
        float lastRot = 0;
        Audio audio;
        #endregion

        public Missiles(int gunSpeed, int missileSpeed, int missileTimer, GameCharacter shooter,
            Vector2 offset, InGame parentGame, Texture2D missileTex, bool canQue, int characterIndex, float requiredEnergy, Audio audio)
        {
            this.gunSpeed = gunSpeed;
            this.missileSpeed = missileSpeed;
            this.missileTimer = missileTimer;
            this.parentGame = parentGame;
            this.offset = offset;
            this.missileTex = missileTex;
            this.shooter = shooter;
            this.canQue = canQue;
            this.characterIndex = characterIndex;
            this.requiredEnergy = requiredEnergy;
            this.audio = audio;
        }

        public bool FireWeapon(GameTime gameTime, float energy)
        {
            if (missileTimer >= gunSpeed && energy >= requiredEnergy)
            {
                Missile b1 = new Missile(missileTex,
                    shooter.getPos(),
                    offset + new Vector2(-10, 0),
                    shooter.getRotation(),
                    missileSpeed + shooter.getVelocity().Length(),
                    parentGame,
                    characterIndex,
                    parentGame.getExplosionGenerator(),
                    audio);
                Missile b2 = new Missile(missileTex,
                   shooter.getPos(),
                   offset + new Vector2(10, 0),
                   shooter.getRotation(),
                   missileSpeed + shooter.getVelocity().Length(),
                   parentGame,
                   characterIndex,
                   parentGame.getExplosionGenerator(),
                   audio);
                missileList.Add(b1);
                missileList.Add(b2);

                audio.playSound("missiles",
                    StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                    0,
                    StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                missileTimer = 0;
                return true;
            }
            else 
            {
                if (missileTimer >= gunSpeed && !(energy >= requiredEnergy))
                {
                    audio.playSound("click",
                        StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                        0,
                        StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                    missileTimer = 0;
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
            if (missileTimer >= gunSpeed && energy >= requiredEnergy)
            {
                Missile b1 = new Missile(missileTex,
                    shooter.getPos(),
                    offset + new Vector2(-10, 0),
                    rotation,
                    missileSpeed + shooter.getVelocity().Length(),
                    parentGame,
                    characterIndex,
                    parentGame.getExplosionGenerator(),
                    audio);
                Missile b2 = new Missile(missileTex,
                   shooter.getPos(),
                   offset + new Vector2(10, 0),
                   rotation,
                   missileSpeed + shooter.getVelocity().Length(),
                   parentGame,
                   characterIndex,
                   parentGame.getExplosionGenerator(),
                   audio);
                missileList.Add(b1);
                missileList.Add(b2);

                audio.playSound("missiles",
                    StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                    0,
                    StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                missileTimer = 0;
                return true;
            }
            else
            {
                if (missileTimer >= gunSpeed && !(energy >= requiredEnergy))
                {
                    audio.playSound("click",
                        StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                        0,
                        StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                    missileTimer = 0;
                }
                if (canQue)
                {
                    lastRot = rotation;
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

        public void UpdateMissiles(GameTime gameTime)
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
            for (int i = 0; i < missileList.Count; i++)
            {
                if (missileList[i].Update(gameTime))
                {
                    missileList.Remove(missileList[i]);
                }
            }
            if (missileTimer < gunSpeed)
                missileTimer += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Missile b in missileList)
            {
                b.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            foreach (Missile b in missileList)
            {
                b.Dispose();
            }
        }
    }
}
