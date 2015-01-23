using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Onoffswitch.NetDuinoUtils.Utils
{
    public delegate void ButtonPushed(bool buttonPushed);
    public static class ButtonUtils
    {
        private static Boolean _lastStatus;
        /// <summary>
        /// Runs an indefinite loop marking when a button was pushed
        /// </summary>
        /// <param name="action"></param>
        public static void OnBoardButtonPushed(ButtonPushed action)
        {
            ThreadUtil.Start(() =>
                                 {
                                     try
                                     {
                                         var button = new InputPort(Pins.ONBOARD_SW1, false, Port.ResistorMode.Disabled);
                                         while (true)
                                         {
                                             var pushed = !button.Read();
                                             if (pushed != _lastStatus)
                                             {
                                                 _lastStatus = pushed;
                                                 action(pushed);
                                             }
                                             Thread.Sleep(50);
                                         }
                                     }
                                     catch (Exception ex)
                                     {

                                     }
                                 });
        }
    }
}
