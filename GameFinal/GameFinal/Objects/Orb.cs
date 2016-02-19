using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameFinal.Misc;

namespace GameFinal.Objects
{
    class Orb
    {
        #region variables
        SpriteSheet orbSheet;
        float scale = 0f;
        float speed = 0.01f;
        bool destroy = false;
        bool dispose = false;
        private float _X { get; set; }
        private float _Y { get; set; }
        float defenceMultiplier { get; set; }
        int characterIndex { get; set; }
        bool isServer;
        GameInterface gameInterface;
        Audio audio;

        bool superGun { get; set; }
        bool freeInvisibility { get; set; }
        bool freeMines { get; set; }
        bool freeRifle { get; set; }
        bool freeMissiles { get; set; }
        bool tripleMines { get; set; }
        bool superTough { get; set; }
        bool ultraStealth { get; set; }
        #endregion

        public Orb(Texture2D orb, Vector2 pos, float defenceMultiplier, int characterIndex, GameInterface gameInterface, Audio audio)
        {
            Initialize(orb, pos, defenceMultiplier, gameInterface, true);
            this.characterIndex = characterIndex;
            this.audio = audio;

            Random rnd = new Random();
            switch (rnd.Next(0, 80))//0, 80
            {
                case 1:
                    superGun = true;
                    break;
                case 2:
                    freeInvisibility = true;
                    break;
                case 3:
                    freeMines = true;
                    break;
                case 4:
                    freeMissiles = true;
                    break;
                case 5:
                    freeRifle = true;
                    break;
                case 6:
                    tripleMines = true;
                    break;
                case 7:
                    superTough = true;
                    break;
                case 8:
                    ultraStealth = true;
                    break;
            }
        }
        public Orb(Texture2D orb, Vector2 pos, float defenceMultiplier, GameInterface gameInterface, Audio audio)
        {
            this.audio = audio;
            Initialize(orb, pos, defenceMultiplier, gameInterface, false);
        }

        private void Initialize(Texture2D orb, Vector2 pos, float defenceMultiplier, GameInterface gameInterface, bool isServer)
        {
            orbSheet = new SpriteSheet(orb, new Point(0, 0), new Point(200, 200), 24, new Point(5, 2));
            _X = pos.X;
            _Y = pos.Y;
            this.defenceMultiplier = defenceMultiplier;
            this.isServer = isServer;
            this.gameInterface = gameInterface;
            this.superGun = false;
            this.freeInvisibility = false;
            this.freeMines = false;
            this.freeRifle = false;
            this.freeMissiles = false;
            this.tripleMines = false;
            this.superTough = false;
            this.ultraStealth = false;
        }

        public bool Update(GameTime gameTime, MainCharacter mainCharacter, OtherCharacter[] otherCharacters, InGame parentGame)
        {
            if (!destroy)
            {
                Rectangle r = new Rectangle((int)(_X - (orbSheet.getOrigin().X * 0.2f)),
                    (int)(_Y - (orbSheet.getOrigin().Y * 0.2f)), (int)(orbSheet.getOrigin().X * 0.2f) * 2, (int)(orbSheet.getOrigin().Y * 0.2f) * 2);

                if (mainCharacter != null && mainCharacter.characterIndex != characterIndex && mainCharacter.getDisplayAABB().Intersects(r))
                {
                    if (superGun || freeRifle || freeInvisibility || freeMissiles || freeMines || tripleMines || superTough || ultraStealth)
                        audio.playSound("powerUp");
                    else
                        audio.playSound("collectOrb");


                    if (isServer)
                        mainCharacter.SetHealthEnergy(mainCharacter.health + (20f / mainCharacter.getDefence()), mainCharacter.maxEnergy);

                    mainCharacter.setDefence(defenceMultiplier);
                    if (superGun)
                    {
                        mainCharacter.superGun();
                        gameInterface.FlashMessage("Super Gun", 1);
                    }
                    else if (freeRifle)
                    {
                        mainCharacter.freeRifle();
                        gameInterface.FlashMessage("Free Rifle", 5);
                    }
                    else if (freeMissiles)
                    {
                        mainCharacter.freeMissiles();
                        gameInterface.FlashMessage("Free Missiles", 4);
                    }
                    else if (freeMines)
                    {
                        mainCharacter.freeMines();
                        gameInterface.FlashMessage("Free Mines", 3);
                    }
                    else if (freeInvisibility)
                    {
                        mainCharacter.freeInvisibility();
                        gameInterface.FlashMessage("Free Stealth", 2);
                    }
                    else if (tripleMines)
                    {
                        mainCharacter.TripleMines();
                        gameInterface.FlashMessage("Triple Mines", 6);
                    }
                    else if (superTough)
                    {
                        mainCharacter.SuperTough();
                        gameInterface.FlashMessage("Super Tank", 7);
                    }
                    else if (ultraStealth)
                    {
                        mainCharacter.UltraStealth();
                        gameInterface.FlashMessage("Ultra Stealth", 8);
                    }
                    destroy = true;
                }
                foreach (OtherCharacter o in otherCharacters)
                {
                    if (o != null && o.characterIndex != characterIndex && o.getDisplayAABB().Intersects(r))
                    {
                        if (superGun || freeRifle || freeInvisibility || freeMissiles || freeMines || tripleMines || superTough || ultraStealth)
                            audio.playSound("powerUp", StaticHelpers.getVolume(o.getPos(), parentGame.getMainCharacterPos()),
                                0,
                                StaticHelpers.getPan(o.getPos(), parentGame.getMainCharacterPos()));
                        else
                            audio.playSound("collectOrb", StaticHelpers.getVolume(o.getPos(), parentGame.getMainCharacterPos()),
                                0,
                                StaticHelpers.getPan(o.getPos(), parentGame.getMainCharacterPos()));

                        if (isServer)
                            o.SetHealthEnergy(o.health + (20f / o.getDefence()), o.maxEnergy);

                        o.setDefence(defenceMultiplier);
                        if (superGun)
                            o.superGun();
                        else if (freeRifle)
                            o.freeRifle();
                        else if (freeMissiles)
                            o.freeMissiles();
                        else if (freeMines)
                            o.freeMines();
                        else if (freeInvisibility)
                            o.freeInvisibility();
                        else if (tripleMines)
                            o.TripleMines();
                        else if (superTough)
                            o.SuperTough();
                        else if (ultraStealth)
                            o.UltraStealth();
                        destroy = true;
                    }
                }
            }

            orbSheet.Update(gameTime);
            if (destroy)
            {
                if (scale > 0)
                    scale -= speed;
                else dispose = true;
            }
            else if (scale < 0.2)
                scale += speed;

            return dispose;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(orbSheet.getTex(),
                new Vector2(_X, _Y),
                orbSheet.getDrawRect(),
                Color.White,
                0,
                orbSheet.getOrigin(),
                scale,
                SpriteEffects.None,
                0.112f);
        }
    }
}
