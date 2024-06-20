namespace ServerCore.Job
{
    public class JobQueue
    {
        object _lock = new object();
        Queue<Action> _jobs = new Queue<Action>();
        bool _executing = false;

        public void PushJob(Action job, bool pushOnly = false)
        {
            bool execute = false;

            lock (_lock)
            {
                _jobs.Enqueue(job);

                // Job을 처리중인 Thread가 없으면 현재 Thread가 처리
                if (pushOnly == false && _executing == false)
                {
                    execute = true;
                }
            }

            if (execute)
            {
                Execute();
            }
        }

        public void Execute()
        {
            while (true)
            {
                Action job = null;

                lock (_lock)
                {
                    // 남은 Job이 없으면 종료
                    if (_jobs.Count == 0)
                    {
                        _executing = false;
                        break;
                    }

                    job = _jobs.Dequeue();
                }

                // lock을 풀고! 실행
                job();
            }
        }
    }
}
