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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klak.Osc
{
    /// OSC over UDP server
    public sealed class OscServer : IDisposable
    {
        #region Public Properties And Methods

        public OscParser.ReceiveDataDelegate receiveDataDelegate {
            get { return _osc.receiveDataDelegate; }
            set { _osc.receiveDataDelegate = value; }
        }

        public OscServer(int listenPort)
        {
            _socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));

            _buffer = new byte[65536];
            _osc = new OscParser();
        }

        public void Start()
        {
            if (_thread == null) {
                _thread = new Thread(ServerLoop);
                _thread.Start();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            var sock = _socket;
            _socket = null;

            sock.Shutdown(SocketShutdown.Receive);
            sock.Close();

            _thread.Join();
            _thread = null;

            _buffer = null;

            _disposed = true;
        }

        #endregion

        #region Private Objects And Methods

        Socket _socket;
        Thread _thread;

        OscParser _osc;
        byte[] _buffer;

        bool _disposed;

        void ServerLoop()
        {
            while (_socket != null && !_disposed)
            {
                try {
                    int dataRead = _socket.Receive(_buffer);
                    lock (_osc) _osc.FeedData(_buffer);
                }
                catch (SocketException) {
                    // It might exited by a timeout, so do nothing.
                }
            }
        }

        #endregion
    }
}
