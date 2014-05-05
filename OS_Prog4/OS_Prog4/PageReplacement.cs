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

        //*******************************************************************//
        //Author: Adam Meaney, Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  This method uses the public property, ReferenceString
        //              to simulate the contents of three frames at each iteration.
        //              The simulation is based on a FIFO page replacement.
        //
        //Parameters:   (nothing)
        //
        //Returns:  frames - conents of frames through each iteration 
        //*******************************************************************//
        public ObservableCollection<ObservableCollection<string>> FirstInFirstOut()
        {
            //Skip the ordering if there are no references
            if (ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

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
            frames[0][0] = ReferenceString[0].ToString();
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];

            for (int i = 1; i < Length; i++)
            {
                //If the current page exists in the existing frame table
                if (ReferenceString[i] == frameContents[0] ||
                    ReferenceString[i] == frameContents[1] ||
                    ReferenceString[i] == frameContents[2])
                {
                    frames[0][i] = frames[0][i - 1];
                    frames[1][i] = frames[1][i - 1];
                    frames[2][i] = frames[2][i - 1];
                    continue;
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
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();  //newest element

                        //Push down the queue
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }
                    else if (frameContents[2] == -1)
                    {
                        //Insert into the open frame
                        frames[2][i] = frames[1][i - 1];
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();

                        //Push down the queue
                        frameContents[2] = frameContents[1];
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }


                    frameContents[2] = frameContents[1];
                    frameContents[1] = frameContents[0];
                    frameContents[0] = ReferenceString[i];

                    //Update the frame table
                    frames[0][i] = frameContents[0].ToString();
                    frames[1][i] = frameContents[1].ToString();
                    frames[2][i] = frameContents[2].ToString();
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }



        //*******************************************************************//
        //Author: Adam Meaney, Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  This method uses the public property, ReferenceString
        //              to simulate the contents of three frames at each iteration.
        //              The simulation is based on a least used page replacement.
        //
        //Parameters:   (nothing)
        //
        //Returns:  frames - conents of frames through each iteration 
        //*******************************************************************//
        public ObservableCollection<ObservableCollection<string>> LeastRecentlyUsed()
        {
            //Skip the ordering if there are no references
            if (ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

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
            frames[0][0] = ReferenceString[0].ToString();
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];

            for (int i = 1; i < Length; i++)
            {
                //If the current page exists in the existing frame table
                if (ReferenceString[i] == frameContents[0])
                {
                    // [0]  <Recently used>  <-- current value
                    // [1]   ..
                    // [2]  <Least used>

                    //No ordering is needed

                    //Copy from the previous frame
                    frames[0][i] = frames[0][i - 1];
                    frames[1][i] = frames[1][i - 1];
                    frames[2][i] = frames[2][i - 1];
                }
                else if (ReferenceString[i] == frameContents[1])
                {
                    // [0]  <Recently used>  
                    // [1]   ..               <-- current value
                    // [2]  <Least used>

                    //Switch 0 and 1
                    int temp = frameContents[0];
                    frameContents[0] = frameContents[1];
                    frameContents[1] = temp;

                    //Output new order
                    frames[0][i] = frameContents[0].ToString();
                    frames[1][i] = frameContents[1].ToString();
                    frames[2][i] = frames[2][i - 1];
                }
                else if (ReferenceString[i] == frameContents[2])
                {
                    // [0]  <Recently used>  
                    // [1]   ..              
                    // [2]  <Least used>      <-- current value

                    //Rotate the three values
                    int temp = frameContents[2];
                    frameContents[2] = frameContents[1];
                    frameContents[1] = frameContents[0];
                    frameContents[0] = temp;

                    //Output new order
                    frames[0][i] = frameContents[0].ToString();
                    frames[1][i] = frameContents[1].ToString();
                    frames[2][i] = frameContents[2].ToString();
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
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();  //newest element

                        //Push down the queue
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }
                    else if (frameContents[2] == -1)
                    {
                        //Insert into the open frame
                        frames[2][i] = frames[1][i - 1];
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();

                        //Push down the queue
                        frameContents[2] = frameContents[1];
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }

                    frameContents[2] = frameContents[1];
                    frameContents[1] = frameContents[0];
                    frameContents[0] = ReferenceString[i];

                    //Update the frame table
                    frames[0][i] = frameContents[0].ToString();
                    frames[1][i] = frameContents[1].ToString();
                    frames[2][i] = frameContents[2].ToString();
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }


        //*******************************************************************//
        //Author: Josh Schultz
        //
        //Date: May 2, 2014
        //
        //Description:  This method uses the public property, ReferenceString
        //              to simulate the contents of three frames at each iteration.
        //              The simulation is based on an optimal page replacement.
        //
        //Parameters:   (nothing)
        //
        //Returns:  frames - conents of frames through each iteration 
        //*******************************************************************//
        public ObservableCollection<ObservableCollection<string>> Optimal()
        {
            //Skip the ordering if there are no references
            if (ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

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
            frames[0][0] = ReferenceString[0].ToString();
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];

            for (int i = 1; i < Length; i++)
            {
                //If the current page exists in the existing frame table
                if (ReferenceString[i] == frameContents[0] ||
                    ReferenceString[i] == frameContents[1] ||
                    ReferenceString[i] == frameContents[2])
                {
                    //Copy from the previous frame
                    frames[0][i] = frames[0][i - 1];
                    frames[1][i] = frames[1][i - 1];
                    frames[2][i] = frames[2][i - 1];
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
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();  //newest element

                        //Push down the queue
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }
                    else if (frameContents[2] == -1)
                    {
                        //Insert into the open frame
                        frames[2][i] = frames[1][i - 1];
                        frames[1][i] = frames[0][i - 1];
                        frames[0][i] = ReferenceString[i].ToString();

                        //Push down the queue
                        frameContents[2] = frameContents[1];
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                        continue;
                    }

                    //Determine the next values referenced in the string
                    bool[] usedSoon = new bool[3];
                    usedSoon[0] = false;
                    usedSoon[1] = false;
                    usedSoon[2] = false;

                    //Number of items in frame that will be referenced in the future
                    int numReferencedSoon = 0;

                    //The element wasn't in the frames and there wasn't an open frame
                    for (int j = i + 1; j < ReferenceString.Count; j++)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            //The a later item in the Reference string is currently in the frame contents
                            if (frameContents[k] == ReferenceString[j])
                            {
                                usedSoon[k] = true;
                                numReferencedSoon++;
                            }

                            //If two will be referenced soon, kick out the third
                            if (numReferencedSoon == 2)
                            {
                                //Update the current frame contents
                                if (!usedSoon[0])
                                {
                                    frameContents[0] = ReferenceString[i];
                                }
                                else if (!usedSoon[1])
                                {
                                    //Push the newest item at the top
                                    frameContents[1] = frameContents[0];
                                    frameContents[0] = ReferenceString[i];
                                }
                                else if (!usedSoon[2])
                                {
                                    //Push the newest item at the top of the frames
                                    frameContents[2] = frameContents[1];
                                    frameContents[1] = frameContents[0];
                                    frameContents[0] = ReferenceString[i];
                                }

                                //force the break out of both for loops
                                j = ReferenceString.Count;
                                break;
                            }
                        } //k < 3 loop
                    } //j < Reference.count loop

                    if (numReferencedSoon < 2)
                    {
                        frameContents[2] = frameContents[1];
                        frameContents[1] = frameContents[0];
                        frameContents[0] = ReferenceString[i];
                    }

                    //Update the frame table
                    frames[0][i] = frameContents[0].ToString();
                    frames[1][i] = frameContents[1].ToString();
                    frames[2][i] = frameContents[2].ToString();
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }



        public ObservableCollection<ObservableCollection<string>> LeastFrequentlyUsed()
        {
            //Skip the ordering if there are no references
            if (ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            //Array to store each frame iteration
            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

            //List to determine how many times a page was referenced
            List<int> referenceCounts = new List<int>(MaxPageValue);

            //Default the pages to zero references
            for (int i = 0; i < MaxPageValue + 1; i++)
                referenceCounts.Add(0);

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
            frames[0][0] = ReferenceString[0].ToString() + "(1)";
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];
            referenceCounts[ReferenceString[0]]++;

            for (int i = 1; i < Length; i++)
            {
                //Update the frequency count
                referenceCounts[ReferenceString[i]]++;

                //If the current page exists in the existing frame table
                if (ReferenceString[i] == frameContents[0])
                {
                    if (frameContents[1] != -1 && frameContents[2] != -1)
                    {
                        //If the frames are full, reorder them
                        //If they weren't full, -1 indexes would cause runtime errors
                        Reorder(frameContents, referenceCounts, ReferenceString[i]);
                        frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                        frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                        frames[2][i] = frameContents[2] + "(" + referenceCounts[frameContents[2]].ToString() + ")";
                    }
                    else if (frameContents[1] != -1)
                    {
                        //There are two options if one of the frames are -1
                        //The last two frames are -1 or only the last frame is -1
                        //This elseif determines that only the last frame is -1
                        if (referenceCounts[frameContents[0]] < referenceCounts[frameContents[1]])
                        {
                            //The ordering is flipped, so fix it
                            int temp = frameContents[0];
                            frameContents[0] = frameContents[1];
                            frameContents[1] = temp;
                        }

                        //Update the frames
                        frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                        frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                        frames[2][i] = "";
                    }
                    else
                    {
                        //The last 2 frames are -1
                        //This means only 1 actual value is in the frames

                        //Update the frames
                        frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                        frames[1][i] = "";
                        frames[2][i] = "";
                    }
                    continue;
                }
                else if (ReferenceString[i] == frameContents[1])
                {

                    if (frameContents[2] != -1)
                    {
                        Reorder(frameContents, referenceCounts, ReferenceString[i]);
                        frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                        frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                        frames[2][i] = frameContents[2] + "(" + referenceCounts[frameContents[2]].ToString() + ")";
                    }
                    else
                    {
                        //The last frame is -1, just compare the first two frames

                        //Determine if the first two need flipped
                        if (referenceCounts[frameContents[0]] < referenceCounts[frameContents[1]])
                        {
                            //The ordering is flipped, so fix it
                            int temp = frameContents[0];
                            frameContents[0] = frameContents[1];
                            frameContents[1] = temp;
                        }

                        //Update the frames
                        frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                        frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                        frames[2][i] = "";
                    }
                    continue;
                }
                else if (ReferenceString[i] == frameContents[2])
                {
                    Reorder(frameContents, referenceCounts, ReferenceString[i]);
                    frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                    frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                    frames[2][i] = frameContents[2] + "(" + referenceCounts[frameContents[2]].ToString() + ")";
                    continue;
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
                        frames[1][i] = ReferenceString[i].ToString() + "(1)";
                        frames[0][i] = frames[0][i - 1];

                        frameContents[1] = ReferenceString[i];

                        continue;
                    }
                    else if (frameContents[2] == -1)
                    {
                        //Insert into the open frame
                        frames[0][i] = frames[0][i - 1];
                        frames[1][i] = frames[1][i - 1];
                        frames[2][i] = ReferenceString[i].ToString() + "(1)";

                        //Push down the queue
                        frameContents[2] = ReferenceString[i];
                        continue;
                    }

                    //Geting to this place means there is a value in every frame spot

                    if (referenceCounts[frameContents[2]] <= referenceCounts[frameContents[0]] &&
                       referenceCounts[frameContents[2]] <= referenceCounts[frameContents[1]])
                    {
                        //If the bottom item is the least value, kick it out

                        frameContents[2] = ReferenceString[i];
                    }
                    else if (referenceCounts[frameContents[1]] <= referenceCounts[frameContents[2]] &&
                             referenceCounts[frameContents[1]] <= referenceCounts[frameContents[0]])
                    {
                        frameContents[1] = ReferenceString[i];
                    }
                    else
                    {
                        frameContents[0] = ReferenceString[i];
                    }

                    Reorder(frameContents, referenceCounts, ReferenceString[i]);

                    frames[0][i] = frameContents[0] + "(" + referenceCounts[frameContents[0]].ToString() + ")";
                    frames[1][i] = frameContents[1] + "(" + referenceCounts[frameContents[1]].ToString() + ")";
                    frames[2][i] = frameContents[2] + "(" + referenceCounts[frameContents[2]].ToString() + ")";
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }

        private void Reorder(List<int> frameContents, List<int> referenceCounts, int lastReferenced)
        {
            List<int> newOrder = new List<int>(3);
            newOrder.Add(0);
            newOrder.Add(0);
            newOrder.Add(0);

            if (referenceCounts[frameContents[0]] >= referenceCounts[frameContents[1]] &&
                referenceCounts[frameContents[0]] >= referenceCounts[frameContents[2]])
            {
                //The top item is the largest value

                newOrder[0] = frameContents[0];
                if (referenceCounts[frameContents[1]] >= referenceCounts[frameContents[2]])
                {
                    newOrder[1] = frameContents[1];
                    newOrder[2] = frameContents[2];
                }
                else
                {
                    newOrder[1] = frameContents[2];
                    newOrder[2] = frameContents[1];
                }
            }
            else if (referenceCounts[frameContents[1]] >= referenceCounts[frameContents[0]] &&
                referenceCounts[frameContents[1]] >= referenceCounts[frameContents[2]])
            {
                newOrder[0] = frameContents[1];
                if (referenceCounts[frameContents[0]] >= referenceCounts[frameContents[2]])
                {
                    newOrder[1] = frameContents[0];
                    newOrder[2] = frameContents[2];
                }
                else
                {
                    newOrder[1] = frameContents[2];
                    newOrder[2] = frameContents[0];
                }
            }
            else if (referenceCounts[frameContents[2]] >= referenceCounts[frameContents[1]] &&
                referenceCounts[frameContents[2]] >= referenceCounts[frameContents[0]])
            {
                newOrder[0] = frameContents[2];
                if (referenceCounts[frameContents[0]] >= referenceCounts[frameContents[1]])
                {
                    newOrder[1] = frameContents[0];
                    newOrder[2] = frameContents[1];
                }
                else
                {
                    newOrder[1] = frameContents[1];
                    newOrder[2] = frameContents[0];
                }
            }

            for (int i = 0; i < 3; i++)
            {
                frameContents[i] = newOrder[i];
            }
        }

        public ObservableCollection<int> ReferenceString { get; set; }
        public int Length { get; set; }
        public int MaxPageValue { get; set; }
    }
}
