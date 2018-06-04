using Microsoft.Xna.Framework;

namespace GameEngineProjectRevive.Physics
{
    public class Simplex
    {
        private Vector2[] points;
        public int Count { get; private set; }

        public Vector2 this[int i]
        {
            get
            {
                if (Count >= i)
                {
                    return points[i];
                }
                else
                {
                    throw new System.IndexOutOfRangeException();
                }
            }
        }

        public Polygon SimplexToPolygon()
        {
            Polygon result = new Polygon();

            for (int i = 0; i < Count; i++)
            {
                result.addPoint(points[i]);
            }

            return result;
        }

        public static Vector2 TripleProduct(Vector2 a, Vector2 b, Vector2 c)
        {
            return b * Vector2.Dot(c, a) - a * Vector2.Dot(c, b);
        }

        public void add(Vector2 point)
        {
            points[Count] = point;
            Count++;
        }

        public void remove(int index)
        {
            for (int i = index; i < Count - 1; i++)
            {
                points[i] = points[i + 1];
            }
            points[Count - 1] = Vector2.Zero;
            Count--;
        }

        public Simplex()
        {
            Count = 0;
            points = new Vector2[3];
        }
    }
}
