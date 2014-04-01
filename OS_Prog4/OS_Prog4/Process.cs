using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    class Process
    {
        public Process(int pId, int startTime, int duration, int priority)
        {
            PId = pId;
            StartTime = startTime;
            Duration = duration;
            Priority = priority;
        }

        public int StartTime { get; set; }
        public int Duration { get; set; }
        public int PId { get; set; }
        public int Priority { get; set; }
        public bool Executed { get; set; }

        public override string ToString()
        {
            return String.Format("Process ID: {0}\n  StartTime: {1}\n  Duration: {2}\n  Priority: {3}\n  Executed: {4}", PId, StartTime, Duration, Priority, Executed);
        }
    }



}
