using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Dynamics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using GameFinal.Misc;

namespace GameFinal.Menu_s
{
    class MainMenu
    {
        #region variables
        Rectangle clientBounds;
        GameFlow gameFlow;
        List<FormElement> formList;
        GameOptions gameOptions;
        Texture2D[] selectableTankTexs;
        Texture2D[] selectableMapTexs;
        Texture2D[] resolutionTexs;
        Texture2D sliderBarTex;

        TankSelector lastTankSelector;
        TankSelector lastMapSelector;
        TankSelector lastResolutionSelector;
        TextBox lastNameBox;
        CheckBox lastFullScreenCheckBox;
        TextBox[] lastIP;

        Texture2D black;
        Audio audio;

        #region Local Variables
        //Other characters
        Camera2D cam;
        SpriteFont spriteFont;
        World world = new World(Vector2.Zero);
        TileMap tileMap;
        Point canvasSize;
        Vector2 cameraPosition = Vector2.Zero;
        GraphicsDevice GraphicsDevice;
        float cameraZoom;
        float elementScale;
        #endregion

        #region Content Variables
        Texture2D[] tankTexs;
        List<Texture2D> tileTexList;
        int[,] tileArray;
        Texture2D[] buttonTexs;
        Texture2D[] checkBoxTexs;
        #endregion
        #endregion

        public MainMenu(Texture2D[] mainMenuTexs, Rectangle clientBounds, GraphicsDevice GraphicsDevice,
            Texture2D[] tankTexs, SpriteFont spriteFont, List<Texture2D> tileTexList,
            float cameraZoom, GameOptions gameOptions, Texture2D black, Audio audio)
        {
            this.clientBounds = clientBounds;
            gameFlow = GameFlow.None;

            this.tankTexs = tankTexs;
            this.tileTexList = tileTexList;
            this.GraphicsDevice = GraphicsDevice;
            this.spriteFont = spriteFont;
            this.cameraZoom = cameraZoom;
            this.gameOptions = gameOptions;
            this.black = black;
            this.audio = audio;
            this.elementScale = (float)clientBounds.Width / 1024f;

            buttonTexs = new Texture2D[3];
            buttonTexs[0] = mainMenuTexs[0];
            buttonTexs[1] = mainMenuTexs[1];
            buttonTexs[2] = mainMenuTexs[2];
            checkBoxTexs = new Texture2D[2];
            checkBoxTexs[0] = mainMenuTexs[3];
            checkBoxTexs[1] = mainMenuTexs[4];
            resolutionTexs = new Texture2D[12];
            resolutionTexs[0] = mainMenuTexs[5];
            resolutionTexs[1] = mainMenuTexs[6];
            resolutionTexs[2] = mainMenuTexs[7];
            resolutionTexs[3] = mainMenuTexs[8];
            resolutionTexs[4] = mainMenuTexs[9];
            resolutionTexs[5] = mainMenuTexs[10];
            resolutionTexs[6] = mainMenuTexs[11];
            resolutionTexs[7] = mainMenuTexs[12];
            resolutionTexs[8] = mainMenuTexs[13];
            resolutionTexs[9] = mainMenuTexs[14];
            resolutionTexs[10] = mainMenuTexs[15];
            resolutionTexs[11] = mainMenuTexs[16];
            sliderBarTex = mainMenuTexs[17];
            selectableMapTexs = new Texture2D[mainMenuTexs.Length - 18];
            for (int i = 18; i < mainMenuTexs.Length; i++)
            {
                selectableMapTexs[i - 18] = mainMenuTexs[i];
            }


            selectableTankTexs = new Texture2D[12];
            for (int i = 0; i < 12; i++)
            {
                selectableTankTexs[i] = tankTexs[i];
            }

            #region Load Map
            try
            {
                System.IO.Stream stream = TitleContainer.OpenStream(@"Maps/0.map");
                BinaryFormatter bf = new BinaryFormatter();
                tileArray = (int[,])bf.Deserialize(stream);
                stream.Close();
            }
            catch (System.IO.FileNotFoundException e)
            {
                MessageBox.Show("Map not found:\n " + e.ToString());
            }
            #endregion

            tileMap = new TileMap(Vector2.Zero, tileTexList, tileArray, world);
            canvasSize = tileMap.canvasSize;
            cam = new Camera2D(new Vector2(clientBounds.Width, clientBounds.Height),
                new Vector2(canvasSize.X, canvasSize.Y), cameraZoom);

            formList = new List<FormElement>();

            if (gameOptions.player_Name == "")
                gameOptions.player_Name = "no name";

            lastIP = new TextBox[4];
            for (int i = 0; i < lastIP.Length; i++)
            {
                lastIP[i] = new TextBox(spriteFont, Vector2.Zero, Vector2.Zero, black,
                "", 1, Color.Red, 3, true, true, audio);
            }

            lastTankSelector = new TankSelector(tankTexs, spriteFont, Vector2.Zero, Vector2.Zero, gameOptions.skin_Number, 1, audio);
            lastMapSelector = new TankSelector(selectableMapTexs, spriteFont, Vector2.Zero, Vector2.Zero, gameOptions.map_Number, 1, audio);
            lastNameBox = new TextBox(spriteFont, Vector2.Zero, Vector2.Zero, black, gameOptions.player_Name, 1, Color.White, 20, false, false, audio);
            lastResolutionSelector = null;
            lastFullScreenCheckBox = null;


            ToMainMenu(this, new EventArgs());
        }

