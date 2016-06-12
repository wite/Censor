using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cenzor
{
    class CalculateProgress
    {
        private static int actualProgress;
        public static int Calculate(int frameNumber, int framesAmount, int statusBarMax = 100)
        {
            
            Calculations(frameNumber, framesAmount, statusBarMax);
            return actualProgress;
        }

        public static int Calculate(int frameNumber, long framesAmount, int statusBarMax = 100)
        {
            Calculations(frameNumber, (int)framesAmount, statusBarMax);
            return actualProgress;
        }

        private static void Calculations(int frameNumber, int framesAmount, int statusBarMax)
        {
            actualProgress = frameNumber * statusBarMax / framesAmount;
        }
    }
}
