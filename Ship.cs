using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battleship
{
    class Ship
    {
        public int StartX;
        public int StartY;
        public int EndX;
        public int EndY;

        private int size = 1;

        public Ship(int startY, int startX, int endY, int endX)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;

            size += EndX - StartX + EndY - StartY;
        }

        public bool CheckShip(char[,] field, char sign)
        {
            bool isKilled = false;
            int sizeWounded = 0;
            for (int i = StartY; i <= EndY; i++)
            {
                for (int j = StartX; j <= EndX; j++)
                {
                    if (field[i, j] == sign)
                    {
                        sizeWounded++;
                    }
                }
            }
            if (sizeWounded >= size)
                isKilled = true;
            return isKilled;
        }

        public void Kill(char[,] field)
        {
            for (int i = StartY; i <= EndY; i++)
            {
                for (int j = StartX; j <= EndX; j++)
                {
                    field[i, j] = 'K';
                }
            }
            BorderForKilled(field);
        }

        public void BorderForKilled(char[,] field)
        {
            for (int i = StartY - 1; i <= EndY + 1; i++)
            {
                for (int j = StartX - 1; j <= EndX + 1; j++)
                {
                    if (field[i, j] != 'K' && i > 0 && i < field.GetLength(0) - 1 && j < field.GetLength(1) - 1 && j > 0)
                    {
                        field[i, j] = 'P';
                    }
                }
            }
        }

        public bool CheckBorders(char[,] field)
        {
            bool isPossible = true;

            for(int i = StartY - 1; i <= EndY + 1; i++)
            {
                for(int j = StartX - 1; j <= EndX + 1; j++)
                {
                    if (field[i, j] == '_')
                        isPossible = false;
                }
            }

            return isPossible;
        }

    }
}
