using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.DemoBaseXNA;
using System.Collections.Generic;

namespace GameFinal
{
    class TileMap
    {

        #region Variables
        List<Body> tileBodyList = new List<Body>();
        List<Fixture> tileFixtureList = new List<Fixture>();
        List<Texture2D> tileTexList;
        List<Vector2> respawnPoints;
        int[,] tileArray;
        Vector2 position;
        public Point canvasSize = new Point(0, 0);
        const int tileSize = 25;

        //Changeable Variables...
        float tileRestitution = 0.01f;
        float tileFriction = 0.01f;

        Point backgroundSize;
        #endregion

        //public TileMap(Vector2 position, List<Texture2D> tileTexList, int[,] tileArray, InGame parentGame)
        //{
        //    this.position = position;
        //    this.tileTexList = tileTexList;
        //    this.tileArray = tileArray;
        //    this.backgroundSize = new Point(tileTexList[0].Width, tileTexList[0].Height);
        //    canvasSize = new Point(tileArray.GetLength(1) * tileSize, tileArray.GetLength(0) * tileSize);
        //    respawnPoints = new List<Vector2>();

        //    #region Adding Tiles to simulator
        //    for (int y = 0; y < tileArray.GetLength(0); y++)
        //    {
        //        for (int x = 0; x < tileArray.GetLength(1); x++)
        //        {
        //            int tileIndex = tileArray[y, x];
        //            if (tileIndex != 0 && tileIndex <= tileTexList.Count - 1)
        //            {
        //                Fixture tileFixture;

        //                #region tileIndex1
        //                if (tileIndex == 1)
        //                {

        //                    tileFixture = FixtureFactory.CreateRectangle(parentGame.world,
        //                        ConvertUnits.ToSimUnits(tileSize),
        //                        ConvertUnits.ToSimUnits(tileSize),
        //                        1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x) + (tileSize / 2), (tileSize * y) + (tileSize / 2)) + position);

        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex2
        //                else if (tileIndex == 2)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 9)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 9)));


        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex3
        //                else if (tileIndex == 3)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 9)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 15)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex4
        //                else if (tileIndex == 4)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 15)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 15)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex5
        //                else if (tileIndex == 5)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 9)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 15)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);

