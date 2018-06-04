using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;
using GameEngineProjectRevive.Interfaces;

namespace GameEngineProjectRevive.Physics
{
    public class StaticObject : PhysicsObject//Make sure you add a static reference to the quadtrees in here. Maybe even a static array of them
    {
        public override void Update(GameTime time)//We don't really need this for the static object
        {
            
        }

        public override LinkedList<Tuple<PhysicsObject, Vector2>> getColliding()//I may want to change this a little in the dynamic physics object method
        {
            LinkedList<Tuple<PhysicsObject, Vector2>> result = new LinkedList<Tuple<PhysicsObject, Vector2>>();

            LinkedList<AlignedBoundingBox> broadPhase = this.BoundingShape.ContainerTree.GetColliding(this.BoundingShape);

            foreach(AlignedBoundingBox b in broadPhase)
            {
                Tuple<bool, Vector2> CollisionResult = b.Container.Collider.CheckForCollision(this.Collider, b.Position + b.RelativeColliderPosition, this.Position + this.ColliderRelativePosition, new Vector2(1, 1), new Vector2(1, 1));
                if (CollisionResult.Item1)
                {
                    result.AddLast(new Tuple<PhysicsObject, Vector2>(b.Container, CollisionResult.Item2));
                }
            }

            return result;
        }

        public StaticObject(IColliderGroup Collider, GameObject gameObject) 
            : base(gameObject)//Scale will be added later
        {
            this.Collider = Collider;
            this.BoundingShape = Collider.GenerateBounds(new Vector2(1, 1));
            this.BoundingShape.Container = this;
            this.ColliderRelativePosition = BoundingShape.RelativeColliderPosition;

        }
    }
}
