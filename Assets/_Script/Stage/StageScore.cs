namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;

public class StageScore {


	public static bool IsOpen = false;

	public static int PerfectNum = 0;
	public static int GoodNum = 0;
	public static int MissNum = 0;
	public static int MaxCombo = 0;
	public static int Combo = 0;
	public static float CurrentScore {
		get {
			if (PerfectNum >= NoteNum) {
				return FullScore;
			}
			return currentScore;
		}
		set {
			currentScore = Mathf.Clamp(value, 0f, FullScore);
		}
	}

	public static int NoteNum {
		get {
			return noteNum;
		}
	}
	private static int noteNum;

	public static readonly float GoodScoreRant = 0.5f;
	public static readonly float FullScore = 1000000f;

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
	private static float currentScore = 0f;


	public static void Open (int num) {
		Close();
		IsOpen = true;
		noteNum = num;
	}


	public static void Close () {
		IsOpen = false;
		PerfectNum = 0;
		GoodNum = 0;
		MissNum = 0;
		MaxCombo = 0;
		Combo = 0;
		noteNum = 0;
		CurrentScore = 0;
		k = -1f;
	}



	public static void AddScore (NoteState state) {

		if (!IsOpen) {
			return;
		}

		if (NoteNum <= 0) {
			Debug.LogWarning("Can NOT add score now. Because the notenum is < 0. Did you forgot to call StageScore.Open() after load a map ?");
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



	





}
}