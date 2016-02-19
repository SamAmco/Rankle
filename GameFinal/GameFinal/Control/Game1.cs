using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.Serialization.Formatters.Binary;
using GameFinal.Menu_s;
using System.Threading;
using GameFinal.Control;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using GameFinal.Misc;

namespace GameFinal
{
    /// <summary>
    /// Menu screens
    /// sound
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        enum GameState
        {
            MainMenu,
            InGame
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int failedMessageTimer = 0;

        GameState gameState;

        InGame inGame;
        GameFinal.Menu_s.MainMenu mainMenu;
        WhiteWhipe whiteWhipe;
        Audio audio;

        float cameraZoom;

        KeyboardState prevKeyState;
        GamePadState prevGamePadState;

        #region InGame Content
        Texture2D[] gameInterfaceTexs;
        Texture2D[] weaponTexs = new Texture2D[5];
        Texture2D[] tankTexs;
        Texture2D[] explosionTexs;
        Texture2D orb;
        Texture2D black;
        int[] weapon = new int[4];
        List<Texture2D> tileTexList = new List<Texture2D>();
        List<string> skinNames;
        SpriteFont spriteFont;
        #endregion
        #region MenuContent
        Texture2D[] mainMenuTexs;
        Texture2D[] whiteWhipeTexs;
        GameOptions gameOptions;
        #endregion

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            gameState = GameState.MainMenu;
            prevKeyState = Keyboard.GetState();
            prevGamePadState = GamePad.GetState(PlayerIndex.One);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(@"Font");

            #region load audio
            SoundEffect bashOther = Content.Load<SoundEffect>(@"Audio/BashOther");
            SoundEffect bashWall = Content.Load<SoundEffect>(@"Audio/BashWall");
            SoundEffect click = Content.Load<SoundEffect>(@"Audio/Click");
            SoundEffect death = Content.Load<SoundEffect>(@"Audio/Death0");
            SoundEffect layMines = Content.Load<SoundEffect>(@"Audio/LayMines");
            SoundEffect minesHit = Content.Load<SoundEffect>(@"Audio/MinesHit0");
            SoundEffect rifle = Content.Load<SoundEffect>(@"Audio/Rifle");
            SoundEffect swishDown = Content.Load<SoundEffect>(@"Audio/SwishDown");
            SoundEffect swishUp = Content.Load<SoundEffect>(@"Audio/SwishUp");
            SoundEffect bulletWall = Content.Load<SoundEffect>(@"Audio/BulletWall");
            SoundEffect bulletPlayer = Content.Load<SoundEffect>(@"Audio/BulletPlayer");
            SoundEffect missiles = Content.Load<SoundEffect>(@"Audio/Missiles");
            SoundEffect stealthIn = Content.Load<SoundEffect>(@"Audio/StealthIn");
            SoundEffect stealthOut = Content.Load<SoundEffect>(@"Audio/StealthOut");
            SoundEffect collectOrb = Content.Load<SoundEffect>(@"Audio/CollectOrb");
            SoundEffect powerUp = Content.Load<SoundEffect>(@"Audio/GainPowerUp");
            SoundEffect selectWeapon = Content.Load<SoundEffect>(@"Audio/SelectWeapon");
            SoundEffect[] movement = new SoundEffect[1];
            movement[0] = Content.Load<SoundEffect>(@"Audio/Engine/10");
            //movement[1] = Content.Load<SoundEffect>(@"Audio/Engine/1");
            //movement[2] = Content.Load<SoundEffect>(@"Audio/Engine/2");
            //movement[3] = Content.Load<SoundEffect>(@"Audio/Engine/3");
            //movement[4] = Content.Load<SoundEffect>(@"Audio/Engine/4");
            //movement[5] = Content.Load<SoundEffect>(@"Audio/Engine/5");
            //movement[6] = Content.Load<SoundEffect>(@"Audio/Engine/6");
            //movement[7] = Content.Load<SoundEffect>(@"Audio/Engine/7");
            //movement[8] = Content.Load<SoundEffect>(@"Audio/Engine/8");
            //movement[9] = Content.Load<SoundEffect>(@"Audio/Engine/9");
            audio = new Audio(movement, bashOther, bashWall, click, death, layMines, minesHit,
                rifle, swishDown, swishUp, bulletWall, bulletPlayer, missiles, stealthIn, stealthOut,
                collectOrb, powerUp, selectWeapon);
            #endregion

            #region CreateMapTextures
            bool fileFound = true;
            int m = 1;
            List<Texture2D> foundMaps = new List<Texture2D>();
            while (fileFound)
            {
                try
                {
                    System.IO.Stream stream = TitleContainer.OpenStream(@"Maps/" + m + ".map");
                    BinaryFormatter bf = new BinaryFormatter();
                    foundMaps.Add(texFromMap((int[,])bf.Deserialize(stream), new Vector2(1000, 1000), true));
                    stream.Close();
                    m++;
                }
                catch 
                {
                    fileFound = false;
                }
            }
            #endregion

            #region Load Textures
            mainMenuTexs = new Texture2D[18 + foundMaps.Count];
            mainMenuTexs[0] = Content.Load<Texture2D>(@"Menu's\ButtonLeftEnd");
            mainMenuTexs[1] = Content.Load<Texture2D>(@"Menu's\ButtonMiddle");
            mainMenuTexs[2] = Content.Load<Texture2D>(@"Menu's\ButtonRightEnd");
            mainMenuTexs[3] = Content.Load<Texture2D>(@"Menu's\CheckBoxU");
            mainMenuTexs[4] = Content.Load<Texture2D>(@"Menu's\CheckBoxC");
            mainMenuTexs[5] = Content.Load<Texture2D>(@"Menu's\640by480");
            mainMenuTexs[6] = Content.Load<Texture2D>(@"Menu's\720by480");
            mainMenuTexs[7] = Content.Load<Texture2D>(@"Menu's\720by576");
            mainMenuTexs[8] = Content.Load<Texture2D>(@"Menu's\800by600");
            mainMenuTexs[9] = Content.Load<Texture2D>(@"Menu's\1024by768");
            mainMenuTexs[10] = Content.Load<Texture2D>(@"Menu's\1152by864");
            mainMenuTexs[11] = Content.Load<Texture2D>(@"Menu's\1280by720");
            mainMenuTexs[12] = Content.Load<Texture2D>(@"Menu's\1280by768");
            mainMenuTexs[13] = Content.Load<Texture2D>(@"Menu's\1280by800");
            mainMenuTexs[14] = Content.Load<Texture2D>(@"Menu's\1366by768");
            mainMenuTexs[15] = Content.Load<Texture2D>(@"Menu's\1600by900");
            mainMenuTexs[16] = Content.Load<Texture2D>(@"Menu's\1920by1080");
            mainMenuTexs[17] = Content.Load<Texture2D>(@"Menu's\SliderBar");
            int x = 18;
            foreach (Texture2D t in foundMaps)
            {
                mainMenuTexs[x] = t;
                x++;
            }


            whiteWhipeTexs = new Texture2D[2];
            whiteWhipeTexs[0] = Content.Load<Texture2D>(@"Menu's\White");
            this.black = whiteWhipeTexs[0];
            whiteWhipeTexs[1] = Content.Load<Texture2D>(@"Menu's\WhiteWhipe");

            Texture2D texture;
            texture = Content.Load<Texture2D>(@"Tiles\Back");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\Block");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\EdgeBottom");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\EdgeLeft");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\EdgeTop");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\EdgeRight");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\CornerBL");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\CornerBR");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\CornerTL");
            tileTexList.Add(texture);
            texture = Content.Load<Texture2D>(@"Tiles\CornerTR");
            tileTexList.Add(texture);

            tankTexs = new Texture2D[17];
            tankTexs[0] = Content.Load<Texture2D>(@"Tanks\Baby");
            tankTexs[1] = Content.Load<Texture2D>(@"Tanks\Camo");
            tankTexs[2] = Content.Load<Texture2D>(@"Tanks\Carbon");
            tankTexs[3] = Content.Load<Texture2D>(@"Tanks\Dragon");
            tankTexs[4] = Content.Load<Texture2D>(@"Tanks\Funky");
            tankTexs[5] = Content.Load<Texture2D>(@"Tanks\Hunk");
            tankTexs[6] = Content.Load<Texture2D>(@"Tanks\Ignite");
            tankTexs[7] = Content.Load<Texture2D>(@"Tanks\Origin");
            tankTexs[8] = Content.Load<Texture2D>(@"Tanks\Polka");
            tankTexs[9] = Content.Load<Texture2D>(@"Tanks\Razor");
            tankTexs[10] = Content.Load<Texture2D>(@"Tanks\Slash");
            tankTexs[11] = Content.Load<Texture2D>(@"Tanks\Tron");
            tankTexs[12] = Content.Load<Texture2D>(@"Exhaust");
            tankTexs[13] = Content.Load<Texture2D>(@"Bullet");
            tankTexs[14] = Content.Load<Texture2D>(@"Mine");
            tankTexs[15] = Content.Load<Texture2D>(@"Missile");
            tankTexs[16] = Content.Load<Texture2D>(@"GameInterface\Style2\Sight");


            skinNames = new List<string>();
            skinNames.Add("Baby");
            skinNames.Add("Camo");
            skinNames.Add("Carbon");
            skinNames.Add("Dragon");
            skinNames.Add("Funky");
            skinNames.Add("Hunk");
            skinNames.Add("Ignite");
            skinNames.Add("Origin");
            skinNames.Add("Polka");
            skinNames.Add("Razor");
            skinNames.Add("Slash");
            skinNames.Add("Tron");

            gameInterfaceTexs = new Texture2D[14];
            /*gameInterfaceTexs[0] = Content.Load<Texture2D>(@"GameInterface\HealthBar2");
            gameInterfaceTexs[1] = Content.Load<Texture2D>(@"GameInterface\Health");
            gameInterfaceTexs[2] = Content.Load<Texture2D>(@"GameInterface\HealthEnd");
            gameInterfaceTexs[3] = Content.Load<Texture2D>(@"GameInterface\Energy");
            gameInterfaceTexs[4] = Content.Load<Texture2D>(@"GameInterface\EnergyEnd");*/
            gameInterfaceTexs[5] = Content.Load<Texture2D>(@"GameInterface\EmptyButton");
            gameInterfaceTexs[0] = Content.Load<Texture2D>(@"GameInterface\Style2\EmptyBar");
            gameInterfaceTexs[1] = Content.Load<Texture2D>(@"GameInterface\Style2\EnergyCentre");
            gameInterfaceTexs[2] = Content.Load<Texture2D>(@"GameInterface\Style2\EnergyEnd");
            gameInterfaceTexs[3] = Content.Load<Texture2D>(@"GameInterface\Style2\HealthCentre");
            gameInterfaceTexs[4] = Content.Load<Texture2D>(@"GameInterface\Style2\HealthEnd");
            gameInterfaceTexs[6] = Content.Load<Texture2D>(@"GameInterface\Buffs\SuperGun");
            gameInterfaceTexs[7] = Content.Load<Texture2D>(@"GameInterface\Buffs\FreeInvisibility");
            gameInterfaceTexs[8] = Content.Load<Texture2D>(@"GameInterface\Buffs\FreeMines");
            gameInterfaceTexs[9] = Content.Load<Texture2D>(@"GameInterface\Buffs\FreeMissiles");
            gameInterfaceTexs[10] = Content.Load<Texture2D>(@"GameInterface\Buffs\FreeRifle");
            gameInterfaceTexs[11] = Content.Load<Texture2D>(@"GameInterface\Buffs\TripleMines");
            gameInterfaceTexs[12] = Content.Load<Texture2D>(@"GameInterface\Buffs\SuperTough");
            gameInterfaceTexs[13] = Content.Load<Texture2D>(@"GameInterface\Buffs\UltraStealth");

            weaponTexs[0] = Content.Load<Texture2D>(@"GameInterface\Weapons\NoWeapon");
            weaponTexs[1] = Content.Load<Texture2D>(@"GameInterface\Weapons\Rifle");
            weaponTexs[2] = Content.Load<Texture2D>(@"GameInterface\Weapons\Invisibility");
            weaponTexs[3] = Content.Load<Texture2D>(@"GameInterface\Weapons\Mines");
            weaponTexs[4] = Content.Load<Texture2D>(@"GameInterface\Weapons\Missiles");

            explosionTexs = new Texture2D[4];
            explosionTexs[0] = Content.Load<Texture2D>(@"Explosions\Explosion1");
            explosionTexs[1] = Content.Load<Texture2D>(@"Explosions\Explosion2");
            explosionTexs[2] = Content.Load<Texture2D>(@"Explosions\Explosion3");
            explosionTexs[3] = Content.Load<Texture2D>(@"Explosions\Explosion4");

            orb = Content.Load<Texture2D>(@"Orb");

            weapon[0] = 1;
            weapon[1] = 2;
            weapon[2] = 3;
            weapon[3] = 4;
            #endregion

            #region initializeGameOptions
            //OptionsForm optionsForm = new OptionsForm(defaultIP, tankTexs.ToList(), skinNames);
            //optionsForm.ShowDialog();
            //if (optionsForm.DialogResult != System.Windows.Forms.DialogResult.OK)
            //{
            //    this.Exit();
            //    return;
            //}

            //gameOptions = new GameOptions(optionsForm.mapNumber, optionsForm.IP_Adress, optionsForm.skinNumber, optionsForm.playerName,
            //    optionsForm.preffered_Width, optionsForm.preffered_Height, optionsForm.isServer, optionsForm.is_Full_Screen); //Worth Noting that you initialize all but the cameraZoom here

            //optionsForm.Dispose();
            gameOptions = loadGameOptions();
            gameOptions.map_Number--;
            #endregion

            #region graphics options
            graphics.PreferredBackBufferHeight = gameOptions.preferred_Height;
            graphics.PreferredBackBufferWidth = gameOptions.preferred_Width;
            //float ideal = 1280 * 800;
            //float c1 = (float)optionsForm.preffered_Height / 800f;
            //float c2 = (float)optionsForm.preffered_Width / 1280f;
            //float f = (float)(optionsForm.preffered_Width * optionsForm.preffered_Height);
            //if (f <= ideal)
            //    cameraZoom = 1f - (float)((f / ideal) * 1.2f);
            //else
                //cameraZoom = 0.6f + (float)((f / ideal) * 0.4f);

            setCameraZoom(gameOptions.preferred_Height);

            if (gameOptions.is_Full_Screen)
                graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            #endregion

            mainMenu = new Menu_s.MainMenu(mainMenuTexs, Window.ClientBounds, GraphicsDevice, tankTexs,
                spriteFont, tileTexList, cameraZoom, gameOptions, whiteWhipeTexs[0], audio);
        }

        private GameOptions loadGameOptions()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            StorageDevice device = StorageDevice.EndShowSelector(result);
            IAsyncResult result2 = device.BeginOpenContainer("RankleOptions", null, null);
            StorageContainer container = device.EndOpenContainer(result2);

            // Check to see if the save exists
            if (!container.FileExists("options.sav"))
            {
                container.Dispose();
                return new GameOptions();
            }
            else
            {
                // Open the file
                Stream stream = container.OpenFile("options.sav", FileMode.Open);
                try
                {
                    // Read the data from the file
                    XmlSerializer serializer = new XmlSerializer(typeof(GameOptions));
                    GameOptions data = (GameOptions)serializer.Deserialize(stream);
                    stream.Close();
                    container.Dispose();
                    return data;
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
                // Close the file
                stream.Close();
                container.Dispose();
            }
            // Dispose the container
            container.Dispose();
            return new GameOptions();
        }

        private void setCameraZoom(int height)
        {
            switch (height)
            {
                case 480:
                    cameraZoom = 0.64f;
                    break;
                case 576:
                    cameraZoom = 0.77f;
                    break;
                case 600:
                    cameraZoom = 0.8f;
                    break;
                case 720:
                    cameraZoom = 0.96f;
                    break;
                case 768:
                    cameraZoom = 1.02f;
                    break;
                case 800:
                    cameraZoom = 1.066f;
                    break;
                case 864:
                    cameraZoom = 1.15f;
                    break;
                case 900:
                    cameraZoom = 1.2f;
                    break;
                case 1080:
                    cameraZoom = 1.44f;
                    break;
            }
        }

        private Texture2D texFromMap(int[,] mapData, Vector2 size, bool nearestNabour)
        {
            if (!nearestNabour)
            {
                int width = mapData.GetLength(1);
                int height = mapData.GetLength(0);
                int xOffset = 0;//((int)size.X - width) / 2;
                if (xOffset < 0)
                    xOffset = 0;
                Color[] pixels = new Color[(width + (2 * xOffset)) * height];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = Color.Black;
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (mapData[y, x] == 0)
                        {
                            pixels[(y * (width + (2 * xOffset))) + (x + xOffset)] = new Color(255, 255, 255, 255);
                        }
                    }

                }

                Texture2D mapTex = new Texture2D(
                    GraphicsDevice,
                    width,
                    height,
                    false,
                    SurfaceFormat.Color);

                mapTex.SetData<Color>(pixels, 0, pixels.Length);

                return mapTex;
            }
            else
            {
                int width = mapData.GetLength(1);
                int height = mapData.GetLength(0);
                int xSpread = (int)(size.X / width);
                int ySpread = (int)(size.Y / height);
                Color[] pixels = new Color[(int)(size.X * size.Y)];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = new Color(150, 150, 150);
                }

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (mapData[y, x] == 1)
                        {
                            for (int xs = 0; xs < xSpread; xs++)
                            {
                                for (int ys = 0; ys < ySpread; ys++)
                                {
                                    pixels[(((y * ySpread) + ys) * (width * xSpread)) + (x * xSpread) + xs] = new Color(0, 0, 0, 255);
                                }
                            }

                        }
                    }
                }

                Texture2D mapTex = new Texture2D(
                    GraphicsDevice,
                    (int)size.X,
                    (int)size.Y,
                    false,
                    SurfaceFormat.Color);

                mapTex.SetData<Color>(pixels, 0, pixels.Length);

                return mapTex;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            //    this.Exit();
            //if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            //    this.Exit();

            if (whiteWhipe != null)
            {
                if (whiteWhipe.Update(gameTime))
                    whiteWhipe = null;
            }

            switch (gameState)
            {

                #region MainMenu
                case GameState.MainMenu:
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                        && !(prevGamePadState.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                        || (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)
                        && !prevKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)))
                        this.Exit();
                    switch (mainMenu.Update(gameTime))
                    {
                        case GameFlow.None:
                            break;
                        case GameFlow.ChangeGraphics:
                            graphics.PreferredBackBufferWidth = gameOptions.preferred_Width;
                            graphics.PreferredBackBufferHeight = gameOptions.preferred_Height;
                            graphics.IsFullScreen = gameOptions.is_Full_Screen;
                            graphics.ApplyChanges();
                            setCameraZoom(gameOptions.preferred_Height);
                            mainMenu = new GameFinal.Menu_s.MainMenu(mainMenuTexs, Window.ClientBounds, GraphicsDevice, tankTexs,
                                spriteFont, tileTexList, cameraZoom, gameOptions, whiteWhipeTexs[0], audio);
                            break;
                        case GameFlow.Whipe:
                            whiteWhipe = new WhiteWhipe(whiteWhipeTexs, Window.ClientBounds, Window.ClientBounds.Height / 8, false, audio);
                            whiteWhipe.on_Blank += ChangeToInGame;
                            break;
                        case GameFlow.Exit:
                            this.Exit();
                            break;
                    }
                    break;
                #endregion
                #region InGame
                case GameState.InGame:
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                        && !(prevGamePadState.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed))
                        || (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)
                        && !prevKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape)))
                    {
                        whiteWhipe = new WhiteWhipe(whiteWhipeTexs, Window.ClientBounds, Window.ClientBounds.Height / 8, false, audio);
                        whiteWhipe.on_Blank += ChangeToMainMenu;
                    }
                    else if (inGame != null && inGame.readyForUpdate)
                    {
                        if (whiteWhipe != null)
                            whiteWhipe.WhipeThrough();
                        inGame.Update(gameTime);
                    }
                    
                    break;
                #endregion
            }
            audio.Update(gameTime);
            prevKeyState = Keyboard.GetState();
            prevGamePadState = GamePad.GetState(PlayerIndex.One);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public void ChangeToInGame(object o, EventArgs e)
        {
            if (gameOptions.is_Server)
            {
                inGame = new Server(this, GraphicsDevice, whiteWhipeTexs[0], tankTexs, spriteFont,
                    tileTexList, gameInterfaceTexs, weaponTexs, explosionTexs, weapon, gameOptions.map_Number,
                    gameOptions.skin_Number, gameOptions.player_Name, orb, cameraZoom, audio, black);
            }
            else
            {
                inGame = new Client(this, GraphicsDevice, whiteWhipeTexs[0], tankTexs, spriteFont,
                    tileTexList, gameInterfaceTexs, weaponTexs, explosionTexs, weapon, gameOptions.map_Number,
                    StaticHelpers.FormatIP(gameOptions.IP_Adress), gameOptions.skin_Number, gameOptions.player_Name, orb, cameraZoom, audio, black);
                if (((Client)inGame).getFailedToConnect())
                {
                    inGame.EndGame();
                    inGame = null;
                    failedMessageTimer = 1;
                    whiteWhipe = new WhiteWhipe(whiteWhipeTexs, Window.ClientBounds, Window.ClientBounds.Height / 8, false, audio);
                    whiteWhipe.on_Blank += ChangeToMainMenu;
                }
            }
            gameState = GameState.InGame;
        }

        public void ChangeToMainMenu(object o, EventArgs e)
        {
            if (whiteWhipe != null)
                whiteWhipe.WhipeThrough();
            if (inGame != null)
            {
                inGame.EndGame();
                inGame = null;
            }
            gameState = GameState.MainMenu;
            mainMenu.ResetGameFlow();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend);

            switch (gameState)
            {
                case GameState.MainMenu:
                    mainMenu.Draw(spriteBatch, gameTime);
                    break;
                case GameState.InGame:
                    if (inGame != null && inGame.readyForUpdate)
                        inGame.Draw(gameTime, spriteBatch);
                    break;
            }
            if (failedMessageTimer > 0)
            {
                failedMessageTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (failedMessageTimer >= 1000)
                    failedMessageTimer = 0;

                spriteBatch.DrawString(spriteFont,
                    "Connection Failed",
                    new Vector2((Window.ClientBounds.Width / 2) - ((spriteFont.MeasureString("Connection Failed").X * StaticHelpers.fontScale) / 2),
                        (Window.ClientBounds.Height / 2) - ((spriteFont.MeasureString("Connection Failed").Y * StaticHelpers.fontScale) / 2)),
                    Color.Red,
                    0,
                    Vector2.Zero,
                    StaticHelpers.fontScale,
                    SpriteEffects.None,
                    0.01f);
            }
            if (whiteWhipe != null)
                whiteWhipe.Draw(spriteBatch);

            spriteBatch.End();
            
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

public enum GameFlow
{
    None,
    Whipe,
    ChangeGraphics,
    StartGame,
    Exit
}
