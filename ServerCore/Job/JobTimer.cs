namespace ServerCore.Job
{
    class ReservedJob
    {
        public JobQueue JobQueue { get; set; } = null;
        public Action Job { get; set; } = null;
        public int ExecuteTick { get; set; } = 0;
    }

    public class JobTimer
    {
        public static JobTimer Instance { get; } = new JobTimer();

        object _lock = new object();
        PriorityQueue<ReservedJob, int> _reservedJobs = new PriorityQueue<ReservedJob, int>();

        private JobTimer()
        { }

        public void ReserveJob(JobQueue jobQueue, Action job, int reserveTick)
        {
            ReservedJob reservedJob = new ReservedJob();
            reservedJob.JobQueue = jobQueue;
            reservedJob.Job = job;
            reservedJob.ExecuteTick = System.Environment.TickCount + reserveTick;

            lock (_lock)
            {
                _reservedJobs.Enqueue(reservedJob, reservedJob.ExecuteTick);
            }
        }

        public void DistributeJobs()
        {
            while (true)
            {
                int currentTick = System.Environment.TickCount;
                ReservedJob reservedJob = null;

                lock (_lock)
                {
                    if (_reservedJobs.Count == 0)
                        break;

                    // 아직 실행해야할 Tick이 아니면 종료
                    if (_reservedJobs.Peek().ExecuteTick > currentTick)
                        break;

                    reservedJob = _reservedJobs.Dequeue();
                }

                // 예약된 작업을 해당하는 JobQueue에 Push
                reservedJob.JobQueue.PushJob(reservedJob.Job, true);
            }
        }
    }
}
