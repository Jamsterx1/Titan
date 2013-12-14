using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Titan
{
    public static class Delta
    {
        public static double     mElapsed;
        public static double     mLastTime;
        public static float      mDelta;
        private static Stopwatch mTimer;

        public static void create()
        {
            mElapsed = 0f;
            mLastTime = 0f;
            mDelta = 0f;

            mTimer = new Stopwatch();
            mTimer.Start();
        }

        public static void update()
        {
            mElapsed = mTimer.ElapsedMilliseconds;
            mDelta = (float)((mElapsed - mLastTime) / (1000 / 60));
            mLastTime = mElapsed;
        }
    }
}
