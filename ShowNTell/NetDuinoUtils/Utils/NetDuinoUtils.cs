using System.Threading;

namespace Onoffswitch.NetDuinoUtils.Utils
{
    public static class NetDuinoUtils
    {
        /// <summary>
        /// Empty while loop to keep the thread running
        /// </summary>
        public static void KeepRunning()
        {
            while (true)
            {
                Thread.Sleep(250);
            }
        }
    }
}
