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
	private int LayerID;



	void Awake () {
		TrackHolding.Initialize();
		LayerID = LayerMask.GetMask("Input");
	}


	
	void Update () {

		if (Stage.TheStageSetting.PlayMod == StagePlayMod.MouseAndKeyboard) {
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerID)) {
					Vector2 ID_String = Stage.TheStageSetting.TrackID(hit.point);
					Stage.ShackString((int)ID_String.y, 0.04f, 0.1f);
					Stage.Beat((int)ID_String.x, (int)ID_String.y);
				}
			}

			if (Input.GetMouseButton(0)) {
			
			}
		}

	}


	void LateUpdate () {
		
		
		// 做出响应



		trackHolding.Initialize();
	}


	public static void Hold (int fret, int stringID) {
		fret = Mathf.Clamp(fret, 0, 23);
		stringID = Mathf.Clamp(stringID, 0, 5);
		trackHolding[fret, stringID] = true;
	}


}
}