using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Delay : ITween
    {
        public Delay(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Time { get; private set; }

        public TimeSpan Duration { get; }

        public bool IsFinished { get; private set; }

        public void Reset()
        {
            IsFinished = false;
            Time = TimeSpan.Zero;
        }

        public bool Update(GameTime time)
        {
            if (!IsFinished)
            {
                var delta = (float)time.ElapsedGameTime.TotalSeconds;
                Time += time.ElapsedGameTime;
                IsFinished = (Time >= Duration);
            }

            return IsFinished;
        }
    }
}