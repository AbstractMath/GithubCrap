using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Objects;
using GameEngineProjectRevive.Physics;

namespace GameEngineProjectRevive.Interfaces
{
    /// <summary>
    /// Made up a bunch of different collider elements
    /// </summary>
    public interface IColliderGroup
    {
        /// <summary>
        /// This method checks for collision against another IColliderGroup object.
        /// </summary>
        /// <param name="other">The IColliderGroup object to check against</param>
        /// <param name="thisPosition">The position where this collider is at</param>
        /// <param name="otherPosition">The position where the other IColliderGroup is</param>
        /// <param name="thisSize">The size of this collider group</param>
        /// <param name="otherSize">The size of the other IColliderGroup object </param>
        /// <returns>A tuple containing a bool of whether or not there was a collision, and a Vector2 that indicates the penetration depth and direction</returns>
        Tuple<bool, Vector2> CheckForCollision(IColliderGroup other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize);

        /// <summary>
        /// Generates a new bounding box that encloses the entire IColliderGroup object
        /// </summary>
        /// <param name="Size">The size of the collider object</param>
        /// <returns>A bounding box for the quadtree</returns>
        AlignedBoundingBox GenerateBounds(Vector2 Size);

        Tuple<bool, Vector2> CheckColliderElement(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 thisSize, Vector2 otherSize);
    }
}
