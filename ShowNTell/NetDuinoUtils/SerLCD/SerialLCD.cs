using System;
using System.IO.Ports;

namespace Onoffswitch.NetDuinoUtils.SerLCD
{
    public class SerialLcd
    {
        public enum Direction { Left, Right }

        public enum DisplayType { C16L2, C16L4, C20L2, C20L4 }

        public enum Status { On, Off }


        private readonly SerialPort _serialPort;

        private readonly DisplayType _displayType;


        public SerialLcd(string portName, DisplayType displayType = DisplayType.C16L2)
        {
            // Defaults for SerialPort are the same as the settings for the LCD, but I'll set them explicitly
            _serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);

            _displayType = displayType;
        }

        public void ClearDisplay()
        {
            Write(new byte[] { 0xFE, 0x01 });
        }

        public void Reset()
        {
            Write(new byte[]{0x12, 0x06});
        }

        public void MoveCursor(Direction direction, int times = 1)
        {
            byte command;

            switch (direction)
            {
                case Direction.Left: command = 0x10; break;
                case Direction.Right: command = 0x14; break;
                default: return;
            }

            for (int i = 0; i < times; i++)
            {
                Write(new byte[] { 0xFE, command });
            }
        }

        private void Open()
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }
        }

        public void SaveAsSplashScreen()
        {
            Write(new byte[] { 0x7C, 0x0A });
        }

        public void Scroll(Direction direction)
        {
            Scroll(direction, 1);
        }

        public void Scroll(Direction direction, int times)
        {
            byte command;

            switch (direction)
            {
                case Direction.Left: command = 0x18; break;
                case Direction.Right: command = 0x1C; break;
                default: return;
            }

            for (int i = 0; i < times; i++)
            {
                Write(new byte[] { 0xFE, command });
            }
        }

        public void SetBlinkingBoxCursor(Status status)
        {
            byte command;

            switch (status)
            {
                case Status.On: command = 0x0D; break;
                case Status.Off: command = 0x0C; break;
                default: return;
            }

            Write(new byte[] { 0xFE, command });
        }

        public void SetBrightness(int brightness)
        {
            if (brightness < 128 || brightness > 157)
                throw new ArgumentOutOfRangeException("brightness", "Value of brightness must be between 128-157.");

            Write(new byte[] { 0x7C, (byte)brightness });
        }

        public void SetCursorPosition(int line, int column)
        {
            if ((_displayType == DisplayType.C16L2 || _displayType == DisplayType.C16L4) && (column < 1 || column > 16))
                throw new ArgumentOutOfRangeException("column", "Column number must be between 1 and 16.");

            if ((_displayType == DisplayType.C20L2 || _displayType == DisplayType.C20L4) && (column < 1 || column > 20))
                throw new ArgumentOutOfRangeException("column", "Column number must be between 1 and 20.");

            if ((_displayType == DisplayType.C16L2 || _displayType == DisplayType.C20L2) && (line < 1 || line > 2))
                throw new ArgumentOutOfRangeException("line", "Line number must be 1 or 2.");

            if ((_displayType == DisplayType.C16L4 || _displayType == DisplayType.C20L4) && (line < 1 || line > 4))
                throw new ArgumentOutOfRangeException("line", "Line number must be between 1 and 4.");

            int[] startPos16 = { 0, 64, 16, 80 };
            int[] startPos20 = { 0, 64, 20, 84 };
            int charPos;

            switch (_displayType)
            {
                case DisplayType.C16L2:
                case DisplayType.C16L4:
                    charPos = startPos16[line - 1] + (column - 1);
                    break;
                case DisplayType.C20L2:
                case DisplayType.C20L4:
                    charPos = startPos20[line - 1] + (column - 1);
                    break;
                default:
                    return;
            }

            Write(new byte[] { 0xFE, (byte)(charPos + 0x80) });
        }

        public void SetDisplay(Status status)
        {
            byte command;

            switch (status)
            {
                case Status.On: command = 0x0C; break;
                case Status.Off: command = 0x08; break;
                default: return;
            }

            Write(new byte[] { 0xFE, command });
        }

        public void SetDisplayType(DisplayType displayType)
        {
            switch (displayType)
            {
                case DisplayType.C16L2:
                    Write(new byte[] { 0x7C, 0x04 }); // 16 characters
                    Write(new byte[] { 0x7C, 0x06 }); // 2 lines
                    break;
                case DisplayType.C16L4:
                    Write(new byte[] { 0x7C, 0x04 }); // 16 characters
                    Write(new byte[] { 0x7C, 0x05 }); // 4 lines
                    break;
                case DisplayType.C20L2:
                    Write(new byte[] { 0x7C, 0x03 }); // 20 characters
                    Write(new byte[] { 0x7C, 0x06 }); // 2 lines
                    break;
                case DisplayType.C20L4:
                    Write(new byte[] { 0x7C, 0x03 }); // 20 characters
                    Write(new byte[] { 0x7C, 0x05 }); // 4 lines
                    break;
            }
        }

        public void SetUnderlineCursor(Status status)
        {
            byte command;

            switch (status)
            {
                case Status.On: command = 0x0E; break;
                case Status.Off: command = 0x0C; break;
                default: return;
            }

            Write(new byte[] { 0xFE, command });
        }

        public void ToggleSplashScreen()
        {
            Write(new byte[] { 0x7C, 0x09 });
        }

        public void SetSplashScreenText(string val)
        {
            Write(val);
            SaveAsSplashScreen();
        }

        public void Write(byte buffer)
        {
            Write(new[] { buffer });
        }

        public void Write(byte[] buffer)
        {
            Open();

            _serialPort.Write(buffer, 0, buffer.Length);
        }

        public void Write(char character)
        {
            Write((byte)character);
        }

        public void Write(string text)
        {
            byte[] buffer = new byte[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                buffer[i] = (byte)text[i];
            }

            Write(buffer);
        }
    }
}