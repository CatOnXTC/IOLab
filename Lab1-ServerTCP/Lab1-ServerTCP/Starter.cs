using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lab1_ServerTCP_Lib;

namespace Lab1_ServerTCP
{
    /// <summary>
    /// Starter is responsible for starting the server
    /// </summary>
    public class Starter
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            AsyncServer server = new AsyncServer(IPAddress.Parse("127.0.0.1"),8080);
            server.Start();
        }
    }
}

//Karol Sienkiewicz 140774