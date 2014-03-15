using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Utilities
{
    public class LoopingID
    {
        private int Current { get; set; }
        private int Minimum { get; set; }
        private int Maximum { get; set; }

        public LoopingID()
        {
            Minimum = Current = 0;
            Maximum = 10000;
        }

        public LoopingID(int min, int max)
        {
            Minimum = Current = min;
            Maximum = max;
        }

        public int NextValue()
        {
            int ret = Current;
            if (Current == Maximum)
            {
                Reset();
            }
            else
            {
                Current++;
            }
            return ret;
        }

        public void Reset()
        {
            Current = Minimum;
        }
    }
}
