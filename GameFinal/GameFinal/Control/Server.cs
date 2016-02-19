using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameFinal;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using FarseerPhysics.DemoBaseXNA;
using GameFinal.Display;
using GameFinal.Control;
using GameFinal.Objects;
using GameFinal.Misc;
using Microsoft.Xna.Framework.Input;

namespace GameFinal
{
    class Server : InGame
    {
        #region Server Variables
        // Clients list of characters
        static List<Character> gameWorldState;
        // Server object
        static NetServer netServer;
        // Configuration object
        static NetPeerConfiguration config;
        // Object that can be used to store and read messages
        NetIncomingMessage inc;
        // Check time
        DateTime time1 = DateTime.Now;//For periodic update
        DateTime time2 = DateTime.Now;//For score update
        // Create timespan of 30ms
        TimeSpan timeToPass1 = new TimeSpan(0, 0, 0, 0, 30);//same as time's
        TimeSpan timeToPass2 = new TimeSpan(0, 0, 0, 2, 0);
        int mapNumber;
        int portNumber = 25564;

        float respawnTimer = 0;

        List<int[]> respawnList;
        #endregion

        public Server(Game1 game1, GraphicsDevice GraphicsDevice, Texture2D red, Texture2D[] tankTexs,
            SpriteFont spriteFont, List<Texture2D> tileTexList, Texture2D[] gameInterfaceTexs,
            Texture2D[] weaponTexs, Texture2D[] explosionTexs, int[] weapon, int mapNumber, int tankSkin, string playerName, Texture2D orb,
            float cameraZoom, Audio audio, Texture2D black)
            : base(game1, GraphicsDevice, red, true, tankTexs, spriteFont,
            tileTexList, gameInterfaceTexs, weaponTexs, explosionTexs, weapon, tankSkin, playerName, orb, cameraZoom, audio, black)
        {
            message = "Server";
            explosionGenerator = new ExplosionGenerator(explosionTexs);

            #region Server Variables
            Console.WriteLine("Server Starting");
            // Create new instance of configs. Parameter is "application Id". It has to be same on client and server.
            config = new NetPeerConfiguration("game");
            // Set server port
            config.Port = portNumber;
            // Max client amount
            config.MaximumConnections = 8;
            // Enable New messagetype. Explained later
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            // Create new server based on the configs just defined
            netServer = new NetServer(config);
            // Start it
            netServer.Start();
            Console.WriteLine("Server Started");
            #endregion

            this.mapNumber = mapNumber;
            Character thisCharacter = new Character(playerName, null, 0, tankSkin);
            gameWorldState = new List<Character>();
            gameWorldState.Add(thisCharacter);
            respawnList = new List<int[]>();
            game1.Exiting += _exiting;
            base.Initialize(mapNumber, thisCharacter);
        }

        public override void Update(GameTime gameTime)
        {
            if (mainCharacter != null)
            {
                mainCharacter.serverTargetPos = mainCharacter.getPos();
                List<int> chFCL = mainCharacter.GetFireCallList();
                List<Vector2> minePlaceList = mainCharacter.GetMinePlaceList();

                if (netServer.ConnectionsCount > 0 && (chFCL.Count > 0 || minePlaceList.Count > 0))
                {
                    NetOutgoingMessage outmsg = netServer.CreateMessage();
                    outmsg.Write((byte)PacketTypes.MOVE);
                    outmsg.Write(0);
                    outmsg.Write(chFCL.Count);
                    foreach (int i in chFCL)
                    {
                        outmsg.Write(i);
                    }
                    outmsg.Write(minePlaceList.Count);
                    foreach (Vector2 v in minePlaceList)
                    {
                        outmsg.Write(v.X);
                        outmsg.Write(v.Y);
                    }
                    netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.Unreliable, 0);
                }
            }
            else
            {
                if (respawnTimer < 3000)
                    respawnTimer += gameTime.ElapsedGameTime.Milliseconds;
                else
                {
                    NewMainCharacter();
                    respawnTimer = 0;
                }
            }

            HandleRespawns(gameTime);

