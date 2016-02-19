using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using Lidgren.Network;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using GameFinal.Display;
using GameFinal.Control;
using GameFinal.Objects;
using GameFinal.Misc;

namespace GameFinal
{
    class Client : InGame
    {
        #region Client Variables
        // Client Object
        static NetClient netClient;

        // Clients list of characters
        static List<Character> gameStateList;

        // Create timer that tells client, when to send update
        static System.Timers.Timer update;

        //Host IP
        string hostIP = "127.0.0.1";

        static int portNumber = 25564;

        int mapNumber;

        List<Character> gameWorldState;

        bool waitingToRespawn = false;

        bool failedToConnect = false;
        bool CanStart = false;
        #endregion

        public Client(Game1 game1, GraphicsDevice GraphicsDevice, Texture2D red, Texture2D[] tankTexs,
            SpriteFont spriteFont, List<Texture2D> tileTexList, Texture2D[] gameInterfaceTexs,
            Texture2D[] weaponTexs, Texture2D[] explosionTexs, int[] weapon, int mapNumber,
            string hostIP, int tankSkin, string playerName, Texture2D orb, float cameraZoom, Audio audio, Texture2D black)
            : base(game1, GraphicsDevice, red, false, tankTexs, spriteFont,
            tileTexList, gameInterfaceTexs, weaponTexs, explosionTexs, weapon, tankSkin, playerName, orb, cameraZoom, audio, black)
        {
            this.mapNumber = mapNumber;
            gameWorldState = new List<Character>();
            this.hostIP = hostIP;

            explosionGenerator = new ExplosionGenerator(explosionTexs);
            try
            {
                #region Client Initialization
                Console.WriteLine("Configuring net peer");
                // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
                NetPeerConfiguration Config = new NetPeerConfiguration("game");
                Config.MaximumHandshakeAttempts = 10;
                Config.ResendHandshakeInterval = 1;

                // Create new client, with previously created configs
                netClient = new NetClient(Config);

                // Create new outgoing message
                NetOutgoingMessage outmsg = netClient.CreateMessage();

                Console.WriteLine("Starting Client");
                // Start client
                netClient.Start();

                // Write byte ( first byte informs server about the message type ) ( This way we know, what kind of variables to read )
                outmsg.Write((byte)PacketTypes.LOGIN);

                // Write String "Name" . Not used, but just showing how to do it
                outmsg.Write(playerName);

                outmsg.Write(tankSkin);

                Console.WriteLine("Connecting");
                // Connect client, to ip previously requested from user
                netClient.Connect(hostIP, portNumber, outmsg);

                // Create the list of characters
                gameStateList = new List<Character>();

                // Set timer to tick every 50ms
                update = new System.Timers.Timer(50);

                // When time has elapsed ( 50ms in this case ), call "update_Elapsed" funtion
                update.Elapsed += new System.Timers.ElapsedEventHandler(update_Elapsed);

                // New incomgin message
                NetIncomingMessage inc;

                // Loop untill we are approved
                double failCount = 0;
                while (!CanStart)
                {
                    // If new messages arrived
                    if ((inc = netClient.ReadMessage()) != null)
                    {
                        Console.WriteLine("Recieving message: " + inc.ToString());
                        // Switch based on the message types
                        switch (inc.MessageType)
                        {
                            // All manually sent messages are type of "Data"
                            case NetIncomingMessageType.Data:
                                byte b = inc.ReadByte();
                                if (b == (byte)12)
                                {
                                    int count = 0;
                                    count = inc.ReadInt32();
                                    for (int i = 0; i < count; i++)
                                    {
                                        Orb o = new Orb(orb, Vector2.Zero, 0, gameInterface, audio);
                                        inc.ReadAllProperties(o);
                                        orbList.Add(o);
                                    }
                                    CanStart = true;
                                }
                                // Read the first byte
                                // This way we can separate packets from each others
                                else if (b == (byte)PacketTypes.WORLDSTATE)
                                {
                                    // Empty the gamestatelist
                                    // new data is coming, so everything we knew on last frame, does not count here
                                    // Even if client would manipulate this list ( hack ), it wont matter, because server handles the real list
                                    gameStateList.Clear();

                                    // Declare count
                                    int count = 0;

                                    mapNumber = inc.ReadInt32();

                                    Console.WriteLine("Reading Players in...");
                                    // Read int
                                    count = inc.ReadInt32();

                                    // Iterate all players
                                    for (int i = 0; i < count; i++)
                                    {
                                        // Create new character to hold the data
                                        Character ch = new Character();


                                        // Read all properties ( Server writes characters all props, so now we can read em here. Easy )
                                        inc.ReadAllProperties(ch);

                                        if (ch.Self)
                                        {
                                            base.Initialize(mapNumber, ch);
                                        }
                                        else
                                        {
                                            characterTankSkins[ch.characterIndex] = ch.tankSkin;
                                            characterNames[ch.characterIndex] = ch.Name;
                                            base.NewCharacter(ch.getIndex(), new Vector2(ch.X, ch.Y), ch.tankSkin, ch.Name);
                                            float[] minesx = new float[inc.ReadInt32()];
                                            float[] minesy = new float[minesx.Length];
                                            for (int f = 0; f < minesx.Length; f++)
                                            {
                                                minesx[f] = inc.ReadFloat();
                                                minesy[f] = inc.ReadFloat();
                                            }
                                            otherPlayers[ch.characterIndex].setMines(minesx, minesy);
                                            //if (keepScore != null)
                                            //    keepScore.AddPlayer(ch.characterIndex, ch.Name);
                                        }
                                        // Add it to list
                                        gameStateList.Add(ch);
                                    }

                                    Console.WriteLine("Starting Game...");
                                    // When all players are added to list, start the game
                                    //CanStart = true;
                                    update.Start();
                                    base.game1.Exiting += new EventHandler<EventArgs>(_exiting);
                                }
                                break;

                            default:
                                failCount++;
                                if (failCount > 11)
                                {
                                    failedToConnect = true;
                                    CanStart = true;
                                }
                                Console.WriteLine("Strange message: " + inc.ReadString());
                                //Console.WriteLine("Sender connection: " + inc.SenderConnection.ToString());
                                //Console.WriteLine("Sender connection stats: " + inc.SenderConnection.Statistics.ToString());
                                break;
                        }
                    }
                    else
                    {
                        //failCount += 0.00000003d;
                        if (failCount >= 10)
                        {
                            failedToConnect = true;
                            CanStart = true;
                        }
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GetInputAndSendItToServer()
        {
            // Enum object

            // Readkey ( NOTE: This normally stops the code flow. Thats why we have timer running, that gets updates)
            // ( Timers run in different threads, so that can be run, even thou we sit here and wait for input 
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                // Disconnect and give the reason
                Console.WriteLine("Disconnecting");
                netClient.Disconnect("bye bye");
            }
            if (!waitingToRespawn)
            {
                try
                {
                    NetOutgoingMessage outmsg = netClient.CreateMessage();
                    outmsg.Write((byte)PacketTypes.MOVE);
                    outmsg.Write(mainCharacter.getPos().X);
                    outmsg.Write(mainCharacter.getPos().Y);
                    outmsg.Write(mainCharacter.getRotation());
                    outmsg.Write(mainCharacter.tankBody.LinearVelocity.X);
                    outmsg.Write(mainCharacter.tankBody.LinearVelocity.Y);

                    List<int> chFCL = mainCharacter.GetFireCallList();
                    List<Vector2> minePlaceList = mainCharacter.GetMinePlaceList();
                    outmsg.Write(chFCL.Count);
                    if (chFCL.Count > 0)
                    {
                        foreach (int i in chFCL)
                        {
                            outmsg.Write(i);
                        }
                    }
                    outmsg.Write(minePlaceList.Count);
                    if (minePlaceList.Count > 0)
                    {
                        foreach (Vector2 v in minePlaceList)
                        {
                            outmsg.Write(v.X);
                            outmsg.Write(v.Y);
                        }
                    }

                    netClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(mainCharacter != null)
                mainCharacter.serverTargetPos = mainCharacter.getPos();
            if(CanStart)
                CheckServerMessages(gameTime);
            base.Update(gameTime);
        }

        private void CheckServerMessages(GameTime gameTime)
        {
            // Create new incoming message holder
            NetIncomingMessage inc;

            // While theres new messages
            //
            // THIS is exactly the same as in WaitForStartingInfo() function
            // Check if its Data message
            // If its WorldState, read all the characters to list
            while ((inc = netClient.ReadMessage()) != null)
            {
                if (inc.MessageType == NetIncomingMessageType.Data)
                {
                    PacketTypes action = (PacketTypes)inc.ReadByte();
                    switch (action)
                    {
                        #region worldstate
                        case PacketTypes.WORLDSTATE:
                            try
                            {
                                gameStateList.Clear();
                                int count = 0;
                                count = inc.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    Character ch = new Character();
                                    inc.ReadAllProperties(ch);
                                    gameStateList.Add(ch);
                                    if (ch.isValid()) //To fix corrupted character data bug
                                    {
                                        if (otherPlayers[ch.characterIndex] == null && ch.characterIndex != thisPlayerIndex && ch.health > 0)
                                        {
                                            characterTankSkins[ch.characterIndex] = ch.tankSkin;
                                            characterNames[ch.characterIndex] = ch.Name;
                                            base.NewCharacter(ch.getIndex(), new Vector2(ch.X, ch.Y), ch.tankSkin, ch.Name);

                                            //if (!keepScore.samePlayer(ch.Name, ch.characterIndex))
                                            //    keepScore.AddPlayer(ch.characterIndex, ch.Name);
                                        }
                                        else if (ch.characterIndex != thisPlayerIndex && otherPlayers[ch.characterIndex] != null)
                                        {
                                            otherPlayers[ch.characterIndex].targetPos = new Vector2(ch.X, ch.Y);
                                            otherPlayers[ch.characterIndex].targetVelocity = new Vector2(ch.VelX, ch.VelY);
                                            otherPlayers[ch.characterIndex].targetRotation = ch.Rotation;
                                            otherPlayers[ch.characterIndex].SetHealthEnergy(ch.health, ch.energy);
                                            otherPlayers[ch.characterIndex].setBuffs(ch.getBuffs());
                                        }
                                        else if (ch.characterIndex == thisPlayerIndex && !waitingToRespawn)
                                        {
                                            mainCharacter.serverTargetPos = new Vector2(ch.X, ch.Y);
                                            mainCharacter.SetHealthEnergy(ch.health, ch.energy);
                                            mainCharacter.setBuffs(ch.getBuffs());
                                            if (ch.health <= 0)
                                                waitingToRespawn = true;
                                        }
                                        else if (ch.characterIndex == thisPlayerIndex && waitingToRespawn)
                                        {
                                            if (ch.health > 0)
                                            {
                                                NewMainCharacter(new Vector2(ch.X, ch.Y));
                                                waitingToRespawn = false;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        #endregion
                        #region remove
                        case PacketTypes.REMOVE:
                            int chIndex = inc.ReadInt32();
                            for (int i = 0; i < gameStateList.Count; i++)
                            {
                                if (gameStateList[i].characterIndex == chIndex)
                                {
                                    if (otherPlayers[chIndex] != null)
                                    {
                                        keepScore.removePlayer(chIndex);
                                        otherPlayers[chIndex].DisposeBody();
                                        otherPlayers[chIndex] = null;
                                    }
                                    gameStateList.Remove(gameStateList[i]);
                                    break;
                                }
                            }
                            break;
                        #endregion
                        #region move
                        case PacketTypes.MOVE:
                            try
                            {
                                int playerIndex = inc.ReadInt32();
                                if (playerIndex != thisPlayerIndex)
                                {
                                    int fireCallCount = inc.ReadInt32();

                                    for (int i = 0; i < fireCallCount; i++)
                                    {
                                        int x = inc.ReadInt32();
                                        if (otherPlayers[playerIndex] != null)
                                            otherPlayers[playerIndex].FireWeapon(gameTime, x);
                                    }
                                    int minePlaceList = inc.ReadInt32();
                                    for (int i = 0; i < minePlaceList; i++)
                                    {
                                        float x = inc.ReadFloat();
                                        float y = inc.ReadFloat();
                                        otherPlayers[playerIndex].PlaceMine(new Vector2(x, y));
                                    }

                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        #endregion
                        #region score
                        case PacketTypes.SCORE:
                            try
                            {
                                int sCount = 0;
                                sCount = inc.ReadInt32();
                                Score[] s = new Score[sCount];
                                for (int i = 0; i < sCount; i++)
                                {
                                    Score temp = new Score("");
                                    inc.ReadAllProperties(temp);
                                    if (temp.name != "")
                                        s[i] = temp;

                                }
                                keepScore.setScores(s);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        #endregion
                        #region orb
                        case PacketTypes.ORB:
                            try
                            {
                                Orb o = new Orb(orb, Vector2.Zero, 1, gameInterface, audio);
                                inc.ReadAllProperties(o);
                                orbList.Add(o);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        #endregion
                        #region message
                        case PacketTypes.MESSAGE:
                            try
                            {
                                appendIntercomMessage(inc.ReadString());
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            break;
                        #endregion
                    }

                }
            }
        }

        /// <summary>
        /// Every 50ms this is fired
        /// </summary>
        void update_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Check if server sent new messages
            GetInputAndSendItToServer();
        }

        void _exiting(object sender, EventArgs e)
        {
            netClient.Disconnect("Game Closed");
        }

        protected override void PlayerEntered(Vector2 position, int index, string name)
        {
            throw new NotImplementedException();
        }
        protected override void PlayerDied(int index)
        {
        }
        protected override void PlaceOrb(Vector2 position, float kD, int characterIndex)
        {
            //throw new NotImplementedException();
        }
        protected override void PushMessage(string message)
        {
            NetOutgoingMessage outmsg = netClient.CreateMessage();
            outmsg.Write((byte)PacketTypes.MESSAGE);
            outmsg.Write(message);
            netClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
        }

        public override void EndGame()
        {
            netClient.Disconnect(playerName + " Has Disconnected");
        }
        public bool getFailedToConnect()
        {
            return failedToConnect;
        }
    }
}
