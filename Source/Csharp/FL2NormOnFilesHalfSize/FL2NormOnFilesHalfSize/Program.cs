using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FL2NormOnFilesHalfSize
{
	class Program
	{

		//This program was originally written for composing elements of Thompson group T, using the generators C, D which are not the Cannon-Floyd-Perry generators.
		//The actions of C and D on elements of T (represented as reduced double trees with a rotation) will appear in future work by U. Haagerup K. K. Olesen and M. Ramirez-Solano.
		//This program is adapted to elements of F with standard generators x0 x1 and is inspired by the paper "Forest diagrams for elements of Thompson’s group F" by Belk & Brown.
		//The algorithm for the action of x0 x1 C or D can be generalized to the action of any element from T.

		static void Main(string[] args) {

			Stopwatch sw = new Stopwatch();
			sw.Start();
			string srcPath = "", destPath = "",destInversesPath="";
			Console.WriteLine("PROGRAM NAME: FL2NormOnFilesHalf.exe");
			int n = 2;
			int threadAntal = 3,readingBlockSizeInLines=30000, writingBlockSizeInLines=100;
			if (args.Length > 0) {
				Console.WriteLine("Arguments given: " + string.Join(",", args));
				n = Convert(args[0],n);

				for (int i = 1; i < args.Length; i++) {

					if (args[i].Length > 1 && args[i][0] == '-' && i + 1 < args.Length) {
						if (args[i].Length != 2) { Console.WriteLine("Could not read option " + args[i] + ". (forgot a space???)"); continue; }
						switch (args[i][1]) {
							case 's'://srcPath
								srcPath = args[i + 1];
								if (srcPath.Length > 0) { if (srcPath[srcPath.Length - 1] != '\\')  srcPath = srcPath + @"\"; }
								break;
							case 'd'://destPath
								destPath = args[i + 1];
								if (destPath.Length > 0) { if (destPath[destPath.Length - 1] != '\\') destPath = destPath + @"\"; }
								break;
							case 'i'://destInversesPath
								destInversesPath = args[i + 1];
								if (destInversesPath.Length > 0) { if (destInversesPath[destInversesPath.Length - 1] != '\\') destInversesPath = destInversesPath + @"\"; }
								break;
							case 't'://numberOfThreads
								threadAntal = Convert(args[i + 1], -1);
								if (threadAntal < 0) threadAntal = 3;
								break;
							case 'r'://ReadingBlockSizeInLines
								readingBlockSizeInLines = Convert(args[i + 1], readingBlockSizeInLines);
								break;
							case 'w'://WritingBlockSizeInLines
								writingBlockSizeInLines = Convert(args[i + 1], writingBlockSizeInLines);
								break;
							default:
								Console.WriteLine("Option " + args[i] + " not available");
								break;
						}
						++i;//compensating.			
					}

				}
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("n=" + n);
				Console.WriteLine("number of Threads= " + threadAntal);
				Console.WriteLine("ReadingBlockSizeInLines= " + readingBlockSizeInLines);
				Console.WriteLine("WritingBlocSinzeInLines= " + writingBlockSizeInLines);
				Console.WriteLine("srcPath= " + srcPath);
				Console.WriteLine("destPath= " + destPath);
				Console.WriteLine("destInversesPath= " + destInversesPath);
			
				//Console.ReadKey();
				Console.ResetColor();
				//return;

			}
			else {
				Console.ForegroundColor = ConsoleColor.Magenta;
				Console.WriteLine("Usage: FL2NormOnFilesHalfSize   number  [OPTIONS]\n");
				Console.WriteLine("FL2NormOnFilesHalfSize 0        creates the file n0p7s.forest");
				Console.WriteLine("FL2NormOnFilesHalfSize number   reads the files n[number-1]p[1-7]ur.forest and  creates the files n[number]p[1-7].forest\n");
				Console.WriteLine("OPTIONS:");
				Console.WriteLine("-t  number of threads (default is 3).");
				Console.WriteLine("-r  reads the srcfile in blocks of r lines (default is 30000).");
				Console.WriteLine("-w  writes the destfile in blocks of s lines (default is 100).");
				Console.WriteLine("-s  path of the source file.");
				Console.WriteLine("-d  path of the destination file.\n");
				Console.WriteLine("EXAMPLE: FL2NormOnFilesHalfSize 10 -r 40000 -w 500");

				Console.ResetColor();
				//Console.ReadKey();
				return;
			}

			Main main = new Main();
			main.start(n,threadAntal,readingBlockSizeInLines,writingBlockSizeInLines, srcPath,destPath,destInversesPath);
                       
			sw.Stop();
			TimeSpan ts = sw.Elapsed;
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
			Console.WriteLine("- End of program. Runtime: " + elapsedTime + ". Press a key to exit -");

            //closing logfile.
            main.FileLog.WriteLine("\n- End of program. Runtime: " + elapsedTime + ". Press a key to exit -");
            main.FileLog.Close();
            if (args.Length == 0) Console.ReadKey();
			//Console.ReadKey();
		}

		private static int Convert(string value,int defaultvalue) {
			int number = defaultvalue;
			try {
				number = Int32.Parse(value);
				
			}
			catch (FormatException) {
				Console.WriteLine("Could not convert " +value+" to a number.");
			}
			return number;
		}
	}
}
