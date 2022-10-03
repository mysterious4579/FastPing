using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace Fast_Ping
{
    class Program
    {
        static byte[] Body = Encoding.ASCII.GetBytes("abcdefghijklmnopqrstuvwxyz 12345");
        static List<bool> LatestPacketSuccessCache = new List<bool>();

        static void Main(string[] args)
        {
            Console.Write("IP Address:");
            string Hostname = Console.ReadLine();
            //new Thread(() =>
            {
                while (true)
                {
                    using (Ping ping = new Ping())
                    {
                        //Stopwatch sw = new Stopwatch();
                        PingReply reply = ping.Send(Hostname, 1000, Body);
                        //sw.Stop();
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine("Reply from " + reply.Address + ": bytes=" + Body.Length + " time=" + reply.RoundtripTime);
                            LatestPacketSuccessCache.Add(true);
                        }
                        if (reply.Status == IPStatus.TimedOut)
                        {
                            Console.WriteLine("Request timed out.");
                            LatestPacketSuccessCache.Add(false);
                        }
                        if (reply.Status == IPStatus.DestinationHostUnreachable)
                        {
                            Console.WriteLine("Destination host unreachable.");
                            LatestPacketSuccessCache.Add(false);
                        }
                        if (reply.Status == IPStatus.BadDestination)
                        {
                            Console.WriteLine("Bad destination host");
                            LatestPacketSuccessCache.Add(false);
                        }
                        if (LatestPacketSuccessCache.Count >= 50)
                        {
                            LatestPacketSuccessCache.RemoveAt(0);
                        }
                        int FailedPackets = 0;
                        foreach(bool Success in LatestPacketSuccessCache)
                        {
                            if (Success == false)
                            {
                                FailedPackets = FailedPackets + 1;
                            }
                        }
                        Console.Title = "Average Packet Loss (Of the last 50 packets) - " + Convert.ToInt32((Convert.ToDouble(FailedPackets) / Convert.ToDouble(LatestPacketSuccessCache.Count)) * 100) + "%";
                    }
                }
            }//).Start();
        }
    }
}
