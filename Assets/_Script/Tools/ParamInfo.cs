namespace TentacleGuitarUnity {
	using UnityEngine;
	using System.Collections;




	public struct NoteInfo {

		public int X;
		public int Y;
		public float Time;
		public NoteType Type;
		public int PitchId;


		public enum NoteType {
			Tap = 0,
			Zero = 1,
			Hold = 2,
			Slide = 3,
		}


		public NoteInfo (int x, int y, float time, NoteType type, int pitchId) {
			X = x;
			Y = y;
			Time = time;
			Type = type;
			PitchId = pitchId;
		}

		public NoteInfo (int x, int y, float time, NoteType type) {
			X = x;
			Y = y;
			Time = time;
			Type = type;
			PitchId = 0;
		}

	}



}
