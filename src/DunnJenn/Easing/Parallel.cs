using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Parallel : ITween
    {
        public Parallel(params ITween[] tweens)
        {
            this.tweens = tweens.ToArray();
        }

        private ITween[] tweens;

        public TimeSpan Time => tweens.Max(x => x.Time);

        public TimeSpan Duration => tweens.Max(x => x.Duration);

        public bool IsFinished => tweens.All(x => x.IsFinished);

        public void Reset()
        {
            foreach (var tween in tweens)
            {
                tween.Reset();
            }
        }


        public bool Update(GameTime time)
        {
            foreach (var tween in tweens)
            {
                if(!tween.IsFinished)
                {
                    tween.Update(time);
                }
            }

            return IsFinished;
        }
    }
}