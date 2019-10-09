﻿using System;
using Common;
using System.Text;
using System.Linq;

namespace ServerFramework.Servers
{
    class Message
    {
        private byte[] data = new byte[1024];
        private int startIndex = 0;

        public byte[] Data
        {
            get { return data; }
        }

        public int StartIndex
        {
            get { return startIndex; }
        }

        public int RemainSize
        {
            get { return data.Length - startIndex; }
        }

        public void Process(int count, Action<RequestCode, ActionCode, string> callback)
        {
            startIndex += count;
            while (true)
            {
                if (startIndex <= 4) { break; }
                int msgCount = BitConverter.ToInt32(data, 0);
                int totalCount = msgCount + 4;
                if (startIndex >= totalCount)
                {
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4);
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8);
                    string msg = Encoding.UTF8.GetString(data, 12, msgCount - 8);
                    callback?.Invoke(requestCode, actionCode, msg);
                    Array.Copy(data, totalCount, data, 0, startIndex - totalCount);
                    startIndex -= totalCount;
                }
                else
                {
                    break;
                }
            }
        }

        public static byte[] Pack(RequestCode requestCode, string data)
        {
            byte[] requestCodeBytes = BitConverter.GetBytes((int)requestCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int length = requestCodeBytes.Length + dataBytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            return lengthBytes.Concat(requestCodeBytes).Concat(dataBytes).ToArray();
        }
    }
}
