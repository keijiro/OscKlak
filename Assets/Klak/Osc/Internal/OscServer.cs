using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Klak.Osc
{
    // OSC over UDP server class
    public class OscServer
    {
        Thread _thread;
        UdpClient _udpClient;
        IPEndPoint _endPoint;
        OscParser _osc;

        public bool IsRunning {
            get { return _thread != null && _thread.IsAlive; }
        }

        public OscParser.ReceiveDataDelegate receiveDataDelegate {
            get { return _osc.receiveDataDelegate; }
            set { _osc.receiveDataDelegate = value; }
        }

        public OscServer(int listenPort)
        {
            _endPoint = new IPEndPoint(IPAddress.Any, listenPort);
            _udpClient = new UdpClient(_endPoint);
            _udpClient.Client.ReceiveTimeout = 500;
            _osc = new OscParser();
        }

        public void Start()
        {
            if (_thread == null) {
                _thread = new Thread(ServerLoop);
                _thread.Start();
            }
        }

        public void Close()
        {
            _udpClient.Close();
            _udpClient = null;
        }

        void ServerLoop()
        {
            while (_udpClient != null)
            {
                try {
                    var data = _udpClient.Receive(ref _endPoint);
                    lock (_osc) _osc.FeedData(data);
                }
                catch (SocketException) {
                    // It might exited by a timeout, so do nothing.
                }
            }
        }
    }
}
