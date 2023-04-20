using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleship
{
    class Game
    {
        public const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public void StartPlay(string path1, string path2)
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            DrawTitle();
            /*
            WriteWithColor("Copy and paste the path to the first file: ", ConsoleColor.DarkMagenta);
            path1 = Console.ReadLine();
            WriteWithColor("Copy and paste the path to the second file: ", ConsoleColor.DarkMagenta);
            path2 = Console.ReadLine();
            */

            List<int[]> coordinates1 = new List<int[]>();
            List<int[]> coordinates2 = new List<int[]>();

            FileToList(path1, coordinates1, out int field1X, out int field1Y);
            FileToList(path2, coordinates2, out int field2X, out int field2Y);

            if (field1X != field2X || field2Y != field1Y)
            {
                WriteWithColor("Fields are not the same size\nReturn to their creation. . .", ConsoleColor.DarkRed);

                Console.ReadKey();
                return;
            }

            char[,] fieldPlayer1 = new char[field1Y + 2, field1X + 2];
            char[,] fieldPlayer2 = new char[field2Y + 2, field2X + 2];

            Ship[] shipsPlayer1 = new Ship[10];
            Ship[] shipsPlayer2 = new Ship[10];
            /*  
             *  S  -  ship is whole
             *  W  -  wounded
             *  K  -  killed
             *  P  -  point
             */
            ShipsInArray(fieldPlayer1, coordinates1, shipsPlayer1);
            ShipsInArray(fieldPlayer2, coordinates2, shipsPlayer2);

            int index = path1.LastIndexOf('_');
            string name1 = "PLAYER ";
            for (int i = 0; i < index; i++)
            {
                name1 += path1[i];
            }
            index = path2.LastIndexOf('_');
            string name2 = "PLAYER ";
            for (int i = 0; i < index; i++)
            {
                name2 += path2[i];
            }

            if (name1 == name2)
            {
                WriteWithColor("Please, change names or numbers. They are the same\n", ConsoleColor.DarkRed);
                return;
            }

            bool isEnd = false;
            while (!isEnd)
            {
                if (PlayGame(fieldPlayer1, field1X, field1Y, shipsPlayer1, name2, ref isEnd))
                {
                    DrawTitle();
                    WriteWithColor($"{name2} IS A WINNER !!!", ConsoleColor.DarkBlue);
                    Console.ReadKey();
                    break;
                }
                DrawFieldWhilePlay(fieldPlayer1);
                Console.ReadKey();
                if (PlayGame(fieldPlayer2, field2X, field2Y, shipsPlayer2, name1, ref isEnd))
                {
                    DrawTitle();
                    WriteWithColor($"\n\n{name1} IS A WINNER !!!", ConsoleColor.Green);
                    Console.ReadKey();
                    break;
                }
                DrawFieldWhilePlay(fieldPlayer2);
                Console.ReadKey();
            }
            Console.WriteLine("\nDelete files? (y/n)");
            string ifDelete = Console.ReadLine();
            ifDelete = ifDelete.ToUpper();
            if(ifDelete == "Y" || ifDelete == "YES")
            {
                File.Delete(path1);
                File.Delete(path2);
                WriteWithColor("FILES HAVE BEEN DELETED", ConsoleColor.DarkGreen);
            }
        }

        static bool PlayGame(char[,] fieldPlayer1, int field1X, int field1Y, Ship[] ships1, string name, ref bool isEnd)
        {
            DrawFieldWhilePlay(fieldPlayer1);
            string cell;
            WriteWithColor($"{name}, choose cell (ex. A1)  -  ", ConsoleColor.DarkMagenta);
            cell = "" + Console.ReadLine();
            cell = cell.ToUpper();
            int cellLetter = 0, cellNum = 0;
            if (cell.Length < 2)
            {
                WriteWithColor("Not a cell!", ConsoleColor.DarkRed);
                Console.ReadKey();
                PlayGame(fieldPlayer1, field1X, field1Y, ships1, name, ref isEnd);
            }
            else if (!CheckCellSizes(cell, field1X, field1Y, out cellLetter, out cellNum))
            {
                WriteWithColor("Cell not in field!", ConsoleColor.DarkRed);
                Console.ReadKey();
                PlayGame(fieldPlayer1, field1X, field1Y, ships1, name, ref isEnd);
            }
            else if (!CheckCellPosition(fieldPlayer1, cellLetter + 1, cellNum))
            {
                WriteWithColor("Wrong position!", ConsoleColor.DarkRed);
                Console.ReadKey();
                PlayGame(fieldPlayer1, field1X, field1Y, ships1, name, ref isEnd);
            }
            if (SetCell(fieldPlayer1, cellLetter, cellNum))
            {
                if (CheckKilled(ships1, fieldPlayer1) >= 10)
                {
                    isEnd = true;
                }
                else if (fieldPlayer1[cellLetter + 1, cellNum] == 'W' || fieldPlayer1[cellLetter + 1, cellNum] == 'K')
                {
                    PlayGame(fieldPlayer1, field1X, field1Y, ships1, name, ref isEnd);
                }
            }
            return isEnd;
        }
        static bool SetCell(char[,] field, int cellLetter, int cellNumber)
        {
            char cell = field[cellLetter + 1, cellNumber];
            if (cellLetter + 1 > 0 && cellLetter + 1 < field.GetLength(0) && cellNumber > 0 && cellNumber < field.GetLength(1) - 1)
            {
                if (cell == 'S')
                {
                    field[cellLetter + 1, cellNumber] = 'W';
                    return true;
                }
                else
                {
                    field[cellLetter + 1, cellNumber] = 'P';
                    return false;
                }
            }
            return false;
        }

        static bool CheckCellSizes(string cell, int xMax, int yMax, out int cellLetter, out int cellNum)
        {
            bool isInField = true;
            cellLetter = 0;
            for (int i = 0; i < yMax; i++)
            {
                if (letters[i] != cell[0])
                    isInField = false;
                else
                {
                    cellLetter = i;
                    isInField = true;
                    break;
                }
            }
            string numInString = "";
            for (int i = 1; i < cell.Length; i++)
            {
                numInString += cell[i];
            }
            if (!Int32.TryParse(numInString, out cellNum) || cellNum < 1 || cellNum > xMax)
                isInField = false;
            return isInField;
        }

        static bool CheckCellPosition(char[,] field, int cellLetter, int cellNumber)
        {
            bool isFree = true;
            char cell = field[cellLetter, cellNumber];
            if (cell == 'W' || cell == 'K' || cell == 'P')
            {
                isFree = false;
            }
            return isFree;
        }

        static void DrawFieldWhilePlay(char[,] field)
        {
            DrawTitle();
            for (int i = 0; i < field.GetLength(0); i++)
            {
                DrawLines('-', 6 * field.GetLength(1) - 1);
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] == 'P')
                    {
                        WriteWithColor("---", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
                    }
                    else if (field[i, j] == 'W')
                    {
                        WriteWithColor("[-]", ConsoleColor.DarkYellow, ConsoleColor.DarkYellow);
                    }
                    else if (field[i, j] == 'K')
                    {
                        WriteWithColor("[-]", ConsoleColor.DarkRed, ConsoleColor.DarkRed);
                    }
                    else if (i == 0 && j > 0 && j < field.GetLength(1) - 1)
                    {
                        if (j < 10)
                            Console.Write($" {j} ");
                        else
                            Console.Write($"{j} ");
                    }
                    else if (j == 0 && i > 0 && i < field.GetLength(0) - 1)
                    {
                        Console.Write($" {letters[i - 1]} ");
                    }
                    else if (i == 0 || i == field.GetLength(0) - 1 || j == 0 || j == field.GetLength(1) - 1)
                    {
                        Console.Write(" # ");
                    }
                    else Console.Write("   ");
                    Console.Write(" | ");
                }
            }
            Console.WriteLine();
        }

        public static void DrawLines(char sign, int times)
        {
            Console.WriteLine();
            for (int i = 0; i < times; i++)
            {
                Console.Write(sign);
            }
            Console.WriteLine();
        }

        static void ShipsInArray(char[,] field, List<int[]> coordinates, Ship[] ships)
        {
            int count = -1;
            foreach (var ship in coordinates)
            {
                Add(ships, new Ship(ship[0], ship[1], ship[2], ship[3]), count);
                count++;
                if (ship[1] == ship[3]) // vertical
                {
                    for (int i = ship[0]; i <= ship[2]; i++)
                    {
                        field[i, ship[1]] = 'S';
                    }
                }
                if (ship[0] == ship[2]) // horizontal
                {
                    for (int i = ship[1]; i <= ship[3]; i++)
                    {
                        field[ship[0], i] = 'S';
                    }
                }
            }
        }

        static int CheckKilled(Ship[] ships, char[,] field)
        {
            int count = 0;
            foreach (var ship in ships)
            {
                if (ship.CheckShip(field, 'W'))
                    ship.Kill(field);
                if (ship.CheckShip(field, 'K'))
                    ++count;
            }
            return count;
        }

        static void Add(Ship[] ships, Ship ship, int yetInArray)
        {
            if (++yetInArray <= ships.Length)
            {
                ships[yetInArray] = ship;
            }
        }

        static void DrawTitle()
        {
            Console.Clear();
            Console.WriteLine("~~~~~~ BATTLESHIP GAME ~~~~~~");
        }

        public static void WriteWithColor(string massage, ConsoleColor colorFG, ConsoleColor colorBG = ConsoleColor.Black)
        {
            Console.BackgroundColor = colorBG;
            Console.ForegroundColor = colorFG;
            Console.Write(massage);
            Console.ResetColor();
        }

        static void FileToList(string path, List<int[]> coordinates, out int x, out int y)
        {
            string lineOfShip;
            string coordOf1Ship;
            x = 0;
            y = 0;
            using (var sr1 = new StreamReader(path))
            {
                while (!sr1.EndOfStream)
                {

                    for (int i = 0; i < 10; i++)
                    {
                        coordinates.Add(new int[4]);
                    }
                    foreach (int[] ship in coordinates)
                    {
                        lineOfShip = sr1.ReadLine();
                        for (int i = 0; i < 4; i++)
                        {
                            coordOf1Ship = "";
                            coordOf1Ship += lineOfShip[i * 2];
                            coordOf1Ship += lineOfShip[i * 2 + 1];
                            coordOf1Ship.Trim();
                            ship[i] = Convert.ToInt32(coordOf1Ship);
                        }
                    }
                    lineOfShip = sr1.ReadLine();
                    coordOf1Ship = "";
                    int indSpace = lineOfShip.IndexOf(' ');
                    for (int i = 0; i < indSpace; i++)
                    {
                        coordOf1Ship += lineOfShip[i];
                    }
                    x = Convert.ToInt32(coordOf1Ship);
                    coordOf1Ship = "";
                    for (int i = indSpace + 1; i < lineOfShip.Length; i++)
                    {
                        coordOf1Ship += lineOfShip[i];
                    }
                    y = Convert.ToInt32(coordOf1Ship);
                }
            }
        }
    }
}

