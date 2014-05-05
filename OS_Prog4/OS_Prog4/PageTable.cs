using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    class PageTable
    {
        private ObservableCollection<char> pageTable;
        public ObservableCollection<char> _PageTable
        {
            get
            {
                return pageTable;
            }
        }

        int[,] frameList = new int[8,4]
        {
            {0,1,2,3},{4,5,6,7},{8,9,10,11},{12,13,14,15},{16,17,18,19},{20,21,22,23},{24,25,26,27},{28,29,30,31}        
        };

        int nextFrame = 0;
        public bool foundInTLB = false;

        private ObservableCollection<int> buffer;
        public ObservableCollection<int> TLB
        {
            get
            {
                return buffer;
            }
        }

        static string notFound = "Value was not found in the TLB";


        //*******************************************************************//
        //Author: Adam Meaney
        //
        //Date: May 3, 2014
        //
        //Description:  Constructor, initiallizes the class
        //
        //Parameters:   (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public PageTable()
        {
            int i;

            //initialize page table
            pageTable = new ObservableCollection<char>();
            for (i = 0; i < 8; i++)
            {
                pageTable.Add('-');
            }

            //initialize buffer
            buffer = new ObservableCollection<int>();
            for (i = 0; i < 4; i++)
            {
                buffer.Add(-1);
            }
        }


        //*******************************************************************//
        //Author: Adam Meaney
        //
        //Date: May 3, 2014
        //
        //Description:  Empties the page table with hypens
        //
        //Parameters:   (nothing)
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void resetTable()
        {
            for ( int i = 0; i < 8; i++ )
            {
                pageTable[i] = '-';
            }
        }


        //*******************************************************************//
        //Author: Adam Meaney
        //
        //Date: May 3, 2014
        //
        //Description:  Looks up the frame value given a page and offset
        //
        //Parameters:   page - page to lookup
        //              offset - Offset of page
        //
        //Returns:  the value in the physical memory
        //*******************************************************************//
        public int goPushed(int page, int offset)
        {
            char frame = pageTable[page];

            if (frame == '-')
            {
                frame = nextFrame.ToString()[0];
                pageTable[page] = frame;
                nextFrame++;
            }

            foundInTLB = checkTLB(page);
            updateTLB(page);

            // The value to be put in the values position in the picture.
            return frameList[(int)Char.GetNumericValue(frame), offset];
        }


        //*******************************************************************//
        //Author: Adam Meaney
        //
        //Date: May 3, 2014
        //
        //Description:  Replaces the TLB with a FIFO algorithm
        //
        //Parameters:   page - new page to insert
        //
        //Returns:  (nothing)
        //*******************************************************************//
        public void updateTLB(int page)
        {
            if (buffer[0] == page || buffer[2] == page)
                return;
            buffer[2] = buffer[0];
            buffer[3] = buffer[1];
            buffer[0] = page;
            buffer[1] = (int) Char.GetNumericValue(pageTable[page]);
        }


        //*******************************************************************//
        //Author: Adam Meaney
        //
        //Date: May 3, 2014
        //
        //Description:  Determines if a page is in the TLB or not.
        //
        //Parameters:   page - Page number to check
        //
        //Returns:  True - The value is in the TLB
        //          False - The page is not in the TLB
        //*******************************************************************//
        public bool checkTLB(int page)
        {
            if (buffer[0] == page || buffer[2] == page)
                return true;
            return false;
        }
    }
}
