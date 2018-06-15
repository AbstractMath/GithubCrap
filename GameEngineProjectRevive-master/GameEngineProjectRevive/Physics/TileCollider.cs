using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using GameEngineProjectRevive.Objects;
using GameEngineProjectRevive.Interfaces;

namespace GameEngineProjectRevive.Physics
{
    public class TileCollider : IColliderGroup
    {
        private PolygonCollider BlockShape; // entirely a placeholder, there will be a whole array of collision tile shapes
        public float TileSize;
        public TileMap Map;

        public Vector2 Corner0 = Vector2.Zero;
        public Vector2 Corner1 = Vector2.Zero;

        private const uint HORIZONTALFLIP = 0x80000000;
        private const uint VERTICALFLIP = 0x40000000;
        private const uint DIAGONALFLIP = 0x20000000;

        public Tuple<bool, Vector2> CheckColliderElement(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize)
        {
            //First figure out where the bounds are in terms of the position of the tile

            Polygon ColliderBox = new Polygon();
            ColliderBox.addPoint(Vector2.Zero);
            ColliderBox.addPoint(new Vector2(TileSize, 0));
            ColliderBox.addPoint(new Vector2(TileSize, TileSize));
            ColliderBox.addPoint(new Vector2(0, TileSize));

            PolygonCollider Box = new PolygonCollider(ColliderBox);

            AlignedBoundingBox otherBounds = other.GenerateBounds(new Vector2(1, 1));

            Vector2 Diff = (thisPosition + new Vector2(Map.Width * TileSize, Map.Height * TileSize) / 2 - otherPosition);
            Vector2 SizeB = new Vector2(Map.Width * TileSize, Map.Height * TileSize) + otherBounds.Size;

            //Vector2 average = topLeftB;
            
            if (Diff.X < SizeB.X / 2 && Diff.X > -SizeB.X / 2 && Diff.Y < SizeB.Y / 2 && Diff.Y > -SizeB.Y / 2)
            {
                Vector2 Dp = otherPosition - thisPosition;

                int x0 = (int)Math.Floor((Dp.X - otherBounds.Size.X / 2) / TileSize);
                int x1 = (int)Math.Ceiling((Dp.X + otherBounds.Size.X / 2) / TileSize);
                int y0 = (int)Math.Floor((Dp.Y - otherBounds.Size.Y / 2) / TileSize);
                int y1 = (int)Math.Ceiling((Dp.Y + otherBounds.Size.Y / 2) / TileSize);



                if (x0 < 0) { x0 = 0; }
                if (x1 >= Map.Width) { x1 = Map.Width; }
                if (y0 < 0) { y0 = 0; }
                if (y1 >= Map.Height) { y1 = Map.Height; }

                Corner0 = new Vector2(x0, y0) * TileSize;
                Corner1 = new Vector2(x1, y1) * TileSize;

                for (int x = x0; x < x1; x++)
                {
                    for (int y = y0; y < y1; y++)
                    {
                        if (Map.TileData[x, y] != -1)
                        {
                            long id = Map.TileData[x, y];
                            int ID;
                            long flippedHorizontal = id & HORIZONTALFLIP;
                            long flippedVertical = id & VERTICALFLIP;
                            long flippedDiagonal = id & DIAGONALFLIP;

                            ID = (int)(id & ~(HORIZONTALFLIP | VERTICALFLIP | DIAGONALFLIP));

                            //
                            //Now check to see if the polygons collide.
                            ColliderElement.FlipProperties FlipType = ColliderElement.FlipProperties.None;

                            if (flippedHorizontal == HORIZONTALFLIP)
                            {
                                FlipType |= ColliderElement.FlipProperties.FlipHorizontal;
                            }
                            if (flippedVertical == VERTICALFLIP)
                            {
                                FlipType |= ColliderElement.FlipProperties.FlipVertical;
                            }
                            if (flippedDiagonal == DIAGONALFLIP)
                            {
                                FlipType |= ColliderElement.FlipProperties.FlipDiagonal;
                            }

                            PolygonCollider CheckAgainst;

                            if (Map.GetTileSet(ID).CollisionShapes.ContainsKey(ID)){ 
                                Map.GetTileSet(ID).CollisionShapes.TryGetValue(ID, out CheckAgainst);
                                Console.WriteLine("Custom shape");
                            }
                            else
                            {
                                //return new Tuple<bool, Vector2>(true, Vector2.Zero);

                                CheckAgainst = Box;
                            }

                            return other.GetCollisionData(CheckAgainst.Collider, otherPosition, thisPosition + new Vector2(x, y) * TileSize, otherSize, thisSize, FlipType, new Vector2(TileSize / 2, TileSize));
                        }
                    }
                }

                return new Tuple<bool, Vector2>(false, Vector2.Zero);
            }
            else
            {
                return new Tuple<bool, Vector2>(false, Vector2.Zero);
            }
        }

        public Tuple<bool, Vector2> CheckForCollision(IColliderGroup other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize)
        {
            return new Tuple<bool, Vector2>(false, Vector2.Zero);
        }

        public AlignedBoundingBox GenerateBounds(Vector2 Scale)
        {
            return new AlignedBoundingBox(Vector2.Zero, Vector2.Zero);
        }
    }
}
