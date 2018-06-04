using System;
using Microsoft.Xna.Framework;
using GameEngineProjectRevive.Physics;

namespace GameEngineProjectRevive.Interfaces
{
    public interface ICollider
    {
        Tuple<bool, Simplex> CheckForCollision(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2);
        Vector2 GetCollisionDepth(Simplex simp, ColliderElement other, Vector2 thisPosition, Vector2 otherPositon, Vector2 Scale1, Vector2 Scale2);

    }
}
