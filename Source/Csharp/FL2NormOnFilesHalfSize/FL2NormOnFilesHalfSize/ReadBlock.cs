using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FL2NormOnFilesHalfSize
{

	class ReadBlock {

		private ReadText sr;
		private int buffersizeinLines;
		public long fileLength;

		public ReadBlock(string filepath, int buffersizeinLines = 4000) {
			this.buffersizeinLines = buffersizeinLines;
			sr = new ReadText(filepath);
			fileLength=sr.fileLength;
		
		}


		public Block fillBuffer() {
			Block block = new Block(buffersizeinLines);
			string line;
			// it's important that length<buffersize comes before sr.readline. Otherwise it will still read the line, when length reached buffersize, but it wont be put in the buffer, so there will be missing lines
			while (block.length < buffersizeinLines && (line = sr.ReadLine()) != null) {
				block.buffer[block.length] = line;
				++block.length;
			}
			return block;
		}

		public void Close() {
			sr.Close();
			
		}


	}
}
