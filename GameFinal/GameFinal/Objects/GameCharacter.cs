using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.DemoBaseXNA;
using GameFinal.Objects;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Dynamics.Contacts;
using GameFinal.Weapons;
using GameFinal.Misc;

namespace GameFinal
{
    abstract class GameCharacter
    {
        #region Variables
        protected float enginePower = 10;
        public float health = 100;
        public float maxHealth = 100;
        public float energy = 100;
        public float maxEnergy = 100;
        protected float defenceMultiplier = 1f;
        int lastHit = 0;
        SpriteFont spriteFont;
        Random rnd;
        public int currentWeapon { get; set; }
        public int[] weapon = new int[5];
        // 0 = null;
        // 1 = Rifle;
        // 2 = Invisibility
        // 3 = Missile
        List<DamageIndicator> damageIndicators = new List<DamageIndicator>();
        List<int> affects;
        public int characterIndex { get; set; }

        protected Texture2D tankTex;
        protected Texture2D bulletTex;
        protected SpriteSheet exhaustSpriteSheet;
        protected bool drawExhaust = false;

        public Body tankBody;
        protected List<Fixture> tankFixtures;
        protected Vector2 tankOrigin;
        protected Rifle rifle;
        protected Invisibility invisibility;
        protected Mines mines;
        protected Missiles missiles;
        protected InGame parentGame;
        protected bool server;
        protected Audio audio;

        protected float eRifle = 0.5f;
        protected float eShotgun = 0.5f;
        protected float eInvisibility = 1.5f;
        protected float eMines = 1f;
        protected float eMissiles = 51f;

        bool freeInvisibilitye = false;
        bool freeRiflee = false;
        bool freeMissilese = false;
        bool freeMinese = false;
        bool ultraStealth = false;
        bool superTough = false;

        int bashWallT = 0;
        int bashOtherT = 0;
        #endregion

        //Each new weapon should be updated: variables, initialisation, draw, update, takeEnergy, fireWeapon, Game1.weaponTexs, checkAffect

