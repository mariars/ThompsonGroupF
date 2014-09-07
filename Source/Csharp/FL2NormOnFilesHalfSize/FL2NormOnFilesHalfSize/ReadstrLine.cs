using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FL2NormOnFilesHalfSize {

	class ReadstrLine {

		static public void Example() {
			string[] strarray = ReadstrLine.WordsMin(" 23434  asdfs23423;jkldsf ");
			for (int i = 0; i < strarray.Length; i++) {
				Console.WriteLine("Word[" + i + "]=" + strarray[i]);
			}
			strarray = ReadstrLine.WordsMin("      2 fred 2000");
			for (int i = 0; i < strarray.Length; i++) {
				Console.WriteLine("Word[" + i + "]=" + strarray[i]);
			}
			string st = strarray[2];
			Console.WriteLine(2 * ReadstrLine.StrToInteger(st));

			Console.ReadKey();

		}

		static public string[] Words(string strLine) {
			return strLine.Split(' ');
		}

		static public string[] WordsMin(string strLine) {
			string str = strLine.Trim();
			return str.Split(' ');
		}

	
		static public int StrToInteger(string strWord) {
			return int.Parse(strWord);
		}

	}

}