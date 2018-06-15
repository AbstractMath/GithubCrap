using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;
using GameEngineProjectRevive.Interfaces;

namespace GameEngineProjectRevive.Physics
{
    public class PolygonCollider : IColliderGroup
    {
        public Polygon Collider { get; private set;  } 

        public Tuple<bool, Vector2> CheckForCollision(IColliderGroup other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize)
        {
            return other.CheckColliderElement(Collider, otherPosition, thisPosition, otherSize, thisSize);
        }

        public AlignedBoundingBox GenerateBounds(Vector2 Size)
        {
            return Collider.GenerateBounds(Size);
        }

        public Tuple<bool, Vector2> CheckColliderElement(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize)
        {
            Tuple<bool, Simplex> CollisionTest = Collider.CheckForCollision(other, thisPosition, otherPosition, thisSize, otherSize);

            if (CollisionTest.Item1)
            {
                return Collider.GetCollisionData(other, thisPosition, otherPosition, thisSize, otherSize);
            }
            else
            {
                return new Tuple<bool, Vector2>(false, Vector2.Zero);
            }
        }

        public void Draw(Color color, Vector2 Position, SpriteBatch batch, Camera cam, Vector2 Scale)
        {
            Collider.Draw(color, Position, batch, cam, Scale);
        }

        public PolygonCollider(Polygon Collider)
        {
            this.Collider = Collider;
        }
    }
}
