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
	class WBuffer//write buffer
	{
        public string filepath;
        private bool failed;
        private string Buffer;
        private int buffersize;
        public string info;
        StreamWriter sw;

        


        public WBuffer(string filepath,int buffersize=30000, bool append = false )//true=yes false=no
        {
            this.filepath = filepath;
            this.buffersize = buffersize;
            failed = false;
         sw = new StreamWriter(filepath, append);//true if you want to append
         Buffer = "";
         info = "WriteFile=" + filepath + " buffersize=" + buffersize  + " append=" + append;
		}

		public void WriteLine(string line="") //Adds \n to the end of the line. Writes line to buffer. Buffer is saved when it is fulled
        {
            if (!failed)
            {
                Buffer += line + Environment.NewLine;
                if (Buffer.Length > buffersize) { sw.Write(Buffer); Buffer = ""; }
            }
                
		}
        public void Write(string line) 
        {
            if (!failed)
            {
                Buffer += line;
                if (Buffer.Length > buffersize) { sw.Write(Buffer); Buffer = ""; }
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