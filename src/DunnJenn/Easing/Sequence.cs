using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Sequence : ITween
    {
        public Sequence(params ITween[] tweens)
        {
            this.tweens = tweens.ToArray();
        }

        private ITween[] tweens;

        public TimeSpan Time 
        {
            get
            {
                var result = TimeSpan.Zero;

                for (int i = 0; i < tweens.Length; i++)
                {
                    var tween = tweens[i];

                    if(i == current)
                        return result + tween.Time;
                    
                    result += tween.Duration;
                }

                return result;
            }
        }

        public TimeSpan Duration => new TimeSpan(tweens.Sum(x => x.Duration.Ticks));

        public bool IsFinished { get; private set; }

        public void Reset()
        {
            foreach (var tween in tweens)
            {
                tween.Reset();
            }

            current = 0;
            IsFinished = false;
        }

        private int current;

        public bool Update(GameTime time)
        {
            if (!IsFinished)
            {
                var tween = tweens[current];

                if(tween.Update(time))
                {
                    current++;
                }
                IsFinished = current >= tweens.Length;
            }

            return IsFinished;
        }
    }
}