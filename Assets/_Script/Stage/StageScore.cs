namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;

public class StageScore {


	public static int PerfectNum = 0;
	public static int GoodNum = 0;
	public static int MissNum = 0;
	public static int MaxCombo = 0;
	public static int Combo = 0;
	public static float CurrentScore = 0f;

	public static int NoteNum {
		get {
			return noteNum;
		}
	}
	private static int noteNum;

	public static readonly float GoodScoreRant = 0.5f;
	public static readonly int FullScore = 1000000;

	private static float K {
		get {
			if (k < 0f) {
				if (NoteNum <= 0) {
					k = -1f;
				} else {
					k = FullScore * 2f / (NoteNum * (NoteNum + 1));
				}
			}
			return k;
		}
	}
	private static float k = -1f;



	public static void Init (int num) {
		Clear();
		noteNum = num;
	}


	public static void AddScore (NoteState state) {

		if (NoteNum <= 0) {
			Debug.LogWarning("Can NOT add score now. Because the notenum is < 0. Did you forgot to call StageScore.Init() after load a map ?");
			return;
		}


		switch (state) {
			case NoteState.Perfect:
				PerfectNum++;
				Combo++;
				MaxCombo = Mathf.Max(MaxCombo, Combo);
				CurrentScore += K * Combo;
				break;
			case NoteState.Good:
				GoodNum++;
				Combo++;
				MaxCombo = Mathf.Max(MaxCombo, Combo);
				CurrentScore += K * Combo * GoodScoreRant;
				break;
			case NoteState.Miss:
				Combo = 0;
				MissNum++;
				break;
			default:
				break;
		}
	}



	public static void Clear () {
		PerfectNum = 0;
		GoodNum = 0;
		MissNum = 0;
		MaxCombo = 0;
		Combo = 0;
		noteNum = 0;
		k = -1f;
	}





}
}