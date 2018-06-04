using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;
using System.Collections.Generic;

namespace GameEngineProjectRevive.Physics
{
    public class Quadtree//I will probably want to make sure this can extend if something is inserted outside of it
    {
        public QuadtreeNode Root { get; private set; }
        public int MaxDepth { get; private set; }

        private void insert(AlignedBoundingBox newBounds, QuadtreeNode current)
        {
            newBounds.ContainerTree = this;
            if (current.isColliding(newBounds) && current.Depth < MaxDepth)
            {
                if (current[0] == null)
                {
                    current.Subdivide();
                }

                insert(newBounds, current[0]);
                insert(newBounds, current[1]);
                insert(newBounds, current[2]);
                insert(newBounds, current[3]);
            }
            else if(current.isColliding(newBounds))
            {
                current.InsertBounds(newBounds);
            }
        }

        private void drawAlg(QuadtreeNode current, Color color, SpriteBatch batch, Camera cam)
        {
            if (current[0] != null)
            {
                drawAlg(current[0], color, batch, cam);
                drawAlg(current[1], color, batch, cam);
                drawAlg(current[2], color, batch, cam);
                drawAlg(current[3], color, batch, cam);
            }
            else
            {
                current.Draw(color, batch, cam);
            }
        }

        public void Draw(Color color, SpriteBatch batch, Camera cam)
        {
            batch.Begin();
            drawAlg(Root, color, batch, cam);
            batch.End();
        }

        private void collisionSearch(AlignedBoundingBox searchBounds, QuadtreeNode current, ref LinkedList<AlignedBoundingBox> list)
        {
            if (current.isColliding(searchBounds) && current[0] != null)
            {
                collisionSearch(searchBounds, current[0], ref list);
                collisionSearch(searchBounds, current[1], ref list);
                collisionSearch(searchBounds, current[2], ref list);
                collisionSearch(searchBounds, current[3], ref list);
            }
            else if(current[0] == null)
            {
                if (current.Boxes.Count != 0)
                {
                    foreach (AlignedBoundingBox b in current.Boxes)
                    {
                        if (b.isColliding(searchBounds) && !list.Contains(b) && b != searchBounds)
                        {
                            list.AddFirst(b);
                        }
                    }
                }
            }
        }

        public void InsertBound(AlignedBoundingBox newBounds)
        {
            insert(newBounds, Root);
        }

        public LinkedList<AlignedBoundingBox> GetColliding(AlignedBoundingBox searchBounds)
        {
            LinkedList<AlignedBoundingBox> result = new LinkedList<AlignedBoundingBox>();
            collisionSearch(searchBounds, Root, ref result);

            return result;
        }

        public Quadtree(Vector2 Position, float Size, int MaxDepth)
        {
            this.MaxDepth = MaxDepth;
            Root = new QuadtreeNode(Size, Position, null, 1);
        }
    }
}
