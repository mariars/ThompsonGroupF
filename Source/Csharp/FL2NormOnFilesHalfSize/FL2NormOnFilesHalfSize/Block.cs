using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FL2NormOnFilesHalfSize
{

	class Block {

		public string[] buffer;
		public int buffersizeInLines;
		public int pos;
		public int length;

		public Block(int buffersize = 30000) {
			this.buffersizeInLines = buffersize;
			buffer = new string[buffersize];
			pos = 0;
			length = 0;
		}

		public string ReadLine() {
			if (pos >= length) return null;
			if (length == 0) return null;
			return buffer[pos++];
		}
		public void AddLine(string line) {
			buffer[length] = line;
			length++;
		}

	}
}
