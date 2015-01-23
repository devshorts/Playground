using System;
using System.Net.Sockets;

namespace NWebREST.Web
{
    public delegate string EndPointAction(EndPointActionArguments arguments, params string[] items);

    public class EndPointActionArguments
    {
        public Socket Connection { get; set; }
    }   
    
    public class EndPoint
    {
        private string[] _arguments;

        public bool UsesManualSocket { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The function to be called when the endpoint is hit
        /// </summary>
        public EndPointAction Action
        {
            private get; set;
        }

        /// <summary>
        /// The name of the endpoint, this is basically the servers route
        /// </summary>
        public String Name { get; set; }

        public string[] Arguments { set { _arguments = value; } }

        /// <summary>
        /// Execute this endpoint. We'll call the action with the supplied arguments and
        /// return whatever string the action returns.
        /// </summary>
        /// <returns></returns>
        public String Execute(EndPointActionArguments misc)
        {
            if (Action != null)
            {
                return Action(misc, _arguments);
            }
            return "Unknown action";
        }
    }

}
