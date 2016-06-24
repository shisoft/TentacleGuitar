namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;


public class StageInput : MonoBehaviour {

	public static bool[,] TrackHolding {
		get {
			return trackHolding;
		}
	}


	private static bool[,] trackHolding = new bool[24, 6];



	void Awake () {
		TrackHolding.Initialize();
	}



	void LateUpdate () {
		
		
		// Hold做出响应



		trackHolding.Initialize();
	}


	public static void Hold (int fret, int stringID) {
		fret = Mathf.Clamp(fret, 0, 23);
		stringID = Mathf.Clamp(stringID, 0, 5);
		trackHolding[fret, stringID] = true;
	}


}
}