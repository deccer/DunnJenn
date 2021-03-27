using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Tween2D : ITween
    {
        public Tween2D(TimeSpan duration, Transform2D transform, Transform2D to, Ease ease)
            : this(duration, transform, new Transform2D()
            {
                Position = transform.Position,
                Rotation = transform.Rotation,
                Scale = transform.Scale,
            }, to, EaseFunctions.Get(ease))
        {
        }

        public Tween2D(TimeSpan duration, Transform2D transform, Transform2D from, Transform2D to, Ease ease)
            :this(duration, transform, from, to, EaseFunctions.Get(ease))
        {
        }

        public Tween2D(TimeSpan duration, Transform2D transform, Transform2D from, Transform2D to, Func<float, float> ease)
        {
            Duration = duration;
            Transform = transform;
            From = from;
            To = to;
            Ease = Ease;
            easeFunction = ease;
        }

        private Func<float, float> easeFunction;

        public TimeSpan Time { get; private set; }

        public TimeSpan Duration { get; }

        public Transform2D Transform { get; }

        public Transform2D From { get; }

        public Transform2D To { get; }

        public bool IsRevert { get; }

        public Ease Ease { get; }

        public bool IsFinished { get; private set; }

        public void Reset()
        {
            IsFinished = false;
            Time = TimeSpan.Zero;

            Transform.Position = From.Position;
            Transform.Scale = From.Scale;
            Transform.Rotation = From.Rotation;
        }

        public bool Update(GameTime time)
        {
            if(!IsFinished)
            {
                var delta = (float)time.ElapsedGameTime.TotalSeconds;

                Time += time.ElapsedGameTime;

                var t = Math.Max(0, Math.Min(1, (float)(Time.TotalMilliseconds / Duration.TotalMilliseconds)));

                var amount = easeFunction(t);

                Transform.Position = From.Position + (To.Position - From.Position) * amount;
                Transform.Scale = From.Scale + (To.Scale - From.Scale) * amount;
                Transform.Rotation = From.Rotation + (To.Rotation - From.Rotation) * amount;

                IsFinished = (t >= 1);
            }

            return IsFinished;
        }
    }
}