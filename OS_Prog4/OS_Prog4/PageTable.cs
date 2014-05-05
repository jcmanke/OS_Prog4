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
        bool showString = false;

        private ObservableCollection<int> buffer;
        public ObservableCollection<int> TLB
        {
            get
            {
                return buffer;
            }
        }

        static string notFound = "Value was not found in the TLB";

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

        public void resetTable()
        {
            for ( int i = 0; i < 8; i++ )
            {
                pageTable[i] = '-';
            }
        }

        public int goPushed(int page, int offset)
        {
            char frame = pageTable[page];

            if (frame == '-')
            {
                frame = nextFrame.ToString()[0];
                pageTable[page] = frame;
                nextFrame++;
            }

            showString = checkTLB(page);
            updateTLB(page);

            // The value to be put in the values position in the picture.
            return frameList[(int)Char.GetNumericValue(frame), offset];
        }

        public void updateTLB(int page)
        {
            if (buffer[0] == page || buffer[2] == page)
                return;
            buffer[2] = buffer[0];
            buffer[3] = buffer[1];
            buffer[0] = page;
            buffer[1] = (int) Char.GetNumericValue(pageTable[page]);
        }

        public bool checkTLB(int page)
        {
            if (buffer[0] == page || buffer[2] == page)
                return true;
            return false;
        }
    }
}
