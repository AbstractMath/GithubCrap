using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameEngineProjectRevive.Objects
{
    public class TileMap : GameObject
    {
        public long[,] TileData { get; private set; }
        public TileSet[] TileSets { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private const uint HORIZONTALFLIP = 0x80000000;
        private const uint VERTICALFLIP = 0x40000000;
        private const uint DIAGONALFLIP = 0x20000000;

        public override void Render(Camera activeCamera, GraphicsDevice device, SpriteBatch batch)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    long id = TileData[x, y];
                    if (id != -1)
                    {
                        int tileSetId = 0;

                        long flippedHorizontal = id & HORIZONTALFLIP;
                        long flippedVertical = id & VERTICALFLIP;
                        long flippedDiagonal = id & DIAGONALFLIP;

                        int ID = (int)(id & ~(HORIZONTALFLIP | VERTICALFLIP | DIAGONALFLIP));

                        for (int i = 0; i < TileSets.Length; i++)
                        {
                            if (ID + 1 >= TileSets[i].StartingId)
                            {
                                tileSetId = i;
                            }
                        }

                        Rectangle source = TileSets[tileSetId].GetSourceRectangle(ID);
                        Rectangle destination;

                        destination = new Rectangle(
                            (int)(GlobalTranslation.X + Scale.X * x * TileSets[tileSetId].TileWidth - activeCamera.Translation.X + TileSets[tileSetId].TileWidth / 2),
                            (int)(GlobalTranslation.Y + Scale.Y * y * TileSets[tileSetId].TileHeight - activeCamera.Translation.Y + TileSets[tileSetId].TileHeight / 2), 

                            (int)(Scale.X * TileSets[tileSetId].TileWidth), 
                            (int)(Scale.Y * TileSets[tileSetId].TileHeight));

                        SpriteEffects s = SpriteEffects.None;
                        if (flippedHorizontal == HORIZONTALFLIP)
                        {
                            s = SpriteEffects.FlipHorizontally;
                        }

                        if (flippedVertical == VERTICALFLIP || flippedDiagonal == DIAGONALFLIP)
                        {
                            
                            s |= SpriteEffects.FlipVertically;
                        }

                        batch.Draw(TileSets[tileSetId].Tiles, destination, source, Color.White, flippedDiagonal == DIAGONALFLIP ? -(float)Math.PI / 2 : 0, new Vector2(TileSets[tileSetId].TileWidth / 2, TileSets[tileSetId].TileHeight / 2), s, 0);
                        //batch.Draw(TileSets[tileSetId].Tiles, destination, source, Color.White);
                    }
                }
            }
        }

        public TileMap(string TilePath, int layerID, ContentManager content)//This should probably have some kind of overload to load tilesets externally, saving a little memory.
        {
            XmlDocument TileDoc = new XmlDocument();
            TileDoc.Load(TilePath);
            
            foreach(XmlNode n in TileDoc)
            {
                if (n.Name == "map")
                {
                    int tileSetCount = 0;

                    foreach(XmlNode tileSet in n.ChildNodes)
                    {
                        if (tileSet.Name == "tileset")
                        {
                            tileSetCount++;
                        }
                    }

                    TileSets = new TileSet[tileSetCount];

                    for (int i = 0; i < tileSetCount; i++)
                    {
                        TileSets[i] = new TileSet("TileSets/" + n.ChildNodes[i].Attributes[1].Value, content, int.Parse(n.ChildNodes[i].Attributes[0].Value));
                    }

                    XmlNode layerData = n.ChildNodes[tileSetCount + layerID - 1];
                    Width = int.Parse(layerData.Attributes[1].Value);
                    Height = int.Parse(layerData.Attributes[2].Value);

                    TileData = new long[Width, Height];

                    string[] raw = layerData.ChildNodes[0].InnerText.Split(',');
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            //Console.WriteLine(raw[y * Width + x]);
                            long buffer = 0;
                            long.TryParse(raw[y * Width + x], out buffer);
                            //Console.WriteLine(buffer);

                            /*if (buffer == 0 && !raw[y * Width + x].Equals("0"))
                            {
                                buffer = 10;
                            }*/

                            TileData[x, y] = buffer - 1;
                        }
                    }
                }
            }
        }
    }

    
}
