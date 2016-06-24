namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;


public struct SongInitInfo {
	
	public string SongFilePath;
	public string BeatMapFilePath;
	public int SpeedScale;
	public StagePlayMod PlayMode;


	public SongInitInfo (string songPath, string beatMapPath, int speedScale, StagePlayMod playMode) {
		SongFilePath = songPath;
		BeatMapFilePath = beatMapPath;
		SpeedScale = speedScale;
		PlayMode = playMode;
	}


	
}



public struct NoteInfo {

	public int X;
	public int Y;
	public float Time;
	public NoteType Type;

	public enum NoteType {
		Tap = 0,
		Zero = 1,
		Hold = 2,
		Slide = 3,
	}


	public NoteInfo (int x, int y, float time, NoteType type) {
		X = x;
		Y = y;
		Time = time;
		Type = type;
	}

}



}
