using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace PathFindAlgorithm
{
    public enum ETileType 
    {
        Empty,
        Wall
    }

    public sealed class Board
    {
        public ETileType[,]? Tile { get; private set; }
        public int Size { get; private set; }

        public int DestY { get; private set; }
        public int DestX { get; private set; }

        const char CIRCLE = '\u25cf';
        Player? _player;
        public void Init(int size, Player player)
        {
            _player = player;
            if ((size & 0x01) == 0)
            {
                Console.WriteLine("Maze Must Odd!!");
                return;
            }
            Tile = new ETileType[size, size];
            Size = size;
            DestY = Size - 2;
            DestX = Size - 2;
            GenerateMazeBySideWinder();
        }

        public void Render()
        {
            Debug.Assert(Tile != null && _player != null);
            ConsoleColor prevColor = Console.ForegroundColor;
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    // 플레이어 좌표를 가지고 와서. 그 좌표랑 현재 y,x가 일치하면 플레이어 전용색상으로 표시.
                    if (y == _player.PosY && x == _player.PosX)
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (y == DestY && x == DestX)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor =  GetTileColor(Tile[y, x]);
                    Console.Write(CIRCLE);
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = prevColor;
            Console.SetCursorPosition(0, 0);
        }

        ConsoleColor GetTileColor(ETileType eType)
        {
            switch (eType)
            {
                case ETileType.Empty:
                    return ConsoleColor.White;
                case ETileType.Wall:
                    return ConsoleColor.Magenta;
            }
            Debug.Assert(false);
            return ConsoleColor.Black;
        }

        void GenerateMazeByBinaryTree()
        {
            Debug.Assert(Tile != null);
            // 일단 길을 다 막아버리는 작업
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if ((x & 0x01) == 0 || (y & 0x01) == 0)
                        Tile[y, x] = ETileType.Wall;
                    else
                        Tile[y, x] = ETileType.Empty;
                }
            }

            // 랜덤으로 우측 혹은 아래로 뚫는 작업
            // BinaryTree Algorithm
            Random rand = new Random();
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if ((x & 0x01) == 0 || (y & 0x01) == 0)
                        continue;
                    if (y == Size - 2 && x == Size - 2)
                        continue;


                    if (y == Size - 2)
                    {
                        Tile[y, x + 1] = ETileType.Empty;
                        continue;
                    }
                    if (x == Size - 2)
                    {
                        Tile[y + 1, x] = ETileType.Empty;
                        continue;
                    }

                    if (rand.Next(0, 2) == 0)
                    {
                        Tile[y, x + 1] = ETileType.Empty;
                    }
                    else
                    {
                        Tile[y + 1, x] = ETileType.Empty;
                    }
                }
            }
        }

        void GenerateMazeBySideWinder()
        {
            Debug.Assert(Tile != null);
            // 일단 길을 다 막아버리는 작업
            for (int y = 0; y < Size; ++y)
            {
                for (int x = 0; x < Size; ++x)
                {
                    if ((x & 0x01) == 0 || (y & 0x01) == 0)
                        Tile[y, x] = ETileType.Wall;
                    else
                        Tile[y, x] = ETileType.Empty;
                }
            }

            // 랜덤으로 우측 혹은 아래로 뚫는 작업
            Random rand = new Random();
            for (int y = 0; y < Size; ++y)
            {
                int count = 1;
                for (int x = 0; x < Size; ++x)
                {
                    if ((x & 0x01) == 0 || (y & 0x01) == 0)
                        continue;
                    if (y == Size - 2 && x == Size - 2)
                        continue;
                    if (y == Size - 2)
                    {
                        Tile[y, x + 1] = ETileType.Empty;
                        continue;
                    }
                    if (x == Size - 2)
                    {
                        Tile[y + 1, x] = ETileType.Empty;
                        continue;
                    }


                    if (rand.Next(0, 2) == 0)
                    {
                        Tile[y, x + 1] = ETileType.Empty;
                        ++count;
                    }
                    else
                    {
                        int randIdx = rand.Next(0, count);
                        Tile[y + 1, x - randIdx * 2] = ETileType.Empty;
                        count = 1;
                    }
                }
            }
        }
    }
}
