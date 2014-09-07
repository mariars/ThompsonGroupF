using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace uniqRIMGMP {
	class ReadText {
		private string srcLocation;
		private long bufferStart;
		private int bufferLength;
		private FileStream srcFile;
		public long fileLength;
		private bool DoneReadingFile;


		public ReadText(string srcLocation) {
			this.srcLocation = srcLocation;
			srcFile = new FileStream(srcLocation, FileMode.Open, FileAccess.Read);
			bufferStart = 0; 
			bufferLength = 1; 
			fileLength = srcFile.Length; //long=9223372036854775807bytes =8589934592 GBytes.
			if (fileLength == 0)
				DoneReadingFile = true;
			else
				DoneReadingFile = false;

		}

		public string ReadLine() {
			if (DoneReadingFile == true) return null;
			string line = "";
			while (bufferStart < fileLength) {
				byte[] buffer = { (byte)bufferStart };
				srcFile.Read(buffer, 0, bufferLength);
				bufferStart++;

				if (buffer[0] == '\r') { buffer = null; continue; }
				if (buffer[0] == '\n') {
					if (bufferStart == fileLength) DoneReadingFile = true;
					return line;
				}
				else
					line +=(char)( buffer[0]);
				buffer = null;
			}
			DoneReadingFile = true;
			return line;
		}

		public void Close() {
			srcFile.Close();
			srcFile.Dispose();
		}
	}
}
