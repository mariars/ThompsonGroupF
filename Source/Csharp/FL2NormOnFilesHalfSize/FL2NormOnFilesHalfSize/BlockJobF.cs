using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FL2NormOnFilesHalfSize {

	class BlockJobF {

		public Block blockInput;
		public Block blockOutput;
		public Block blockOutputInverses;
		public long numberOfIdentities;
		public long numberOfSecondHalfs;



		public BlockJobF(Block blockInput) {
			this.blockInput = blockInput;
			blockOutput = new Block(blockInput.length * 4);//For each line we will be apply A1 A2 A3 A4 A5 A6
			blockOutputInverses = new Block(blockInput.length * 4);
			blockOutput.length = 0;
			numberOfIdentities = 0;
			numberOfSecondHalfs = 0;
		}

		//DumpToConsole applies x0 x1 x01 x11 to all the lines.Length stringForests in the string of lines "lines".
		public void DumpToConsole() {
			for (int i = 0; i < blockInput.length; i++) {
				blockOutput.buffer[blockOutput.length] = blockInput.ReadLine() + " " + i;
				blockOutput.length++;
			}

		}

		public void ApplyXiToBlock() {


			for (int i = 0; i < blockInput.length; i++) {

				string wordPlusfreq = blockInput.ReadLine();
				string[] words = ReadstrLine.Words(wordPlusfreq);

				string line = words[0];
				//x0:
				string strDoubleTree = "";
				DoubleTree doubleTree = DoubleTreeFunctions.StringToDoubleTree(line);


				DoubleTreeFunctions.functionX0(ref doubleTree);
				int partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}


				//X0inv:
				DoubleTreeFunctions.functionX0inv(ref doubleTree);
				DoubleTreeFunctions.functionX0inv(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}


				//X1:
				DoubleTreeFunctions.functionX0(ref doubleTree);
				DoubleTreeFunctions.functionX1(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}


				//X1inv:
				DoubleTreeFunctions.functionX1inv(ref doubleTree);
				DoubleTreeFunctions.functionX1inv(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}

				if (line[0] == 'z') return; //the forest is the identity so we do not apply inverses to it.

				//inverse of doubleTree
				DoubleTreeFunctions.functionX1(ref doubleTree);//to get back the forest
				DoubleTreeFunctions.DoubleTreeInverse(ref doubleTree);  //apply inverse.


				//X0:
				DoubleTreeFunctions.functionX0(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}



				//X0inv:
				DoubleTreeFunctions.functionX0inv(ref doubleTree);
				DoubleTreeFunctions.functionX0inv(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}


				//X1:
				DoubleTreeFunctions.functionX0(ref doubleTree);
				DoubleTreeFunctions.functionX1(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}

				//X1inv:
				DoubleTreeFunctions.functionX1inv(ref doubleTree);
				DoubleTreeFunctions.functionX1inv(ref doubleTree);
				partition = 0;
				strDoubleTree = DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree, ref partition);
				if (partition < 0) {
					if (words.Length == 2) {
						blockOutput.AddLine(strDoubleTree + " " + words[1]);// the frequency is spread to all words
					}
					else {
						blockOutput.AddLine(strDoubleTree);
					}
				}
				else {
					if (partition == 0) { if (words.Length == 2) numberOfIdentities += long.Parse(words[1]); else ++numberOfIdentities; }
					else { if (words.Length == 2) numberOfSecondHalfs += long.Parse(words[1]); else ++numberOfSecondHalfs; }
				}





			}

		}
	}
}