        public GameCharacter(float enginePower, float health, float maxHealth,
            float energy, float maxEnergy, float defenceMultiplier, SpriteFont spriteFont, int characterIndex,
            Texture2D[] tankTexs, InGame parentGame, Vector2 Position, int[] weapon, bool server, int tankSkin, Audio audio)
        {
            this.enginePower = enginePower;
            this.health = health;
            this.maxHealth = maxHealth;
            this.energy = energy;
            this.maxEnergy = maxEnergy;
            this.defenceMultiplier = defenceMultiplier;
            this.spriteFont = spriteFont;
            this.characterIndex = characterIndex;
            this.tankTex = tankTexs[tankSkin];
            this.bulletTex = tankTexs[13];
            this.parentGame = parentGame;
            this.weapon = weapon;
            this.server = server;
            this.audio = audio;
            this.exhaustSpriteSheet = new SpriteSheet(tankTexs[12], new Point(0, 0), new Point(150, 300), 22, new Point(4, 2));
            tankOrigin = new Vector2((tankTex.Width * 0.1f) / 2, (tankTex.Height * 0.1f) / 2);
            affects = new List<int>();
            

            #region verts
            Vertices verts = new Vertices();
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(24, 0)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(47, 16)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(43, 31)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(32, 50)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(38, 61)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(38, 68)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(36, 72)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(32, 72)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(23, 66)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(13, 72)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 72)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(7, 67)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(7, 61)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(13, 50)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(3, 31)));
            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 16)));
            #endregion

            tankBody = new Body(parentGame.world);
            tankBody.Position = Position - ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(tankOrigin, tankBody.Rotation));
            tankBody.tankIndex = characterIndex;
            tankBody.isTank = true;

            List<Vertices> polygons = EarclipDecomposer.ConvexPartition(verts);
            tankFixtures = FixtureFactory.CreateCompoundPolygon(polygons, 1.0f, tankBody);
            foreach (Fixture f in tankFixtures)
            {
                f.Friction = 0.1f;
                f.Restitution = 0.1f;
                f.Body.BodyType = BodyType.Dynamic;
                f.Body.LinearDamping = 4;
                f.Body.AngularDamping = 4;
                f.OnCollision += new OnCollisionEventHandler(this.On_Collision);
            }
            rnd = new Random();
        }

        public bool Update(GameTime gameTime, OtherCharacter[] otherCharacters, MainCharacter m)
        {
            exhaustSpriteSheet.Update(gameTime);
            rifle.UpdateBullets(gameTime);
            invisibility.Update(gameTime);
            mines.Update(gameTime, otherCharacters, m);
            missiles.UpdateMissiles(gameTime);

            if (bashWallT > 0)
                bashWallT -= gameTime.ElapsedGameTime.Milliseconds;
            if (bashOtherT > 0)
                bashOtherT -= gameTime.ElapsedGameTime.Milliseconds;

            if (server)
                CheckAffect();
            if (health > maxHealth)
                health = maxHealth;

            audio.playMovement(Math.Abs(tankBody.LinearVelocity.Length()), tankBody.Position, parentGame.getMainCharacterPos(), characterIndex, invisibility.getAlpha());

            if (health <= 0)
            {
                //Console.WriteLine(StaticHelpers.getVolume(tankBody.Position, parentGame.getMainCharacterPos()));
                audio.playSound("death",
                    StaticHelpers.getVolume(tankBody.Position, parentGame.getMainCharacterPos()) * 0.3f,
                    (0.025f * characterIndex) - 0.15f,
                    StaticHelpers.getPan(tankBody.Position, parentGame.getMainCharacterPos()));
                return true;
            }
            else
                return false;
        }

        public bool On_Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (!(fixtureB.Body.affectPlayer == 0))
            {
                if (!((fixtureB.Body.affectPlayer / 100) == characterIndex))
                {
                    lastHit = fixtureB.Body.affectPlayer / 100;
                    affects.Add(fixtureB.Body.getAffect() % 100);
                }
            }
            else if (fixtureB.Body.IsStatic && bashWallT <= 0)
            {
                //Console.WriteLine(fixtureA.Body.LinearVelocity.Length());
                float vol = fixtureA.Body.LinearVelocity.Length() / 8f;
                if (vol > 1)
                    vol = 1;
                
                audio.playSound("bashWall",
                    vol * StaticHelpers.getVolume(tankBody.Position, parentGame.getMainCharacterPos()),
                    (vol * 0.6f) - 0.4f,
                    StaticHelpers.getPan(tankBody.Position, parentGame.getMainCharacterPos()));
                bashWallT = 400;
            }
            else if (fixtureB.Body.isTank && bashOtherT <= 0)
            {
                bashPlayer(fixtureA, fixtureB);
                bashOtherT = 250;
            }
            return true;
        }

        public void SetHealthEnergy(float health, float energy)
        {
            if (this.health != health)
            {
                damageIndicators.Add(new DamageIndicator(ConvertUnits.ToDisplayUnits(tankBody.Position) + new Vector2(rnd.Next(-40, 40), rnd.Next(-40, 40)),
                        spriteFont, this.health - health, characterIndex));
                this.health = health;
            }
            if (this.energy != energy)
            {
                this.energy = energy;
            }
        }

        public void CheckAffect()
        {
            if (superTough)
                defenceMultiplier += 5;
            for (int i = 0; i < affects.Count; i++)
            {
                switch (affects[i])
                {
                    case 1:
                        health -= 2f / defenceMultiplier;
                        damageIndicators.Add(new DamageIndicator(ConvertUnits.ToDisplayUnits(tankBody.Position) + new Vector2(rnd.Next(-40, 40), rnd.Next(-40, 40)),
                        spriteFont, ((2) / defenceMultiplier), characterIndex));
                        break;
                    case 2:
                        health -= 20f / defenceMultiplier;
                        damageIndicators.Add(new DamageIndicator(ConvertUnits.ToDisplayUnits(tankBody.Position) + new Vector2(rnd.Next(-40, 40), rnd.Next(-40, 40)),
                        spriteFont, ((20) / defenceMultiplier), characterIndex));
                        break;
                    case 3:
                        health -= 60f / defenceMultiplier;
                        damageIndicators.Add(new DamageIndicator(ConvertUnits.ToDisplayUnits(tankBody.Position) + new Vector2(rnd.Next(-40, 40), rnd.Next(-40, 40)),
                        spriteFont, ((60) / defenceMultiplier), characterIndex));
                        break;
                }
                affects.Remove(affects[i]);
            }
            if (superTough)
                defenceMultiplier -= 5;
        }

        public void TakeEnergy(int weaponIndex)
        {
            switch (weapon[weaponIndex])
            {
                case 1:
                    if (!freeRiflee)
                        energy -= eRifle;
                    break;
                case 2:
                    if (!freeInvisibilitye)
                        energy -= eInvisibility;
                    break;
                case 3:
                    if (!freeMinese)
                        energy -= eMines;
                    break;
                case 4:
                    if (!freeMissilese)
                        energy -= eMissiles;
                    break;
            }
        }

        public int getLastHit()
        {
            return lastHit;
        }

        public void setDefence(float defence)
        {
            this.defenceMultiplier += defence * 0.1f;
        }
        public float getDefence()
        {
            return defenceMultiplier;
        }

        protected void Draw(SpriteBatch spriteBatch)
        {
            rifle.Draw(spriteBatch);
            mines.Draw(spriteBatch);
            missiles.Draw(spriteBatch);

            if (!ultraStealth)
            {
                spriteBatch.Draw(tankTex,
                    ConvertUnits.ToDisplayUnits(tankBody.Position),
                    null,
                    Color.White * invisibility.getAlpha(),
                    tankBody.Rotation,
                    Vector2.Zero,
                    0.1f,
                    SpriteEffects.None,
                    0.11f);
            }
            else
            {
                spriteBatch.Draw(tankTex,
                    ConvertUnits.ToDisplayUnits(tankBody.Position),
                    null,
                    Color.White * invisibility.getMaxAlpha(),
                    tankBody.Rotation,
                    Vector2.Zero,
                    0.1f,
                    SpriteEffects.None,
                    0.11f);
            }
            if (drawExhaust)
            {
                float alpha;
                if (!ultraStealth)
                    alpha = invisibility.getAlpha();
                else
                    alpha = invisibility.getMaxAlpha();

                spriteBatch.Draw(exhaustSpriteSheet.getTex(),
                    (ConvertUnits.ToDisplayUnits(tankBody.Position) + StaticHelpers.RotateVector(new Vector2(10, 67), tankBody.Rotation)),
                    exhaustSpriteSheet.getDrawRect(),
                    Color.White * alpha,
                    tankBody.Rotation,
                    Vector2.Zero,
                    0.1f,
                    SpriteEffects.None,
                    0.12f);
                spriteBatch.Draw(exhaustSpriteSheet.getTex(),
                    (ConvertUnits.ToDisplayUnits(tankBody.Position) + StaticHelpers.RotateVector(new Vector2(21, 67), tankBody.Rotation)),
                    exhaustSpriteSheet.getDrawRect(),
                    Color.White * alpha,
                    tankBody.Rotation,
                    Vector2.Zero,
                    0.1f,
                    SpriteEffects.None,
                    0.12f);
            }

            for (int i = 0; i < damageIndicators.Count; i++)
            {
                if (damageIndicators[i].Draw(spriteBatch))
                {
                    damageIndicators.Remove(damageIndicators[i]);
                }
            }
        }

        public void DisposeBody()
        {
            rifle.Dispose();
            mines.Dispose();
            missiles.Dispose();
            parentGame.world.RemoveBody(tankBody);
        }

        public Vector2 getPos()
        {
            return tankBody.Position + ConvertUnits.ToSimUnits(StaticHelpers.RotateVector(tankOrigin, tankBody.Rotation));
        }
        public float getRotation()
        {
            return tankBody.Rotation;
        }
        public Vector2 getVelocity()
        {
            return tankBody.LinearVelocity;
        }
        public Rectangle getDisplayAABB()
        {
            return new Rectangle((int)(ConvertUnits.ToDisplayUnits(getPos()).X - ((tankTex.Height * 0.1f) / 2)),
                (int)(ConvertUnits.ToDisplayUnits(getPos()).Y - ((tankTex.Height * 0.1f) / 2)),
                (int)(tankTex.Height * 0.1f),
                (int)(tankTex.Height * 0.1f));
        }
        public Vector2[] getMinePos()
        {
            return mines.getMinePos();
        }
        public void setMines(float[] minesx, float[] minesy)
        {
            mines.setMines(minesx, minesy);
        }
        public void MineHit(int characterIndex)
        {
            affects.Add(2);
            lastHit = characterIndex;
        }

        public void superGun()
        {
            rifle.superGun();
        }
        public void freeInvisibility()
        {
            freeInvisibilitye = true;
        }
        public void freeMines()
        {
            freeMinese = true;
        }
        public void freeMissiles()
        {
            freeMissilese = true;
        }
        public void freeRifle()
        {
            freeRiflee = true;
        }
        public void TripleMines()
        {
            mines.TripleMines();
        }
        public void SuperTough()
        {
            superTough = true;
        }
        public void UltraStealth()
        {
            ultraStealth = true;
        }

        public bool[] getBuffs()
        {
            bool[] buffs = new bool[8];
            buffs[0] = rifle.getSuperGun();
            buffs[1] = freeInvisibilitye;
            buffs[2] = freeMinese;
            buffs[3] = freeMissilese;
            buffs[4] = freeRiflee;
            buffs[5] = mines.getTripleMines();
            buffs[6] = superTough;
            buffs[7] = ultraStealth;
            return buffs;
        }
        public void setBuffs(bool[] buffs)
        {
            if (buffs[0])
                superGun();
            else rifle.StopSuperGun();

            if (buffs[1])
                freeInvisibilitye = true;
            else freeInvisibilitye = false;

            if (buffs[2])
                freeMinese = true;
            else freeMinese = false;

            if (buffs[3])
                freeMissilese = true;
            else freeMissilese = false;

            if (buffs[4])
                freeRiflee = true;
            else freeRiflee = false;

            if (buffs[5])
                TripleMines();
            else mines.StopTripleMines();

            if (buffs[6])
                superTough = true;
            else superTough = false;

            if (buffs[7])
                ultraStealth = true;
            else ultraStealth = false;
        }

        protected abstract void bashPlayer(Fixture fixtureA, Fixture fixtureB);
    }
}
