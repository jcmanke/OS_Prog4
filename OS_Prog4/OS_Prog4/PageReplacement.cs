using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    public class PageReplacement
    {
        public PageReplacement()
        {
            //Set defaults
            Length = 10;
            MaxPageValue = 5;
            ReferenceString = new ObservableCollection<int>();
        }

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

        public ObservableCollection<ObservableCollection<string>> SecondChance()
        {
            //Skip the ordering if there are no references
            if(ReferenceString.Count <= 0)
            {
                return new ObservableCollection<ObservableCollection<string>>();
            }

            ObservableCollection<ObservableCollection<string>> frames = new ObservableCollection<ObservableCollection<string>>();

            //List to determine if the page number has already executed
            List<bool> referenceBits = new List<bool>(MaxPageValue);
            

            //Default the pages to false
            for (int i = 0; i < MaxPageValue; i++)
                referenceBits.Add(false);

            List<int> frameContents = new List<int>(3);

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

            int faults = 1;

            //Insert the first element
            frames[0][0] = ReferenceString[0].ToString() + "(0)";
            frames[3][0] = "F";
            frameContents[0] = ReferenceString[0];

            int nextOut = 2;

            for (int i = 1; i < Length; i++)
            {
                //If the current page exists in the existing table
                if (ReferenceString[i] == frameContents[0])
                {
                    //String representation of zero or one
                    string bitValue;

                    //Alternate the reference bit
                    referenceBits[ReferenceString[i]] = !referenceBits[ReferenceString[i]];

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

                    frames[3][i] = "F";
                    faults++;

                    if (frameContents[1] == -1)
                    {
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
                        frames[Math.Abs(nextOut + 2) % 3][i] = frames[Math.Abs(nextOut + 2) % 3][i - 1];
                        frames[Math.Abs(nextOut + 1) % 3][i] = frames[Math.Abs(nextOut + 1) % 3][i - 1];

                        //The next item is 1 less than the current index
                        //2->1 , 1->0, 0->2
                        nextOut = Math.Abs(nextOut + 2) % 3;               
                    }
                    else if (!referenceBits[frameContents[Math.Abs(nextOut + 2) % 3]])
                    {
                        
                        referenceBits[frameContents[nextOut]] = false;
                        frameContents[Math.Abs(nextOut + 2) % 3] = ReferenceString[i];

                        //Update the previous value from (1) to (0)
                        frames[nextOut][i] = frameContents[nextOut].ToString() + "(0)";

                        frames[Math.Abs(nextOut + 2) % 3][i] = ReferenceString[i].ToString() + "(0)";
                        frames[Math.Abs(nextOut + 1) % 3][i] = frames[Math.Abs(nextOut + 1) % 3][i - 1];

                        nextOut = Math.Abs(nextOut + 1) % 3;
                    }
                    else if (!referenceBits[frameContents[Math.Abs(nextOut + 1) % 3]])
                    {
                        //Update the previous two values from (1) to (0)
                        referenceBits[frameContents[nextOut]] = false;
                        referenceBits[frameContents[Math.Abs(nextOut + 2)%3]] = false;

                        //New value inserted into frameContent
                        frameContents[Math.Abs(nextOut + 1) % 3] = ReferenceString[i];

                        frames[nextOut][i] = frameContents[nextOut].ToString() + "(0)";
                        frames[Math.Abs(nextOut + 2) % 3][i] = frameContents[Math.Abs(nextOut + 2)%3].ToString() + "(0)";
                        frames[Math.Abs(nextOut + 1) % 3][i] = ReferenceString[i].ToString() + "(0)";
                    }
                    else
                    {
                        //Everything was referenced recently

                        //update the replaced value to zero
                        referenceBits[frameContents[nextOut]] = false;

                        frames[nextOut][i] = ReferenceString[i].ToString() + "(0)";
                        frames[Math.Abs(nextOut + 2) % 3][i] = frames[Math.Abs(nextOut + 2) % 3][i - 1];
                        frames[Math.Abs(nextOut + 1) % 3][i] = frames[Math.Abs(nextOut + 1) % 3][i - 1];

                        nextOut = Math.Abs(nextOut + 2) % 3; 
                    }
                }
            }

            //return faults;  //Number of faults
            return frames;  //4 rows, top 3 contains numbers, last row contains F's
        }
         
        public ObservableCollection<int> ReferenceString { get; set; }
        public int Length { get; set; }
        public int MaxPageValue { get; set; }
    }
}
