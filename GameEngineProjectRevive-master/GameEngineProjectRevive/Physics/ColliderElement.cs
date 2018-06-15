using System;
using GameEngineProjectRevive.Interfaces;
using Microsoft.Xna.Framework;

namespace GameEngineProjectRevive.Physics
{
    public delegate Vector2 SupportDelegate(Vector2 direction);

    public abstract class ColliderElement : ICollidable
    {
        public enum FlipProperties
        {
            None = 0x0000,
            FlipHorizontal = 0x100,
            FlipVertical = 0x010,
            FlipDiagonal = 0x001
        }

        //Implement bitfield enums for flipping diagonally, horizontally, and vertically.
        //Implement enum input overloads for collision methods w/ flipping enum inputs
        public Vector2 Size;
        public Vector2 relativeBoundPosition;

        #region ABSTRACT METHODS
        /// <summary>
        /// A method that implements the support function for this particular shape
        /// </summary>
        /// <param name="direction">The starting direction that GJK uses</param>
        /// <returns>The result of the support function</returns>
        public abstract Vector2 Support(Vector2 direction);
        /// <summary>
        /// Probably going to be depricated
        /// </summary>
        /// <returns>The centroid of this shape</returns>
        public abstract Vector2 GetCentroid();

        public abstract Tuple<bool, Simplex> CheckForCollision(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2);
        public abstract Tuple<bool, Simplex> CheckForCollision(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint);

        public abstract Vector2 GetCollisionDepth(Simplex simp, ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint);
        public abstract Vector2 GetCollisionDepth(Simplex simp, ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2);

        public abstract Tuple<bool, Vector2> GetCollisionData(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2);
        public abstract Tuple<bool, Vector2> GetCollisionData(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint);

        public abstract AlignedBoundingBox GenerateBounds(Vector2 Scale);
        #endregion

        protected Vector2 GetPenetrationDepth(Simplex simp, SupportDelegate support2, Vector2 startDirection, Vector2 Scale1, Vector2 Scale2)
        {
            return GetPenetrationDepth(simp, support2, startDirection, Scale1, Scale2, FlipProperties.None, Vector2.Zero);
        }

        #region IMPLEMENTED METHODS
        /// <summary>
        /// Gets the smallest vector required to move the shapes out of each other
        /// </summary>
        /// <param name="simp">The resulting simplex of the boolean GJK algorithm</param>
        /// <param name="support2">The support method of the other shape</param>
        /// <param name="startDirection">The starting direction for this algorithm</param>
        protected Vector2 GetPenetrationDepth(Simplex simp, SupportDelegate support2, Vector2 startDirection, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint)
        {
            Polygon polytope = simp.SimplexToPolygon();
            float lastDistance = -1;
            float currentDistance = 0;
            Vector2 result = Vector2.Zero;

            while (true)
            {
                currentDistance = result.Length();

                Tuple<Vector2, PolygonPoint> closest = polytope.GetClosestPoint(Vector2.Zero, startDirection);

                polytope.addPoint(Scale1 * Support(closest.Item1) - Scale2 * FlippedSupport(support2, -closest.Item1, flipPoint, flipType) + startDirection, closest.Item2);

                result = closest.Item1;

                if (currentDistance - lastDistance <= 0.0001)
                {
                    return result;
                }

                lastDistance = currentDistance;
            }
        }

        protected Tuple<bool, Simplex> IsColliding(Vector2 startDirection, SupportDelegate support2, Vector2 Scale1, Vector2 Scale2)
        {
            return IsColliding(startDirection, support2, Scale1, Scale2, FlipProperties.None, Vector2.Zero);
        }

        internal Vector2 FlipByPoint(Vector2 point, Vector2 origin, FlipProperties flipType)
        {
            Vector2 Transformed = point - origin;

            if ((flipType & FlipProperties.FlipVertical) == FlipProperties.FlipVertical)
            {
                Transformed = new Vector2(Transformed.X, -Transformed.Y);
            }
            if ((flipType & FlipProperties.FlipHorizontal) == FlipProperties.FlipHorizontal)
            {
                Transformed = new Vector2(-Transformed.X, Transformed.Y);
            }
            if((flipType & FlipProperties.FlipDiagonal) == FlipProperties.FlipDiagonal)
            {
                Transformed = new Vector2(Transformed.Y, Transformed.X);
            }

            return origin + Transformed;
        }

        internal Vector2 FlippedSupport(SupportDelegate support, Vector2 direction, Vector2 flipPoint, FlipProperties flipType)
        {
            return FlipByPoint(support(FlipByPoint(direction, Vector2.Zero, flipType)), flipPoint, flipType);
        }

        /// <summary>
        /// Returns a simplex and a boolean representing whether or not two objects collide
        /// </summary>
        /// <param name="startDirection">The starting direction for GJK</param>
        /// <param name="support2">The support function for the other shape</param>
        protected Tuple<bool, Simplex> IsColliding(Vector2 startDirection, SupportDelegate support2, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint)
        {
            //The flip options will only have to do with the second object, not the first. It shouldn't matter which one, but it should still be noted in case of confusion later on. 
            //If necessary, I can extend this the same way I did originally to include a way to flip 

            //So for the flipping procedure, we need to get the support of the flipped direction, and then flip the output of the support function. 
            Simplex simplex = new Simplex();
            Vector2 direction = startDirection;

            simplex.add(Scale1 * Support(direction) - Scale2 * FlippedSupport(support2, -direction, flipPoint, flipType) + startDirection);

            direction = -direction;

            while (true)
            {
                simplex.add(Scale1 * Support(direction) - Scale2 * FlippedSupport(support2, -direction, flipPoint, flipType) + startDirection);

                if (Vector2.Dot(simplex[simplex.Count - 1], direction) <= 0)
                {
                    return new Tuple<bool, Simplex>(false, simplex);
                }
                else
                {
                    Vector2 a = simplex[simplex.Count - 1];
                    Vector2 ao = -a;

                    if (simplex.Count == 3)
                    {
                        Vector2 b = simplex[1];
                        Vector2 c = simplex[0];

                        Vector2 ab = b - a;
                        Vector2 ac = c - a;

                        Vector2 abPerp = Simplex.TripleProduct(ac, ab, ab);
                        Vector2 acPerp = Simplex.TripleProduct(ab, ac, ac);

                        if (Vector2.Dot(abPerp, ao) > 0)
                        {
                            simplex.remove(0);
                            direction = abPerp;
                        }
                        else if (Vector2.Dot(acPerp, ao) > 0)
                        {
                            simplex.remove(1);
                            direction = acPerp;
                        }
                        else
                        {
                            return new Tuple<bool, Simplex>(true, simplex);
                        }
                    }
                    else
                    {
                        Vector2 b = simplex[0];
                        Vector2 ab = b - a;
                        Vector2 bcPerp = Simplex.TripleProduct(ab, ao, ab);
                        direction = bcPerp;
                    }
                }
            }
            #endregion
        }
    }
}
