using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PathFindAlgorithm
{
    struct Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }


    public sealed class Player
    {
        static int[,] DIR = new int[8, 2]
        {
            { -1, -1},  {-1, 0 },   {-1, 1},
            { 0, -1 },              { 0, 1},
            { 1, -1},   {1, 0 },    {1, 1 }
        };

        static int[] MOVE_COST = new int[8] { 14, 10, 14, 10, 10, 14, 10, 14};

        public int PosY { get; private set; }
        public int PosX { get; private set; }
        Random _rand = new();
        Board? _board;
        List<Pos> _points = new();
        enum EDir
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        }
        int _dir = (int)EDir.Up;

        public void Init(int posY, int posX, Board board)
        {
            PosX = posX;
            PosY = posY;
            _board = board;

            AStar();
        }

        const int MOVE_TICK = 100;
        int _sumTick = 0;
        int _lastIdx = 0;
        public void Update(int dt)
        {
            if (_lastIdx >= _points.Count)
            {
                Debug.Assert(_board != null);
                _lastIdx = 0;
                _points.Clear();
                _board.Init(_board.Size, this);
                Init(1, 1, _board);
                return;
            }

            Debug.Assert(_board != null && _board.Tile != null);
            _sumTick += dt;
            if (_sumTick >= MOVE_TICK)
            {
                _sumTick = 0;
                PosY = _points[_lastIdx].Y;
                PosX = _points[_lastIdx].X;
                ++_lastIdx;
            }
        }

        struct PQNode : IComparable<PQNode>
        {
            public int F;
            public int G;
            public int Y;
            public int X;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
            }
        }

        void AStar()
        {
            // 점수 매기기
            // F = G + H
            // F = 최종 점수(작을수록 좋음, 경로에 따라 달라짐)
            // G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 (작을수록 좋음, 경로에 따라 달라짐)
            // H = 목적지에서 얼마나 가까운지 (작을수록 좋음, 고정)

            Debug.Assert(_board != null && _board.Tile != null);
            Pos[,] parent = new Pos[_board.Size, _board.Size];

            // (y,x) 이미 방문했는지 여부 (방문 = closed) 리스트로 구현하면 메모리 아끼겠쥐?
            bool[,] closed = new bool[_board.Size, _board.Size];

            // (y,x) 가는 길을 한 번이라도 발견 했는지
            // 발견 못했다면 MaxValue
            // 발견 했다면 => F = G + H
            int[,] open = new int[_board.Size, _board.Size];
            for (int i = 0; i < _board.Size; ++i)
                for (int j = 0; j < _board.Size; ++j)
                    open[i, j] = Int32.MaxValue;


            // open에 있는 정보들 중에서 가장 좋은 후보를 빠르게 뽑아오기 위한 도구.
            PriorityQueue<PQNode> pq = new();

            // 시작점 발견 (예약 진행)
            open[PosY, PosX] = GetHuristicCost(PosY, PosX);
            pq.Push(new PQNode() 
            { 
                F = GetHuristicCost(PosY, PosX), 
                G = 0, 
                Y = PosY, 
                X = PosX 
            });
            parent[PosY, PosX] = new Pos(PosY, PosX);

            while (pq.Count > 0)
            {
                // 제일 좋은 후보를 찾는다.
                var curr = pq.Pop();

                // 동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
                if (closed[curr.Y, curr.X])
                    continue;

                // 방문한다.
                closed[curr.Y, curr.X] = true;
                // 목적지 도착했으면 바로 종료
                if (curr.Y == _board.DestY && curr.X == _board.DestX)
                    break;

                // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
                for (int i = 0; i < 8; ++i)
                {
                    int ny = curr.Y + DIR[i, 0];
                    int nx = curr.X + DIR[i, 1];

                    if (ny < 0 || ny >= _board.Size || nx < 0 || nx >= _board.Size)
                        continue;
                    if (_board.Tile[ny, nx] == ETileType.Wall || closed[ny, nx])
                        continue;

                    // 비용계산 시작.
                    int g = curr.G + MOVE_COST[i];
                    int h = GetHuristicCost(ny, nx);

                    // 다른 경로에서 더 빠른 길을 이미 찾았다면 스킵.
                    if (open[ny, nx] < g + h)
                        continue;

                    // 위의 모든 과정을 통과했다면 드디어 예약!
                    open[ny, nx] = g + h;
                    pq.Push(new PQNode() { F = g + h, G = g, Y = ny, X = nx});
                    parent[ny, nx] = new Pos(curr.Y, curr.X);
                }
            }
            CalcPathFromParent(parent);
        }

        void CalcPathFromParent(Pos[,] parent)
        {
            Debug.Assert(_board != null);
            int y = _board.DestY;
            int x = _board.DestX;
            while (parent[y, x].Y != y || parent[y, x].X != x)
            {
                _points.Add(new Pos(y, x));
                Pos pos = parent[y, x];
                y = pos.Y;
                x = pos.X;
            }
            _points.Add(new Pos(y, x));
            _points.Reverse();
        }
        int GetHuristicCost(int y, int x)
        {
            Debug.Assert(_board != null);
            return 10 * (Math.Abs(_board.DestY - y) + Math.Abs(_board.DestX - x));
        }
        void BFS()
        {
            int[,] dir = new int[4, 2]
            {
                { -1, 0 },
                { 1, 0 },
                { 0, 1 },
                { 0, -1 } 
            };
            Debug.Assert(_board != null);
            bool[,] visited = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];
            parent[PosY, PosX] = new Pos(PosY, PosX);

            Queue<Pos> q = new();
            q.Enqueue(new Pos(PosY, PosX));
            visited[PosY, PosX] = true;
            Debug.Assert(_board != null && _board.Tile != null);
            while (q.Count > 0)
            {
                Pos curr = q.Dequeue();
                int cy = curr.Y;
                int cx = curr.X;
                for (int i = 0; i < 4; ++i)
                {
                    int ny = cy + dir[i, 0];
                    int nx = cx + dir[i, 1];
                    if (nx < 0 || nx >= _board.Size || ny < 0 || ny >= _board.Size)
                        continue;
                    if (_board.Tile[ny, nx] == ETileType.Wall)
                        continue;
                    if (visited[ny, nx])
                        continue;
                    q.Enqueue(new Pos(ny, nx));
                    visited[ny, nx] = true;
                    parent[ny, nx] = new Pos(cy, cx);
                }
            }
            CalcPathFromParent(parent);
        }

        void RightHand()
        {
            // 현재 바라보고 있는 방향을 기준으로, 좌표변화를 나타낸다.
            int[] frontY = new int[] { -1, 0, 1, 0 };
            int[] frontX = new int[] { 0, -1, 0, 1 };
            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };
            _points.Add(new Pos(PosY, PosX));
            Debug.Assert(_board != null && _board.Tile != null);
            while (PosY != _board.DestY || PosX != _board.DestX)
            {
                // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는지 확인.
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == ETileType.Empty)
                {
                    // 오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;
                    // 앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == ETileType.Empty)
                {
                    // 앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {
                    // 왼쪽방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }

    }

    
}
