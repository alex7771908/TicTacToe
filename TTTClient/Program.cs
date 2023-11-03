using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ClientMagicBall
{
    internal class Program
    {
        public static string[,] field = new string[3, 3];
        public static string player = "";
        static void Main(string[] args)
        {


            Console.WriteLine("[CLIENT]");

            string ipLine = "127.0.0.1";
            int portServer = 7632;
            IPAddress iPAddressServer = IPAddress.Parse(ipLine);
            IPEndPoint iPEndPointServer = new IPEndPoint(iPAddressServer, portServer);
            Socket socketClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);


            Console.WriteLine("Нажмите на энтер чтобы подключиться");
            Console.ReadLine();

           socketClient.Connect(iPEndPointServer);

            Console.WriteLine("Клиент подключился");
            Console.WriteLine("Вы играете в крестики нолики: первым ходит клиент(ТЫ) - крестики\n" +
                "Вторыми сервер - нолики \n" +
                "Вводите координаты в формате (x y), где x - ряд, а y - столбец\n" +
                "Первый кто соберет 3 знака в ряд победит");
            while (true)
            {
                Console.WriteLine("Ходите: ");
                string question = Console.ReadLine();
                string[] index = question.Split(' ');
                bool isCorrect = true; 
                foreach (string item in index)
                {
                    if (int.TryParse(item, out int res) == false)
                    {
                        isCorrect = false;
                    }
                    if (res < 0 || res > 3)
                    {
                        isCorrect = false;
                    }
                }
                while (!isCorrect)
                {
                    Console.WriteLine("INCORRECT FORMAT");
                    question = Console.ReadLine();
                    index = question.Split(' ');
                    foreach (string item in index)
                    {
                        if (!int.TryParse(item, out int res))
                        {
                            isCorrect = false;
                            break;
                        }
                        if (res < 0 || res > 3)
                        {
                            isCorrect = false;
                            break;
                        }
                        isCorrect = true;
                    }
                }
                
                while (field[int.Parse(index[0]) - 1, int.Parse(index[1]) - 1] != null)
                {
                    Console.WriteLine("Эта клетка уже выбрана выберите другую");
                    question = Console.ReadLine();

                    index = question.Split(' ');
                    foreach (string item in index)
                    {
                        if (int.TryParse(item, out int res) == false)
                        {
                            isCorrect = false;
                        }
                        if(res < 0 || res > 3)
                        {
                            isCorrect = false;
                        }
                    }
                    while (!isCorrect)
                    {
                        Console.WriteLine("INCORRECT FORMAT");
                        question = Console.ReadLine();
                        index = question.Split(' ');
                        foreach (string item in index)
                        {
                            if (!int.TryParse(item, out int res))
                            {
                                isCorrect = false;
                                break;
                            }
                            if (res < 0 || res > 3)
                            {
                                isCorrect = false;
                                break;
                            }
                            isCorrect = true;
                        }
                    }
                }
                field[int.Parse(index[0]) - 1, int.Parse(index[1]) - 1] = "X";

                byte[] buffer = Encoding.UTF8.GetBytes(question);
                socketClient.Send(buffer);
                
                TTT();

                byte[] bytes = new byte[1024];
                int numData = socketClient.Receive(bytes);
                string turnServer = Encoding.UTF8.GetString(bytes, 0, numData);
                string[] move = turnServer.Split(' ');
                if (move.Length == 1)
                {
                    player = move[0];
                    break;
                }
                field[int.Parse(move[0]) - 1, int.Parse(move[1]) - 1] = "O";
                Console.WriteLine("Сервер сходил");
                TTT();
                if (move.Length == 3)
                {
                    player = move[2];
                    break;
                }
                
            }
            Console.WriteLine("ПОБЕДИЛ " + player);
            Console.ReadLine();

        }

        public static void TTT()
        {
            Console.WriteLine("--------");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write("|" + field[i, j]);
                    if (field[i, j] != "X" && field[i, j] != "O")
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write("|");
                Console.WriteLine();
                Console.WriteLine("--------");
            }
        }
    }
}
