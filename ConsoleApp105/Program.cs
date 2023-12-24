using System;
using System.Net;
using System.Net.Sockets;
using System.Text;  
namespace ConsoleApp105
{
    internal class Program
    {
        static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 12345);
        server.Start();
        Console.WriteLine("Сервер запущен...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Новое подключение!");

            // Создаем отдельный поток для каждого клиента
            System.Threading.Thread clientThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream clientStream = tcpClient.GetStream();

        int clientScore = 0;
        int serverScore = 0;

        while (clientScore < 3 && serverScore < 3)
        {
            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                    break;

                string clientChoice = Encoding.UTF8.GetString(message, 0, bytesRead);
                Console.WriteLine("Клиент выбрал: " + clientChoice);

                // Логика игры
                string serverChoice = GetServerChoice();
                Console.WriteLine("Сервер выбрал: " + serverChoice);

                string result = DetermineWinner(clientChoice, serverChoice);

                // Отправка результата клиенту
                byte[] serverMessage = Encoding.UTF8.GetBytes(result);
                clientStream.Write(serverMessage, 0, serverMessage.Length);
                clientStream.Flush();

                // Обновление счета
                if (result.Contains("выиграли"))
                    clientScore++;
                else if (result.Contains("проиграли"))
                    serverScore++;

                Console.WriteLine($"Счет: Клиент {clientScore}, Сервер {serverScore}");
            }
        }

        // Определение победителя игры
        string gameResult = (clientScore == 3) ? "Вы победили!" : "Вы проиграли!";
        byte[] finalMessage = Encoding.UTF8.GetBytes(gameResult);
        clientStream.Write(finalMessage, 0, finalMessage.Length);
        clientStream.Flush();

        tcpClient.Close();
    }

    static string GetServerChoice()
    {
        // Простая реализация выбора сервера (камень, ножницы или бумага)
        Random random = new Random();
        int choiceIndex = random.Next(3);
        string[] choices = { "Камень", "Ножницы", "Бумага" };
        return choices[choiceIndex];
    }

    static string DetermineWinner(string clientChoice, string serverChoice)
    {
        // Простая логика определения победителя
        if (clientChoice == serverChoice)
            return "Ничья!";
        else if (
            (clientChoice == "Камень" && serverChoice == "Ножницы") ||
            (clientChoice == "Ножницы" && serverChoice == "Бумага") ||
            (clientChoice == "Бумага" && serverChoice == "Камень")
        )
            return "Вы выиграли!";
        else
            return "Вы проиграли!";
    }



    }
}