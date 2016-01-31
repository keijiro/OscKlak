using System;
using System.Text;
using System.Collections.Generic;

namespace Klak.Osc
{
    // OSC message storage struct
    public struct OscMessage
    {
        public string address;
        public object[] data;

        public OscMessage(string address, object[] data)
        {
            this.address = address;
            this.data = data;
        }

        public override string ToString ()
        {
            var temp = address + ":";
            if (data.Length > 0)
            {
                for (var i = 0; i < data.Length - 1; i++)
                    temp += data[i] + ",";
                temp += data[data.Length - 1];
            }
            return temp;
        }
    }

    // OSC packet parser
    public class OscParser
    {
        #region Public Methods And Properties

        public void FeedData(Byte[] data)
        {
            _readBuffer = data;
            _readPoint = 0;

            ReadMessage();

            _readBuffer = null;
        }

        #endregion

        #region Event Delegates

        public delegate void ReceiveDataDelegate(string path, float data);
        public ReceiveDataDelegate receiveDataDelegate { get; set; }

        #endregion

        #region Private Implementation

        Byte[] _readBuffer;
        int _readPoint;

        void ReadMessage()
        {
            var address = ReadString();

            if (address == "#bundle")
            {
                ReadInt64();

                while (true)
                {
                    if (_readPoint >= _readBuffer.Length) return;

                    var peek = _readBuffer[_readPoint];
                    if (peek == '/' || peek == '#') {
                        ReadMessage();
                        return;
                    }

                    var bundleEnd = _readPoint + ReadInt32();
                    while (_readPoint < bundleEnd)
                        ReadMessage();
                }
            }

            var types = ReadString();

            for (var i = 0; i < types.Length - 1; i++)
            {
                switch (types[i + 1])
                {
                case 'f':
                    receiveDataDelegate(address, ReadFloat32());
                    break;
                case 'i':
                    receiveDataDelegate(address, ReadInt32());
                    break;
                case 's':
                    ReadString();
                    break;
                case 'b':
                    ReadBlob();
                    break;
                }
            }
        }

        float ReadFloat32()
        {
            Byte[] temp = {
                _readBuffer[_readPoint + 3],
                _readBuffer[_readPoint + 2],
                _readBuffer[_readPoint + 1],
                _readBuffer[_readPoint]
            };
            _readPoint += 4;
            return BitConverter.ToSingle(temp, 0);
        }

        int ReadInt32 ()
        {
            int temp =
                (_readBuffer[_readPoint + 0] << 24) +
                (_readBuffer[_readPoint + 1] << 16) +
                (_readBuffer[_readPoint + 2] << 8) +
                (_readBuffer[_readPoint + 3]);
            _readPoint += 4;
            return temp;
        }

        long ReadInt64 ()
        {
            long temp =
                ((long)_readBuffer[_readPoint + 0] << 56) +
                ((long)_readBuffer[_readPoint + 1] << 48) +
                ((long)_readBuffer[_readPoint + 2] << 40) +
                ((long)_readBuffer[_readPoint + 3] << 32) +
                ((long)_readBuffer[_readPoint + 4] << 24) +
                ((long)_readBuffer[_readPoint + 5] << 16) +
                ((long)_readBuffer[_readPoint + 6] << 8) +
                ((long)_readBuffer[_readPoint + 7]);
            _readPoint += 8;
            return temp;
        }

        string ReadString()
        {
            var offset = 0;
            while (_readBuffer[_readPoint + offset] != 0) offset++;
            var s = Encoding.UTF8.GetString(_readBuffer, _readPoint, offset);
            _readPoint += (offset + 4) & ~3;
            return s;
        }

        Byte[] ReadBlob()
        {
            var length = ReadInt32();
            var temp = new Byte[length];
            Array.Copy(_readBuffer, _readPoint, temp, 0, length);
            _readPoint += (length + 3) & ~3;
            return temp;
        }

        #endregion
    }
}
