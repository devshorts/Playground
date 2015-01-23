using System;
using Microsoft.SPOT;

namespace Onoffswitch.NetDuinoUtils.Utils
{
    public class Threshhold
    {
        private DateTime Start { get; set; }

        private TimeSpan Threshold { get; set; }

        public Threshhold(TimeSpan threshold)
        {
            Start = DateTime.MinValue;

            Threshold = threshold;
        }

        public Boolean Mark()
        {
            var now = DateTime.Now;

            if (now - Start >= Threshold)
            {
                Start = now;

                return true;
            }

            return false;
        }
    }
}
