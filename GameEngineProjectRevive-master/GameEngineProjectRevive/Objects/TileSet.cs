using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using GameEngineProjectRevive.Physics;

namespace GameEngineProjectRevive.Objects
{
    public class TileSet
    {
        public Texture2D Tiles;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TileHeight { get; private set; }
        public int TileWidth { get; private set; }
        public string TileSetImage { get; private set; }//
        public int StartingId { get; private set; }//The starting ID of this tileset 
        public Dictionary<int, Tuple<float, int>[]> Animations { get; private set; }
        public Dictionary<int, PolygonCollider> CollisionShapes { get; private set; }

        private int TileColumns;
        private int TileCount;

        public Rectangle GetSourceRectangle(int ID)
        {
            int id = ID - StartingId + 1;
            int XCoord = id % TileColumns;
            int YCoord = (id - XCoord) / TileColumns;

            return new Rectangle(XCoord * TileWidth, YCoord * TileHeight, TileWidth, TileHeight);
        }

        public TileSet(string TilesPath, ContentManager content, int StartingId)
        {
            XmlDocument TilesDoc = new XmlDocument();
            TilesDoc.Load(TilesPath);

            this.StartingId = StartingId;
            XmlNode mainNode = TilesDoc.ChildNodes[1];
            TileWidth = int.Parse(mainNode.Attributes[1].Value);
            TileHeight = int.Parse(mainNode.Attributes[2].Value);
            TileCount = int.Parse(mainNode.Attributes[3].Value);
            TileColumns = int.Parse(mainNode.Attributes[4].Value);
            TileSetImage = mainNode.ChildNodes[0].Attributes[0].Value;
            Tiles = content.Load<Texture2D>(TileSetImage.Substring(0, TileSetImage.Length - 4));

            Width = int.Parse(mainNode.ChildNodes[0].Attributes[1].Value);
            Height = int.Parse(mainNode.ChildNodes[0].Attributes[2].Value);

            CollisionShapes = new Dictionary<int, PolygonCollider>();

            //Now parse stuff involving the collision shapes as well as the tile animations.
            //Console.WriteLine(mainNode.ChildNodes[2].ChildNodes[0].Name);
            for (int i = 2; i < mainNode.ChildNodes.Count; i++)
            {
                int id = int.Parse(mainNode.ChildNodes[i].Attributes[0].Value);
                
                if (mainNode.ChildNodes[i].ChildNodes[0].Name == "objectgroup")
                {
                    //x -> 1
                    //y -> 2

                    float x = float.Parse(mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].Attributes[1].Value);
                    float y = float.Parse(mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].Attributes[2].Value);
                    Polygon ColliderShape = new Polygon();

                    if (mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].ChildNodes.Count > 0)
                    {
                        //Polyline
                        //Just load all the points and add them to the offset vector, so that way it's where it should be
                        string[] pairs = mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].ChildNodes[0].Attributes[0].Value.Split(' ');
                        
                        for (int j = 0; j < pairs.Length; j++)
                        {
                            string[] Coords = pairs[j].Split(',');
                            float xC = x + float.Parse(Coords[0]);
                            float yC = y + float.Parse(Coords[1]);

                            ColliderShape.addPoint(new Vector2(xC, yC));
                        }

                        CollisionShapes.Add(id, new PolygonCollider(ColliderShape));
                    }
                    else
                    {
                        //Rectangular
                        float Width = float.Parse(mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].Attributes[3].Value);
                        float Height = float.Parse(mainNode.ChildNodes[i].ChildNodes[0].ChildNodes[0].Attributes[4].Value);

                        ColliderShape.addPoint(new Vector2(x, y));
                        ColliderShape.addPoint(new Vector2(x + Width, y));
                        ColliderShape.addPoint(new Vector2(x + Width, y + Height));
                        ColliderShape.addPoint(new Vector2(x, y + Height));

                        CollisionShapes.Add(id, new PolygonCollider(ColliderShape));
                    }
                }
            }
        }
    }
}
