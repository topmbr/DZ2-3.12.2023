using System;
using System.Net.Sockets;
using System.Text;
namespace ConsoleApp1
{
    internal class Program
    {
        static void Main()
        {
            TcpClient client = new TcpClient("127.0.0.1", 12345);
            NetworkStream clientStream = client.GetStream();

            while (true)
            {
                Console.WriteLine("Введите свой выбор (Камень, Ножницы или Бумага), или 'exit' для выхода:");
                string choice = Console.ReadLine();

                if (choice.ToLower() == "exit")
                    break;

                byte[] message = Encoding.UTF8.GetBytes(choice);
                clientStream.Write(message, 0, message.Length);
                clientStream.Flush();

                // Чтение ответа от сервера
                byte[] serverMessage = new byte[4096];
                int bytesRead = clientStream.Read(serverMessage, 0, 4096);
                string serverResponse = Encoding.UTF8.GetString(serverMessage, 0, bytesRead);

                Console.WriteLine("Ответ от сервера: " + serverResponse);
            }

            client.Close();
        }

    }
}