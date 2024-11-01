namespace PathFindAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Board board = new();
            Player player = new();
            board.Init(25, player);
            player.Init(1, 1, board);
            int lastTick = 0;

            const int WAIT_TICK = 1000 / 30;
            while (true)
            {
                #region FrameTick
                int currTick = Environment.TickCount & Int32.MaxValue;
                if (currTick - lastTick < WAIT_TICK)
                    continue;
                int dt = currTick - lastTick;
                lastTick = currTick;
                #endregion

                // 입력

                // 로직
                player.Update(dt);
                // 렌더링
                board.Render();
            }
        }
    }

}