using DunnJenn.Easing;

namespace DunnJenn.Extensions
{
    public static class Transform2DExtensions
    {
        public static Velocity2D WithVelocity(this Transform2D transform)
        {
            return new Velocity2D(transform);
        }
    }
}