using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace GameFinal.Misc
{
    class Audio
    {
        #region variables
        SoundEffect[] movement;
        SoundEffect rifle;
        SoundEffect bulletWall;
        SoundEffect bulletPlayer;
        SoundEffect bashWall;
        SoundEffect bashOther;
        SoundEffect buttonClick;
        //SoundEffect menuAmbiance;
        SoundEffect swishDown;
        SoundEffect swishUp;
        SoundEffect layMines;
        SoundEffect minesHit;
        SoundEffect death;
        SoundEffect selectWeapon;
        SoundEffect stealthIn;
        SoundEffect stealthOut;
        SoundEffect missiles;
        SoundEffect collectOrb;
        SoundEffect powerUp;
        float effectVolume = 1;
        Random rnd;
        int[] movTimers;
        SoundEffectInstance[] sei;
        int bulletSoundTimer = 0;
        int explosionTimer = 0;
        #endregion

        public Audio(SoundEffect[] movement, SoundEffect bashOther, SoundEffect bashWall, SoundEffect click, SoundEffect death, SoundEffect layMines,
            SoundEffect minesHit, SoundEffect rifle, SoundEffect swishDown, SoundEffect swishUp, SoundEffect bulletWall, SoundEffect bulletPlayer,
            SoundEffect missiles, SoundEffect stealthIn, SoundEffect stealthOut, SoundEffect collectOrb, SoundEffect powerUp, SoundEffect selectWeapon)
        {
            this.bashOther = bashOther;
            this.bashWall = bashWall;
            this.buttonClick = click;
            this.death = death;
            this.layMines = layMines;
            this.minesHit = minesHit;
            this.rifle = rifle;
            this.swishDown = swishDown;
            this.swishUp = swishUp;
            this.movTimers = new int[12];
            this.movement = movement;
            this.bulletWall = bulletWall;
            this.bulletPlayer = bulletPlayer;
            this.missiles = missiles;
            this.stealthIn = stealthIn;
            this.stealthOut = stealthOut;
            this.collectOrb = collectOrb;
            this.selectWeapon = selectWeapon;
            this.powerUp = powerUp;
            for (int i = 0; i < movTimers.Length; i++)
            {
                movTimers[i] = 0;
            }
            this.sei = new SoundEffectInstance[12];
            for (int i = 0; i < sei.Length; i++)
            {
                sei[i] = movement[0].CreateInstance();
            }
            rnd = new Random();
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < movTimers.Length; i++)
            {
                if (movTimers[i] > 0)
                {
                    movTimers[i] -= gameTime.ElapsedGameTime.Milliseconds;
                    if (movTimers[i] <= 0)
                        sei[i].Stop();
                }
            }
            if (bulletSoundTimer > 0)
                bulletSoundTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if (explosionTimer > 0)
                explosionTimer -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public void playSound(string name, float volume, float pitch, float pan)
        { //always multiply volume by effect volume.
            if (volume > 0)
            {
                switch (name)
                {
                    case "rifle":
                        rifle.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "bulletWall":
                        if (bulletSoundTimer <= 0)
                        {
                            bulletWall.Play(volume * effectVolume, pitch, pan);
                            bulletSoundTimer = 20;
                        }
                        break;
                    case "bulletPlayer":
                        if (bulletSoundTimer <= 0)
                        {
                            bulletPlayer.Play(volume * effectVolume, pitch, pan);
                            bulletSoundTimer = 20;
                        }
                        break;
                    case "bashWall":
                        bashWall.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "bashOther":
                        bashOther.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "click":
                        buttonClick.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "menuAmbiance":
                        break;
                    case "swishUp":
                        swishUp.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "swishDown":
                        swishDown.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "minesHit":
                        if (explosionTimer <= 0)
                        {
                            minesHit.Play(volume * effectVolume, pitch, pan);//
                            explosionTimer = 20;
                        }
                        break;
                    case "layMines":
                        layMines.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "death":
                        death.Play(volume * effectVolume, pitch, pan);//
                        break;
                    case "stealthIn":
                        stealthIn.Play(volume * effectVolume, pitch, pan);
                        break;
                    case "stealthOut":
                        stealthOut.Play(volume * effectVolume, pitch, pan);
                        break;
                    case "selectWeapon":
                        selectWeapon.Play(volume * effectVolume, pitch, pan);
                        break;
                    case "missiles":
                        missiles.Play(volume * effectVolume, pitch, pan);
                        break;
                    case "collectOrb":
                        collectOrb.Play(volume * effectVolume, pitch, pan);
                        break;
                    case "powerUp":
                        powerUp.Play(volume * effectVolume, pitch, pan);
                        break;
                }
            }
        }

        public void playSound(string name)
        {
            playSound(name, 1, 0, 0);
        }

        public void playMovement(float velocity, Vector2 pos, Vector2 centre, int charIndex, float alpha)
        {
            movTimers[charIndex] = 500;
            if (!(sei[charIndex].State == SoundState.Playing))
            {
                if (!sei[charIndex].IsLooped)
                    sei[charIndex].IsLooped = true;
                sei[charIndex].Play();
            }
            float pitch = (velocity / 7.31f) - 1f;
            if (pitch > 1)
                pitch = 1;
            if (pitch < -1)
                pitch = -1;

            if (alpha > 1)
                alpha = 1;
            else if (alpha < 0)
                alpha = 0;
            float vol = StaticHelpers.getVolume(pos, centre) * alpha;
            sei[charIndex].Volume = vol * effectVolume;
            sei[charIndex].Pan = StaticHelpers.getPan(pos, centre);
            sei[charIndex].Pitch = pitch;
        }

        public void setEffectVolume(float vol)
        {
            this.effectVolume = vol;
        }
        public float getEffectVolume()
        {
            return effectVolume;
        }
    }
}
