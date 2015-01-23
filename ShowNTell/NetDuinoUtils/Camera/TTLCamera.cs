using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO.Ports;
using Microsoft.SPOT;
using Onoffswitch.NetDuinoUtils.Utils;

namespace Onoffswitch.NetDuinoUtils
{
    public delegate void WriterDelegate(byte[] bytes, int length, int totalSize);  

    public class TTLCamera : IDisposable
    {
        
        protected enum Command : byte
        {
            GEN_VERSION = 0x11,
            SET_SERIAL_NUMBER = 0x21,
            SET_PORT = 0x24,
            SYSTEM_RESET = 0x26,
            READ_DATA = 0x30,
            WRITE_DATA = 0x31,
            READ_FBUF = 0x32,
            WRITE_FBUF = 0x33,
            GET_FBUF_LEN = 0x34,
            SET_FBUF_LEN = 0x35,
            FBUF_CTRL = 0x36,
            COMM_MOTION_CTRL = 0x37,
            COMM_MOTION_STATUS = 0x38,
            COMM_MOTION_DETECTED = 0x39,
            MIRROR_CTRL = 0x3A,
            MIRROR_STATUS = 0x3B,
            COLOR_CTRL = 0x3C,
            COLOR_STATUS = 0x3D,
            POWER_SAVE_CTRL = 0x3E,
            POWER_SAVE_STATUS = 0x3F,
            AE_CTRL = 0x40,
            AE_STATUS = 0x41,
            MOTION_CTRL = 0x42,
            MOTION_STATUS = 0x43,
            TV_OUT_CTRL = 0x44,
            OSD_ADD_CHAR = 0x45, // unsupported by the VC0706 firmware
            DOWNSIZE_SIZE_SET = 0x52,
            DOWNSIZE_SIZE_GET = 0x53,
            DOWNSIZE_CTRL = 0x54,
            DOWNSIZE_STATUS = 0x55,
            GET_FLASH_SIZE = 0x60,
            ERASE_FLASH_SECTOR = 0x61,
            ERASE_FLASH_ALL = 0x62,
            READ_LOGO = 0x70,
            SET_BITMAP = 0x71,
            BATCH_WRITE = 0x80
        }

        protected enum FrameCommand : byte
        {
            STOPCURRENTFRAME = 0x0,
            STOPNEXTFRAME = 0x1,
            RESUMEFRAME = 0x3,
            STEPFRAME = 0x2,
            MOTIONCONTROL = 0x0,
            UARTMOTION = 0x01,
            ACTIVATEMOTION = 0x01,
        }

        public enum ImageSize : byte
        {
            Res640x480 = 0x00,
            Res320x240 = 0x11,
            Res160x120 = 0x22,
        }

        private const int MaxCommandLength = 20;
        private byte[] _command = new byte[MaxCommandLength];
        private int _commandIndex;

        private const int MaxResponseLength = 21;
        private byte[] _response = new byte[MaxResponseLength];
        private int _responseIndex;

        private SerialPort _comPort;
        private ManualResetEvent _dataReceivedEvent = new ManualResetEvent(false);

        private const int _bufferSize = 120;
        private byte[] _frameBuffer = new byte[_bufferSize];

        public void Initialize(string port, PortSpeed baudRate, ImageSize imageSize)
        {
            AutoDetectBaudRate(port);
            SetImageSize(imageSize);
            AutoDetectBaudRate(port);
            SetPortSpeed(baudRate);
        }

        protected void AutoDetectBaudRate(string port)
        {
            Open(port);
            var baudRates = new int[] { 115200, 57600, 38400, 19200, 9600 };
            foreach (int baudRate in baudRates)
            {
                Open(port, baudRate);
                try
                {
                    GetImageSize();
                    return;
                }
                catch (Exception e)
                {
                    Debug.Print("AutoDetect failed @ " + baudRate.ToString() + ". Exception: " + e.Message);
                }
                Shutdown();
            }
            throw new ApplicationException("auto detect failed");
        }

        protected void Open(string port = "COM1", int baudRate = 38400)
        {
            Shutdown();
            _comPort = new SerialPort(port, baudRate, Parity.None, 8, StopBits.One);
            _comPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _comPort.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceivedHandler);
            CameraDelayMilliSec = 10;
            _comPort.Open();
            ResponseLength = 0;
        }

        public void Shutdown()
        {
            if (_comPort != null)
            {
                StopAutoMotionDetection();
                _comPort.Flush();
                _comPort.Close();
                _comPort.Dispose();
                _comPort = null;
            }
        }

        
        private int _autoMotionImageSequenceNumber = 0;
        private string _autoMotionStoragePath;
        private Action _autoMotionDetectionHandler;
        private Thread _autoMotionThread;
        private bool _autoMotionStop { get; set; }

