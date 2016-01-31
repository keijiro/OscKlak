using System;

namespace Klak.Osc
{
    public static class OscMaster
    {
        public static OscParser.ReceiveDataDelegate receiveDataDelegate {
            get { return ServerInstance.receiveDataDelegate; }
            set { ServerInstance.receiveDataDelegate = value; }
        }

        static OscServer _server;

        static OscServer ServerInstance {
            get {
                if (_server == null) {
                    _server = new OscServer(9000);
                    _server.Start();
                }
                return _server;
            }
        }
    }
}
