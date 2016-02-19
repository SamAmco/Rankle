using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameFinal.Misc;

namespace GameFinal.Weapons
{
    class Invisibility
    {
        #region variables
        int timer = 0;
        float alpha = 1;
        float requiredEnergy = 0;
        bool isMain;
        Audio audio;
        InGame parentGame;
        GameCharacter shooter;
        #endregion

        public Invisibility(float requiredEnergy, bool isMain, Audio audio, InGame parentGame, GameCharacter shooter)
        {
            this.requiredEnergy = requiredEnergy;
            this.isMain = isMain;
            this.audio = audio;
            this.parentGame = parentGame;
            this.shooter = shooter;
        }

        public bool FireWeapon(bool canQue, float energy)
        {
            if ((timer <= 0 || canQue) && energy >= requiredEnergy)
            {
                if (timer <= 100 && canQue)
                    timer += 500;

                timer += 500;
                return true;
            }
            else return false;
        }

        public void Update(GameTime gameTime)
        {
            if (timer <= 0 && alpha < 1)
            {
                if (alpha == 0)
                    audio.playSound("stealthOut",
                        StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                        -0.5f,
                        StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));

                alpha += gameTime.ElapsedGameTime.Milliseconds * 0.002f;
                if (alpha >= 1)
                {
                    alpha = 1;
                }
            }
            else if (timer > 0)
            {
                timer -= gameTime.ElapsedGameTime.Milliseconds;
                if (alpha > 0)
                {
                    if (alpha == 1)
                        audio.playSound("stealthIn",
                            StaticHelpers.getVolume(shooter.getPos(), parentGame.getMainCharacterPos()),
                            -0.5f,
                            StaticHelpers.getPan(shooter.getPos(), parentGame.getMainCharacterPos()));

                    alpha -= gameTime.ElapsedGameTime.Milliseconds * 0.002f;
                    if (alpha <= 0)
                    {
                        alpha = 0;
                    }
                }
            }
        }

        public float getAlpha()
        {
            if (isMain)
                return alpha + 0.4f;
            else return alpha;
        }
        public float getMaxAlpha()
        {
            if (isMain)
                return 0.4f;
            else return 0;
        }
    }
}
