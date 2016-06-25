using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class NotesMap
	{
		public static List<double> freqs = new List<double>();
		public static List<string> notes = new List<string>();
	
		private static void Add(string note, double freq){
			freqs.Add (freq);
			notes.Add (note);
		}

		public static void init(){
			Add("E2",82.4);
			Add("F2",87.3);
			Add("F2#",92.5);
			Add("G2",98);
			Add("G2#",103.8);
			Add("A2",110);
			Add("A2#",116.5);
			Add("B2",123.5);
			Add("C3",130.8);
			Add("C3#",138.6);
			Add("D3",146.8);
			Add("D3#",155.6);
			Add("E3",164.8);
			Add("F3",174.6);
			Add("F3#",185);
			Add("G3",196);
			Add("G3#",207.7);
			Add("A3",220);
			Add("A3#",233.1);
			Add("B3",246.9);
			Add("C4",261.6);
			Add("C4#",277.2);
			Add("D4",293.7);
			Add("D4#",311.1);
			Add("E4",329.6);
			Add("F4",349.2);
			Add("F4#",370);
			Add("G4",392);
			Add("G4#",415.3);
			Add("A4",440);
			Add("A4#",466.2);
			Add("B4",493.9);
			Add("C5",523.3);
			Add("C5#",554.4);
			Add("D5",587.3);
			Add("D5#",622.3);
			Add("E5",659.3);
			Add("F5",698.5);
			Add("F5#",740);
			Add("G5",784);
			Add("G5#",830.6);
			Add("A5",880);
			Add("A5#",932.3);
			Add("B5",987.8);
			Add("C6",1046.50);
		}
	}
}

