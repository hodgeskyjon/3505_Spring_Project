﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CustomNetworking;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SS
{
    public class SpreadsheetClientModel
    {
        private String password;
        private StringSocket clientSSocket;
        public List<string> fileList;
        public event Action<String> IncomingLineEvent;

        public SpreadsheetClientModel()
        {
            clientSSocket = null;
        }

        public void Connect(string hostname, int port, string clientPassword)
        {
            // initial connection to the server
            if (clientSSocket == null)
            {
                password = clientPassword;
                try
                {
                    TcpClient client = new TcpClient(hostname, port);
                    clientSSocket = new StringSocket(client.Client, UTF8Encoding.Default);
                    clientSSocket.BeginSend("PASSWORD" + "\\e" + password + "\n", (ex, p) => { }, null);
                    clientSSocket.BeginReceive(LineReceived, null);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            // not sure if this part is necessary?
            else
            {
                //clientSSocket.Close(); we don't want to close the socket unless the client is disconnecting
                TcpClient client = new TcpClient(hostname, port);
                clientSSocket = new StringSocket(client.Client, UTF8Encoding.Default);
                clientSSocket.BeginSend("PASSWORD"+ "\\e" + password + "\n", (ex, p) => { }, null);
                clientSSocket.BeginReceive(LineReceived, null);
            }
        }

        /// <summary>
        /// Send a line of text to the server. 
        /// Need to refactor for the messages that will be sent
        /// </summary>
        /// <param name="line"></param>
        public void SendMessage(String line)
        {
            if (clientSSocket != null)
            {
                clientSSocket.BeginSend(line, (ex, p) => { }, null);
            }
        }

        /// <summary>
        /// Deal with an arriving line of text.
        /// </summary>
        private void LineReceived(String s, Exception e, object p)
        {
            if (IncomingLineEvent != null)
            {
                IncomingLineEvent(s);
            }

            clientSSocket.BeginReceive(LineReceived, null);
        }

        public void Close()
        {
            clientSSocket.Close();

        }

    }
}
