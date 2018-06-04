using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;
using System.Collections.Generic;

//Note to self: I have to clean this code up before too long

namespace GameEngineProjectRevive.Physics
{
    public class AlignedBoundingBox : Bounds
    {
        public Vector2 Size;
        public PhysicsObject Container;
        public Quadtree ContainerTree;
        public Vector2 RelativeColliderPosition;
        public new Vector2 Position {
            get
            {
                return Container.Position;
            }
        }

        public override bool isColliding(AlignedBoundingBox bounds)
        {
            Vector2 PDiff = this.Position - bounds.Position;
            Vector2 PSize = this.Size + bounds.Size;
            return (
                PDiff.X < PSize.X / 2 && PDiff.X > -PSize.X / 2 && 
                PDiff.Y < PSize.Y / 2 && PDiff.Y > -PSize.Y / 2);
        }

        public void Draw(Color color, SpriteBatch batch, Camera cam)
        {
            Vector2 c0 = Position + new Vector2(Size.X / 2, Size.Y / 2);
            Vector2 c1 = Position + new Vector2(-Size.X / 2, Size.Y / 2);
            Vector2 c2 = Position + new Vector2(-Size.X / 2, -Size.Y / 2);
            Vector2 c3 = Position + new Vector2(Size.X / 2, -Size.Y / 2);
            batch.Begin();
            GameWindow.DrawLine(c0, c1, color, cam, batch);
            GameWindow.DrawLine(c1, c2, color, cam, batch);
            GameWindow.DrawLine(c2, c3, color, cam, batch);
            GameWindow.DrawLine(c3, c0, color, cam, batch);
            batch.End();
        }

        public AlignedBoundingBox(Vector2 Position, Vector2 Size)
        {
            this.Size = Size;
        }
    }
}
