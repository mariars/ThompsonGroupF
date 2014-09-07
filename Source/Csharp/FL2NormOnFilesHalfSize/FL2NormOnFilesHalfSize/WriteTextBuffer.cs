using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FL2NormOnFilesHalfSize
{
	class WriteTextBuffer//write buffer
	{
        public string destLocation;
        private bool failed;
        private string Buffer;
        private int buffersizeinLines;
        public string info;
        WriteText sw;

        


        public WriteTextBuffer(string destLocation,int buffersizeinLines=1000 )
        {
            this.destLocation = destLocation;
            this.buffersizeinLines = buffersizeinLines;
            failed = false;
			sw = new WriteText(destLocation);

         Buffer = "";
         info = "WriteFile=" + destLocation + " buffersizeinLines=" + buffersizeinLines;
		}

		public void WriteLine(string line="") //Adds \n to the end of the line. Writes line to buffer. Buffer is saved when it is fulled
        {
            if (!failed)
            {
                Buffer += line + Environment.NewLine;
                if (Buffer.Length > buffersizeinLines) { sw.Write(Buffer); Buffer = ""; }
            }
                
		}
        public void Write(string line) 
        {
            if (!failed)
            {
                Buffer += line;
                if (Buffer.Length > buffersizeinLines) { sw.Write(Buffer); Buffer = ""; }
            }

        }


        public void Close()
        {
            if (!failed)
            {
                if (Buffer.Length > 0) { sw.Write(Buffer); Buffer = ""; }
                sw.Close();
            }
        }
        			


	}
}