        //                }
        //                #endregion
        //                #region tileIndex6
        //                else if (tileIndex == 6)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 14)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex7
        //                else if (tileIndex == 7)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(24, 15)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(14, 24)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex8
        //                else if (tileIndex == 8)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 9)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(10, 0)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //                #region tileIndex9
        //                else if (tileIndex == 9)
        //                {
        //                    Vertices verts = new Vertices();
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 0)));
        //                    verts.Add(ConvertUnits.ToSimUnits(new Vector2(24, 10)));

        //                    tileFixture = FixtureFactory.CreatePolygon(parentGame.world, verts, 1);
        //                    tileFixture.Friction = tileFriction;
        //                    tileFixture.Restitution = tileRestitution;
        //                    Body tileBody = tileFixture.Body;
        //                    tileBody.IsBullet = false;
        //                    tileFixture.Body.BodyType = BodyType.Static;
        //                    tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
        //                    tileBodyList.Add(tileBody);
        //                    tileFixtureList.Add(tileFixture);
        //                }
        //                #endregion
        //            }
        //            #region respawnPoint
        //            else if (tileIndex == 100)
        //            {
        //                respawnPoints.Add(new Vector2(tileSize * x, tileSize * y));
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion
        //}

        public TileMap(Vector2 position, List<Texture2D> tileTexList, int[,] tileArray, World world)
        {
            this.position = position;
            this.tileTexList = tileTexList;
            this.tileArray = tileArray;
            this.backgroundSize = new Point(tileTexList[0].Width, tileTexList[0].Height);
            canvasSize = new Point(tileArray.GetLength(1) * tileSize, tileArray.GetLength(0) * tileSize);
            respawnPoints = new List<Vector2>();

            #region Adding Tiles to simulator
            for (int y = 0; y < tileArray.GetLength(0); y++)
            {
                for (int x = 0; x < tileArray.GetLength(1); x++)
                {
                    int tileIndex = tileArray[y, x];
                    if (tileIndex != 0 && tileIndex <= tileTexList.Count - 1)
                    {
                        Fixture tileFixture;

                        #region tileIndex1
                        if (tileIndex == 1)
                        {

                            tileFixture = FixtureFactory.CreateRectangle(world,
                                ConvertUnits.ToSimUnits(tileSize),
                                ConvertUnits.ToSimUnits(tileSize),
                                1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x) + (tileSize / 2), (tileSize * y) + (tileSize / 2)) + position);

                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex2
                        else if (tileIndex == 2)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 9)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 9)));


                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex3
                        else if (tileIndex == 3)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 9)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 15)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex4
                        else if (tileIndex == 4)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 15)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 15)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex5
                        else if (tileIndex == 5)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 9)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 15)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);

                        }
                        #endregion
                        #region tileIndex6
                        else if (tileIndex == 6)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(9, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 14)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex7
                        else if (tileIndex == 7)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(24, 15)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(14, 24)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex8
                        else if (tileIndex == 8)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 9)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(10, 0)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                        #region tileIndex9
                        else if (tileIndex == 9)
                        {
                            Vertices verts = new Vertices();
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(25, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 25)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(0, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(15, 0)));
                            verts.Add(ConvertUnits.ToSimUnits(new Vector2(24, 10)));

                            tileFixture = FixtureFactory.CreatePolygon(world, verts, 1);
                            tileFixture.Friction = tileFriction;
                            tileFixture.Restitution = tileRestitution;
                            Body tileBody = tileFixture.Body;
                            tileBody.IsBullet = false;
                            tileFixture.Body.BodyType = BodyType.Static;
                            tileBody.Position = ConvertUnits.ToSimUnits(new Vector2((tileSize * x), (tileSize * y)) + position);
                            tileBodyList.Add(tileBody);
                            tileFixtureList.Add(tileFixture);
                        }
                        #endregion
                    }
                    #region respawnPoint
                    else if (tileIndex == 100)
                    {
                        respawnPoints.Add(new Vector2(tileSize * x, tileSize * y));
                    }
                    #endregion
                }
            }
            #endregion
        }

        public List<Vector2> getRespawnPoints()
        {
            return respawnPoints;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < tileArray.GetLength(0); y += (tileTexList[0].Height / 50))
            {
                int yHeight = tileTexList[0].Height;
                if (tileArray.GetLength(0) - y < (tileTexList[0].Height / 50))
                {
                    yHeight = (tileArray.GetLength(0) - y) * 50;
                }
                for (int x = 0; x < tileArray.GetLength(1); x += (tileTexList[0].Width / 50))
                {
                    if (tileArray.GetLength(1) - x < (tileTexList[0].Height / 50))
                    {
                        int xWidth = (tileArray.GetLength(1) - x) * 50;
                        spriteBatch.Draw(tileTexList[0],
                            new Vector2(x * tileSize, y * tileSize),
                            new Rectangle(0, 0, xWidth, yHeight),
                            Color.White,
                            0,
                            Vector2.Zero,
                            0.5f,
                            SpriteEffects.None,
                            1);
                    }
                    else
                    {
                        spriteBatch.Draw(tileTexList[0],
                            new Vector2(x * tileSize, y * tileSize),
                            new Rectangle(0, 0, tileTexList[0].Width, tileTexList[0].Height),
                            Color.White,
                            0,
                            Vector2.Zero,
                            0.5f,
                            SpriteEffects.None,
                            1);
                    }
                }
            }

            for (int y = 0; y < tileArray.GetLength(0); y++)
            {
                for (int x = 0; x < tileArray.GetLength(1); x++)
                {
                    int textureIndex = tileArray[y, x];
                    if (textureIndex != 0 && textureIndex <= tileTexList.Count - 1)
                    {
                        Texture2D texture = tileTexList[textureIndex];
                        spriteBatch.Draw(texture,
                            new Vector2(tileSize * x, tileSize * y) + position,
                            null,
                            Color.White,
                            0,
                            Vector2.Zero,
                            0.25f,
                            SpriteEffects.None,
                            0.9f);
                    }
                }
            }

        }

    }
}
