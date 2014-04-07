using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    public class Process
    {
        public Process(uint pId, uint startTime, uint duration, uint priority)
        {
            PId = pId;
            StartTime = startTime;
            Duration = duration;
            Priority = priority;
        }

        public Process(Process process)
        {
            StartTime = process.StartTime;
            Duration = process.Duration;
            PId = process.PId;
            Priority = process.Priority;
            Executed = process.Executed;
        }

        public uint StartTime { get; set; }
        public uint Duration { get; set; }
        public uint PId { get; set; }
        public uint Priority { get; set; }
        public bool Executed { get; set; }

        public override string ToString()
        {
            return String.Format("Process ID: {0}\n  StartTime: {1}\n  Duration: {2}\n  Priority: {3}\n  Executed: {4}", PId, StartTime, Duration, Priority, Executed);
        }
    }



}
