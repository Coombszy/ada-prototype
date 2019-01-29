﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace SocketHandler
{
    public class SocketHandler
    {
        public TcpClient ClientSocket;
        public NetworkStream DataStream;
        private IPAddress TargetIp = IPAddress.Parse("127.0.0.1");
        private int TargetPort = 25565;
        public SocketHandler()
        {
            ClientSocket = new System.Net.Sockets.TcpClient();
        }
        public void ConfigSocket(string IpAddress, int Port)
        {
            this.TargetIp = IPAddress.Parse(IpAddress);
            this.TargetPort = Port;
        }
        public void StartSocket()
        {
            if(Connect())
            {
                Console.WriteLine("Successfully Connected to server!");
            }
        }


        //Basic Functions for Socket Interaction
        public void Dispose()
        {
            ClientSocket.Close();
            ClientSocket = null;
            Console.WriteLine("CLIENTSOCKETCLOSED");
        }
        private bool Connect()
        {
            try
            {
                ClientSocket.Connect(TargetIp, TargetPort);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed To Connect!");
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        private void Disconnect()
        {
            Console.WriteLine("Disconnecting now!");
            Send("/DISCONNECT");
        }
        public void Send(string ToSend)
        {
            try
            {
                NetworkStream SendStream = ClientSocket.GetStream();
                string data = ToSend;
                //Console.WriteLine("ISent=" + data); debuig to make sure the correct data is sent
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(data + "$");
                SendStream.Write(outStream, 0, outStream.Length);
                SendStream.Flush();
            }
            catch { }
        }
        private string ListenOnInstant()
        {
            byte[] BytesReceived = new byte[1280];
            string DataReceived = null;
            while (true)
            {
                try
                {
                    DataStream = ClientSocket.GetStream();
                    if (DataStream.DataAvailable)
                    {
                        DataStream.Read(BytesReceived, 0, BytesReceived.Length);
                        DataReceived = System.Text.Encoding.ASCII.GetString(BytesReceived);
                        DataReceived = DataReceived.Substring(0, DataReceived.IndexOf("$"));
                    }
                    else { DataReceived = ""; }
                    DataStream.Flush();
                    break;
                }
                catch { break; }
            }
            //Method for checking if a client is still connected
            if (DataReceived == "/AREYOUTHERE")
            {
                Send("/IMHERE");
                DataReceived = "";
            }
            return DataReceived;
        }
        public string Listen()
        {
            byte[] BytesReceived = new byte[1280];
            string DataReceived = null;
            while (true)
            {
                try
                {
                    DataStream = ClientSocket.GetStream();
                    DataStream.Read(BytesReceived, 0, BytesReceived.Length);
                    DataReceived = System.Text.Encoding.ASCII.GetString(BytesReceived);
                    DataReceived = DataReceived.Substring(0, DataReceived.IndexOf("$"));
                    DataStream.Flush();
                    break;
                }
                catch { break; }
            }
            //Method for checking if a client is still connected
            if (DataReceived == "/AREYOUTHERE")
            {
                Send("/IMHERE");
                DataReceived = "";
            }
            return DataReceived;
        }
    }
}