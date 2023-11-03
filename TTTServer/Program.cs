using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TTTServer
{
    public class Program
    {
        public static string[,] field = new string[3, 3];
        public static bool isFinished = false;
        public static string player = "";
        static void Main(string[] args)
        {
            Console.WriteLine("[Server]");

            string IpLine = "127.0.0.1";
            int portServer = 7632;
            IPAddress iPAddressServer = IPAddress.Parse(IpLine);
            IPEndPoint iPEndPointServer = new IPEndPoint(iPAddressServer, portServer);
            Socket socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServer.Bind(iPEndPointServer);

            socketServer.Listen(1);
            Console.WriteLine("Сервер запущен");

            Socket socketClient = socketServer.Accept();

            Console.WriteLine("Подключилось");
            byte[] bytes = new byte[1024];
            Console.WriteLine("Вы играете в крестики нолики: первым ходит клиент - крестики\n" +
                "Вторыми сервер(ТЫ) - нолики \n" +
                "Вводите координаты в формате (x y), где x - ряд, а y - столбец\n" +
                "Первый кто соберет 3 знака в ряд победит");
            while (!isFinished)
            {
                int numData = socketClient.Receive(bytes);
                string turnClient = Encoding.UTF8.GetString(bytes, 0, numData);
                string[] index = turnClient.Split(' ');
                field[int.Parse(index[0]) - 1, int.Parse(index[1]) - 1] = "X";

                //Console.WriteLine(turnClient);
                Console.WriteLine("Клиент сходил");
                TTT();

                if(Win(index, "X"))
                {
                    player = "X";
                    socketClient.Send(Encoding.UTF8.GetBytes(player));
                    isFinished = true;
                    break;
                }
                Console.WriteLine("Ходите:");
                string answer = Console.ReadLine();

                string[] indexServer = answer.Split(' ');
                bool isCorrect = true;
                foreach (string item in indexServer)
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
                    answer = Console.ReadLine();
                    indexServer = answer.Split(' ');
                    foreach (string item in indexServer)
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

                while (field[int.Parse(indexServer[0]) - 1, int.Parse(indexServer[1]) - 1] != null)
                {
                    Console.WriteLine("Эта клетка уже выбрана выберите другую");
                    answer = Console.ReadLine();

                    indexServer = answer.Split(' ');
                    foreach (string item in indexServer)
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
                        answer = Console.ReadLine();
                        indexServer = answer.Split(' ');
                        foreach (string item in indexServer)
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
                field[int.Parse(indexServer[0])-1, int.Parse(indexServer[1]) - 1] = "O";

                TTT();
                //Console.WriteLine(Win(indexServer, "O"));
                bytes = Encoding.UTF8.GetBytes(answer);
                if (Win(indexServer, "O"))
                {
                    player = "O";
                    isFinished=true;
                    socketClient.Send(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes) + ' ' + player));
                    break;
                }
         
                socketClient.Send(bytes);

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
                    if(field[i, j] != "X" && field[i, j] != "O")
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write("|");
                Console.WriteLine();
                Console.WriteLine("--------");
            }
        }

        public static bool Win(string[] coordinates, string player)
        {
            int r = int.Parse(coordinates[0] ) - 1;
            int c = int.Parse(coordinates[1]) - 1;
            (int, int)[] horizontal = { (r, 0), (r, 1), (r, 2) };
            (int, int)[] vertical = { (0, c), (1, c), (2, c) };
            (int, int)[] diag = { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antidiag = { (0, 2), (1, 1), (2, 0) };
            if(Line(diag, player) || Line(antidiag, player) || Line(horizontal, player) || Line(vertical, player))
            {
                return true;
            }
            return false;

        }

        public static bool Line((int, int)[] line, string player)
        {
            foreach ((int r, int c) in line)
            {
                if(field[r,c] != player)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
