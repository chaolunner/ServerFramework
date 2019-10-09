using System;
using Common;
using ServerFramework.Servers;

namespace ServerFramework.Controller
{
    abstract class ControllerBase
    {
        RequestCode requestCode = RequestCode.None;

        public RequestCode RequestCode
        {
            get
            {
                return requestCode;
            }
        }

        public virtual string DefaultHandle(Server server, Client client, string data) { return null; }
    }
}
