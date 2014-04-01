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
            if (_processes == null)
            {
                _processes = new ObservableCollection<Process> ();
            }
        }

        public void AddProcess(Process process)
        {
            
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
                        //Add the process to the list
                        ScheduledList.Add(_processes[i]);

                        //Add it's duration to the current time
                        _currentTime += _processes[i].Duration;

                        //Mark it as executed
                        _processes[i].Executed = true;

                        //Go back to the beginning element (since the list is ordered by duration times
                        i = 0;
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
                        ScheduledList.Add(_processes[i]);
                        _currentTime += _processes[i].Duration;
                        _processes[i].Executed = true;
                        i = 0;
                    }
                }
                _currentTime++;
            }

            return ScheduledList;
        }


        public ObservableCollection<Process> ReorderByRoundRobin(int quantum)
        {
            _processes = new ObservableCollection<Process>(_processes.OrderBy(o => o.StartTime));
            Reset();


            for (int i = 0; i < _processes.Count; i++)
            {
                if (_processes[i].StartTime <= _currentTime && _processes[i].Duration != 0)
                {
                    ScheduledList.Add(_processes[i]);
                    if (_processes[i].Duration < quantum)
                    {
                        _currentTime += _processes[i].Duration;
                    }
                    else
                    {
                        _currentTime += quantum;
                    }
                }
            }
            _currentTime++;

            return ScheduledList;
        }

        private void Reorder()
        {
           //_processes =  new ObservableCollection<Process>(_processes.OrderBy(o => o.Priority));
        }

        //I'm not sure if Joe will have to bind to an ObservableCollection property or if he can just bind to a return type of a function
        public ObservableCollection<Process> ScheduledList { get; set; }

        private ObservableCollection<Process> _processes;
        private int _currentTime;
    }
}
