using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    public enum Scheduler { RoundRobin, Priority, ShortestJobFirst };
    class ProcessScheduler
    {
        public ProcessScheduler(Scheduler scheduler, ObservableCollection<Process> processes)
        {
            _processes = processes;
            _scheduleType = scheduler;

            if (_processes == null)
            {
                _processes = new ObservableCollection<Process> ();
            }
        }

        public void AddProcess(Process process)
        {
            _processes.Add(process);
            Reorder();
        }

        private void Reset()
        {
            ScheduledList = new ObservableCollection<Process>();
            _currentTime = 0;

            //Reset the executed state
            foreach (Process p in _processes)
            {
                p.Executed = false;
            }
        }

        public ObservableCollection<Process> ReorderByShortestJobFirst()
        {
            _processes = new ObservableCollection<Process>(_processes.OrderBy(o => o.Duration));
            Reset();

            while (_processes.Count != ScheduledList.Count)
            {
                for (int i = 0; i < _processes.Count; i++)
                {
                    //If the starttime was before the current time and the process hasn't executed yet
                    if (_processes[i].StartTime <= _currentTime && !_processes[i].Executed)
                    {
                        Process queuedProcess = new Process(_processes[i]);

                        queuedProcess.StartTime = _currentTime;

                        //Add it's duration to the current time
                        _currentTime += _processes[i].Duration;

                        //Mark it as executed
                        _processes[i].Executed = true;

                        //Go back to the beginning element (since the list is ordered by duration times
                        i = -1;

                        //Add the process to the list
                        ScheduledList.Add(queuedProcess);
                    }
                }

                //Assuming the entire list can not start, increment the current time
                _currentTime++;
            }

            return ScheduledList;
        }

        public ObservableCollection<Process> ReorderByPriority()
        {
            _processes = new ObservableCollection<Process>(_processes.OrderBy(o => o.Priority));
            Reset();

            while (_processes.Count != ScheduledList.Count)
            {
                for (int i = 0; i < _processes.Count; i++)
                {
                    if (_processes[i].StartTime <= _currentTime && !_processes[i].Executed)
                    {
                        Process queuedProcess = new Process(_processes[i]);
                        queuedProcess.StartTime = _currentTime;
                        _currentTime += _processes[i].Duration;
                        _processes[i].Executed = true;
                        i = -1;
                        ScheduledList.Add(queuedProcess);
                    }
                }
                _currentTime++;
            }

            return ScheduledList;
        }


        public ObservableCollection<Process> ReorderByRoundRobin(uint quantum)
        {
            if (quantum == 0)
            {
                throw new ArgumentException("Quantum can not be zero!");
            }

            _processes = new ObservableCollection<Process>(_processes.OrderBy(o => o.StartTime));
            Reset();

            uint totalDuration = 0;
            for (int i = 0; i < _processes.Count; i++)
            {
                totalDuration += _processes[i].Duration;
            }

            //Since the durations will be modified directly, I'm creating a temporary array
            ObservableCollection<Process> tempProcesses = _processes;

            uint currentDuration = 0;
            uint prevDuration = 0;
            while (currentDuration != totalDuration)
            {
                //The list is in order of startTimes
                //Running through the list once will be one rotation 
                for (int i = 0; i < tempProcesses.Count; i++)
                {

                    if (tempProcesses[i].StartTime <= _currentTime && tempProcesses[i].Duration != 0)
                    {
                        Process queuedProcess = new Process(tempProcesses[i]);
                        queuedProcess.StartTime = _currentTime;

                        if (tempProcesses[i].Duration < quantum)
                        {
                            _currentTime += tempProcesses[i].Duration;
                            currentDuration += tempProcesses[i].Duration;
                            queuedProcess.Duration = tempProcesses[i].Duration;
                            tempProcesses[i].Duration = 0;
                        }
                        else
                        {
                            _currentTime += quantum;
                            currentDuration += quantum;
                            queuedProcess.Duration = quantum;
                            tempProcesses[i].Duration -= quantum;
                        }

                        ScheduledList.Add(queuedProcess);
                    }
                }

                //If we went through the for loop without adding anything to the queue,
                //There's an empty timeslot where no processes can execute
                if (prevDuration == currentDuration)
                {
                    _currentTime++;
                }
               // Console.WriteLine("Current Duration: {0}   Previous Duration: {1}    CurrentTime: {2}", currentDuration, prevDuration, _currentTime);
                prevDuration = currentDuration;
            }

            return ScheduledList;
        }

        public void Reorder()
        {
            switch (_scheduleType)
            {
                case Scheduler.Priority:
                    ReorderByPriority();
                    break;
                case Scheduler.RoundRobin:
                    ReorderByRoundRobin(Quantum);
                    break;
                case Scheduler.ShortestJobFirst:
                    ReorderByShortestJobFirst();
                    break;
            }
        }

        //I'm not sure if Joe will have to bind to an ObservableCollection property or if he can just bind to a return type of a function
        public ObservableCollection<Process> ScheduledList { get; set; }

        public uint Quantum { get; set; }

        private Scheduler _scheduleType;
        private ObservableCollection<Process> _processes;
        private uint _currentTime;
    }
}
