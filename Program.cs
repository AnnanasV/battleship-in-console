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
                                WriteWithColor($"File \"{path1}\" or \"{path2}\" isn`t exist !!!", ConsoleColor.DarkRed);
                                Console.ReadKey();
                            }
                        }
                        break;
                    case "3":
                        DrawAndClear();
                        Console.WriteLine("Enter your number: ");
                        if(Int32.TryParse(Console.ReadLine(), out int num))
                        {
                            Console.WriteLine("Enter your name:");
                            string path = $"{Console.ReadLine()}_PlayerField{num}.txt";
                            if (File.Exists(path))
                            {
                                File.Delete(path);
                                WriteWithColor("FILE HAS BEEN DELETED", ConsoleColor.DarkGreen);
                            }
                            else
                            {
                                WriteWithColor($"File \"{path}\" isn`t exist !!!", ConsoleColor.DarkRed);
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
            WriteWithColor("======= CHOOSE OPTION =======\n", ConsoleColor.DarkCyan);
        }

        static void CreateFields()
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            DrawTitle();
            Console.Write("\nEnter your name: ");
            string name = Console.ReadLine();
            DrawTitle();
            Console.WriteLine($"\nSize of field(max: 25 25):\n\n(if the opponent has already specified it, specify the same)");
            Console.SetCursorPosition(0, 3);
            string[] sizeOfField = Console.ReadLine().Split(' ');
            if (!Int32.TryParse(sizeOfField[0], out int fieldX) || !Int32.TryParse(sizeOfField[1], out int fieldY) || fieldX >= 26 || fieldY >= 26)
            {
                DrawTitle();
                WriteWithColor("Try again. . .", ConsoleColor.DarkRed);
                Console.ReadKey();
                return;
            }

            DrawTitle();
            Console.WriteLine();
            string[,] field = new string[fieldY + 2, fieldX + 2]; // 2 - borders
            string[,] fieldForShip = new string[field.GetLength(0), field.GetLength(1)];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (i == 0 && j != 0 && j < field.GetLength(1) - 1)
                    {
                        field[i, j] = $" {j}";
                        if (j < 10)
                            field[i, j] = $" {j} ";
                    }

                    else if (j == 0 && i != 0 && i < field.GetLength(0) - 1)
                    {
                        field[i, j] = $" {i}";
                        if (i < 10)
                            field[i, j] = $" {i} ";
                    }

                    else if (i == 0 || i == field.GetLength(0) - 1 || j == 0 || j == field.GetLength(1) - 1)
                    {
                        field[i, j] = " # ";
                    }
                    else
                    {
                        field[i, j] = "   ";
                    }
                }
            }
            DrawField(field, fieldForShip);
            Console.CursorVisible = false;
            int[,] coordinates;
            List<int[]> allCoordinates = new List<int[]>();
            WriteWithColor("First ship 4х1", ConsoleColor.Red);
            coordinates = SetShips(field, fieldForShip, 1, 4);
            RedistributeValues(coordinates, allCoordinates, 1);
            WriteWithColor("Second two ships 3х1", ConsoleColor.Red);
            coordinates = SetShips(field, fieldForShip, 2, 3);
            RedistributeValues(coordinates, allCoordinates, 2);
            WriteWithColor("Three ships 2х1", ConsoleColor.Red);
            coordinates = SetShips(field, fieldForShip, 3, 2);
            RedistributeValues(coordinates, allCoordinates, 3);
            WriteWithColor("Last four ships 1х1", ConsoleColor.Red);
            coordinates = SetShips(field, fieldForShip, 4, 1);
            RedistributeValues(coordinates, allCoordinates, 4);
            DrawField(field, fieldForShip);

            Console.SetCursorPosition(0, field.GetLength(0) + 2);
            Console.WriteLine("Number of player (1/2)");
            Int32.TryParse(Console.ReadLine(), out int playerNumber);
            WriteWithColor("Ok, you are " + playerNumber + " player", ConsoleColor.DarkMagenta);
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
            WriteWithColor($"File \"{path}\" already ready!)", ConsoleColor.DarkGreen);
            Console.ReadKey();
        }

        static void RedistributeValues(int[,] fromThis, List<int[]> toThis, int amountOfTimes)
        {
            for (int i = 0; i < amountOfTimes; i++)
            {
                int[] arr = new int[4];
                for (int j = 0; j < 4; j++)
                {
                    arr[j] = fromThis[i, j];
                }
                toThis.Add(arr);
            }
        }

        static int[,] SetShips(string[,] field, string[,] fieldForShip, int amount, int sizeOfShip)
        {
            int[,] forFile = new int[amount, 4];
            for (int i = 0; i < amount; i++)
            {
                int[] firstCoordinates = GetCoordinates(field, fieldForShip, sizeOfShip);
                for (int j = 0; j < 4; j++)
                {
                    forFile[i, j] = firstCoordinates[j];
                }
            }
            return forFile;
        }

        static void WriteWithColor(string massage, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(massage);
            Console.ResetColor();
        }

        static void DrawField(string[,] Field, string[,] FieldNewShip)
        {
            DrawTitle();
            for (int y = 0; y < Field.GetLength(0); y++)
            {
                for (int x = 0; x < Field.GetLength(1); x++)
                {
                    if (FieldNewShip[y, x] == "===")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(FieldNewShip[y, x]);
                        Console.ResetColor();
                    }
                    else if (Field[y, x] == "___")
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(Field[y, x]);
                        Console.ResetColor();
                    }
                    else
                        Console.Write(Field[y, x]);
                }
                Console.WriteLine();
            }
        }
        static void DrawShip(string[,] Field, string[,] FieldNewShip, int shipX, int shipY, int sizeOfShip, char side, string sign)
        {
            if (side == 'H') //horizontal
            {
                for (int i = 0; i < sizeOfShip; i++)
                {
                    FieldNewShip[shipY, shipX + i] = sign;
                }
                DrawField(Field, FieldNewShip);
            }
            else if (side == 'V') // vertical
            {
                for (int i = 0; i < sizeOfShip; i++)
                {
                    FieldNewShip[shipY + i, shipX] = sign;
                }
                DrawField(Field, FieldNewShip);
            }
        }

        static int[] GetCoordinates(string[,] Field, string[,] FieldForShip, int sizeOfShip)
        {
            int sizeOfShipX = sizeOfShip, sizeOfShipY = 1;
            char direction = 'H';
            int shipX = 1, shipY = 1;
            ConsoleKey pressedKey;
            DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
            while (true)
            {
                Console.SetCursorPosition(Field.GetLength(1) * 3 + 3, shipY);
                pressedKey = Console.ReadKey().Key;
                if ((pressedKey == ConsoleKey.RightArrow) && (shipX + sizeOfShipX < Field.GetLength(1) - 1))
                {
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                    shipX += 1;
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                }
                else if (pressedKey == ConsoleKey.LeftArrow && shipX - 1 >= 1)
                {
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                    shipX -= 1;
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                }
                else if ((pressedKey == ConsoleKey.DownArrow) && shipY + sizeOfShipY + 1 < Field.GetLength(0))
                {
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                    shipY += 1;
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                }
                else if ((pressedKey == ConsoleKey.UpArrow) && shipY - 1 >= 1)
                {
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                    shipY -= 1;
                    DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                }
                else if (pressedKey == ConsoleKey.Spacebar)
                {
                    if (sizeOfShipX > 1 && shipY + sizeOfShip < Field.GetLength(0))
                    {
                        sizeOfShipY = sizeOfShip;
                        sizeOfShipX = 1;
                        DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                        direction = 'V';
                        DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                    }
                    else if (sizeOfShipY > 1 && shipX + sizeOfShip < Field.GetLength(1))
                    {
                        sizeOfShipX = sizeOfShip;
                        sizeOfShipY = 1;
                        DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                        direction = 'H';
                        DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "===");
                    }
                }
                else if (pressedKey == ConsoleKey.Enter)
                {
                    if (CheckPosition(Field, shipX, shipY, sizeOfShip, direction))
                    {
                        DrawShip(Field, FieldForShip, shipX, shipY, sizeOfShip, direction, "   ");
                        DrawShip(FieldForShip, Field, shipX, shipY, sizeOfShip, direction, "___");
                        break;
                    }
                }
            }

            return new int[] { shipY, shipX, shipY + sizeOfShipY - 1, shipX + sizeOfShipX - 1 };
        }

        static bool CheckPosition(string[,] Field, int shipX, int shipY, int sizeOfShip, char side)
        {
            bool isPossible = true;
            if (side == 'H')
            {
                for (int y = shipY - 1; y <= shipY + 1; y++)
                {
                    for (int x = shipX - 1; x <= shipX + sizeOfShip; x++)
                    {
                        if (Field[y, x] == "___")
                            isPossible = false;
                    }
                }
            }
            else if (side == 'V')
            {
                for (int y = shipY - 1; y <= shipY + sizeOfShip; y++)
                {
                    for (int x = shipX - 1; x <= shipX + 1; x++)
                    {
                        if (Field[y, x] == "___")
                            isPossible = false;
                    }
                }
            }
            return isPossible;
        }

        static void DrawTitle()
        {
            Console.Clear();
            Console.WriteLine("~~~~~~ BATTLESHIP COORDINATES ~~~~~~");
        }
    }
}
