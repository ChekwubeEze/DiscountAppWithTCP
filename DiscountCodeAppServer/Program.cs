using DiscountCodeAppServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscountCodeAppServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            Console.WriteLine(" Discount Code Application started .............");
            while(true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected");

                Thread clientThread = new Thread(() => CommonHelper.HandleClient(client));
                clientThread.Start();
            }
        }
    }
}
