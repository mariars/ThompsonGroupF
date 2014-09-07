using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Emil.GMP;

namespace uniqRIMGMP {
	//Reads nonempty lines with format strWord strInteger where strInteger is assumed 1 if missing
	//it returns the line: strWord  strInteger - weight strInteger from previousSrcFile if found. 
	// it returns "" if the new frequency is zero.

	class RemoveImmediateReturns {

		public bool done;
		private string line1;//points to current line in previousSrcFile
		//string line2;//points to current line in srcFile.
		private ReadText readTextprevious;
		private string[] words1;
		private int weight;

		public RemoveImmediateReturns(string previousSrcLocation,  int weight) {
			if (File.Exists(previousSrcLocation) == false) {//previousSrcFile does not exist
				Console.WriteLine("Warning: source file " + previousSrcLocation + "does not exist.");
				done = true;
				return;
			}
			done = false;
			this.weight = weight;
			readTextprevious = new ReadText(previousSrcLocation);
			if ((line1 = readTextprevious.ReadLine()) == null) { done = true; readTextprevious.Close(); return; }//previousSrcFile is empty
			words1 = ReadstrLine.Words(line1);
		}
		
		//returns line2 - weight(=3) line1 if the words of the two lines match. It returns "" if the frequency is 0.
		public string SubtractWeightTimes(string line2) {
			string output = "";
			if (done == true) return line2;// nothing is done if previousSrcFile has been read or does not exist
			if (line2 == null) { done = true; return line2; }
			string[] words2 = ReadstrLine.Words(line2);
			int cmp = 0;
			if ((cmp = string.CompareOrdinal(words1[0], words2[0])) != 0) return line2;
			//they are the same
			BigInt freq1 = 1;
			if (words1.Length == 2) freq1 = ReadstrLine.StrToInteger(words1[1]);
			BigInt freq2 = 1;
			if (words2.Length == 2) freq2 = ReadstrLine.StrToInteger(words2[1]);
			freq2 = freq2 - weight * freq1;
			//fileWrite.WriteLine(line1 + " == " + line2 + " ==> " + words2[0] + " " + freq2);
			if (freq2 == 1)
				output= words2[0];
			else {
				if (freq2 != 0) output= words2[0] + " " + freq2;
			}
			if ((line1 = readTextprevious.ReadLine()) == null) { done = true; readTextprevious.Close(); }//reads the next line of previousSrcFile
			else { words1 = ReadstrLine.Words(line1); }
			

			return output;

		}

	}
}
