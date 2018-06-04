using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;

//For me tomorrow: Make this nasty as shit look spiffy as shit dog! Also add raycasting. 
namespace GameEngineProjectRevive.Physics
{
    public class QuadtreeNode : Bounds
    {
        private QuadtreeNode child0;
        private QuadtreeNode child1;
        private QuadtreeNode child2;
        private QuadtreeNode child3;

        public int Depth { get; private set; }
        public float Size { get; private set; }//Float because these are always square
        public QuadtreeNode this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return child0;
                    case 1:
                        return child1;
                    case 2:
                        return child2;
                    case 3:
                        return child3;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
        public QuadtreeNode Parent { get; private set; }
        public LinkedList<AlignedBoundingBox> Boxes { get; private set; }

        public void InsertBounds(AlignedBoundingBox newBounds)
        {
            Boxes.AddLast(newBounds);
        }

        public override bool isColliding(AlignedBoundingBox bounds)
        {
            Vector2 PDiff = this.Position - bounds.Position;
            Vector2 PSize = bounds.Size + new Vector2(this.Size, this.Size);
            return (
                PDiff.X < PSize.X / 2 && PDiff.X > -PSize.X / 2 &&
                PDiff.Y < PSize.Y / 2 && PDiff.Y > -PSize.Y / 2);
        }

        public void Draw(Color color, SpriteBatch batch, Camera cam)
        {
            Vector2 c0 = Position + new Vector2(Size / 2, Size / 2);
            Vector2 c1 = Position + new Vector2(-Size / 2, Size / 2);
            Vector2 c2 = Position + new Vector2(-Size / 2, -Size / 2);
            Vector2 c3 = Position + new Vector2(Size / 2, -Size / 2);

            GameWindow.DrawLine(c0, c1, color, cam, batch);
            GameWindow.DrawLine(c1, c2, color, cam, batch);
            GameWindow.DrawLine(c2, c3, color, cam, batch);
            GameWindow.DrawLine(c3, c0, color, cam, batch);
        }

        public void Subdivide()
        {
            float newSize = this.Size / 2;
            child0 = new QuadtreeNode(newSize, this.Position + newSize / 2 * new Vector2(1, 1), this, Depth + 1);
            child1 = new QuadtreeNode(newSize, this.Position + newSize / 2 * new Vector2(-1, 1), this, Depth + 1);
            child2 = new QuadtreeNode(newSize, this.Position + newSize / 2 * new Vector2(-1, -1), this, Depth + 1);
            child3 = new QuadtreeNode(newSize, this.Position + newSize / 2 * new Vector2(1, -1), this, Depth + 1);
        }

        public QuadtreeNode(float Size, Vector2 Position, QuadtreeNode Parent, int Depth)
        {
            this.Size = Size;
            this.Position = Position;
            this.Parent = Parent;
            this.Depth = Depth;
            Boxes = new LinkedList<AlignedBoundingBox>();
        }
    }
}
