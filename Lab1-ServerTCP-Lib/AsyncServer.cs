﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Lab1_ServerTCP_Lib
{
    /// <summary>
    /// This class implements the TCP Server as a child object of the abstract server.
    /// </summary>
    public class AsyncServer : AbstractServer
    {
        bool important = true;
        Regex regexNumbers = new Regex(@"^\d{1,9}$");
        Regex regexPrint = new Regex(@"^[Pp][Rr][Ii][Nn][Tt]$");
        Regex regexKill = new Regex(@"^[Kk][Ii][Ll][Ll]$");
        Regex regexError = new Regex(@"^(\n\r|\r\n)$");
        Random rnd = new Random();
        char[] trim = { (char)0x0 };

        public delegate void TransmissionDataDelegate(NetworkStream nStream);
        public AsyncServer(IPAddress IP, int port) : base(IP, port)
        {
        }
        protected override void AcceptClient()
        {
            while (true)
            {
                tcpClient = TcpListener.AcceptTcpClient();
                stream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                transmissionDelegate.BeginInvoke(stream, TransmissionCallback, tcpClient);
            }
        }

        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[buffer_size];

            string hello = "You can send a number between 0 and 999999999.\n\r-If you do the server will respond with a random\n\r number between 1 and the given number.\n\r-Sending a 0 or 1 responds always with the result of 1.\n\r";
            var helloB = Encoding.UTF8.GetBytes(hello);
            stream.Write(helloB, 0, helloB.Length);
            hello = "You can also send the keyword \"print\" to see all generated numbers.\n\rIf you send the keyword \"end\" the connection will be closed.\n\r";
            helloB = Encoding.UTF8.GetBytes(hello);
            stream.Write(helloB, 0, helloB.Length);
            while (true)
            {
                try
                {
                    stream.Read(buffer, 0, buffer_size);
                    if (important)
                    {
                        string maxS = Encoding.UTF8.GetString(buffer).Trim(trim);
                        Match matchNumbers = regexNumbers.Match(maxS);
                        Match matchPrint = regexPrint.Match(maxS);
                        Match matchKill = regexKill.Match(maxS);
                        Match matchError = regexError.Match(maxS);
                        if (matchNumbers.Success)
                        {
                            int max = Int32.Parse(matchNumbers.Value);
                            max++;
                            var preValue = rnd.Next(1, max).ToString();
                            var value = Encoding.UTF8.GetBytes("Answer: " + preValue);
                            stream.Write(value, 0, value.Length);

                            using (StreamWriter writeFile = new StreamWriter(Path.GetFileName("Numbers.txt"), true))
                            {
                                writeFile.WriteLine(preValue);
                            }
                        }
                        else if (matchPrint.Success)
                        {
                            using (StreamReader readFile = new StreamReader("Numbers.txt"))
                            {
                                string line;
                                while ((line = readFile.ReadLine()) != null)
                                {
                                    helloB = Encoding.UTF8.GetBytes(line);
                                    stream.Write(helloB, 0, helloB.Length);
                                    hello = "\n\r";
                                    helloB = Encoding.UTF8.GetBytes(hello);
                                    stream.Write(helloB, 0, helloB.Length);
                                }
                            }
                        }
                        else if (matchKill.Success)
                        {
                            hello = "The connection has been closed!\n\r";
                            helloB = Encoding.UTF8.GetBytes(hello);
                            stream.Write(helloB, 0, helloB.Length);
                            break;
                        }
                        else if (matchError.Success)
                        {
                            hello = "\n\r";
                            helloB = Encoding.UTF8.GetBytes(hello);
                            stream.Write(helloB, 0, helloB.Length);
                            important = !important;
                        }
                        else
                        {
                            hello = "Wrong input!\n\r";
                            helloB = Encoding.UTF8.GetBytes(hello);
                            stream.Write(helloB, 0, helloB.Length);
                        }
                    }
                    else
                    {
                        stream.Write(buffer, 0, buffer_size);
                    }
                    buffer = new byte[buffer_size];
                    important = !important;
                }
                catch (IOException e)
                {
                    break;
                }
            }
        }
        private void TransmissionCallback(IAsyncResult ar)
        {
        }
        /// <summary>
        /// Overrided comment.
        /// </summary>
        public override void Start()
        {
            StartListening();
            //transmission starts within the accept function
            AcceptClient();
        }
    }
}

//Karol Sienkiewicz 140774