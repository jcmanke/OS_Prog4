using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{

    /**************************************************************************
     * Author: Josh Schultz
     * 
     * Date: April 14, 2014
     * 
     * Class:          Process
     * Inherits From:  (nothing)
     * Imeplements:    (nothing)
     * 
     * Description:  This class is used mimic a process.  It simple contains
     *               5 properties.
     *               PID - the process id
     *               StartTime - the start time of the process
     *               Duration - the length of the process
     *               Priority - the priority number of the process
     *               Executed - Used to mark the process as already executed or not
     *               
     * Constructors:  public Process(uint pId, uint startTime, uint duration, uint priority)
     *                public Process(Process process)
     *                
     * Methods: public override string ToString()
     * 
    **************************************************************************/
    public class Process
    {

        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  The constructor simply initializes the public properties
        //
        //Parameters:   pID - the process id
        //              startTime - the start time of the process
        //              duration - the amount of time the process executes
        //              priority - the priority of the process
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public Process(uint pId, uint startTime, uint duration, uint priority)
        {
            PId = pId;
            StartTime = startTime;
            Duration = duration;
            Priority = priority;
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  Copy constructor, allows a copy to be made of an existing
        //              process
        //
        //Parameters:   process - the existing process to duplicate
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public Process(Process process)
        {
            StartTime = process.StartTime;
            Duration = process.Duration;
            PId = process.PId;
            Priority = process.Priority;
            Executed = process.Executed;
        }

        //Start time of the process
        public uint StartTime { get; set; }

        //Duration of the process
        public uint Duration { get; set; }

        //Process ID
        public uint PId { get; set; }

        //Priority of the process
        public uint Priority { get; set; }

        //Determines if the process has executed or not
        public bool Executed { get; set; }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: April 14, 2014
        //
        //Description:  Returns a string to easily tell what's inside the process
        //
        //Parameters:   (nothing)
        //
        //Returns:  A string representative of the process
        //*******************************************************************//
        public override string ToString()
        {
            return String.Format("Process ID: {0}\n  StartTime: {1}\n  Duration: {2}\n  Priority: {3}\n  Executed: {4}", PId, StartTime, Duration, Priority, Executed);
        }
    }



}
