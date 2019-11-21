using System.Text;
using System.Linq;
using System;

namespace Common
{
    public class Message
    {
        private byte[] data = new byte[MaxLength];
        private int startIndex = 0;
        private const int MaxLength = 8 * 1024;
        private const string MessagePackageTooLargeError = "Message package too large error! [{0}KB/{1}KB]";

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

        public void Process(int count, Action<RequestCode, string> callback)
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
                    string msg = Encoding.UTF8.GetString(data, 8, msgCount - 4);
                    callback?.Invoke(requestCode, msg);
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
            int totalLength = length + lengthBytes.Length;
            if (totalLength > MaxLength)
            {
                ConsoleUtility.WriteLine(string.Format(MessagePackageTooLargeError, (totalLength / 1024f).ToString("F2"), (MaxLength / 1024f).ToString("F2")), ConsoleColor.Red);
            }
            return lengthBytes.Concat(requestCodeBytes).Concat(dataBytes).ToArray();
        }
    }
}
