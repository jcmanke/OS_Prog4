using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Prog4
{
    class PageTable
    {
        char[] pageTable = new char[]
        {
              '-','-','-','-','-','-','-','-' 
        };

        int[,] frameList = new int[8,4]
        {
            {0,1,2,3},{4,5,6,7},{8,9,10,11},{12,13,14,15},{16,17,18,19},{20,21,22,23},{24,25,26,27},{28,29,30,31}        
        };

        int nextFrame = 0;
        bool showString = false;

        int[,] buffer = new int[2, 2]
        {
            {-1,-1},{-1,-1}
        };

        static string notFound = "Value was not found in the TLB";

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
                frame = (char) nextFrame;
                pageTable[page] = frame;
                nextFrame++;
            }

            showString = checkTLB(page);
            updateTLB(page);

            // The value to be put in the values position in the picture.
            return frameList[(int) Char.GetNumericValue(frame), offset];
        }

        public void updateTLB(int page)
        {
            if (buffer[0,0] == page || buffer[1,0] == page)
                return;
            buffer[1,0] = buffer[0,0];
            buffer[1, 1] = buffer[0, 1];
            buffer[0, 0] = page;
            buffer[0, 1] = (int) Char.GetNumericValue(pageTable[page]);
        }

        public bool checkTLB(int page)
        {
            if (buffer[0, 0] == page || buffer[1, 0] == page)
                return true;
            return false;
        }
    }
}
