using System.Net.Sockets;

namespace NWebREST.Web
{
    /// <summary>
    /// Event arguments of an incoming web command.
    /// </summary>
    public class EndPoinEventArgs
    {
        /// <summary>
        /// Allows us to tell the web server that we manually replied back
        /// via the socket. If false, the server will reply back with our string response
        /// This lets us write things other than the generic response around (for example if you want
        /// to stream custom binary)
        /// </summary>
        public bool ManualSent { get; set; }

        public EndPoinEventArgs()
        {
        }

        public EndPoinEventArgs(EndPoint command)
        {
            Command = command;
        }

        public EndPoinEventArgs(EndPoint command, Socket connection)
        {
            Command = command;
            Connection = connection;
            Connection.SendTimeout = 5000;
        }

        public EndPoint Command { get; set; }
        public string ReturnString { get; set; }
        public Socket Connection { get; set; }
    }
}