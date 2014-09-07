using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace FL2NormOnFilesHalfSize {
	class TL2Norm {
		

		//64-ALPHABET: "0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmno";

		private void strToFiles(WriteTextBuffer[] FilesForests, string str) {
			if (str.Length == 0) {
				Console.WriteLine("Error in strToFiles: Attempting to save an empty doubleTree??");
				return;
			}
			int i = BinarySearcher.BinarySearch(str[0]);//i is between 0 and 7. Run BinarySearcher.Example() to test it
			FilesForests[i].WriteLine(str);
		}

		public void NextTrinParFSymmetric(int n, int threadAntal, int readingBlockSizeInLines, int writingBlockSizeInLines, string srcPath, string destPath, string destInversesPath, WBuffer FileLog) {
			long numberOfIdentities = 0;
			long numberOfSecondHalfs = 0;
			Console.WriteLine("srcPath= " + (srcPath == "" ? "same folder" : srcPath));
			Console.WriteLine("destPath= " + (destPath == "" ? "same folder" : destPath));
			Console.WriteLine("destInversesPath= " + (destInversesPath == "" ? "same folder" : destInversesPath));
			Console.WriteLine("Computing n(" + n + ") using " + threadAntal + " threads...");
			FileLog.WriteLine("Computing n(" + n + ") using " + threadAntal + " threads...");
			Console.WriteLine("Any key for progress info. x to stop program.");
			Console.WriteLine(DateTime.Now.ToString("R") + " started...");

			if (n == 0) {  //case FL2NormOnFilesHalfSize 0
				string destfilename = "n0p7s.forest";// the identity is z and it should be in volume 7
				Console.WriteLine("Saving to file: " + destfilename);
				WriteTextBuffer fileDoubleTree = new WriteTextBuffer(destPath + destfilename);
				DoubleTree doubleTree = new DoubleTree();
				string str = "";
				int partition = 0;
				str=DoubleTreeFunctions.DoubleTreeToDoubleStringBinOnFPartition(doubleTree,ref partition);
				fileDoubleTree.WriteLine(str);
				fileDoubleTree.Close();
				return;
			}

			string[] destfilesnames = new string[8];
			WriteTextBuffer[] filesDoubleTrees = new WriteTextBuffer[8];

			//initialize files to where the doubletrees will go
			for (int i = 0; i < 8; i++) {
				destfilesnames[i] = "n" + n + "p" + i + ".forest";//FL2NormOnFilesHalfSize_n_Part_i
				Console.WriteLine("Saving to file: " + destfilesnames[i]);
				filesDoubleTrees[i] = new WriteTextBuffer(destPath + destfilesnames[i], writingBlockSizeInLines);
			}

			Console.WriteLine("Warning: PROGRAM DESIGNED FOR SYMMETRIC CASE A+B+A^-1+B^-1: reading ...ur.forest and not ...s.forest");

			//reading 8 files
			for (int j = 0; j < 8; j++) {
				string srcfilename = "n" + (n - 1) + "p" + j + "ur.forest";

				Console.WriteLine("Reading File: " + srcfilename);
				srcfilename = srcPath + srcfilename;

				if (File.Exists(srcfilename) == true) {
					if (new FileInfo(srcfilename).Length == 0) continue;
					ReadBlock currentSnFile = new ReadBlock(srcfilename, readingBlockSizeInLines);
					long lengthOfReadingFile = currentSnFile.fileLength;
					long numberOfReadLines = 0;
					
					bool doneReadingFile = false;

					BlockJobF[] tasks = new BlockJobF[threadAntal];
					Thread[] threads = new Thread[threadAntal];
					bool[] launchedTask = new bool[threadAntal];
					Block[] blocks = new Block[threadAntal];
					Array.Clear(tasks, 0, threadAntal);
					Array.Clear(launchedTask, 0, threadAntal);
					Array.Clear(threads, 0, threadAntal);

					//Thread.CurrentThread.Priority = ThreadPriority.Highest;

					while (doneReadingFile == false) {
						for (int i = 0; i < threadAntal; i++) {
							if (launchedTask[i] == false) {
								blocks[i] = currentSnFile.fillBuffer(); 
								if (blocks[i].length == 0) {
									doneReadingFile = true;
								}
								else {
									numberOfReadLines += blocks[i].length;
									launchedTask[i] = true;
									tasks[i] = new BlockJobF(blocks[i]);
									threads[i] = new Thread(tasks[i].ApplyXiToBlock);
									threads[i].Priority = ThreadPriority.Highest;
									threads[i].Start();
								}
							}
							else {//Task i has been launched
								if (threads[i].IsAlive == false) {
									//Collect the results and send them to file
									//If we comment the following line the processor jumps to 80% with 3 threads
									for (int k = 0; k < tasks[i].blockOutput.length; k++) {
										strToFiles(filesDoubleTrees, tasks[i].blockOutput.ReadLine());
									}									
									numberOfIdentities += tasks[i].numberOfIdentities;
									numberOfSecondHalfs += tasks[i].numberOfSecondHalfs;


									launchedTask[i] = false;
								}
							}
						}
						// Check if key has been pressed. Exit if x has been pressed
						if (Console.KeyAvailable) {
							Console.WriteLine(DateTime.Now.ToString("R") + "readLines/ReadingFileLength=" + numberOfReadLines + "/" + lengthOfReadingFile + " =" + (((double)numberOfReadLines /(double)lengthOfReadingFile) * 100) + "%");
							ConsoleKeyInfo cki = Console.ReadKey(true);
							if (cki.Key == ConsoleKey.X) { Console.WriteLine("Key x pressed... ABORTING PROGRAM !!! ..."); break; }
						}

					}
					//Wait for all threads to be done.
					Console.WriteLine("Done reading file " + srcfilename + ". Waiting for threads to be done.");
					for (int i = 0; i < threadAntal; i++) {
						if (launchedTask[i] == true) threads[i].Join();
					}
					//Collect the results and send them to file
					for (int i = 0; i < threadAntal; i++) {
						if (launchedTask[i] == true) {
							numberOfIdentities += tasks[i].numberOfIdentities;
							numberOfSecondHalfs += tasks[i].numberOfSecondHalfs;
							for (int k = 0; k < tasks[i].blockOutput.length; k++) {
								strToFiles(filesDoubleTrees, tasks[i].blockOutput.ReadLine());
							}

						}


					}

					currentSnFile.Close();
				}
			}

			if (numberOfIdentities != 0) {
				if (numberOfIdentities == 1) filesDoubleTrees[7].WriteLine("z");//adding the identities in vol 7.
				else
					filesDoubleTrees[7].WriteLine("z " + numberOfIdentities);//adding the identities in vol 7.			
			}
			//close the doubletrees files
			for (int i = 0; i < 8; i++) {
				filesDoubleTrees[i].Close();
			}
			Console.WriteLine("numberOfIdentities(" + n + ")= " + numberOfIdentities+ ", numberOfSecondHalfs(" + n + ")= " + numberOfSecondHalfs);
			FileLog.WriteLine("numberOfIdentities(" + n + ")= " + numberOfIdentities+ ", numberOfSecondHalfs(" + n + ")= " + numberOfSecondHalfs);
			
		}

	}
}
