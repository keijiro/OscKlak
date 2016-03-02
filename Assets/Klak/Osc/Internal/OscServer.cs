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

        public MessageHandler messageHandler {
            get { return _messageHandler; }
        }

        public OscServer(int listenPort)
        {
            _messageHandler = new MessageHandler();

            _socket = new Socket(
                AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // On some platforms, it's not possible to cancel Receive() by
            // just calling Close() -- it'll block the thread forever!
            // Therefore, we heve to set timeout to break ServerLoop.
            _socket.ReceiveTimeout = 100;

            _socket.Bind(new IPEndPoint(IPAddress.Any, listenPort));
        }

        public void Start()
        {
            if (_thread == null && !_disposed)
            {
                _thread = new Thread(ServerLoop);
                _thread.Start();
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _socket.Shutdown(SocketShutdown.Receive);
            _socket.Close();

            _thread.Join();

            _messageHandler = null;
        }

        #endregion

        #region Private Objects And Methods

        MessageHandler _messageHandler;
        Socket _socket;
        Thread _thread;
        bool _disposed;

        void ServerLoop()
        {
            var parser = new PacketParser(_messageHandler);
            var buffer = new byte[65536];

            while (!_disposed)
            {
                try {
                    int dataRead = _socket.Receive(buffer);
                    if (!_disposed && dataRead > 0)
                        parser.ProcessPacket(buffer, dataRead);
                }
                catch (SocketException) {
                    // It might exited by timeout. Nothing to do.
                }
                catch (ThreadAbortException) {
                    // Abort silently.
                }
                catch (Exception e) {
                    if (!_disposed) UnityEngine.Debug.Log(e);
                    break;
                }
            }
        }

        #endregion
    }
}
