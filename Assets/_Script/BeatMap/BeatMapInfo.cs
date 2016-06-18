namespace TentacleGuitarUnity {


using UnityEngine;
using System.Collections;

public class BeatMapInfo {

	public static BeatMapInfo Current = null;



	public int BPM {
		get {
			return bpm;
		}
		set {
			bpm = Mathf.Clamp(value, 1, 6000);
		}
	}
	public float MPB {
		get {
			return 1f / bpm;
		}
	}


	private int bpm;


}
}
