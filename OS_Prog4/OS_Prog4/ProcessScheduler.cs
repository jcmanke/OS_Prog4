using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OS_Prog4
{
    /**************************************************************************
     * Author: Josh Schultz
     * 
     * Date: May 1, 2014
     * 
     * Class:          ProcessScheduler
     * Inherits From:  (nothing)
     * Imeplements:    (nothing)
     * 
     * Description:  This class is used to monitor processes, and provide three
     *               different methods to schedule those processes.  Each process
     *               contains a PID, start time, duration, and priority.  
     *               Processes are scheduled by shortest job first, priority, and
     *               round robin.  For each of these three scheduling schemes, 
     *               a property exposes the ordering of these lists.
     *               
     * Constructor: public ProcessScheduler(uint quantum)
     * 
     * Methods: public void AddProcess(Process process)
     *          public void ClearProcesses()
     *          public void GenerateProcesses(uint number)
     *          
     *          private void Reset()
     *          private void ReorderByShortestJobFirst()
     *          private void ReorderByPriority()
     *          private void ReorderByRoundRobin(uint quantum)
     *          private void Reorder()
     *          
    **************************************************************************/

    class ProcessScheduler
    {

        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  The constructor simply initializes the public properties
        //
        //Parameters:   quantum - Number to rotate for round robin
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public ProcessScheduler(uint quantum, MainWindow listener)
        {
            Quantum = quantum;
            _listener = listener;

            SJFSchedule = new ObservableCollection<Process>();
            PrioritySchedule = new ObservableCollection<Process>();
            RoundRobinSchedule = new ObservableCollection<Process>();
            Processes = new ObservableCollection<Process>();
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  Given a process, this method simply inserts it into the
        //              list and reorders the schedules
        //
        //Parameters:   process - The process to add to the list
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void AddProcess(Process process)
        {
            Processes.Add(process);

            //Update the three schedulers
            Reorder();
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 1, 2014
        //
        //Description: Removes all processes from the list
        //
        //Parameters:  (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void ClearProcesses()
        {
            Processes.Clear();
            SJFSchedule.Clear();
            PrioritySchedule.Clear();
            RoundRobinSchedule.Clear();
        }

        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 1, 2014
        //
        //Description:  Generates random processes with a random priority,
        //              start time and duration.
        //
        //Parameters:   number - Number of processes to generate
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void GenerateProcesses(uint number)
        {
            //Remove the existing processes before adding more
            Processes.Clear();

            for (uint i = 0; i < number; i++)
            {
                //Avoid creating the same random number
                System.Threading.Thread.Sleep(20);

                Random rand = new Random();

                //Randomness
                uint priority = (uint)rand.Next(0,5);
                uint startTime = (uint)rand.Next(0,20);
                uint duration = (uint)rand.Next(1,6);

                //Add the process to the list
                Process process = new Process(i+1,startTime,duration,priority);
                Processes.Add(process);
            }

            //Update the schedules for the three schedulers
            Reorder();
        }

        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  Before updating the schedulers, reset the time, and
        //              verify the processes have not executed.
        //
        //Parameters:  (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void Reset()
        {
            _currentTime = 0;

            //Reset the executed state
            foreach (Process p in Processes)
            {
                p.Executed = false;
            }
        }

        //*******************************************************************//
        //Author: Josh Schultz, Joe Manke
        //
        //Date: April 14, 2014
        //
        //Description:  Using the processes from this class, this method
        //              orders the SJFSchedule property based on the shortest
        //              job first scheduling scheme.
        //
        //Parameters:  (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void ReorderByShortestJobFirst()
        {            
            //Reset the currentTime, and verify all the processes have not executed
            Reset();

            //Clear the existing schedule
            SJFSchedule.Clear();

            //Order the processes by duration
            ObservableCollection<Process> orderedProcesses = new ObservableCollection<Process>(Processes.OrderBy(o => o.Duration).ToList());

            //Continue to cycle through until all of the processes are scheduled
            while (orderedProcesses.Count != SJFSchedule.Count)
            {
                for (int i = 0; i < orderedProcesses.Count; i++)
                {
                    //If the starttime was before the current time and the process hasn't executed yet
                    if (orderedProcesses[i].StartTime <= _currentTime && !orderedProcesses[i].Executed)
                    {
                        Process queuedProcess = new Process(orderedProcesses[i]);

                        //Set the starttime to the current time
                        queuedProcess.StartTime = _currentTime;

                        //Add it's duration to the current time
                        _currentTime += queuedProcess.Duration;

                        //Mark it as executed
                        orderedProcesses[i].Executed = true;

                        //Go back to the beginning element (since the list is ordered by duration times)
                        i = -1;

                        //Add the process to the list
                        SJFSchedule.Add(queuedProcess);
                    }
                }

                //Assuming the entire list can not start, increment the current time
                _currentTime++;
            }

            //draw gantt chart in MainWindow
            long totalDuration = SJFSchedule.Sum(o => o.Duration);
            _listener.SJFPanel.Children.Clear();

            foreach (Process process in SJFSchedule)
            {
                Label l = new Label();
                l.Content = process.PId;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.Width =  (double) process.Duration / totalDuration * _listener.SJFPanel.Width;
                l.BorderThickness = new System.Windows.Thickness(1);
                l.BorderBrush = System.Windows.Media.Brushes.Black;
                _listener.SJFPanel.Children.Add(l);
            }
        }


        //*******************************************************************//
        //Author: Josh Schultz, Joe Manke
        //
        //Date: April 14, 2014
        //
        //Description:  Using the Processes property, this method reorders
        //              the prioritySchedule based on the priority scheduling
        //              scheme.
        //
        //Parameters:  (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void ReorderByPriority()
        {
            //Reset the currentTime, and verify all the processes have not executed
            Reset();

            //Clear the existing schedule
            PrioritySchedule.Clear();

            //Reorder the processes by priority
            ObservableCollection<Process> orderedProcesses = new ObservableCollection<Process>(Processes.OrderBy(o => o.Priority).ToList());

            //Continue to schedule processes until all have been scheduled
            while (orderedProcesses.Count != PrioritySchedule.Count)
            {
                for (int i = 0; i < orderedProcesses.Count; i++)
                {
                    //If the current process is before the start time and yet executed
                    if (orderedProcesses[i].StartTime <= _currentTime && !orderedProcesses[i].Executed)
                    {
                        Process queuedProcess = new Process(orderedProcesses[i]);

                        //Set the start time of the process
                        queuedProcess.StartTime = _currentTime;

                        //Add the process's duration to the current time
                        _currentTime += orderedProcesses[i].Duration;

                        //Set the process as executed
                        orderedProcesses[i].Executed = true;

                        //Start at the top of the priority list
                        i = -1;

                        //Add the process to the scheduled list
                        PrioritySchedule.Add(queuedProcess);
                    }
                }

                //If no items were able to execute, increment the time
                _currentTime++;
            }

            //draw gantt chart in MainWindow
            long totalDuration = PrioritySchedule.Sum(o => o.Duration);
            _listener.PriorityPanel.Children.Clear();

            foreach (Process process in PrioritySchedule)
            {
                Label l = new Label();
                l.Content = process.PId;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.Width = (double)process.Duration / totalDuration * _listener.PriorityPanel.Width;
                l.BorderThickness = new System.Windows.Thickness(1);
                l.BorderBrush = System.Windows.Media.Brushes.Black;
                _listener.PriorityPanel.Children.Add(l);
            }
        }

        //*******************************************************************//
        //Author: Josh Schultz, Joe Manke
        //
        //Date: April 14, 2014
        //
        //Description:  Using the Processes property, this method orders the
        //              RoundRobinSchedule property to follow a round robin
        //              scheduling scheme.  The quantum is determined by the
        //              only parameter.
        //
        //Parameters:  quantum - time chunks for round robin
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void ReorderByRoundRobin(uint quantum)
        {
            //Verify the quantum value is valid
            if (quantum == 0)
            {
                quantum = 1;
                Console.WriteLine("Defaulting to 1 quantum value");
            }

            //Reset the currentTime, and verify all the processes have not executed
            Reset();

            //Clear the existing schedule
            RoundRobinSchedule.Clear();

            //Order the processes by start time
            ObservableCollection<Process> ordered = new ObservableCollection<Process>(Processes.OrderBy(o => o.StartTime).ToList());

            //The bindings to the GUI are still there for each process, so each one must be remade
            ObservableCollection<Process> orderedProcesses = new ObservableCollection<Process>();
            foreach (Process p in ordered)
            {
                Process copy = new Process(p);
                orderedProcesses.Add(copy);
            }

            //Count the total duration of the processes
            uint totalDuration = 0;
            for (int i = 0; i < orderedProcesses.Count; i++)
            {
                totalDuration += orderedProcesses[i].Duration;
            }

            //Total duration currently scheduled 
            uint currentDuration = 0;

            //Last iteration through the process list's total duration
            //This is used to determine if the time should be incremented after
            //The entire list is executed
            uint prevDuration = 0;

            //Continue to order the processes until the entire duration is used
            while (currentDuration != totalDuration)
            {
                //The list is in order of startTimes
                //Running through the list once will be one rotation 
                for (int i = 0; i < orderedProcesses.Count; i++)
                {
                    //If the process's start time is before the current time and there is time
                    //remaining to execute
                    if (orderedProcesses[i].StartTime <= _currentTime && orderedProcesses[i].Duration != 0)
                    {
                        Process queuedProcess = new Process(orderedProcesses[i]);

                        //Set the start time to the current time
                        queuedProcess.StartTime = _currentTime;

                        if (orderedProcesses[i].Duration < quantum)
                        {
                            //Add the duration to the current time/duration
                            _currentTime += orderedProcesses[i].Duration;
                            currentDuration += orderedProcesses[i].Duration;

                            //Set the list of processes duration to zero
                            orderedProcesses[i].Duration = 0;
                        }
                        else
                        {
                            //Add the quantum time to the current time/duration
                            _currentTime += quantum;
                            currentDuration += quantum;

                            //Set the queued processes duration 
                            queuedProcess.Duration = quantum;

                            //Subtract the quantum from the list of processes
                            orderedProcesses[i].Duration -= quantum;
                        }

                        //Add the process to the scheduled list
                        RoundRobinSchedule.Add(queuedProcess);
                    }
                }

                //If we went through the for loop without adding anything to the queue,
                //There's an empty timeslot where no processes can execute
                if (prevDuration == currentDuration)
                {
                    _currentTime++;
                }

                prevDuration = currentDuration;
            }

            //draw gantt chart in MainWindow
            _listener.RRPanel.Children.Clear();

            foreach (Process process in RoundRobinSchedule)
            {
                Label l = new Label();
                l.Content = process.PId;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.Width = (double)process.Duration / totalDuration * _listener.RRPanel.Width;
                l.BorderThickness = new System.Windows.Thickness(1);
                l.BorderBrush = System.Windows.Media.Brushes.Black;
                _listener.RRPanel.Children.Add(l);
            }
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  Updates the SJFSchedule, PrioritySchedule, and
        //              RoundRobinSchedule properties.
        //
        //Parameters:  (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        private void Reorder()
        {
            ReorderByPriority();
            ReorderByRoundRobin(Quantum);
            ReorderByShortestJobFirst();
        }

        //The three scheduler lists
        public ObservableCollection<Process> SJFSchedule { get; set; }
        public ObservableCollection<Process> PrioritySchedule { get; set; }
        public ObservableCollection<Process> RoundRobinSchedule { get; set; }

        //Original Process list
        public ObservableCollection<Process> Processes { get; private set; }

        //Time chunks for Round Robin
        public uint Quantum { get; set; }

        //Used when scheduling the SJF, Priority, and RR
        private uint _currentTime;

        //used for programmatically drawing gantt charts
        private MainWindow _listener;
    }
}
