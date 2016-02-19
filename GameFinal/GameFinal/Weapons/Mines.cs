using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using GameFinal.Objects;
using GameFinal.Control;
using FarseerPhysics.DemoBaseXNA;
using GameFinal.Misc;

namespace GameFinal.Weapons
{
    class Mines
    {
        #region variables
        List<Mine> mineList = new List<Mine>();
        List<Vector2> placeList = new List<Vector2>();
        int maxMines = 6;
        int gunSpeed;
        int gunTimer;
        Texture2D mineTex;
        GameCharacter shooter;
        float requiredEnergy = 0;
        //bool canQue = false;
        int characterIndex;
        List<Fixture> tankFixtures;
        ExplosionGenerator expGen;
        Random rnd;
        InGame parentGame;
        bool tripleMines = false;
        Audio audio;
        //float lastRot = 0;
        #endregion

        public Mines(Texture2D mineTex, float requiredEnergy,
            GameCharacter shooter, InGame parentGame, List<Fixture> tankFixtures, int characterIndex, bool canQue, int gunSpeed, Audio audio)
        {
            this.mineTex = mineTex;
            this.requiredEnergy = requiredEnergy;
            this.shooter = shooter;
            this.tankFixtures = tankFixtures;
            this.expGen = parentGame.getExplosionGenerator();
            this.characterIndex = characterIndex;
            this.parentGame = parentGame;
            this.audio = audio;
            //this.canQue = canQue;
            this.gunSpeed = gunSpeed;
            rnd = new Random();
        }
        

        public bool FireWeapon(GameTime gameTime, float energy)
        {
            if (gunTimer >= gunSpeed && energy >= requiredEnergy)
            {
                placeMines(shooter.getRotation());
                return true;
            }
            else if (gunTimer >= gunSpeed && !(energy >= requiredEnergy))
            {
                audio.playSound("click",
                    StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                    0,
                    StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
                gunTimer = 0;
                return false;
            }
            //else if (canQue)
            //{
            //    que += 1;
            //    if (fired)
            //    {
            //        fired = false;
            //        return true;
            //    }
            //    else return false;
            //}
            else
                return false;
        }
        //public bool FireWeapon(GameTime gameTime, float energy, float rotation)
        //{
        //    if (gunTimer >= gunSpeed && energy >= requiredEnergy)
        //    {
        //        placeMines(rotation);
        //        return true;
        //    }
        //    else if (canQue)
        //    {
        //        que += 1;
        //        lastRot = rotation;
        //        if (fired)
        //        {
        //            fired = false;
        //            return true;
        //        }
        //        else return false;
        //    }
        //    else
        //        return false;
        //}
        public void PlaceMine(Vector2 v)
        {
            mineList.Add(new Mine(mineTex,
                ConvertUnits.ToDisplayUnits(v),
                Vector2.Zero, expGen, characterIndex, parentGame, audio));
            if (mineList.Count > maxMines)
            {
                mineList[0].Dispose();
                mineList.Remove(mineList[0]);
            }
        }

        private void placeMines(float rotation)
        {
            if (!tripleMines)
            {
                mineList.Add(new Mine(mineTex,
                    ConvertUnits.ToDisplayUnits(shooter.getPos()) + StaticHelpers.RotateVector(new Vector2(0, 70), rotation),
                    Vector2.Zero, expGen, characterIndex, parentGame, audio));
                placeList.Add(shooter.getPos() + ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(new Vector2(0, 70), rotation)));
                if (mineList.Count > maxMines)
                {
                    mineList[0].Dispose();
                    mineList.Remove(mineList[0]);
                }
                gunTimer = 0;
            }
            else
            {
                mineList.Add(new Mine(mineTex,
                    ConvertUnits.ToDisplayUnits(shooter.getPos()) + StaticHelpers.RotateVector(new Vector2(0, 70), rotation),
                    Vector2.Zero, expGen, characterIndex, parentGame, audio));
                mineList.Add(new Mine(mineTex,
                    ConvertUnits.ToDisplayUnits(shooter.getPos()) + StaticHelpers.RotateVector(new Vector2(40, 70), rotation),
                    Vector2.Zero, expGen, characterIndex, parentGame, audio));
                mineList.Add(new Mine(mineTex,
                    ConvertUnits.ToDisplayUnits(shooter.getPos()) + StaticHelpers.RotateVector(new Vector2(-40, 70), rotation),
                    Vector2.Zero, expGen, characterIndex, parentGame, audio));
                placeList.Add(shooter.getPos() + ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(new Vector2(0, 70), rotation)));
                placeList.Add(shooter.getPos() + ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(new Vector2(40, 70), rotation)));
                placeList.Add(shooter.getPos() + ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(new Vector2(-40, 70), rotation)));
                while (mineList.Count > maxMines)
                {
                    mineList[0].Dispose();
                    mineList.Remove(mineList[0]);
                }
                gunTimer = 0;
            }
            audio.playSound("layMines",
                StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                -0.6f,
                StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));
        }

        public void Update(GameTime gameTime, OtherCharacter[] otherCharacters, MainCharacter m)
        {
            //if (que > 0)
            //{
            //    if (que > 2)
            //        que = 2;
            //    if (lastRot == 0)
            //        FireWeapon(gameTime, shooter.energy);
            //    else FireWeapon(gameTime, shooter.energy, lastRot);
            //    que -= 1;
            //    fired = true;
            //}
            for (int i = 0; i < mineList.Count; i++)
            {
                if (mineList[i].Update(gameTime, otherCharacters, m))
                {
                    mineList.Remove(mineList[i]);
                }
            }
            if (gunTimer < gunSpeed)
                gunTimer += gameTime.ElapsedGameTime.Milliseconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Mine m in mineList)
            {
                m.Draw(spriteBatch);
            }
        }

        public void Dispose()
        {
            foreach (Mine m in mineList)
            {
                m.Dispose();
            }
        }

        public Vector2[] getMinePos()
        {
            Vector2[] r = new Vector2[mineList.Count];
            for (int i = 0; i < mineList.Count; i++)
            {
                r[i] = mineList[i].getPos();
            }
            return r;
        }
        public void setMines(float[] minesx, float[] minesy)
        {
            for (int i = 0; i < minesx.Length; i++)
            {
                mineList.Add(new Mine(mineTex, ConvertUnits.ToDisplayUnits(new Vector2(minesx[i], minesy[i])), Vector2.Zero,
                    parentGame.getExplosionGenerator(), characterIndex, parentGame, audio));
            }
        }
        public List<Vector2> getPlaceList()
        {
            List<Vector2> v = placeList;
            placeList = new List<Vector2>();
            return v;
        }
        public void TripleMines()
        {
            if (!tripleMines)
            {
                maxMines *= 3;
                tripleMines = true;
            }
        }
        public void StopTripleMines()
        {
            if (tripleMines)
            {
                maxMines /= 3;
                tripleMines = false;
            }
        }

        public bool getTripleMines()
        {
            return tripleMines;
        }
    }
}
