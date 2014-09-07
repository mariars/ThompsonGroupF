using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emil.GMP;


namespace uniqRIMGMP {
	class Program {
		static void Main(string[] args) {

			string srcLocation = "", previousSrcLocation = "", destLocation = "";
			int weight = 3;// for RIM=remove inmediate returns
			if (args.Length < 3) {
				Console.WriteLine("arguments given: " + string.Join(",", args));
				Console.WriteLine("Usage: uniqRIMGMP srcLocation previousSrcLocation destLocation [weight]\n");
				Console.WriteLine("Example: uniqRIMGMP n10p7s.forest n8p7ur.forest n10p7ur.forest 3\n");
				Console.WriteLine("EXPLANATION FOR THE EXAMPLE:");
				Console.WriteLine("The forests in n10p7ur.forest are assumed to be just sorted ");
				Console.WriteLine("The forests in n8p7ur.forest are assumed to be unique and sorted (the r is to remind us that we are using words with no inmediate returns)");
				Console.WriteLine("First the program collects the frequences of the same forest in n10p7s.forest. The file n8p7ur.forest is a subset of n10p7ur.forest when we ignore the frequencies. The program removes 3 times the frequency of a forest in n8p7ur.forest from the frequency of the same forest found in n10p7ur.forest ");
				Console.WriteLine("The weight is optional. By default weight=3 and (n[number]p[vol]s.forest n[number-2]p[vol]ur.forest)");
				//Console.ReadKey();
				return;
			}
			else {
				srcLocation = args[0];
				previousSrcLocation = args[1];
				destLocation = args[2];
				if (args.Length == 4) weight = int.Parse(args[3]);
				//n = int.Parse(args[0]);
				//voln = int.Parse(args[1]);
			}


			string fileNamePrev = previousSrcLocation;// "N" + (n - 2) + "p" + voln + "s.forest"; FOR RIM
			string fileName = srcLocation;// "N" + n + "p" + voln + "s.forest"; FOR RIM
			string fileNameWrite = destLocation;// "N" + n + "p" + voln + "ss.forest"; FOR RIM
			string statsLocation = destLocation + ".stats";

			Console.WriteLine("sourceLocation= " + srcLocation);
			Console.WriteLine("previousSourceLocation= " + previousSrcLocation);
			Console.WriteLine("destinationLocation= " + destLocation);


			

			if (File.Exists(srcLocation) == false) {
				Console.WriteLine("Error: source file "+srcLocation+"does not exist.");
				return;
			}

			



			RemoveImmediateReturns rim = new RemoveImmediateReturns(previousSrcLocation,weight);

			ReadText readText = new ReadText(srcLocation);
			WriteText writeText = new WriteText(destLocation);
			WriteText writeTextstats = new WriteText(statsLocation);
			BigInt count = 0;
			BigInt sum = 0;
			BigInt squaressum = 0;

			string line = readText.ReadLine();
			if (line != null) { //file is not empty
			
				if (line == "") { Console.WriteLine("0 0 Error[Uniq]:Recieved an empty line"); }
				else {
					string[] words = ReadstrLine.Words(line);
					string wordp = words[0];
					BigInt freqp = 1;
					if (words.Length == 2) freqp = ReadstrLine.StrToInteger(words[1]);
					string word = "";
					BigInt freq = 0;

					while ((line = readText.ReadLine()) != null) {
						if (line == "") { Console.WriteLine("0 0 Error[Uniq]:Recieved an empty line"); break; }
						//Console.WriteLine("You have inputtet " + line);

						words = ReadstrLine.Words(line);
						word = words[0];
						freq = 1;
						if (words.Length == 2) freq = ReadstrLine.StrToInteger(words[1]);

						if (wordp == word) freqp += freq;
						else {
							if (freqp == 1) writeAndCount(line: rim.SubtractWeightTimes(wordp), writeText: writeText, count: ref count, sum: ref  sum, squaressum: ref squaressum);  //writeText.WriteLine(wordp);
							else
								writeAndCount(line: rim.SubtractWeightTimes(wordp + " " + freqp), writeText: writeText, count: ref count, sum: ref  sum, squaressum: ref squaressum); //writeText.WriteLine(wordp + " " + freqp);
							freqp = freq;
							wordp = word;
						}
					}

					if (freqp == 1) writeAndCount(line: rim.SubtractWeightTimes(wordp), writeText: writeText, count: ref count, sum: ref  sum, squaressum: ref squaressum);	//writeText.WriteLine(wordp);
					else
						writeAndCount(line: rim.SubtractWeightTimes(wordp + " " + freqp), writeText: writeText,count: ref count,sum: ref  sum, squaressum: ref squaressum); //writeText.WriteLine(wordp + " " + freqp);
				}
				
			}
			if (rim.done == false) { Console.WriteLine("WARNING: The file "+previousSrcLocation+"was not read completely.");  }
			readText.Close();
			writeText.Close();


			writeTextstats.WriteLine("{" + count + "," + sum + "," + squaressum + "}+" + "\t\t{count, sum, squaressum}" + "src= " + srcLocation + " previousSrc= " + previousSrcLocation + " dest= " + destLocation);
			Console.WriteLine("{" + count + "," + sum + "," + squaressum + "},");

		}

		static void writeAndCount(string line, WriteText writeText,ref BigInt count, ref BigInt sum, ref BigInt squaressum) {
			if (line == "") return;
			writeText.WriteLine(line);
			string[] words = ReadstrLine.Words(line);
			BigInt freq = 1;
			if (words.Length == 2) freq = ReadstrLine.StrToInteger(words[1]);
			++count;
			sum += freq;
			squaressum += freq * freq;
		}
	}
}
