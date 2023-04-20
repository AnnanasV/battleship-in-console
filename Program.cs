using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleship
{
    class Program
    {
        public const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static void Main(string[] args)
        {

            while (true)
            {
                DrawAndClear();
                Console.WriteLine("1  -  Create your field\n2  -  Play with friend\n3  -  Delete wrong file\n");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        CreateFields();
                        break;
                    case "2":
                        Game game = new Game();
                        DrawAndClear();
                        Console.WriteLine("Enter your number: ");
                        string sNum1 = Console.ReadLine();
                        Console.WriteLine("Enter number of your friend: ");
                        string sNum2 = Console.ReadLine();
                        DrawAndClear();
                        if (Int32.TryParse(sNum1, out int num1) && Int32.TryParse(sNum2, out int num2))
                        {
                            Console.WriteLine("Enter your name:");
                            string path1 = $"{Console.ReadLine()}_PlayerField{num1}.txt";
                            Console.WriteLine("Enter name of your friend:");
                            string path2 = $"{Console.ReadLine()}_PlayerField{num2}.txt";
                            if (File.Exists(path1) && File.Exists(path2))
                                game.StartPlay(path1, path2);
                            else
                            {
                                Game.WriteWithColor($"File \"{path1}\" or \"{path2}\" isn`t exist !!!", ConsoleColor.DarkRed);
                                Console.ReadKey();
                            }
                        }
                        break;
                    case "3":
                        DrawAndClear();
                        Console.WriteLine("Enter your number: ");
                        if (Int32.TryParse(Console.ReadLine(), out int num))
                        {
                            Console.WriteLine("Enter your name:");
                            string path = $"{Console.ReadLine()}_PlayerField{num}.txt";
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                                Game.WriteWithColor("FILE HAS BEEN DELETED", ConsoleColor.DarkGreen);
                            }
                            else
                            {
                                Game.WriteWithColor($"File \"{path}\" isn`t exist !!!", ConsoleColor.DarkRed);
                                Console.ReadKey();
                            }
                        }
                        break;
                    default:
                        return;
                }
            }

        }

        static void DrawAndClear()
        {
            Console.Clear();
            Game.WriteWithColor("======= CHOOSE OPTION =======\n", ConsoleColor.DarkCyan);
        }

        static void CreateFields()
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            DrawTitle();

            Game.WriteWithColor("Enter your name: ", ConsoleColor.DarkCyan);
            string name = Console.ReadLine();
            DrawTitle();
            Console.WriteLine($"\nSize of field(max: 25 25):\n\n(if the opponent has already specified it, specify the same)");
            Console.SetCursorPosition(0, 3);
            string[] sizeOfField = Console.ReadLine().Split(' ');
            if (!Int32.TryParse(sizeOfField[0], out int fieldX) || !Int32.TryParse(sizeOfField[1], out int fieldY) || fieldX >= 26 || fieldY >= 26)
            {
                DrawTitle();
                Game.WriteWithColor("Try again. . .", ConsoleColor.DarkRed);
                Console.ReadKey();
                return;
            }

            DrawTitle();
            Console.WriteLine();
            char[,] field = new char[fieldY + 2, fieldX + 2]; // 2 - borders

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (i == 0 || i == field.GetLength(0) - 1 || j == 0 || j == field.GetLength(1) - 1)
                    {
                        field[i, j] = '#';
                    }
                    else
                    {
                        field[i, j] = ' ';
                    }
                }
            }
            Console.CursorVisible = false;
            Ship[] coordinates;
            List<int[]> allCoordinates = new List<int[]>();
            //Game.WriteWithColor("First ship 4х1", ConsoleColor.Red);
            coordinates = SetShips(field, 1, 4);
            RedistributeValues(coordinates, allCoordinates, 1);
            //Game.WriteWithColor("Second two ships 3х1", ConsoleColor.Red);
            coordinates = SetShips(field, 2, 3);
            RedistributeValues(coordinates, allCoordinates, 2);
            //Game.WriteWithColor("Three ships 2х1", ConsoleColor.Red);
            coordinates = SetShips(field,  3, 2);
            RedistributeValues(coordinates, allCoordinates, 3);
            //Game.WriteWithColor("Last four ships 1х1", ConsoleColor.Red);
            coordinates = SetShips(field, 4, 1);
            RedistributeValues(coordinates, allCoordinates, 4);
            DrawField(field);

            Console.SetCursorPosition(0, field.GetLength(0) * 2 + 3);
            Console.WriteLine("Number of player (1/2)");
            Int32.TryParse(Console.ReadLine(), out int playerNumber);

            string path = $"{name}_PlayerField{playerNumber}.txt";
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                foreach (var coord in allCoordinates)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i == 3)
                        {
                            sw.WriteLine(coord[i] + " ");
                            continue;
                        }
                        if (coord[i] < 10)
                        {
                            sw.Write(coord[i] + " ");
                        }
                        else
                            sw.Write(coord[i]);
                    }
                }
                sw.WriteLine(Convert.ToString(fieldX + " " + fieldY));
            }
            Game.WriteWithColor($"\nFile \"{path}\" already ready!)", ConsoleColor.DarkGreen);
            Console.ReadKey();
        }

        static void RedistributeValues(Ship[] fromThis, List<int[]> toThis, int amountOfTimes)
        {
            for (int i = 0; i < amountOfTimes; i++)
            {
                int[] arr = new int[4] { fromThis[i].StartY, fromThis[i].StartX, fromThis[i].EndY, fromThis[i].EndX };
                toThis.Add(arr);
            }
        }

        static Ship[] SetShips(char[,] field, int amount, int sizeOfShip)
        {
            Ship[] forFile = new Ship[amount];
            for (int i = 0; i < amount; i++)
            {
                Ship firstCoordinates = new Ship(1, 1, 1, sizeOfShip);
                firstCoordinates = GetCoordinates(field, sizeOfShip, firstCoordinates);
                for (int j = 0; j < 4; j++)
                {
                    forFile[i] = firstCoordinates;
                }
            }
            return forFile;
        }

        static void DrawField(char[,] Field)
        {
            DrawTitle();
            for (int i = 0; i < Field.GetLength(0); i++)
            {
                Game.DrawLines('-', 6 * Field.GetLength(1) - 1);
                for (int j = 0; j < Field.GetLength(1); j++)
                {
                    if (Field[i, j] == '_')
                    {
                        Game.WriteWithColor("---", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                    }
                    else if (i == 0 && j > 0 && j < Field.GetLength(1) - 1)
                    {
                        if (j < 10)
                            Console.Write($" {j} ");
                        else
                            Console.Write($"{j} ");
                    }
                    else if (j == 0 && i > 0 && i < Field.GetLength(0) - 1)
                    {
                        Console.Write($" {letters[i - 1]} ");
                    }
                    else
                        Console.Write($" {Field[i, j]} ");
                    Console.Write(" | ");
                }
            }
            Console.WriteLine();
        }

        static void DrawShipInConsole(Ship ship, string sign, char[,] Field)
        {
            for(int i = ship.StartY; i <= ship.EndY; i++)
            {
                for(int j = ship.StartX; j <= ship.EndX; j++)
                {
                    Console.SetCursorPosition(6 * j, 3 + 2 * i);
                    if(sign != "")
                        Game.WriteWithColor(sign, ConsoleColor.DarkBlue, ConsoleColor.DarkBlue);
                    else if(Field[i, j] == ' ')
                        Game.WriteWithColor("   ", ConsoleColor.Black, ConsoleColor.Black);
                    else Game.WriteWithColor("---", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                }
            }
        }

        static Ship GetCoordinates(char[,] Field, int sizeOfShip, Ship currentShip)
        {
            ConsoleKey pressedKey;
            bool isEnd = false;
            DrawField(Field);
            DrawShipInConsole(currentShip, "===", Field);
            while (!isEnd)
            {
                pressedKey = Console.ReadKey(true).Key;
                switch (pressedKey)
                {
                    case ConsoleKey.RightArrow:
                        if (currentShip.EndX + 1 < Field.GetLength(1) - 1)
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            ++currentShip.EndX;
                            ++currentShip.StartX;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (currentShip.StartX - 1 >= 1)
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            --currentShip.EndX;
                            --currentShip.StartX;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (currentShip.EndY + 1 < Field.GetLength(0) - 1)
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            ++currentShip.EndY;
                            ++currentShip.StartY;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (currentShip.StartY - 1 >= 1)
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            --currentShip.EndY;
                            --currentShip.StartY;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        break;
                    case ConsoleKey.Spacebar:
                        if (currentShip.EndY - currentShip.StartY == 0 && currentShip.StartY + sizeOfShip < Field.GetLength(0)) // if horizontal
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            currentShip.EndY += sizeOfShip - 1;
                            currentShip.EndX -= sizeOfShip - 1;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        else if (currentShip.EndX - currentShip.StartX == 0 && currentShip.StartX + sizeOfShip < Field.GetLength(1)) // if vertical
                        {
                            DrawShipInConsole(currentShip, "", Field);
                            currentShip.EndY -= sizeOfShip - 1;
                            currentShip.EndX += sizeOfShip - 1;
                            DrawShipInConsole(currentShip, "===", Field);
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (currentShip.CheckBorders(Field))
                        {
                            WriteShipInField(currentShip, Field);
                            isEnd = true;
                        }
                        break;
                }
            }
            return currentShip;
        }

        static void WriteShipInField(Ship ship, char[,] Field)
        {
            for(int i = ship.StartY; i <= ship.EndY; i++)
            {
                for(int j = ship.StartX; j <= ship.EndX; j++)
                {
                    Field[i, j] = '_';
                }
            }
        }

        static void DrawTitle()
        {
            Console.Clear();
            Console.WriteLine("~~~~~~ BATTLESHIP COORDINATES ~~~~~~");
        }
    }
}