        public GameFlow Update(GameTime gameTime)
        {
            //if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            //    gameFlow = GameFlow.Whipe;

            for (int i = 0; i < formList.Count; i++ )
            {
                if (formList[i].Update(gameTime))
                    formList.Remove(formList[i]);
            }

            cam.Pos = cameraPosition;

            cam.Update();
            world.Step(gameTime.ElapsedGameTime.Milliseconds * .001f);

            GameFlow g = gameFlow;

            if (gameFlow != GameFlow.None)
                gameFlow = GameFlow.None;

            return g;
        }

        public void ResetGameFlow()
        {
            gameFlow = GameFlow.None;
            ToMainMenu(this, new EventArgs());
        }

        #region EventHandlers
        public void startCreate(object o, EventArgs e)
        {
            gameOptions.skin_Number = lastTankSelector.getSelectedIndex();
            gameOptions.player_Name = lastNameBox.getText();
            gameOptions.map_Number = lastMapSelector.getSelectedIndex() + 1;
            gameOptions.is_Server = true;
            saveGameOptions();

            ClearFormElementsLeft();
            gameFlow = GameFlow.Whipe;
        }
        public void createGame(object o, EventArgs e)
        {
            ClearFormElementsLeft();
            gameOptions.is_Server = true;

            Button b = new Button(buttonTexs, spriteFont, getScreenVector(6, 5, 9, 10),
                "Back", getScreenVector(2, 5, 9, 10), elementScale, audio);
            Button b2 = new Button(buttonTexs, spriteFont, getScreenVector(6, 5, 9, 10),
                "Create", getScreenVector(3, 5, 9, 10), elementScale,audio);

            TankSelector t2 = new TankSelector(selectableMapTexs, spriteFont, getScreenVector(6, 5, 9, 13),
                getScreenVector(4, 7, 9, 13), gameOptions.map_Number - 1, elementScale * new Vector2(100, 100), audio);
            Label l3 = new Label(spriteFont, getScreenVector(6, 5, 9, 13), getScreenVector(2, 6, 9, 13), "Map: ", elementScale, Color.Red);

            TankSelector t = new TankSelector(selectableTankTexs, spriteFont, getScreenVector(6, 5, 1, 8),
                getScreenVector(4, 7, 1, 8), gameOptions.skin_Number, elementScale * 0.09f, audio);
            Label l = new Label(spriteFont, getScreenVector(6, 5, 1, 8),
                 getScreenVector(2, 6, 1, 8), "Tank Skin: ", elementScale, Color.Red);

            TextBox te = new TextBox(spriteFont, getScreenVector(6, 5, 2, 8),
                getScreenVector(1, 2, 2, 8), black, gameOptions.player_Name, elementScale, Color.White, 20, false, false, audio);
            Label l2 = new Label(spriteFont, getScreenVector(6, 5, 2, 8), getScreenVector(2, 6, 2, 8), "Name: ", elementScale, Color.Red);

            lastTankSelector = t;
            lastNameBox = te;
            lastMapSelector = t2;
            formList.Add(l3);
            formList.Add(t2);
            formList.Add(t);
            formList.Add(l);
            formList.Add(te);
            formList.Add(l2);
            formList.Add(b);
            formList.Add(b2);
            b.on_Click += ToMainMenu;
            b2.on_Click += startCreate;
        }
        public void startJoin(object o, EventArgs e)
        {
            gameOptions.is_Server = false;
            gameOptions.skin_Number = lastTankSelector.getSelectedIndex();
            gameOptions.player_Name = lastNameBox.getText();
            gameOptions.IP_Adress = lastIP[0].getText() + "." + lastIP[1].getText() + "." + lastIP[2].getText() + "." + lastIP[3].getText();
            saveGameOptions();
            gameFlow = GameFlow.Whipe;
        }
        public void joinGame(object o, EventArgs e)
        {
            ClearFormElementsLeft();
            gameOptions.is_Server = false;
            Button b = new Button(buttonTexs, spriteFont, getScreenVector(6, 5, 9, 10),
                "Back", getScreenVector(2, 5, 9, 10), elementScale, audio);
            Button b2 = new Button(buttonTexs, spriteFont, getScreenVector(6, 5, 9, 10),
                "Join", getScreenVector(3, 5, 9, 10), elementScale,audio);

            TankSelector t = new TankSelector(selectableTankTexs, spriteFont, getScreenVector(6, 5, 7, 11),
                getScreenVector(4, 7, 7, 11), gameOptions.skin_Number, elementScale * 0.09f, audio);
            Label l = new Label(spriteFont, getScreenVector(6, 5, 7, 11),
                 getScreenVector(2, 6, 7, 11), "Tank Skin: ", elementScale, Color.Red);

            TextBox te = new TextBox(spriteFont, getScreenVector(6, 5, 10, 13),
                getScreenVector(3, 7, 10, 13), black, gameOptions.player_Name, elementScale, Color.White, 20, false, false, audio);
            Label l2 = new Label(spriteFont, getScreenVector(6, 5, 10, 13), getScreenVector(2, 6, 10, 13), "Name: ", elementScale, Color.Red);
            te.setActive(false);

            TextBox t1 = new TextBox(spriteFont, getScreenVectorS(6,5,1,4), getScreenVectorS(4,5,1,4), black,
                lastIP[3].getText(), elementScale * 2, Color.Red, 3, true, true, audio);
            t1.setActive(false);
            TextBox t2 = new TextBox(spriteFont, getScreenVectorS(6, 5, 1, 4), getScreenVectorS(3, 5, 1, 4), black,
                lastIP[2].getText(), elementScale * 2, Color.Red, 3, true, true, t1, audio);
            TextBox t3 = new TextBox(spriteFont, getScreenVectorS(6, 5, 1, 4), getScreenVectorS(2, 5, 1, 4), black,
                lastIP[1].getText(), elementScale * 2, Color.Red, 3, true, true, t2, audio);
            TextBox t4 = new TextBox(spriteFont, getScreenVectorS(6, 5, 1, 4), getScreenVectorS(1, 5, 1, 4), black,
                lastIP[0].getText(), elementScale * 2, Color.Red, 3, true, true, t3, audio);
            t4.setActive(true);

            lastTankSelector = t;
            lastNameBox = te;
            formList.Add(t);
            formList.Add(l);
            formList.Add(te);
            formList.Add(l2);
            lastIP[0] = t4;
            lastIP[1] = t3;
            lastIP[2] = t2;
            lastIP[3] = t1;
            b.on_Click += ToMainMenu;
            b2.on_Click += startJoin;
            formList.Add(b);
            formList.Add(b2);
            formList.Add(t1);
            formList.Add(t2);
            formList.Add(t3);
            formList.Add(t4);
        }
        public void options(object o, EventArgs e)
        {
            ClearFormElementsLeft();
            Button b = new Button(buttonTexs, spriteFont, getScreenVector(6,5,9,10),
                "Cancel", getScreenVector(2, 5, 9, 10), elementScale,audio);
            Button b2 = new Button(buttonTexs, spriteFont, getScreenVector(6, 5, 9, 10),
                "Save", getScreenVector(3, 5, 9, 10), elementScale,audio);

            Slider s = new Slider(sliderBarTex, checkBoxTexs[1], spriteFont, getScreenVector(6,5,1,4),
                getScreenVector(4,7,1,4), audio.getEffectVolume(), elementScale * 0.2f, true, audio);
            Label l3 = new Label(spriteFont, getScreenVector(6,5,1,4), getScreenVector(2,6,1,4), "Volume: ", elementScale, Color.Red);

            TankSelector t3 = new TankSelector(resolutionTexs, spriteFont, getScreenVector(6,5,9,13),
                getScreenVector(4,7,9,13), StaticHelpers.resolutionToIndex(gameOptions.preferred_Width, gameOptions.preferred_Height), elementScale * 0.6f, audio);
            Label l4 = new Label(spriteFont, getScreenVector(6, 5, 9, 13), getScreenVector(2, 6, 9, 13), "Resolution: ", elementScale, Color.Red);

            CheckBox c = new CheckBox(spriteFont, getScreenVector(6,5,10,13),
                getScreenVector(4,7,10,13), checkBoxTexs, elementScale * 0.2f, gameOptions.is_Full_Screen, audio);
            Label l5 = new Label(spriteFont, getScreenVector(6, 5, 10, 13), getScreenVector(2, 6, 10, 13), "Full Screen: ", elementScale, Color.Red);
            

            lastFullScreenCheckBox = c;
            lastResolutionSelector = t3;
            b.on_Click += ToMainMenu;
            b2.on_Click += SaveOptions;
            formList.Add(b);
            formList.Add(b2);
            formList.Add(c);
            formList.Add(t3);
            formList.Add(l3);
            formList.Add(l4);
            formList.Add(l5);
            formList.Add(s);
        }
        public void ToMainMenu(object o, EventArgs e)
        {
            gameOptions.skin_Number = lastTankSelector.getSelectedIndex();
            gameOptions.player_Name = lastNameBox.getText();
            gameOptions.map_Number = lastMapSelector.getSelectedIndex() + 1;
            saveGameOptions();

            ClearFormElementsLeft();
            Button b1 = new Button(buttonTexs, spriteFont, getScreenVector(8,7,1,9),
                "Create Game", getScreenVector(1, 2, 1, 9), elementScale,audio);
            Button b2 = new Button(buttonTexs, spriteFont, getScreenVector(8, 7, 2, 9),
                "Join Game", getScreenVector(1, 2, 2, 9), elementScale, audio);
            Button b3 = new Button(buttonTexs, spriteFont, getScreenVector(8, 7, 3, 9),
                "Options", getScreenVector(1, 2, 3, 9), elementScale, audio);
            Button b4 = new Button(buttonTexs, spriteFont, getScreenVector(8, 7, 6, 8),
                "Exit", getScreenVector(1, 2, 6, 8), elementScale, audio);

            b1.on_Click += createGame;
            b2.on_Click += joinGame;
            b3.on_Click += options;
            b4.on_Click += exit;

            formList.Add(b1);
            formList.Add(b2);
            formList.Add(b3);
            formList.Add(b4);
        }
        public void exit(object o, EventArgs e)
        {
            gameFlow = GameFlow.Exit;
        }
        public void SaveOptions(object o, EventArgs e)
        {
            if (!gameOptions.is_Full_Screen == lastFullScreenCheckBox.getChecked())
            {
                gameOptions.is_Full_Screen = lastFullScreenCheckBox.getChecked();
                gameFlow = GameFlow.ChangeGraphics;
            }
            int[] res = StaticHelpers.indexToResolution(lastResolutionSelector.getSelectedIndex());
            if (!(res[0] == gameOptions.preferred_Width && res[1] == gameOptions.preferred_Height))
            {
                gameOptions.preferred_Width = res[0];
                gameOptions.preferred_Height = res[1];
                gameFlow = GameFlow.ChangeGraphics;
            }
            ClearFormElementsLeft();
            ToMainMenu(this, new EventArgs());
        }
        #endregion

