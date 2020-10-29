using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Lab1_ServerTCP_Lib
{
    /// <summary>
    /// Server is a class handeling the client requests
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Listener
        /// </summary>
        TcpListener listener;
        /// <summary>
        /// Buffer to save data to
        /// </summary>
        byte[] buffer;
        /// <summary>
        /// Size of the buffer
        /// </summary>
        int buffersize;

        /// <summary>
        /// During construction the listener and buffer are set as well
        /// </summary>
        public Server()
        {
            listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 8080);
            buffersize = 1024;
            buffer = new byte[buffersize];
        }

        /// <summary>
        /// Funktion responsible for the functionality of the server. Doesn't take nor return any variables.
        /// </summary>
        public void Run()
        {
            listener.Start();
            var client = listener.AcceptTcpClient();
            var nStream = client.GetStream();
            bool important = true;

            string hello = "Get random number from 1 to the number you send.\n\rIf no numerical values are send the connection closes!\n\r";
            var helloB = Encoding.UTF8.GetBytes(hello);
            nStream.Write(helloB, 0, helloB.Length);

            Regex regex = new Regex(@"\d+");
            Random rnd = new Random();

            while (true)
            {
                nStream.Read(buffer, 0, buffersize);
                if (important)
                {
                    string maxS = Encoding.UTF8.GetString(buffer);
                    Match match = regex.Match(maxS);
                    if(match.Success)
                    {
                        int max = Int32.Parse(match.Value);
                        max++;
                        var value = Encoding.UTF8.GetBytes(rnd.Next(1,max).ToString());
                        nStream.Write(value, 0, value.Length);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    nStream.Write(buffer, 0, buffersize);
                }
                buffer = new byte[buffersize];
                important = !important;
            }
            client.Close();
            listener.Stop();
        }
    }
}

//Karol Sienkiewicz 140774