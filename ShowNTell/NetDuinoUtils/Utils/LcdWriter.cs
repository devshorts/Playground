using System.Threading;
using NetDuinoPlusHelloWorld.Utils;
using Onoffswitch.NetDuinoUtils.SerLCD;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

namespace Onoffswitch.NetDuinoUtils.Utils
{
public class LcdWriter
{
    #region Data

    private SerialLcd _serialInterface;
    private static object _lockObject = new object();

    private static string _lcdDisplay = string.Empty;
    private static AutoResetEvent mutex = new AutoResetEvent(false);

    private static LcdWriter _instance;

    #endregion

    #region Singleton and Constructor 

    public static LcdWriter Instance
    {
        get
        {
            lock (_lockObject)
            {
                return _instance ?? (_instance = new LcdWriter());
            }
        }
    }

    private LcdWriter()
    {
        _serialInterface = new SerialLcd(SerialPorts.COM2);

        ThreadUtil.Start(() =>
        {
            while (true)
            {
                _serialInterface.ClearDisplay();
                _serialInterface.SetCursorPosition(1, 1);

                lock (_lockObject)
                {
                    _serialInterface.Write(_lcdDisplay);
                }

                mutex.WaitOne();
            }
        });
    }

    #endregion


    #region Writer

    public void Write(string text)
    {
        lock(_lockObject)
        {
            _lcdDisplay = text;
            mutex.Set();
        }
    }

    #endregion
}
}
