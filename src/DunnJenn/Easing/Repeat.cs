﻿using System;
using Microsoft.Xna.Framework;

namespace DunnJenn.Easing
{
    public class Repeat : ITween
    {
        public Repeat(ITween tween, int times = -1)
        {
            this.tween = tween;
            RepeatTimes = times;
        }

        #region Fields

        private ITween tween;

        private int current;

        #endregion

        public TimeSpan Time { get; set; }

        public int RepeatTimes { get; }

        public TimeSpan Duration => TimeSpan.MaxValue;

        public bool IsFinished => RepeatTimes > 0 && current >= RepeatTimes;

        public void Reset()
        {
            current = 0;
            tween.Reset();
        }

        public bool Update(GameTime time)
        {
            var isFinished = IsFinished;
            if (!isFinished)
            {
                if(tween.Update(time))
                {
                    current++;

                    isFinished = IsFinished;
                    if(!isFinished)
                    {
                        tween.Reset();
                    }
                }

            }

            return isFinished;
        }
    }
}