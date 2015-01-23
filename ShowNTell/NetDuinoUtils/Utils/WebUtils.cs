using System.Text;

namespace Onoffswitch.NetDuinoUtils.Utils
{
    public static class WebUtils
    {
        private static string Boundary = "ThisRandomString";
        public static byte[] StartMJPEGHeader()
        {
            var s =
"HTTP/1.0 200 OK\r\n" + 
"Connection: Close\r\n" +
"Server: Camd\r\n" +"Content-Type: multipart/x-mixed-replace;boundary=--" + Boundary + "\r\n\r\n";
            return Encoding.UTF8.GetBytes(s);
        }

        public static byte[] CreateMPEGFrameHeader(int length)
        {
            string header =
                "--" + Boundary + "\r\n" +
                "Content-Type:image/jpeg\r\n" +
                "Content-Length:" + length + "\r\n"
                + "\r\n"; // there are always 2 new line character before the actual data

            // using ascii encoder is fine since there is no international character used in this string.
            return Encoding.UTF8.GetBytes(header);
        }

        public static byte[] CreateFooter()
        {
            return Encoding.UTF8.GetBytes("\r\n\r\n");
        }
    }
}
