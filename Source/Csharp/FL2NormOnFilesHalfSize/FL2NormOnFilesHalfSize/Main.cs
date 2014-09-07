using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FL2NormOnFilesHalfSize {
	class Main {
		public WBuffer FileLog;

		public Main() {
			FileLog = new WBuffer("logfile.txt", 2, true);//Append to logfile.txt
			Console.WriteLine("LOGFILE INFO: "+FileLog.info);
			FileLog.WriteLine("***LOG ENTRY " + DateTime.Now.ToString("R") + " ***");
		}

		public void start(int n, int threadAntal,int readingBlockSizeInLines,int writingBlockSizeInLines,string srcPath, string destPath,string destInversesPath) {
	
			if (n >= 0) {
				TL2Norm tL2Norm = new TL2Norm();
				tL2Norm.NextTrinParFSymmetric(n, threadAntal, readingBlockSizeInLines, writingBlockSizeInLines, srcPath, destPath, destInversesPath, FileLog);
				return;
			}
		}
	}
}
