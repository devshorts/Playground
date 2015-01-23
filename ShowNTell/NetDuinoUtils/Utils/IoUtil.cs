using System.IO;

namespace Onoffswitch.NetDuinoUtils.Utils
{
    public static class IoUtil
    {
        public static void CopyStream(this Stream input, Stream outputStream)
        {
            var bytesRead = 0;
            
            while (bytesRead < input.Length)
            {
                var buffer = new byte[4096];
                var currentReadAmount = input.Read(buffer, bytesRead, buffer.Length);
                bytesRead += currentReadAmount;
                outputStream.Write(buffer, bytesRead, currentReadAmount);
            }
        }


    }
}
