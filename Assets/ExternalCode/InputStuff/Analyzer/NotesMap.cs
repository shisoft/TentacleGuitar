using System;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class NotesMap
	{
		public static Dictionary<String, float> noteFreq = new Dictionary<string, float> ();

		public void init(){
			noteFreq.Add("E2",82.4);
			noteFreq.Add("F2",87.3);
			noteFreq.Add("F2#",92.5);
			noteFreq.Add("G2",98);
			noteFreq.Add("G2#",103.8);
			noteFreq.Add("A2",110);
			noteFreq.Add("A2#",116.5);
			noteFreq.Add("B2",123.5);
			noteFreq.Add("C3",130.8);
			noteFreq.Add("C3#",138.6);
			noteFreq.Add("D3",146.8);
			noteFreq.Add("D3#",155.6);
			noteFreq.Add("E3",164.8);
			noteFreq.Add("F3",174.6);
			noteFreq.Add("F3#",185);
			noteFreq.Add("G3",196);
			noteFreq.Add("G3#",207.7);
			noteFreq.Add("A3",220);
			noteFreq.Add("A3#",233.1);
			noteFreq.Add("B3",246.9);
			noteFreq.Add("C4",261.6);
			noteFreq.Add("C4#",277.2);
			noteFreq.Add("D4",293.7);
			noteFreq.Add("D4#",311.1);
			noteFreq.Add("E4",329.6);
			noteFreq.Add("F4",349.2);
			noteFreq.Add("F4#",370);
			noteFreq.Add("G4",392);
			noteFreq.Add("G4#",415.3);
			noteFreq.Add("A4",440);
			noteFreq.Add("A4#",466.2);
			noteFreq.Add("B4",493.9);
			noteFreq.Add("C5",523.3);
			noteFreq.Add("C5#",554.4);
			noteFreq.Add("D5",587.3);
			noteFreq.Add("D5#",622.3);
			noteFreq.Add("E5",659.3);
			noteFreq.Add("F5",698.5);
			noteFreq.Add("F5#",740);
			noteFreq.Add("G5",784);
			noteFreq.Add("G5#",830.6);
			noteFreq.Add("A5",880);
			noteFreq.Add("A5#",932.3);
			noteFreq.Add("B5",987.8);
			noteFreq.Add("C6",1046.50);
		}
	}
}

