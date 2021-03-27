using DunnJenn.Easing;

namespace DunnJenn.Extensions
{
    public static class Velocity2DExtensions
    {
        public static Acceleration2D WithAcceleration(this Velocity2D velocity)
        {
            return new Acceleration2D(velocity);
        }
    }
}