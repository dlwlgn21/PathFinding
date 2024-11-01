using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Excercise
{
    class Graph
    {
        int[,] adj = new int[6, 6] 
        { 
            { -1, 15, -1, 35, -1, -1 },
            { 15, -1, 5, 10, -1, -1 },
            { -1, 5, -1, -1, -1, -1 },
            { 35, 10, -1, -1, 5, -1 },
            { -1, -1, -1, 5, -1, 5 },
            { -1, -1, -1, -1, 5, -1 } 
        };

        bool[] visited = new bool[6];

        public void Dijikstra(int start)
        {
            visited = new bool[6];
            int[] dist = new int[6];
            int[] parent = new int[6];
            Array.Fill(dist, Int32.MaxValue);
            dist[start] = 0;
            parent[start] = start;
            //PriorityQueue

            while (true)
            {
                // (가장 가까이에 있는) 유력한 후보의 거리와 번호를 저장한다.
                int closest = Int32.MaxValue;
                int curr = -1;
                for (int i = 0; i < 6; ++i)
                {
                    // 이미 방문한 정점은 스킵
                    if (visited[i])
                        continue;
                    // 아직 발견(예약)된 적이 없거나, 기존 후보보다 멀리 있으면 스킵
                    if (dist[i] == Int32.MaxValue || dist[i] >= closest)
                        continue;

                    // 여태껏 발견한 가장 좋은 후보라는 의미니까 정보를 갱신
                    closest = dist[i];
                    curr = i;
                }
                // 다음 후보가 하나도 없다 -> 종료
                if (curr == -1)
                    break;

                // 제일 좋은 후보를 찾았으니까 방문한다.
                visited[curr] = true;

                // 방문한 정점과 인접한 정점들을 조사해서,
                // 상황에 따라 발견한 최단 거리를 갱신한다.
                for (int next = 0; next < 6; ++next)
                {
                    // 연결되지 않은 정점 스킵
                    if (adj[curr, next] == -1)
                        continue;
                    // 이미 방문한 정점은 스킵
                    if (visited[next])
                        continue;
                    // 새로 조사된 정점의 최단거리를 계산한다.
                    int nextDist = dist[curr] + adj[curr, next];
                    // 만약에 기존에 발견한 최단거리가, 새로 조사된 최단거리보다 크면, 정보를 갱신한다.
                    if (nextDist < dist[next])
                    {
                        dist[next] = nextDist;
                        parent[next] = curr;
                    }
                }
            }
        }

        public void BFS(int start)
        {
            Queue<int> q = new();
            int[] parent = new int[6];
            int[] dist = new int[6];
            q.Enqueue(start);
            visited[start] = true;
            parent[start] = start;
            dist[start] = 0;
            while (q.Count > 0)
            {
                int curr = q.Dequeue();
                Console.WriteLine(curr);
                for (int next = 0; next < 6; ++next)
                {
                    if (adj[curr, next] != 0 && !visited[next])
                    {
                        q.Enqueue(next);
                        visited[next] = true;
                        parent[next] = curr;
                        dist[next] = dist[curr] + 1;
                    }
                }
            }
        }
    }

    class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new();
    }
    

    class PriorityQueue
    {
        List<int> _heap = new(128);

        public void Push(int data)
        {
            _heap.Add(data);
            int currIdx = GetLastIdx();
            while (currIdx > 0)
            {
                int parentIdx = (currIdx - 1) / 2;
                if (_heap[parentIdx] > _heap[currIdx])
                    break;
                int tmp = _heap[currIdx];
                _heap[currIdx] = _heap[parentIdx];
                _heap[parentIdx] = tmp;
                currIdx = parentIdx;
            }
        }
        public int Pop()
        {
            Debug.Assert(_heap.Count >= 1);
            int ret = _heap[0];
            int lastIdx = GetLastIdx();
            _heap[0] = _heap[lastIdx];
            _heap.RemoveAt(lastIdx);
            --lastIdx;

            int currIdx = 0;
            while (true)
            {
                int l = currIdx * 2 + 1;
                int r = currIdx * 2 + 2;
                int nextIdx = currIdx;
                if (l <= lastIdx && _heap[l] > _heap[nextIdx])
                    nextIdx = l;
                if (r <= lastIdx && _heap[r] > _heap[nextIdx])
                    nextIdx = r;

                if (nextIdx == currIdx)
                    break;

                int tmp = _heap[currIdx];
                _heap[currIdx] = _heap[nextIdx];
                _heap[nextIdx] = tmp;
                currIdx = nextIdx;
            }

            return ret;
        }
        public int Count()
        {
            return _heap.Count;
        }

        int GetLastIdx()
        {
            return _heap.Count - 1;
        }
    }

    class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new(128);

        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);
            int curr = GetLastIdx();
            // 도장꺠기 시작
            while (curr > 0)
            {
                // 도장깨기 시도
                int parent = (curr - 1) / 2;
                if (_heap[curr].CompareTo(_heap[parent]) < 0)
                    break;

                // 자식이 부모보다 크거나 같을경우 두 값을 교체한다.
                T tmp = _heap[curr];
                _heap[curr] = _heap[parent];
                _heap[parent] = tmp;

                // 검사 위치를 이동.
                curr = parent;
            }
        }
        public T Pop()
        {
            Debug.Assert(_heap.Count >= 1);
            // 반환할 데이터를 따로 저장.
            T ret = _heap[0];
            // 마지막 데이터를 루트로 이동.
            int lastIdx = GetLastIdx();
            _heap[0] = _heap[lastIdx];
            _heap.RemoveAt(lastIdx);
            --lastIdx;

            // 역으로 내려가는 도장깨기 시작
            int currIdx = 0;
            while (true)
            {
                int l = 2 * currIdx + 1;
                int r = 2 * currIdx + 2;
                int nextIdx = currIdx;
                // 왼쪽값이 현재 값보다 크면 왼쪽으로 이동
                if (l <= lastIdx && _heap[nextIdx].CompareTo(_heap[l]) < 0)
                    nextIdx = l;
                // 오른쪽 값이 현재값(왼쪽이동 포함)보다 크면 오른쪽으로 이동
                if (r <= lastIdx && _heap[nextIdx].CompareTo(_heap[r]) < 0)
                    nextIdx = r;

                // 왼쪽/오른쪽 모두 현재 값보다 작으면 종료.
                if (nextIdx == currIdx)
                    break;

                // 두 값을 교체한다.
                T tmp = _heap[currIdx];
                _heap[currIdx] = _heap[nextIdx];
                _heap[nextIdx] = tmp;

                // 검사 위치를 이동한다.
                currIdx = nextIdx;
            }

            return ret;
        }
        public int Count()
        {
            return _heap.Count;
        }

        int GetLastIdx()
        {
            return _heap.Count - 1;
        }
    }

    class Knight : IComparable<Knight>
    {
        public int Id { get; set; }

        public int CompareTo(Knight? other)
        {
            Debug.Assert(other != null);
            if (Id == other.Id)
                return 0;
            return Id > other.Id ? 1 : -1;
        }
    }


    class Program
    {
        static TreeNode<string> MakeTree()
        {
            TreeNode<string> root = new TreeNode<string>() { Data = "R1 개발실" };
            {
                TreeNode<string> node = new() { Data = "디자인팀" };
                node.Children.Add(new() { Data = "전투" });
                node.Children.Add(new() { Data = "경제" });
                node.Children.Add(new() { Data = "스토리" });
                root.Children.Add(node);

                TreeNode<string> node1 = new() { Data = "프로그래밍팀" };
                node1.Children.Add(new() { Data = "서버" });
                node1.Children.Add(new() { Data = "클라" });
                node1.Children.Add(new() { Data = "엔진" });
                root.Children.Add(node1);

                TreeNode<string> node2 = new() { Data = "아트팀" };
                node2.Children.Add(new() { Data = "배경" });
                node2.Children.Add(new() { Data = "캐릭터" });
                root.Children.Add(node2);
            }
            return root;
        }

        static void PrintTree(TreeNode<string> curr)
        {
            Console.WriteLine(curr.Data);
            foreach (var child in curr.Children)
                PrintTree(child);
        }

        static int GetHeight(TreeNode<string> curr)
        {
            int height = 0;
            foreach (var child in curr.Children)
            {
                height = Math.Max(height, GetHeight(child) + 1);
            }
            return height;
        }
        static void Main(string[] args)
        {
            //Graph g = new();
            //g.Dijikstra(0);

            //var root = MakeTree();
            //PrintTree(root);
            //Console.WriteLine(GetHeight(root));

            PriorityQueue<int> q = new();
            q.Push(20);
            q.Push(10);
            q.Push(30);
            q.Push(90);
            q.Push(40);
            q.Push(150);
            q.Push(10);
            q.Push(13);
            while (q.Count() > 0)
            {
                Console.WriteLine(q.Pop());
            }

            //PriorityQueue<Knight> q = new();
            //q.Push(new Knight(){Id = 20});
            //q.Push(new Knight(){Id = 30});
            //q.Push(new Knight(){Id = 50});
            //q.Push(new Knight(){Id = 90});
            //q.Push(new Knight(){Id = 10});


        }
    }
}