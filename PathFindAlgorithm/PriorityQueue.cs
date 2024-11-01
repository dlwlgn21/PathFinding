using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFindAlgorithm
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new(128);
        public int Count { get { return _heap.Count; } }
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

        int GetLastIdx()
        {
            return Count - 1;
        }
    }

}
