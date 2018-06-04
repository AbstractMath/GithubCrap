using Microsoft.Xna.Framework;

namespace GameEngineProjectRevive.Physics
{
    public class PolygonPoint
    {
        public Vector2 Point;
        public PolygonPoint Next;
        public PolygonPoint Previous;

        public PolygonPoint(Vector2 Point)
        {
            this.Point = Point;
        }
    }
}
