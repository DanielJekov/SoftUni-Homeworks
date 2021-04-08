using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace SimpleTcpChatClient
{
    class ChatClient
    {
        static void Main(string[] args)
        {
            string username = SettingUserData();

            TcpConnection(username);
        }

        static string SettingUserData()
        {
            const string askingAboutName = "Please insert name that will help other member to recognize you.";

            Console.WriteLine(askingAboutName);

            string username = Console.ReadLine();

            return username;
        }

        static string  MessageReadet()
        {
            const string label = "Write text and send with \"Enter\" button.";
            Console.WriteLine(label);

            string messageInput = Console.ReadLine();

            return messageInput;
        }

        public static void TcpConnection(string username)
        {
            connection:
            try
            {
                //Here is the Tcp connection with the server
                TcpClient client = new TcpClient("127.0.0.1", 1302);

                string messageReadet = MessageReadet();
                while (messageReadet.ToLower() != "exit")
                {
                    var messageToSend = username + Environment.NewLine + messageReadet;
                    //Here we parse to byte couse network message must be in byte format
                    int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                    byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);
                    //Sending Data to server
                    NetworkStream stream = client.GetStream();
                    stream.Write(sendData, 0, sendData.Length);

                    //Reading Data from server
                    StreamReader sr = new StreamReader(stream);
                    string response = sr.ReadLine();
                    Console.WriteLine(response);

                    stream.Close();
                    messageReadet = MessageReadet();
                }
                
                client.Close();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                goto connection;
            }
        }
    }
}

