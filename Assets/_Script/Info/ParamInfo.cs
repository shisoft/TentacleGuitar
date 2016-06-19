namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;

public struct SongInitInfo {
	
	public string SongFilePath;
	public string BeatMapFilePath;
	public bool PlayAfterLoad;
	public StagePlayMod PlayMode;


	public SongInitInfo (string songPath, string beatMapPath, StagePlayMod playMode = StagePlayMod.RealGuitar, bool playAfterLoad = true) {
		SongFilePath = songPath;
		BeatMapFilePath = beatMapPath;
		PlayAfterLoad = playAfterLoad;
		PlayMode = playMode;
	}


	
}



public struct TapNoteInfo {

	public int X;
	public int Y;
	public float Time;

	public TapNoteInfo (int x, int y, float time) {
		X = x;
		Y = y;
		Time = time;
	}

}


public struct HoldNoteInfo {

	public int X;
	public int Y;
	public float Time;
	public float Length;

	public HoldNoteInfo (int x, int y, float time, float length) {
		X = x;
		Y = y;
		Time = time;
		Length = length;
	}

}


}
