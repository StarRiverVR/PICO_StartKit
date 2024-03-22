using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using proto;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace com.vivo.codelibrary
{
    public class IPUtility
    {
        public static bool PortIsOccupy(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipGlobalProperties.GetActiveUdpListeners();
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (IPEndPoint ipEndPoint in ipEndPoints)
            {
                if (ipEndPoint.Port== port)
                {
                    return true;
                }
            }

            foreach (IPEndPoint ipEndPoint in ipsUDP)
            {
                if (ipEndPoint.Port == port)
                {
                    return true;
                }
            }

            foreach (TcpConnectionInformation info in tcpConnInfoArray)
            {
                if (info.LocalEndPoint.Port== port || info.RemoteEndPoint.Port == port)
                {
                    return true;
                }
            }
            return false;
        }

        //List<string> ips= GetLocalIpAddress("");//获取本地所有ip
        //List<string> ipv4_ips = GetLocalIpAddress("InterNetwork");//获取ipv4类型的ip  局域网 拨号上网的或具有独立公网IP
        //List<string> ipv6_ips = GetLocalIpAddress("InterNetworkV6");//获取ipv6类型的ip
        /// <summary>
        /// 获取本机所有ip地址
        /// </summary>
        /// <param name="netType">"InterNetwork":ipv4地址，"InterNetworkV6":ipv6地址</param>
        /// <returns>ip地址集合</returns>
        public static List<string> GetLocalIpAddress(string netType)
        {
            try
            {
                string hostName = Dns.GetHostName();                    //获取主机名称
                IPAddress[] addresses = Dns.GetHostAddresses(hostName); //解析主机IP地址

                List<string> IPList = new List<string>();
                if (netType == string.Empty)
                {
                    for (int i = 0; i < addresses.Length; i++)
                    {
                        IPList.Add(addresses[i].ToString());
                    }
                }
                else
                {
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    for (int i = 0; i < addresses.Length; i++)
                    {
                        if (addresses[i].AddressFamily.ToString() == netType)
                        {
                            IPList.Add(addresses[i].ToString());
                        }
                    }
                }
                return IPList;
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
                return null;
            }
        }

        /// <summary>
        /// 获得外网ip
        /// </summary>
        /// <returns></returns>
        public static string GetOuterNet()
        {
            Dictionary<string, string> nets = new Dictionary<string, string>();
            nets.Add("https://www.ip.cn", "utf-8");
            nets.Add("http://www.ip138.com/ips138.asp", "gbk");
            nets.Add("http://www.net.cn/static/customercare/yourip.asp", "gbk");
            Dictionary<string, string>.Enumerator enumerator = nets.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string ip = GetIPFromHtml(HttpGetPageHtml(enumerator.Current.Key, enumerator.Current.Value));
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] strs = ip.Split('.');
                    if (strs.Length==4)
                    {
                        return ip;
                    }
                }
            }
            return null;
        }

        //如果是路由上网的，想获取网关的外网IP，只能通过访问一些公网的地址来获取外网IP了；
        //var t0_html = HttpGetPageHtml("https://www.ip.cn", "utf-8");
        //var t1_html = HttpGetPageHtml("http://www.ip138.com/ips138.asp", "gbk");
        //var t2_html = HttpGetPageHtml("http://www.net.cn/static/customercare/yourip.asp", "gbk");
        //var t0_ip = GetIPFromHtml(t0_html);// 111.198.29.123
        //var t1_ip = GetIPFromHtml(t1_html);// 111.198.29.123
        //var t2_ip = GetIPFromHtml(t2_html);// 111.198.29.123
        //目前这几个url地址都可以用，但不保证长久稳定，如果想稳定可靠的使用，可以使用一些收费的API接口，比如：http://user.ip138.com/ip/   
        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="url">请求的地址</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string HttpGetPageHtml(string url, string encoding)
        {
            string pageHtml = string.Empty;
            try
            {
                using (WebClient MyWebClient = new WebClient())
                {
                    Encoding encode = Encoding.GetEncoding(encoding);
                    MyWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                    Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                    pageHtml = encode.GetString(pageData);
                }
            }
            catch (Exception e)
            {
                VLog.Exception(e);
            }
            return pageHtml;
        }
        /// <summary>
        /// 从html中通过正则找到ip信息(只支持ipv4地址)
        /// </summary>
        /// <param name="pageHtml"></param>
        /// <returns></returns>
        public static string GetIPFromHtml(String pageHtml)
        {
            //验证ipv4地址
            string reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
            string ip = "";
            Match m = Regex.Match(pageHtml, reg);
            if (m.Success)
            {
                ip = m.Value;
            }
            return ip;
        }
    }

}
