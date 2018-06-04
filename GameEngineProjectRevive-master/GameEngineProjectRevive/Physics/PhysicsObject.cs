using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Interfaces;
using GameEngineProjectRevive.Objects;

namespace GameEngineProjectRevive.Physics
{
    public abstract class PhysicsObject//Must have a Game Object attached to it.
    {
        public Vector2 Position
        {
            get
            {
                return gameObject.GlobalTranslation;
            }
        }
        protected Vector2 ColliderRelativePosition;
        public IColliderGroup Collider;
        public AlignedBoundingBox BoundingShape;
        public GameObject gameObject;//This name will change

        public abstract void Update(GameTime time);
        public abstract LinkedList<Tuple<PhysicsObject, Vector2>> getColliding();

        public PhysicsObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}
