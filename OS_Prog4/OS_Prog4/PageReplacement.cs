using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{

    /**************************************************************************
     * Author: Josh Schultz, Adam Meaney
     * 
     * Date: May 2, 2014
     * 
     * Class:          PageReplacement
     * Inherits From:  (nothing)
     * Imeplements:    (nothing)
     * 
     * Description:  This class is used to handle page replacement in physical
     *               memory.  It simulates a FIFO, LRU, LFU, Optimal, Second
     *               Chance, and Clock algorithm.  Since Second Chance and the Clock
     *               algorithm are so similar, both methods use the exact same code.
     *               
     * Constructor: public PageReplacement()
     * 
     * Methods: public void GenerateReferenceString()
     *          public ObservableCollection<ObservableCollection<string>> SecondChance()
     *          
    **************************************************************************/
    public class PageReplacement
    {
        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  The constructor simply initializes the public properties
        //
        //Parameters:   (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public PageReplacement()
        {
            //Set defaults
            Length = 10;
            MaxPageValue = 5;
            ReferenceString = new ObservableCollection<int>();
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  The reference string for the pages are randomly generated.
        //              The public properties are used to determine the max page
        //              number, and the number of entries in the reference string.
        //
        //Parameters:   (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void GenerateReferenceString()
        {
            //Clear the existing reference string
            ReferenceString.Clear();

            //Insert random numbers into the reference string
            Random r = new Random();

            for (int i = 0; i < Length; i++)
            {
                //Generate a new random number and put it into the reference string
                int nextValue = r.Next(1, MaxPageValue);
                ReferenceString.Add(nextValue);

                //Verify the random values aren't generated the same for multiple iterations
                System.Threading.Thread.Sleep(20);
            }
            
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  This method uses the public property, ReferenceString
        //              to simulate the contents of three frames at each iteration.
        //              The simulation is based on a SecondChance page replacement,
        //              but works the exact same way as a Clock algorithm.
        //
        //Parameters:   (nothing)
        //
        //Returns:  frames - The result of the algorithm using three frames
        //*******************************************************************//
        public ObservableCollection<ObservableCollection<string>> SecondChance()
        {
            //Skip the ordering if there are no references
            if(ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

            //List to determine if the page number has already executed
            List<bool> referenceBits = new List<bool>(MaxPageValue);
            
            //Default the pages to false
            for (int i = 0; i < MaxPageValue+1; i++)
                referenceBits.Add(false);

            //The current contents of the frame
            List<int> frameContents = new List<int>(3);

            //Initialize the contents to an invalid number
            for (int i = 0; i < 3; i++)
            {
                frameContents.Add(-1);
            }

            //Initialize the frames with 3 frames
            for (int frameCount = 4; frameCount > 0; frameCount--)
            {
                ObservableCollection<string> row = new ObservableCollection<string>();
                for (int i = 0; i < ReferenceString.Count; i++)
                    row.Add("");
                frames.Add(row);
            }

            //Total number of faults
            int faults = 1;

            //Insert the first element
            frames[0][0] = ReferenceString[0].ToString() + "(0)";
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];

            //Index of next value to replace in the frame
            int nextOut = 2;

            for (int i = 1; i < Length; i++)
            {
                //If the current page exists in the existing frame table
                if (ReferenceString[i] == frameContents[0])
                {
                    //String representation of zero or one
                    string bitValue;

                    //Alternate the reference bit
                    referenceBits[ReferenceString[i]] = !referenceBits[ReferenceString[i]];

                    //Get the string literal of the reference bit
                    if (referenceBits[ReferenceString[i]])
                        bitValue = "1";
                    else
                        bitValue = "0";

                    //Insert into the current frame
                    frames[0][i] = ReferenceString[i].ToString() + "(" + bitValue + ")";
                    frames[1][i] = frames[1][i-1];
                    frames[2][i] = frames[2][i-1];
                }
                else if (ReferenceString[i] == frameContents[1])
                {
                    //String representation of zero or one
                    string bitValue;

                    //Alternate the reference bit
                    referenceBits[ReferenceString[i]] = !referenceBits[ReferenceString[i]];

                    //Get the string literal of the reference bit
                    if (referenceBits[ReferenceString[i]])
                        bitValue = "1";
                    else
                        bitValue = "0";

                    //Insert into the current frame
                    frames[0][i] = frames[0][i-1];
                    frames[1][i] = ReferenceString[i].ToString() + "(" + bitValue + ")";
                    frames[2][i] = frames[2][i-1];
                }
                else if (ReferenceString[i] == frameContents[2])
                {
                    //String representation of zero or one
                    string bitValue;

                    //Alternate the reference bit
                    referenceBits[ReferenceString[i]] = !referenceBits[ReferenceString[i]];

                    //Get the string literal of the reference bit
                    if (referenceBits[ReferenceString[i]])
                        bitValue = "1";
                    else
                        bitValue = "0";

                    //Insert into the current frame
                    frames[0][i] = frames[0][i-1];
                    frames[1][i] = frames[1][i-1];
                    frames[2][i] = ReferenceString[i].ToString() + "(" + bitValue + ")";
                }
                else
                {
                    //The item wasn't found in the frames

                    //Since it wasn't found, generate a fault
                    frames[3][i] = "F";
                    faults++;

                    //Determine if there is an open frame
                    if (frameContents[1] == -1)
                    {
                        //Insert into the open frame
                        frames[2][i] = "";
                        frames[1][i] = frames[0][i-1];
                        frames[0][i] = ReferenceString[i].ToString() + "(0)";

                        //Push down the queue
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }
                    else if (frameContents[2] == -1)
                    {
                        //Insert into the open frame
                        frames[2][i] = frames[1][i-1];
                        frames[1][i] = frames[0][i-1];
                        frames[0][i] = ReferenceString[i].ToString() + "(0)";

                        //Push down the queue
                        frameContents[2] = frameContents[1];
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }
                    
                    //If the next item to be removed hasn't been referenced recently,
                    //remove the frame
                    if (!referenceBits[frameContents[nextOut]])
                    {
                        //If the first item can be replaced, update the frameConents
                        frameContents[nextOut] = ReferenceString[i];

                        frames[nextOut][i] = ReferenceString[i].ToString() + "(0)";
                        frames[(nextOut + 2) % 3][i] = frames[(nextOut + 2) % 3][i - 1];
                        frames[(nextOut + 1) % 3][i] = frames[(nextOut + 1) % 3][i - 1];

                        //The next item is 1 less than the current index
                        //2->1 , 1->0, 0->2
                        nextOut = (nextOut + 2) % 3;               
                    }
                    else if (!referenceBits[frameContents[(nextOut + 2) % 3]])
                    {
                        
                        referenceBits[frameContents[nextOut]] = false;
                        frameContents[(nextOut + 2) % 3] = ReferenceString[i];

                        //Update the previous value from (1) to (0)
                        frames[nextOut][i] = frameContents[nextOut].ToString() + "(0)";

                        frames[(nextOut + 2) % 3][i] = ReferenceString[i].ToString() + "(0)";
                        frames[(nextOut + 1) % 3][i] = frames[(nextOut + 1) % 3][i - 1];

                        nextOut = (nextOut + 1) % 3;
                    }
                    else if (!referenceBits[frameContents[(nextOut + 1) % 3]])
                    {
                        //Update the previous two values from (1) to (0)
                        referenceBits[frameContents[nextOut]] = false;
                        referenceBits[frameContents[(nextOut + 2)%3]] = false;

                        //New value inserted into frameContent
                        frameContents[(nextOut + 1) % 3] = ReferenceString[i];

                        frames[nextOut][i] = frameContents[nextOut].ToString() + "(0)";
                        frames[(nextOut + 2) % 3][i] = frameContents[(nextOut + 2)%3].ToString() + "(0)";
                        frames[(nextOut + 1) % 3][i] = ReferenceString[i].ToString() + "(0)";
                    }
                    else
                    {
                        //Everything was referenced recently

                        //update the replaced value to zero
                        referenceBits[frameContents[nextOut]] = false;

                        frames[nextOut][i] = ReferenceString[i].ToString() + "(0)";
                        frames[(nextOut + 2) % 3][i] = frames[(nextOut + 2) % 3][i - 1];
                        frames[(nextOut + 1) % 3][i] = frames[(nextOut + 1) % 3][i - 1];

                        nextOut = (nextOut + 2) % 3; 
                    }
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }

        public ObservableCollection<ObservableCollection<string>> fifo()
        {
            int next = 0;

            //Skip the ordering if there are no references
            if (ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

            frames[0][0] = ReferenceString[0].ToString();
            frames[1][0] = "-";
            frames[2][0] = "-";
            frames[3][0] = "F";

            if (ReferenceString.Count > 1)
            {
                frames[0][1] = frames[0][0];
                frames[1][1] = ReferenceString[0].ToString();
                frames[2][1] = "-";
                frames[3][1] = "F";

                if (ReferenceString.Count > 2)
                {
                    frames[0][2] = frames[0][0];
                    frames[1][2] = frames[1][1];
                    frames[2][2] = ReferenceString[2].ToString();
                    frames[3][2] = "F";

                    for (int index = 3; index < ReferenceString.Count; index++)
                    {
                        frames[0][index] = frames[0][index - 1];
                        frames[1][index] = frames[1][index - 1];
                        frames[2][index] = frames[2][index - 1];
                        frames[3][index] = "-";

                        if    (frames[0][index] != ReferenceString[index].ToString()
                            && frames[1][index] != ReferenceString[index].ToString()
                            && frames[2][index] != ReferenceString[index].ToString())
                        {
                            frames[3][index] = "F";
                            frames[next][index] = ReferenceString[index].ToString();
                            next = (next + 1 ) % 3;
                        }
                    }

                }
            }

            return frames;
        }


        public ObservableCollection<int> ReferenceString { get; set; }
        public int Length { get; set; }
        public int MaxPageValue { get; set; }
    }
}
