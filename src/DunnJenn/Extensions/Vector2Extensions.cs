using Microsoft.Xna.Framework;

namespace DunnJenn.Extensions
{
    public static class Vector2Extensions
    {
        public static Transform2D ToTransform(this Vector2 position)
        {
            return new Transform2D()
            {
                Position = position,
            };
        }
    }
}