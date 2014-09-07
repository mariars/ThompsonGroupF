using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace uniqRIMGMP {
	//The following code for this class was obtained from the following link:
	//http://thedeveloperpage.wordpress.com/c-articles/using-file-streams-to-write-any-size-file-introduction/part-2-putting-it-into-action/
	class WriteText {
		private string destLocation;
		private FileStream destFile;
		private int bufferLength;
		public WriteText(string destLocation) {
			this.destLocation = destLocation;
			destFile = new FileStream(destLocation, FileMode.Create, FileAccess.Write);
			bufferLength = 1; 
		}

		public void WriteLine(string line) {
			line += "\r\n";
			int length = line.Length;
			for (int i = 0; i < length; i++) {
				byte[] buffer = { (byte)line[i] };
				destFile.Write(buffer, 0, bufferLength);
				buffer = null;
			}
		}

		public void Write(string line) {
			int length = line.Length;
			for (int i = 0; i < length; i++) {
				byte[] buffer = { (byte)line[i] };
				destFile.Write(buffer, 0, bufferLength);
				buffer = null;
			}
		}

		public void Close() {
			destFile.Close();
			destFile.Dispose();
		}
	}
}
