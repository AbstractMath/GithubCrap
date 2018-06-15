using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngineProjectRevive.Interfaces;
using GameEngineProjectRevive.Objects;
using System;

namespace GameEngineProjectRevive.Physics
{
    public class Polygon : ColliderElement
    {
        public PolygonPoint First { get; private set; }

        public override Tuple<bool, Simplex> CheckForCollision(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2)
        {
            return base.IsColliding(thisPosition - otherPosition, new SupportDelegate(other.Support), Scale1, Scale2);
        }

        public override Tuple<bool, Simplex> CheckForCollision(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint)
        {
            return base.IsColliding(thisPosition - otherPosition, new SupportDelegate(other.Support), Scale1, Scale2, flipType, flipPoint);
        }

        public override Tuple<bool, Vector2> GetCollisionData(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2)
        {
            Tuple<bool, Simplex> DidCollide = CheckForCollision(other, thisPosition, otherPosition, Scale1, Scale2);

            if (DidCollide.Item1)
            {
                return new Tuple<bool, Vector2>(true, GetCollisionDepth(DidCollide.Item2, other, thisPosition, otherPosition, Scale1, Scale2));
            }
            else
            {
                return new Tuple<bool, Vector2>(false, new Vector2(0, 0));
            }
        }

        public override Tuple<bool, Vector2> GetCollisionData(ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint)
        {
            Tuple<bool, Simplex> DidCollide = CheckForCollision(other, thisPosition, otherPosition, Scale1, Scale2, flipType, flipPoint);

            if (DidCollide.Item1)
            {
                return new Tuple<bool, Vector2>(true, GetCollisionDepth(DidCollide.Item2, other, thisPosition, otherPosition, Scale1, Scale2, flipType, flipPoint));
            }
            else
            {
                return new Tuple<bool, Vector2>(false, new Vector2(0, 0));
            }
        }

        public override AlignedBoundingBox GenerateBounds(Vector2 Scale)
        {
            Vector2 minVector = new Vector2(0, 0);
            Vector2 maxVector = new Vector2(0, 0);
            bool doTerminate = false;
            PolygonPoint current = this.First;

            while (current != First || !doTerminate)
            {
                if (current.Point.X <= minVector.X)
                {
                    minVector.X = current.Point.X;
                }
                if (current.Point.Y <= minVector.Y)
                {
                    minVector.Y = current.Point.Y;
                }

                if (current.Point.X >= maxVector.X)
                {
                    maxVector.X = current.Point.X;
                }
                if (current.Point.Y >= maxVector.Y)
                {
                    maxVector.Y = current.Point.Y;
                }

                current = current.Next;

                doTerminate = true;
            }

            Vector2 Position = (minVector + maxVector) / 2;
            Vector2 Size = maxVector - minVector;
            AlignedBoundingBox result = new AlignedBoundingBox(new Vector2(0, 0), Size);
            result.RelativeColliderPosition = -Position;

            return result;
        }

        public override Vector2 GetCollisionDepth(Simplex simp, ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2)
        {
            return base.GetPenetrationDepth(simp, new SupportDelegate(other.Support), thisPosition - otherPosition, Scale1, Scale2);
        }

        public override Vector2 GetCollisionDepth(Simplex simp, ColliderElement other, Vector2 thisPosition, Vector2 otherPosition, Vector2 Scale1, Vector2 Scale2, FlipProperties flipType, Vector2 flipPoint)
        {
            return base.GetPenetrationDepth(simp, new SupportDelegate(other.Support), thisPosition - otherPosition, Scale1, Scale2, flipType, flipPoint);
        }

        public override Vector2 Support(Vector2 direction)
        {
            Vector2 result = Vector2.Zero;
            float dot = -1;
            bool doTerminate = false;
            PolygonPoint current = this.First;

            while (current != First || !doTerminate)
            {
                if (dot < Vector2.Dot(direction, current.Point))
                {
                    dot = Vector2.Dot(direction, current.Point);
                    result = current.Point;
                }

                current = current.Next;
                doTerminate = true;
            }

            return result;
        }

        public override Vector2 GetCentroid()
        {
            return Vector2.Zero;
        }

        public void Draw(Color color, Vector2 Position, SpriteBatch batch, Camera cam, Vector2 Scale)//For debugging only!
        {
            bool doTerminate = false;
            PolygonPoint current = this.First;

            batch.Begin();
            while (current.Next != First.Next || !doTerminate)
            {
                GameWindow.DrawLine(Position + Scale * current.Point, Position + Scale * current.Next.Point, color, cam, batch);
                current = current.Next;
                doTerminate = true;
            }
            batch.End();
        }

        public Tuple<Vector2, PolygonPoint> GetClosestPoint(Vector2 point, Vector2 position)
        {
            PolygonPoint current = First;
            PolygonPoint precedingPoint = First;
            Vector2 currentPoint = Vector2.Zero;
            float dist = int.MaxValue;
            bool doTerminate = false;

            while(current != First || !doTerminate)
            {
                Vector2 buffP = closestToEdge(current.Point, current.Next.Point, point);

                if (dist > (buffP - point).Length())
                {
                    dist = (buffP - point).Length();
                    currentPoint = buffP;
                    precedingPoint = current;
                }

                doTerminate = true;

                current = current.Next;
            }

            return new Tuple<Vector2, PolygonPoint>(currentPoint, precedingPoint);
        }

        private Vector2 closestToEdge(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = (b - a);
            ab.Normalize();

            return a + ab * Vector2.Dot(p - a, ab);
        }

        public void addPoint(Vector2 pt)
        {
            PolygonPoint newPt = new PolygonPoint(pt);
            if (First == null)
            {
                First = newPt;
            }
            else if (First.Previous == null)
            {
                First.Next = newPt;
                First.Previous = newPt;
                newPt.Next = First;
                newPt.Previous = First;
            }
            else
            {
                newPt.Next = First;
                newPt.Previous = First.Previous;
                First.Previous.Next = newPt;
                First.Previous = newPt;
            }
        }

        public void addPoint(Vector2 pt, PolygonPoint addAfter)
        {
            PolygonPoint newPt = new PolygonPoint(pt);
            newPt.Previous = addAfter;
            newPt.Next = addAfter.Next;
            addAfter.Next = newPt;
        }

        public void updateBounds()
        {
            Vector2 minVector = new Vector2(0, 0);
            Vector2 maxVector = new Vector2(0, 0);
            bool doTerminate = false;
            PolygonPoint current = this.First;

            while (current != First || !doTerminate)
            {
                if (current.Point.X <= minVector.X)
                {
                    minVector.X = current.Point.X;
                }
                if (current.Point.Y <= minVector.Y)
                {
                    minVector.Y = current.Point.Y;
                }

                if (current.Point.X >= maxVector.X)
                {
                    maxVector.X = current.Point.X;
                }
                if (current.Point.Y >= maxVector.Y)
                {
                    maxVector.Y = current.Point.Y;
                }

                current = current.Next;

                doTerminate = true;
            }

            Vector2 Position = (minVector + maxVector) / 2;
            Vector2 Size = maxVector - minVector;
            //AlignedBoundingBox result = new AlignedBoundingBox(new Vector2(0, 0), Size);
            //result.RelativeColliderPosition = -Position;

            this.Size = Size;
            this.relativeBoundPosition = Position;
        }
    }
}
