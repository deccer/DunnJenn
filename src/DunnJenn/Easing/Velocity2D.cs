using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Velocity2D
    {
        public Velocity2D(Transform2D transform)
        {
            Transform = transform ?? throw new ArgumentException(nameof(transform));
        }

        public Transform2D Transform { get; }

        public Vector2 Position { get; set; }

        public Vector2 Scale { get; set; }

        public float Rotation { get; set; }

        public void Update(GameTime time)
        {
            var delta = (float)time.ElapsedGameTime.TotalSeconds;
            Transform.Position += Position * delta;
            Transform.Scale += Scale * delta;
            Transform.Rotation += Rotation * delta;
        }
    }
}