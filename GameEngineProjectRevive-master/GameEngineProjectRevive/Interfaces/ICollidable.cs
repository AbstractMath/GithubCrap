using System;
using Microsoft.Xna.Framework;
using GameEngineProjectRevive.Physics;

namespace GameEngineProjectRevive.Interfaces
{
    public interface ICollidable
    {
        Vector2 Support(Vector2 Direction);
        Vector2 GetCentroid();
    }
}