        private void saveGameOptions()
        {
            // Open a storage container
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            StorageDevice device = StorageDevice.EndShowSelector(result);
            IAsyncResult result2 = device.BeginOpenContainer("RankleOptions", null, null);
            StorageContainer container = device.EndOpenContainer(result2);

            try
            {
                // Open the file
                Stream stream = container.OpenFile("options.sav", FileMode.Create);

                // Read the data from the file
                XmlSerializer serializer = new XmlSerializer(typeof(GameOptions));
                serializer.Serialize(stream, gameOptions);

                // Close the file
                stream.Close();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            container.Dispose();
        }

        private void ClearFormElementsLeft()
        {
            foreach (FormElement f in formList)
            {
                f.setDestroyTarget(new Vector2(-320, f.getPos().Y));
            }
        }

        private Vector2 getScreenVector(int xNum, int xDen, int yNum, int yDen)
        {
            return new Vector2((clientBounds.Width * xNum) / xDen, (clientBounds.Height * yNum) / yDen);
        }
        private Vector2 getScreenVectorS(int xNum, int xDen, int yNum, int yDen)
        {
            return new Vector2(((clientBounds.Width - clientBounds.Height) / 2) + (((clientBounds.Height) * xNum) / xDen),
                (clientBounds.Height * yNum) / yDen);
        }

        public GameOptions getGameOptions()
        {
            return gameOptions;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
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

            tileMap.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend);

            foreach (FormElement f in formList)
            {
                f.Draw(spriteBatch);
            }
        }
    }
}
