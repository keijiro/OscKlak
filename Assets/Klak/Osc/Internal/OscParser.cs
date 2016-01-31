//
// OscKlak - OSC (Open Sound Control) extension for Klak
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using System.Text;

namespace Klak.Osc
{
    /// OSC packet parser
    public class OscParser
    {
        #region Public Members

        public OscCore.MessageDelegate messageDelegate { get; set; }

        public void ProcessPacket(Byte[] data, int length)
        {
            _dataBuffer = data;
            _dataLength = length;

            _position = 0;
            ReadElement(length);

            _dataBuffer = null;
        }

        #endregion

        #region Private Methods

        Byte[] _dataBuffer;
        int _dataLength;
        int _position;

        void ReadElement(int length)
        {
            // where the next element begins
            var next = _position + length;

            // OSC address pattern
            var address = ReadString();

            if (address == "#bundle")
            {
                _position += 8; // ignore timestamp

                // read until the next element begins
                while (_position < next) ReadElement(ReadInt32());
            }
            else
            {
                // read type tags
                var types = ReadString();

                // read the first argument if exists
                var arg = types.Length > 1 ? ReadArgument(types[1]) : 0.0f;

                // skip the rest of the arguments
                _position = next;

                // invoke the delegate with the retrieved argument
                messageDelegate(address, arg);
            }
        }

        float ReadArgument(char type)
        {
            if (type == 'f') return ReadFloat32();
            if (type == 'i') return ReadInt32();
            // return zero for unsupported types
            return 0.0f;
        }

        int ReadInt32()
        {
            if (_position + 4 > _dataLength)
                throw new SystemException("Invalid packet");
            var temp =
                (_dataBuffer[_position + 0] << 24) +
                (_dataBuffer[_position + 1] << 16) +
                (_dataBuffer[_position + 2] << 8) +
                (_dataBuffer[_position + 3]);
            _position += 4;
            return temp;
        }

        float ReadFloat32()
        {
            if (_position + 4 > _dataLength)
                throw new SystemException("Invalid packet");
            Byte[] temp = {
                _dataBuffer[_position + 3],
                _dataBuffer[_position + 2],
                _dataBuffer[_position + 1],
                _dataBuffer[_position]
            };
            _position += 4;
            return BitConverter.ToSingle(temp, 0);
        }

        string ReadString()
        {
            var offset = 0;
            while (_dataBuffer[_position + offset] != 0) offset++;
            if (_position + offset > _dataLength)
                throw new SystemException("Invalid packet");
            var s = Encoding.UTF8.GetString(_dataBuffer, _position, offset);
            _position += (offset + 4) & ~3;
            return s;
        }

        #endregion
    }
}
