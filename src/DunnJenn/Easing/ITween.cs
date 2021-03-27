using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public interface ITween
    {
        TimeSpan Time { get; }

        TimeSpan Duration { get; }

        bool IsFinished { get; }

        void Reset();

        bool Update(GameTime time);
    }
}