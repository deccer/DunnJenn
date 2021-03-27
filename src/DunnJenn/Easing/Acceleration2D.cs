using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Acceleration2D
    {
        public Acceleration2D(Velocity2D velocity)
        {
            Velocity = velocity ?? throw new ArgumentException(nameof(velocity));
        }

        public Transform2D Transform => Velocity.Transform;

        public Velocity2D Velocity { get; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public void Update(GameTime time)
        {
            var delta = (float)time.ElapsedGameTime.TotalSeconds;
            Velocity.Position += Position * delta;
            Velocity.Scale += Scale * delta;
            Velocity.Rotation += Rotation * delta;

            Velocity.Update(time);
        }
    }
}