        public void StartAutoMotionDetection(Action motionDetectionHandler)
        {
            if (_autoMotionThread != null)
            {
                throw new ApplicationException("_autoMotionThread");
            }

            if (motionDetectionHandler == null)
            {
                throw new ApplicationException("motionDetectionHandler");
            }
            _autoMotionStop = false;
            _autoMotionDetectionHandler = motionDetectionHandler;
            _autoMotionThread = new Thread(AutoMotionDetectionThread);
            _autoMotionThread.Start();
        }

        public void StopAutoMotionDetection()
        {
            if (_autoMotionThread != null)
            {
                _autoMotionStop = true;
                _autoMotionThread.Join();
                _autoMotionThread = null;
            }
        }

        private void AutoMotionDetectionThread()
        {
            SetMotionDetection(true);
            
            while (!_autoMotionStop)
            {
                try
                {
                    if (GetMotionDetected())
                    {
                        if (_autoMotionDetectionHandler != null)
                        {
                            _autoMotionDetectionHandler();
                            _autoMotionImageSequenceNumber++;
                        }
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
            SetMotionDetection(false);
        }

        public void SetMotionDetection(bool flag)
        {
            SetMotionStatus(FrameCommand.MOTIONCONTROL, FrameCommand.UARTMOTION, FrameCommand.ACTIVATEMOTION);
            RunCommand(Command.COMM_MOTION_CTRL, new byte[] { (flag) ? (byte)1 : (byte)0 }, 5);
        }

        public bool GetMotionDetectionCommStatus()
        {
            RunCommand(Command.COMM_MOTION_STATUS, null, 6);
            return (_response[5] == 1) ? true : false;
        }

        public bool GetMotionDetected()
        {
            if (ReadResponse(4, false) != 0)
            {
                return VerifyResponse(Command.COMM_MOTION_DETECTED);
            }
            return false;
        }

        protected void SetMotionStatus(FrameCommand x, FrameCommand d1, FrameCommand d2)
        {
            RunCommand(Command.MOTION_CTRL, new byte[] { (byte)x, (byte)d1, (byte)d2 }, 5);
        }

        public enum ColorControl : byte
        {
            BlackWhiteColor,
            Color,  // Does not appear to be supported by the camera
            BlackWhite
        }

        public void SetColorControl(ColorControl color)
        {
            RunCommand(Command.COLOR_CTRL, new byte[] { 0x1, (byte)color }, 5);
        }

        public void GetColorStatus(out byte showMode, out ColorControl currentColor)
        {
            showMode = 0;
            currentColor = ColorControl.BlackWhiteColor;
            RunCommand(Command.COLOR_STATUS, new byte[] { 0x1 }, 8);
            showMode = _response[6];
            currentColor = (ColorControl)_response[7];
        }

        public ImageSize GetImageSize()
        {
            RunCommand(Command.READ_DATA, new byte[] { 0x4, 0x1, 0x00, 0x19 }, 6);
            return (ImageSize)_response[5];
        }

        protected void SetImageSize(ImageSize size)
        {
            RunCommand(Command.WRITE_DATA, new byte[] { 0x04, 0x01, 0x00, 0x19, (byte)size }, 5);
            Reset();
        }

        public enum Proportion : byte
        {
            NoZoom,
            HalfSize,
            QuarterSize
        }

        public void GetDownSize(out Proportion width, out Proportion height)
        {
            RunCommand(Command.DOWNSIZE_STATUS, null, 6);
            var temp = _response[5];
            temp &= 0x3;
            width = (Proportion)temp;
            temp = _response[5];
            temp >>= 4;
            temp &= 0x3;
            height = (Proportion)temp;
        }

        public byte GetDownSize()
        {
            RunCommand(Command.DOWNSIZE_STATUS, null, 6);
            return _response[5];
        }

        public void SetDownSize(Proportion width, Proportion height)
        {
            byte size = (byte)(((byte)height & 0x3) << 4);
            size |= (byte)((byte)width & 0x3);
            RunCommand(Command.DOWNSIZE_CTRL, new byte[] { size }, 5);
        }

        public void SetDownSize(byte downSize)
        {
            RunCommand(Command.DOWNSIZE_CTRL, new byte[] { downSize }, 5);
        }

        public string GetVersion()
        {
            RunCommand(Command.GEN_VERSION, new byte[] { 0x01 }, 16);
            Array.Copy(_response, 5, _response, 0, 11);
            _response[12] = _response[13] = 0;
            var encoding = new UTF8Encoding();
            return new string(encoding.GetChars(_response));
        }

        public byte GetCompression()
        {
            RunCommand(Command.READ_DATA, new byte[] { 0x1, 0x1, 0x12, 0x04 }, 6);
            return _response[5];
        }

        public void SetCompression(byte compression)
        {
            RunCommand(Command.WRITE_DATA, new byte[] { 0x1, 0x1, 0x12, 0x04, compression }, 5);
        }

        public enum PortSpeed
        {
            Baud9600 = 0xAEC8,
            Baud19200 = 0x56E4,
            Baud38400 = 0x2AF2,
            Baud57600 = 0x1C4C,
            Baud115200 = 0x0DA6
        }

        protected void SetPortSpeed(PortSpeed speed)
        {
            RunCommand(Command.SET_PORT, new byte[] { 0x1, (byte)((short)speed >> 8), (byte)((short)speed & 0xFF) }, 5);
            var comPortName = _comPort.PortName;
            switch (speed)
            {
                case PortSpeed.Baud9600:
                    Open(comPortName, 9600);
                    break;
                case PortSpeed.Baud19200:
                    Open(comPortName, 19200);
                    break;
                case PortSpeed.Baud38400:
                    Open(comPortName, 38400);
                    break;
                case PortSpeed.Baud57600:
                    Open(comPortName, 57600);
                    break;
                case PortSpeed.Baud115200:
                    Open(comPortName, 115200);
                    break;
            }
            Thread.Sleep(100);
        }

        public void SetPanTiltZoom(short zoomWidth, short zoomHeight, short pan, short tilt)
        {
            RunCommand(Command.DOWNSIZE_SIZE_SET, new byte[] {
                (byte)(zoomWidth >> 8),
                (byte)(zoomWidth & 0xFF),
                (byte)(zoomHeight >> 8),
                (byte)(zoomWidth & 0xFF),
                (byte)(pan>>8),
                (byte)(pan & 0xFF),
                (byte)(tilt>>8),
                (byte)(tilt & 0xFF) }, 5);
        }

        public void GetPanTiltZoom(out ushort width, out ushort height, out ushort zoomWidth, out ushort zoomHeight, out ushort pan, out ushort tilt)
        {
            width = height = zoomWidth = zoomHeight = pan = tilt = 0;
            RunCommand(Command.DOWNSIZE_SIZE_GET, null, 16);
            width = _response[5];
            width <<= 8;
            width |= _response[6];
            height = _response[7];
            height <<= 8;
            height |= _response[8];
            zoomWidth = _response[9];
            zoomWidth <<= 8;
            zoomWidth |= _response[10];
            zoomHeight = _response[11];
            zoomHeight <<= 8;
            zoomHeight |= _response[12];
            pan = _response[13];
            pan <<= 8;
            pan |= _response[14];
            tilt = _response[15];
            tilt <<= 8;
            tilt |= _response[16];
        }

        protected void FreezeFrame()
        {
            ControlFrame(FrameCommand.STOPCURRENTFRAME);
        }

        protected void ResumeFrame()
        {
            ControlFrame(FrameCommand.RESUMEFRAME);
        }

        protected void ControlFrame(FrameCommand command)
        {
            RunCommand(Command.FBUF_CTRL, new byte[] { (byte)command }, 5);
        }

        protected int GetFrameLength()
        {
            RunCommand(Command.GET_FBUF_LEN, new byte[] { 0x00 }, 9);
            int length = _response[5];
            length <<= 8;
            length |= _response[6];
            length <<= 8;
            length |= _response[7];
            length <<= 8;
            length |= _response[8];
            return length;
        }

        private short _cameraDelayMilliSec;

        protected short CameraDelayMilliSec
        {
            get
            {
                return _cameraDelayMilliSec;
            }
            set
            {
                _cameraDelayMilliSec = value;
                _comPort.ReadTimeout = _cameraDelayMilliSec * 2;
                _comPort.WriteTimeout = _cameraDelayMilliSec * 2;
            }
        }

        public void TakePictureTo(WriterDelegate action)
        {
            FreezeFrame();
            var frameAddress = 0;
            var frameLength = GetFrameLength();
            Debug.Print("Frame length: " + frameLength);
            
            var totalBytesRead = 0;
            while (frameLength > 0)
            {
                var segmentLength = System.Math.Min(frameLength, _frameBuffer.Length);

                totalBytesRead += ReadFrameSegment(segmentLength, _frameBuffer, frameAddress);

                action(_frameBuffer, segmentLength, frameLength);

                frameLength -= segmentLength;
                frameAddress += segmentLength;
            }

            ReadResponse(5, false);
            ResumeFrame();
        }

        public void TakePictureToPath(string path)
        {
            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                TakePictureTo((bytes, size, totalSize) => file.Write(bytes, 0, size));
            }
        }

        protected int ReadFrameSegment(int segmentLength, byte[] frameBuffer, int frameAddress)
        {
            RunCommand(Command.READ_FBUF,
                new byte[] { 0x0, 0x0A,
                    (byte)((frameAddress >> 24) & 0xFF), (byte)((frameAddress >> 16) & 0xFF), (byte)((frameAddress >> 8) & 0xFF), (byte)(frameAddress & 0xFF),
                    (byte)((segmentLength >> 24) & 0xFF), (byte)((segmentLength >> 16) & 0xFF), (byte)((segmentLength >> 8) & 0xFF), (byte)(segmentLength & 0xFF),
                    (byte)((CameraDelayMilliSec >> 8) & 0xFF), (byte)(CameraDelayMilliSec & 0xFF)
                }, 5);

            var totalBytesRead = 0;

            while (segmentLength > 0)
            {
                if (WaitForIncomingData(GetTimeoutMilliSec(segmentLength)) == false) throw new ApplicationException("Timeout");
                var bytesToRead = System.Math.Min(_comPort.BytesToRead, segmentLength);
                var bytesRead = _comPort.Read(frameBuffer, 0, bytesToRead);
                segmentLength -= bytesRead;
                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        private bool WaitForIncomingData(int timeout)
        {
            var result = _dataReceivedEvent.WaitOne(timeout, false);
            if (result) _dataReceivedEvent.Reset();
            if (_comPort.BytesToRead > 0)
            {
                return true;
            }
            return result;
        }

        private const int _timeoutToleranceMilliSec = 50;

        private int GetTimeoutMilliSec(int bufferLength)
        {
            var timeout = ((bufferLength / (_comPort.BaudRate >> 3)) * 1000) + CameraDelayMilliSec + _timeoutToleranceMilliSec;
            return timeout;
        }

        public void TvOutput(bool enable)
        {
            RunCommand(Command.TV_OUT_CTRL, new byte[] { (enable) ? (byte)0x01 : (byte)0x00 }, 5);
        }

        protected void Reset(int delayMS = 1000)
        {
            RunCommand(Command.SYSTEM_RESET, null, 5);
            Thread.Sleep(delayMS);
        }

        public void Dispose()
        {
            Shutdown();
            _command = null;
            _response = null;
            _frameBuffer = null;
            _dataReceivedEvent = null;
        }

        protected void RunCommand(Command command, byte[] args, int expectedResponseLength, bool flushBeforeCommand = true)
        {
            if (flushBeforeCommand) _comPort.DiscardInBuffer();
            SendCommand(command, args);
            ReadResponse(expectedResponseLength);
            VerifyResponse(command);
        }

        protected int ReadResponse(int expectedResponseLength, bool throwExceptionOnTimeout = true)
        {
            ResponseLength = 0;
            _responseIndex = 0;
            while (expectedResponseLength > 0)
            {
                if (WaitForIncomingData(GetTimeoutMilliSec(_response.Length)) == false)
                {
                    if (throwExceptionOnTimeout) throw new ApplicationException("Timeout");
                    else break;
                }
                var bytesRead = _comPort.Read(_response, _responseIndex, expectedResponseLength);
                ResponseLength += bytesRead;
                expectedResponseLength -= bytesRead;
                _responseIndex += bytesRead;
            }
            return ResponseLength;
        }

        protected int ResponseLength { get; set; }

        private const byte CommandReply = 0x76;
        private const byte ReplyIndex = 0;
        private const byte ReplySerialIndex = 1;
        private const byte ReplyCommandIndex = 2;
        private const byte ReplyStatusIndex = 3;

        protected bool VerifyResponse(Command command)
        {
            if ((_response[ReplyIndex] != CommandReply) ||
                (_response[ReplySerialIndex] != CameraSerial) ||
                (_response[ReplyCommandIndex] != (byte)command) ||
                (_response[ReplyStatusIndex] != 0x0))
            {
                switch (_response[ReplyStatusIndex])
                {
                    case 1:
                        throw new ApplicationException("Command not received");
                    case 2:
                        throw new ApplicationException("Invalid data length");
                    case 3:
                        throw new ApplicationException("Invalid data format");
                    case 4:
                        throw new ApplicationException("Command cannot be executed now");
                    case 5:
                        throw new ApplicationException("Command executed incorrectly");
                }
            }
            return true;
        }

        private const byte CameraSerial = 0;
        private const byte CommandSend = 0x56;

        protected void SendCommand(Command command, byte[] args)
        {
            _commandIndex = 0;
            _command[_commandIndex++] = CommandSend;
            _command[_commandIndex++] = CameraSerial;
            _command[_commandIndex++] = (byte)command;
            var argsLength = 0;
            if (args != null)
            {
                argsLength = args.Length;
                _command[_commandIndex++] = (byte)argsLength;
                Array.Copy(args, 0, _command, _commandIndex, argsLength);
            }
            else { _command[_commandIndex++] = 0; }
            _comPort.Write(_command, 0, argsLength + 4);
        }

        protected virtual void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            _dataReceivedEvent.Set();
        }

        protected virtual void ErrorReceivedHandler(object sender, SerialErrorReceivedEventArgs e)
        {
        }
    }
}
