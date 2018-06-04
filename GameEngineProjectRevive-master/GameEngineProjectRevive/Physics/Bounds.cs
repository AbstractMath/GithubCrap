using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngineProjectRevive.Physics
{
  public abstract class Bounds
    {
        public Vector2 Position;

        public abstract bool isColliding(AlignedBoundingBox bounds);
    }
}
