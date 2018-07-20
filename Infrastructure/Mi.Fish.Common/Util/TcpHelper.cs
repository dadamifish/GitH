using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Mi.Fish.Common
{
    /// <summary>
    /// TCP客户端，可发送数据到服务器
    /// qinwen 20091205    
    /// </summary>
    public class TcpHelper
    {

        private static TcpClient m_tcpclient;
        private static NetworkStream ns;
        private static int timeout = 30000;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendstr"></param>
        /// <param name="server_ip"></param>
        /// <param name="server_port"></param>
        /// <returns></returns>
        public static int Sendstring(string sendstr, string ip, int port)
        {
            try
            {
                m_tcpclient = new TcpClient(ip, port);
                m_tcpclient.SendTimeout = timeout;
                ns = m_tcpclient.GetStream(); //获得网络流对象   
                byte[] bytes = Encoding.UTF8.GetBytes(sendstr);
                ns.Write(bytes, 0, bytes.Length);
                ns.Close();
                m_tcpclient.Close();
                return 1;
            }
            catch
            {
                return 0;
            }
        }
    }
}
