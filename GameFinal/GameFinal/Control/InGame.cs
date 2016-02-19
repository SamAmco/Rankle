using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.DemoBaseXNA;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization.Formatters.Binary;
using GameFinal.Control;
using GameFinal.Display;
//using System.Windows.Forms;
using GameFinal.Objects;
using GameFinal.Misc;
using GameFinal.Menu_s;
using System.Windows.Forms;

namespace GameFinal
{
    abstract class InGame
    {
        #region Local Variables
        //Other characters
        protected OtherCharacter[] otherPlayers = new OtherCharacter[8];
        KeyboardState keyState;
        Camera2D cam;
        protected string message = "";
        protected MainCharacter mainCharacter;
        protected int thisPlayerIndex = 0;
        SpriteFont spriteFont;
        public World world = new World(Vector2.Zero);
        public bool readyForUpdate = false;
        TileMap tileMap;
        Point canvasSize;
        Point cameraPosition = new Point(0, 0);
        GraphicsDevice GraphicsDevice;
        protected Game1 game1;
        bool server;
        protected GameInterface gameInterface;
        protected ExplosionGenerator explosionGenerator;
        protected KeepScore keepScore;
        protected Vector2 temporaryCamPos;
        protected List<Vector2> respawnPoints;
        protected List<Orb> orbList;
        List<int> pendingRespawn;
        int tankSkin;
        protected string playerName;
        float cameraZoom;
        protected Audio audio;

        protected int[] characterTankSkins;
        protected string[] characterNames;

        KeyboardState prevKeyState;
        List<string> intercomList;
        GameFinal.Menu_s.TextBox inputBox;
        bool typing = false;
        Texture2D black;
        #endregion

        #region Content Variables
        int[] weapon;
        Texture2D[] weaponTexs;
        Texture2D[] gameInterfaceTexs;
        Texture2D[] tankTexs;
        Texture2D[] explosionTexs;
        Texture2D red;
        protected Texture2D orb;
        List<Texture2D> tileTexList;
        int[,] tileArray;
        #endregion

        public InGame(Game1 game1, GraphicsDevice GraphicsDevice, Texture2D white, bool server,
            Texture2D[] tankTexs, SpriteFont spriteFont, List<Texture2D> tileTexList, Texture2D[] gameInterfaceTexs,
            Texture2D[] weaponTexs, Texture2D[] explosionTexs, int[] weapon, int tankSkin, string playerName, Texture2D orb,
            float cameraZoom, Audio audio, Texture2D black)
        {
            this.tankTexs = tankTexs;
            this.tileTexList = tileTexList;
            this.gameInterfaceTexs = gameInterfaceTexs;
            this.weaponTexs = weaponTexs;
            this.weapon = weapon;
            this.GraphicsDevice = GraphicsDevice;
            this.game1 = game1;
            this.spriteFont = spriteFont;
            this.server = server;
            this.explosionTexs = explosionTexs;
            this.tankSkin = tankSkin;
            this.red = white;
            characterTankSkins = new int[8];
            characterNames = new string[8];
            this.playerName = playerName;
            this.orb = orb;
            this.cameraZoom = cameraZoom;
            this.audio = audio;
            this.intercomList = new List<string>();
            this.black = black;
            prevKeyState = Keyboard.GetState();
            keepScore = new KeepScore(red, spriteFont, game1.Window.ClientBounds);
        }

        public void Initialize(int mapNumber, Character ch)
        {
            #region Load Map
            try
            {
                System.IO.Stream stream = TitleContainer.OpenStream(@"Maps/" + mapNumber + ".map");
                BinaryFormatter bf = new BinaryFormatter();
                tileArray = (int[,])bf.Deserialize(stream);
                stream.Close();
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show("Map not found:\n " + e.ToString());
            }
            #endregion
            thisPlayerIndex = ch.characterIndex;
            tileMap = new TileMap(Vector2.Zero, tileTexList, tileArray, world);
            canvasSize = tileMap.canvasSize;
            cam = new Camera2D(new Vector2(GetWindow().Width, GetWindow().Height),
                new Vector2(canvasSize.X, canvasSize.Y), cameraZoom);
            orbList = new List<Orb>();
            readyForUpdate = true;

            pendingRespawn = new List<int>();
            respawnPoints = tileMap.getRespawnPoints();
            //respawnPoints.Add(new Vector2(100, 1000));
            //respawnPoints.Add(new Vector2(200, 100));
            NewMainCharacter();
            //mainCharacter = new MainCharacter(tankTexs,
            //    ConvertUnits.ToSimUnits(respawnPoints[0]), this, server, cam, gameInterfaceTexs, spriteFont, weapon, ch.characterIndex, tankSkin);
            gameInterface = new GameInterface(gameInterfaceTexs, GetWindow(), mainCharacter.tankBody, mainCharacter, weaponTexs, spriteFont, audio);
            
            
            //keepScore.AddPlayer(ch.characterIndex, playerName);
            //for (int i = 0; i < characterNames.Length; i++)
            //{
            //    if(characterNames[i] != null)
            //        keepScore.AddPlayer(i, characterNames[i]);
            //}
            
            //NewCharacter(1);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter)
                && prevKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                if (!typing)
                {
                    inputBox = new GameFinal.Menu_s.TextBox(spriteFont, new Vector2(30, GetWindow().Height - 30), new Vector2(30, GetWindow().Height - 30),
                        black, "", 1, Color.White, 50, false, true, audio);
                    if (mainCharacter != null)
                        mainCharacter.setTyping(true);
                    typing = true;
                }
                else if (typing)
                {
                    if (inputBox.getText() != "")
                    {
                        string s = playerName + "::    " + inputBox.getText();
                        PushMessage(s);
                        appendIntercomMessage(s);
                    }
                    if (mainCharacter != null)
                        mainCharacter.setTyping(false);
                    inputBox = null;
                    typing = false;
                }
            }
            if (inputBox != null)
            {
                inputBox.Update(gameTime);
            }

