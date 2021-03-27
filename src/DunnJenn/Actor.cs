using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public class Actor
    {
        public Vector2 Position { get; set; }

        public Point PositionAsPoint
        {
            get { return new Point((int) Position.X, (int) Position.Y); }
        }
    }

    public class Hero : Actor
    {
        
    }
}