            #region check messages
            // Server.ReadMessage() Returns new messages, that have not yet been read.
            // If "inc" is null -> ReadMessage returned null -> Its null, so dont do this :)
            if ((inc = netServer.ReadMessage()) != null)
            {
                // Theres few different types of messages. To simplify this process, i left only 2 of em here
                switch (inc.MessageType)
                {
                    // If incoming message is Request for connection approval
                    // This is the very first packet/message that is sent from client
                    // Here you can do new player initialisation stuff
                    #region new player
                    case NetIncomingMessageType.ConnectionApproval:
                        // Read the first byte of the packet
                        // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                        if (inc.ReadByte() == (byte)PacketTypes.LOGIN && gameWorldState.Count < 8)
                        {
                            Console.WriteLine("Incoming LOGIN");

                            // Approve clients connection ( Its sort of an agreenment. "You can be my client and i will host you" )
                            for (int i = 1; i < 8; i++)
                            {
                                if (otherPlayers[i] == null)
                                {
                                    characterNames[i] = inc.ReadString();
                                    string s = characterNames[i] + " has entered the game";
                                    appendIntercomMessage(s);
                                    PushMessage(s);
                                    characterTankSkins[i] = inc.ReadInt32();
                                    gameWorldState.Add(new Character(characterNames[i],
                                        inc.SenderConnection, i, characterTankSkins[i]));
                                    NewCharacter(i, characterTankSkins[i], characterNames[i], true);
                                    break;
                                }
                            }
                            NetOutgoingMessage outmsg = netServer.CreateMessage();
                            outmsg.Write((byte)12);
                            outmsg.Write(orbList.Count);
                            foreach (Orb o in orbList)
                            {
                                outmsg.WriteAllProperties(o);
                            }
                            netServer.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                        }

                        break;
                    #endregion
                    // Data type is all messages manually sent from client
                    // ( Approval is automated process )
                    #region Character Input
                    case NetIncomingMessageType.Data:
                        byte packet = inc.ReadByte();
                        // Read first byte
                        if (packet == (byte)PacketTypes.MOVE)
                        {
                            // Check who sent the message
                            // This way we know, what character belongs to message sender
                            foreach (Character ch in gameWorldState)
                            {
                                // If stored connection ( check approved message. We stored ip+port there, to character obj )
                                // Find the correct character
                                if (ch.Connection != inc.SenderConnection)
                                    continue;

                                try
                                {
                                    if (otherPlayers[ch.characterIndex] != null)
                                    {
                                        otherPlayers[ch.getIndex()].targetPos = new Vector2(inc.ReadFloat(), inc.ReadFloat());

                                        otherPlayers[ch.getIndex()].targetRotation = inc.ReadFloat();

                                        otherPlayers[ch.getIndex()].targetVelocity = new Vector2(inc.ReadFloat(), inc.ReadFloat());


                                        NetOutgoingMessage outmsg = netServer.CreateMessage();
                                        outmsg.Write((byte)PacketTypes.MOVE);
                                        bool send = false;

                                        int fireCallCount = inc.ReadInt32();
                                        outmsg.Write(ch.getIndex());
                                        outmsg.Write(fireCallCount);

                                        for (int i = 0; i < fireCallCount; i++)
                                        {
                                            int x = inc.ReadInt32();
                                            otherPlayers[ch.getIndex()].FireWeapon(gameTime, x);
                                            outmsg.Write(x);
                                            send = true;
                                        }
                                        int minePlaceList = inc.ReadInt32();
                                        outmsg.Write(minePlaceList);
                                        for (int i = 0; i < minePlaceList; i++)
                                        {
                                            float x = inc.ReadFloat();
                                            float y = inc.ReadFloat();
                                            otherPlayers[ch.getIndex()].PlaceMine(new Vector2(x, y));
                                            outmsg.Write(x);
                                            outmsg.Write(y);
                                            send = true;
                                        }

                                        if (send)
                                            netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.Unreliable, 0);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }

                                break;
                            }
                        }
                        else if (packet == (byte)PacketTypes.MESSAGE)
                        {
                            try
                            {
                                appendIntercomMessage(inc.ReadString());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        break;
                    #endregion
                    #region removing players
                    case NetIncomingMessageType.StatusChanged:
                        // In case status changed
                        // It can be one of these
                        // NetConnectionStatus.Connected;
                        // NetConnectionStatus.Connecting;
                        // NetConnectionStatus.Disconnected;
                        // NetConnectionStatus.Disconnecting;
                        // NetConnectionStatus.None;
                        Console.WriteLine("Potential disconnect");
                        // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                        //message = inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status;
                        if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        {
                            // Find disconnected character and remove it
                            int i = 0;
                            int characterIndex = 0;

                            foreach (Character cha in gameWorldState)
                            {
                                if (cha.Connection == inc.SenderConnection)
                                {
                                    characterIndex = cha.characterIndex;
                                    if (otherPlayers[cha.characterIndex] != null)
                                    {
                                        keepScore.removePlayer(cha.characterIndex);
                                        otherPlayers[cha.characterIndex].DisposeBody();
                                        otherPlayers[cha.characterIndex] = null;
                                    }
                                    gameWorldState.Remove(cha);
                                    string s = characterNames[i] + " has left the game";
                                    appendIntercomMessage(s);
                                    PushMessage(s);
                                    Console.WriteLine("Player " + cha.characterIndex + " removed from game");
                                    break;
                                }
                                i++;
                            }
                            if (gameWorldState.Count > 1)
                            {
                                Console.WriteLine("Sending remove messages to remaining players");
                                NetOutgoingMessage outmsg = netServer.CreateMessage();
                                outmsg.Write((byte)PacketTypes.REMOVE);
                                outmsg.Write(characterIndex);
                                List<NetConnection> conList = new List<NetConnection>();
                                foreach (Character cha in gameWorldState)
                                {
                                    if (cha.characterIndex != 0)
                                        conList.Add(cha.Connection);
                                }
                                netServer.SendMessage(outmsg, conList, NetDeliveryMethod.ReliableOrdered, 0);
                            }
                        }
                        break;
                    #endregion

                    default:
                        // As i statet previously, there are a few other kind of messages also, but i dont cover those in this example
                        // Uncommenting next line, informs you, when ever some other kind of message is received
                        Console.WriteLine(inc.ReadString());
                        break;
                }
            } // If New messages
            #endregion

            #region Periodic Update
            // if 30ms has passed
            if ((time1 + timeToPass1) < DateTime.Now)
            {
                // If there is even 1 client
                if (netServer.ConnectionsCount != 0)
                {
                    // Create new message
                    NetOutgoingMessage outmsg = netServer.CreateMessage();
                    // Write byte
                    outmsg.Write((byte)PacketTypes.WORLDSTATE);
                    // Write Int
                    outmsg.Write(gameWorldState.Count);
                    // Iterate throught all the players in game
                    for (int i = 0; i < gameWorldState.Count; i++)
                    {
                        if (i == 0)
                        {
                            Character thisCharacter = gameWorldState[0];
                            if (mainCharacter != null)
                            {
                                thisCharacter.X = mainCharacter.getPos().X;
                                thisCharacter.Y = mainCharacter.getPos().Y;
                                thisCharacter.VelX = mainCharacter.tankBody.LinearVelocity.X;
                                thisCharacter.VelY = mainCharacter.tankBody.LinearVelocity.Y;
                                thisCharacter.Rotation = mainCharacter.tankBody.Rotation;
                                thisCharacter.health = mainCharacter.health;
                                thisCharacter.energy = mainCharacter.energy;
                                thisCharacter.setBuffs(mainCharacter.getBuffs());
                            }
                            else
                            {
                                thisCharacter.health = 0;
                                thisCharacter.X = ConvertUnits.ToSimUnits(temporaryCamPos.X);
                                thisCharacter.Y = ConvertUnits.ToSimUnits(temporaryCamPos.Y);
                            }
                            outmsg.WriteAllProperties(thisCharacter);
                            
                        }
                        else
                        {
                            if (otherPlayers[gameWorldState[i].characterIndex] != null)
                            {
                                // Write all properties of character, to the message
                                Vector2 p = otherPlayers[gameWorldState[i].characterIndex].getPos();
                                Vector2 v = otherPlayers[gameWorldState[i].characterIndex].tankBody.LinearVelocity;
                                gameWorldState[i].X = p.X;
                                gameWorldState[i].Y = p.Y;
                                gameWorldState[i].VelX = v.X;
                                gameWorldState[i].VelY = v.Y;
                                gameWorldState[i].Rotation = otherPlayers[gameWorldState[i].characterIndex].targetRotation;
                                gameWorldState[i].health = otherPlayers[gameWorldState[i].characterIndex].health;
                                gameWorldState[i].energy = otherPlayers[gameWorldState[i].characterIndex].energy;
                                gameWorldState[i].setBuffs(otherPlayers[gameWorldState[i].characterIndex].getBuffs());
                                //message = gameWorldState[i].Velocity.ToString();
                            }
                            else
                            {
                                gameWorldState[i].health = 0;
                            }
                            outmsg.WriteAllProperties(gameWorldState[i]);
                        }
                    }
                    


                    // Message contains
                    // byte = Type
                    // Int = Player count
                    // Character obj * Player count
                    // Send messsage to clients ( All connections, in reliable order, channel 0)
                    try
                    {
                        //Console.WriteLine(outmsg.ToString());
                        netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.Unreliable, 0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                // Update current time
                time1 = DateTime.Now;
            }
            #endregion
            #region Score update
            if ((time2 + timeToPass2) < DateTime.Now)
            {
                // If there is even 1 client
                if (netServer.ConnectionsCount != 0)
                {
                    // Create new message
                    NetOutgoingMessage outmsg = netServer.CreateMessage();
                    // Write byte
                    outmsg.Write((byte)PacketTypes.SCORE);
                    // Write Int
                    Score[] s = keepScore.getScores();
                    outmsg.Write(s.Length);
                    // Iterate throught all the players in game
                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] != null)
                            outmsg.WriteAllProperties(s[i]);
                        else
                        {
                            Score temp = new Score("");
                            outmsg.WriteAllProperties(temp);
                        }
                    }
                    try
                    {
                        netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                // Update current time
                time2 = DateTime.Now;
            }
            #endregion
            base.Update(gameTime);
        }

        private void HandleRespawns(GameTime gameTime)
        {
            for (int i = 0; i < respawnList.Count; i++)
            {
                if ((respawnList[i])[1] <= 0)
                {
                    NewCharacter(respawnList[i][0], characterTankSkins[respawnList[i][0]], characterNames[respawnList[i][0]], false);
                    respawnList.Remove(respawnList[i]);
                }
                else
                {
                    (respawnList[i])[1] -= gameTime.ElapsedGameTime.Milliseconds;
                }
            }
        }

        private void _exiting(object o, EventArgs e)
        {
            string s = "Server has ended the game";
            appendIntercomMessage(s);
            PushMessage(s);
            netServer.Shutdown("Server Shutdown Un-expectedly");
        }

        //For approving a players connection
        protected override void PlayerEntered(Vector2 position, int index, string name)
        {
            int gameWorldStateIndex = 0;
            bool playerFound = false;
            for (int i = 0; i < gameWorldState.Count; i++)
            {
                if (gameWorldState[i].characterIndex == index)
                {
                    gameWorldState[i].X = position.X;
                    gameWorldState[i].Y = position.Y;
                    gameWorldState[i].characterIndex = index;
                    gameWorldStateIndex = i;
                    playerFound = true;
                    gameWorldState[i].Connection.Approve();
                    //if(!keepScore.samePlayer(name, index))
                    //    keepScore.AddPlayer(index, name);
                    Console.WriteLine("LOGIN approved... Spawning");
                }
            }
            if (playerFound)
            {
                // Create message, that can be written and sent
                NetOutgoingMessage outmsg = netServer.CreateMessage();

                // first we write byte
                outmsg.Write((byte)PacketTypes.WORLDSTATE);

                outmsg.Write(mapNumber);

                // then int
                outmsg.Write(gameWorldState.Count);

                // iterate trough every character ingame
                foreach (Character c in gameWorldState)
                {
                    if (c.characterIndex == index)
                    {
                        c.Self = true;
                        outmsg.WriteAllProperties(c);
                    }
                    else
                    {
                        outmsg.WriteAllProperties(c);
                        Vector2[] mines;
                        if (c.characterIndex == 0)
                            mines = mainCharacter.getMinePos();
                        else
                            mines = otherPlayers[c.characterIndex].getMinePos();

                        outmsg.Write(mines.Length);
                        for (int i = 0; i < mines.Length; i++)
                        {
                            outmsg.Write(mines[i].X);
                            outmsg.Write(mines[i].Y);
                        }
                    }
                    c.Self = false;
                }

                Console.WriteLine("Server sending worldstate");
                netServer.SendMessage(outmsg, gameWorldState[gameWorldStateIndex].Connection, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        protected override void PlayerDied(int index)
        {
            int[] i = new int[2];
            i[0] = index;
            i[1] = 3000;
            respawnList.Add(i);
        }

        protected override void PlaceOrb(Vector2 position, float kD, int characterIndex)
        {
            Orb o = new Orb(orb, position, kD, characterIndex, gameInterface, audio);
            orbList.Add(o);
            NetOutgoingMessage outmsg = netServer.CreateMessage();
            outmsg.Write((byte)PacketTypes.ORB);
            outmsg.WriteAllProperties(o);
            if (netServer.Connections.Count > 0)
                netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        protected override void PushMessage(string message)
        {
            NetOutgoingMessage outmsg = netServer.CreateMessage();
            outmsg.Write((byte)PacketTypes.MESSAGE);
            outmsg.Write(message);
            if (netServer.Connections.Count > 0)
                netServer.SendMessage(outmsg, netServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
            //throw new NotImplementedException();
        }

        public override void EndGame()
        {
            //string s = "Server has closed the game... press escape to return to the main menu";
            //PushMessage(s);
            netServer.Shutdown("Server Closed");
        }
    }
}
