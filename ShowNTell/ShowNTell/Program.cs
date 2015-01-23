using Microsoft.SPOT;
using Onoffswitch.NetDuinoUtils.Motor;
using Onoffswitch.NetDuinoUtils.RFID;
using Onoffswitch.NetDuinoUtils.Utils;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Toolbox.NETMF.Hardware;
using Toolbox.NETMF.NET;

namespace ShowNTell
{
    public class Program
    {
        private static ServoControl pwmPin;

        public static void Main()
        {
//            var idReader = new Id12Reader(SerialPorts.COM2, Pins.GPIO_PIN_D4);
//            idReader.RfidEvent += IDReaderRfidEvent;
//            idReader.Start();

            InitWifi();

            NetDuinoUtils.KeepRunning();
        }

        private static void InitWifi()
        {
            // Declares the WiFly module, configures the IP address and joins a wireless network
            WiFlyGSX WifiModule = new WiFlyGSX(SerialPorts.COM1);
            WifiModule.EnableDHCP();
            WifiModule.JoinNetwork("BOKBOKBOK", 0, WiFlyGSX.AuthMode.MixedWPA1_WPA2, "bezout1983");
            

            // Showing some interesting output
            Debug.Print("Local IP: " + WifiModule.LocalIP);
            Debug.Print("MAC address: " + WifiModule.MacAddress);

            // Creates a socket
            var Socket = new WiFlySocket("www.netmftoolbox.com", 80, WifiModule);

            // Connects to the socket
            Socket.Connect();

            // Does a plain HTTP request
            Socket.Send("GET /helloworld/ HTTP/1.1\r\n");
            Socket.Send("Host: " + Socket.Hostname + "\r\n");
            Socket.Send("Connection: Close\r\n");
            Socket.Send("\r\n");

            // Prints all received data to the debug window, until the connection is terminated and there's no data left anymore
            while (Socket.IsConnected || Socket.BytesAvailable > 0)
            {
                string Text = Socket.Receive();
                if (Text != "")
                    Debug.Print(Text);
            }

            // Closes down the socket
            Socket.Close();
        }

        static void IDReaderRfidEvent(object sender, Id12Reader.RfidEventArgs e)
        {
            Debug.Print("Card scanned: " + e.RFID + ", time: " + e.ReadTime);
        }
    }
}