            keyState = Keyboard.GetState();
            for (int i = 0; i < orbList.Count; i++)
            {
                if (orbList[i].Update(gameTime, mainCharacter, otherPlayers, this))
                {
                    orbList.Remove(orbList[i]);
                }
            }

            if (mainCharacter != null && mainCharacter.energy <= 2 && orbList.Count <= 0)
            {
                bool empty = true;
                foreach (OtherCharacter o in otherPlayers)
                {
                    if (o != null && !(o.energy <= 2))
                        empty = false;
                }
                if (empty)
                {
                    foreach (Vector2 position in respawnPoints)
                    {
                        if (SpawnPointClear(position))
                        {
                            PlaceOrb(position, 0, 1000);
                        }
                    }
                }
            }

            if (mainCharacter != null)
                cam.Pos = ConvertUnits.ToDisplayUnits(mainCharacter.getPos());
            else
                cam.Pos = temporaryCamPos;

            cam.Update();
            for (int i = 0; i < otherPlayers.Length; i++)
            {
                if (otherPlayers[i] != null)
                {
                    if (otherPlayers[i].Update(gameTime, cam, otherPlayers, mainCharacter))
                    {
                        explosionGenerator.CreateExplosion(ConvertUnits.ToDisplayUnits(otherPlayers[i].getPos()), 0);
                        PlayerDied(otherPlayers[i].characterIndex);
                        PlaceOrb(ConvertUnits.ToDisplayUnits(otherPlayers[i].getPos()),
                            keepScore.getKD(otherPlayers[i].characterIndex), otherPlayers[i].characterIndex);
                        keepScore.Death(otherPlayers[i].characterIndex);
                        keepScore.Kill(otherPlayers[i].getLastHit());
                        if (otherPlayers[i].getLastHit() == thisPlayerIndex)
                            gameInterface.FlashMessage("You Killed:\n" + characterNames[otherPlayers[i].characterIndex]);
                        otherPlayers[i].DisposeBody();
                        otherPlayers[i] = null;
                    }
                }
            }
            if (mainCharacter != null)
            {
                gameInterface.Update(gameTime);
                if (mainCharacter.Update(gameTime, cam, otherPlayers))
                {
                    explosionGenerator.CreateExplosion(ConvertUnits.ToDisplayUnits(mainCharacter.getPos()), 0);
                    temporaryCamPos = ConvertUnits.ToDisplayUnits(mainCharacter.getPos());
                    PlaceOrb(ConvertUnits.ToDisplayUnits(mainCharacter.getPos()), keepScore.getKD(thisPlayerIndex), thisPlayerIndex);
                    mainCharacter.DisposeBody();
                    keepScore.Kill(mainCharacter.getLastHit());
                    mainCharacter = null;
                    gameInterface = null;
                    keepScore.Death(thisPlayerIndex);
                }
            }
            try
            {
                for(int i = 0; i < pendingRespawn.Count; i++)
                {
                    NewCharacter(pendingRespawn[i], characterTankSkins[pendingRespawn[i]], characterNames[i], false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            prevKeyState = Keyboard.GetState();
            explosionGenerator.Update(gameTime);
            world.Step(gameTime.ElapsedGameTime.Milliseconds * .001f);
        }

        protected void NewMainCharacter(Vector2 position)
        {
            if (!keepScore.samePlayer(playerName, thisPlayerIndex))
                keepScore.AddPlayer(thisPlayerIndex, playerName);
            mainCharacter = new MainCharacter(tankTexs,
                position, this, server, cam, gameInterfaceTexs, spriteFont, weapon, thisPlayerIndex, tankSkin, audio);
            gameInterface = new GameInterface(gameInterfaceTexs, GetWindow(), mainCharacter.tankBody, mainCharacter, weaponTexs, spriteFont, audio);
        }

        protected bool NewMainCharacter()
        {
            respawnPoints = StaticHelpers.ShuffleVectors(respawnPoints);
            foreach (Vector2 position in respawnPoints)
            {
                if (SpawnPointClear(position))
                {
                    NewMainCharacter(ConvertUnits.ToSimUnits(position));
                    //mainCharacter = new MainCharacter(tankTexs,
                    //    ConvertUnits.ToSimUnits(position), this, server, cam, gameInterfaceTexs, spriteFont, weapon, thisPlayerIndex, tankSkin);
                    //gameInterface = new GameInterface(gameInterfaceTexs, GetWindow(), cam, mainCharacter.tankBody, mainCharacter, weaponTexs, spriteFont);
                    return true;
                }
            }
            return false;
        }

        protected void NewCharacter(int index, Vector2 position, int skin, string name)
        {
            if (!keepScore.samePlayer(name, index))
                keepScore.AddPlayer(index, name);
            otherPlayers[index] = new OtherCharacter(tankTexs,
                        position, this, server, gameInterfaceTexs, spriteFont, weapon, index, skin, name, audio);
        }

        protected void NewCharacter(int index, int skin, string name, bool newCh)
        {
            bool success = false;
            respawnPoints = StaticHelpers.ShuffleVectors(respawnPoints);
            foreach (Vector2 position in respawnPoints)
            {
                if(SpawnPointClear(position))
                {
                    NewCharacter(index, ConvertUnits.ToSimUnits(position), skin, name);
                    //otherPlayers[index] = new OtherCharacter(tankTexs,
                    //    ConvertUnits.ToSimUnits(position), this, server, gameInterfaceTexs, spriteFont, weapon, index, skin, name);
                    if (newCh)
                        PlayerEntered(ConvertUnits.ToSimUnits(position), index, name);
                    success = true; 
                    break;
                }
                else
                    success = false;
            }

            #region organise pending list
            if (success)
            {
                for (int i = 0; i < pendingRespawn.Count; i++)
                {
                    if (pendingRespawn[i] == index)
                    {
                        pendingRespawn.Remove(pendingRespawn[i]);
                        break;
                    }
                }
            }
            if (!success)
            {
                bool isPending = false;
                for (int i = 0; i < pendingRespawn.Count; i++)
                {
                    if (pendingRespawn[i] == index)
                    {
                        isPending = true;
                        break;
                    }
                }
                if (!isPending)
                    pendingRespawn.Add(index);
            }
            #endregion
        }

        private bool SpawnPointClear(Vector2 spawnPoint)
        {
            bool isClear = true;
            foreach (OtherCharacter o in otherPlayers)
            {
                if (o != null)
                {
                    if ((ConvertUnits.ToDisplayUnits(o.getPos()) - spawnPoint).Length() < 150)
                        isClear = false;
                }
            }
            if (mainCharacter != null)
            {
                if ((ConvertUnits.ToDisplayUnits(mainCharacter.getPos()) - spawnPoint).Length() < 150)
                    isClear = false;
            }
            return isClear;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation(GraphicsDevice));

            foreach (OtherCharacter m in otherPlayers)
            {
                if (m != null)
                    m.Draw(gameTime, spriteBatch);
            }
            tileMap.Draw(spriteBatch);

            if (mainCharacter != null)
                mainCharacter.Draw(gameTime, spriteBatch);

            explosionGenerator.Draw(spriteBatch);

            foreach (Orb o in orbList)
            {
                o.Draw(spriteBatch);
            }
            spriteBatch.DrawString(spriteFont, message, Vector2.Zero + cam.TLCorner, Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            message = "";
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend);

            if (gameInterface != null)
                gameInterface.Draw(gameTime, spriteBatch, mainCharacter.health, mainCharacter.getDefence(), mainCharacter.maxHealth,
                    mainCharacter.energy, mainCharacter.maxEnergy, otherPlayers, keepScore, intercomList);

            //foreach (string s in characterNames)
            //{
            //    message += s + ", ";
            //}

            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Tab) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.X))
                keepScore.Draw(spriteBatch, cam.TLCorner);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend);

            if (inputBox != null)
                inputBox.Draw(spriteBatch);
        }

        public Rectangle GetWindow()
        {
            return game1.Window.ClientBounds;
        }

        protected void appendIntercomMessage(string s)
        {
            intercomList.Add(s);
            if (intercomList.Count > 5)
            {
                intercomList.RemoveAt(0);
            }
        }

        /// <summary>
        /// returns sim units
        /// </summary>
        /// <returns></returns>
        public Vector2 getMainCharacterPos()
        {
            if (mainCharacter != null)
                return mainCharacter.getPos();
            else return ConvertUnits.ToSimUnits(temporaryCamPos);
        }

        public ExplosionGenerator getExplosionGenerator()
        {
            return explosionGenerator;
        }

        protected abstract void PlayerEntered(Vector2 position, int index, string name);

        protected abstract void PlayerDied(int index);

        protected abstract void PlaceOrb(Vector2 position, float kD, int characterIndex);

        protected abstract void PushMessage(string message);

        public abstract void EndGame();
    }

    enum PacketTypes
    {
        LOGIN,
        MOVE,
        WORLDSTATE,
        REMOVE,
        SCORE, 
        ORB,
        MESSAGE
    }
}
