namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;


public class TapNote : Note {


	public TapNoteInfo NoteInfo {
		get {
			return new TapNoteInfo(base.X, base.Y, base.Time);
		}
		set {
			base.X = value.X;
			base.Y = value.Y;
			base.Time = value.Time;
		}
	}



	void OnEnable () {
		base.SetColor();
		base.SetLayer();
		base.BeatOffset = -Stage.TheStageSetting.MissTime - 1f;
		base.State = NoteState.None;
		TransformUpdate();
	}


	void OnDisable () {
		base.Clear();
	}

	

	void Update () {

		base.IsReady = true;

		StateUpdate();

		TransformUpdate();



	}


	void StateUpdate () {
		//      -missTime    -perTime     perTime   missTime
		//  none    |     good   |    per    |   good  |    miss
		NoteState prevState = base.State;
		switch (Stage.TheStageSetting.PlayMod) {
			default:
			case StagePlayMod.None:
				base.State = NoteState.None;
				break;
			case StagePlayMod.Auto:
				base.BeatOffset = 0f;
				base.State = Stage.Time > this.Time ? NoteState.Perfect : NoteState.None;
				break;
			case StagePlayMod.Mouse:
			case StagePlayMod.RealGuitar:
				if (base.BeatOffset < -Stage.TheStageSetting.MissTime) {
					base.State = NoteState.None;
				} else if (base.BeatOffset < -Stage.TheStageSetting.PerfectTime) {
					base.State = NoteState.Good;
				} else if (base.BeatOffset < Stage.TheStageSetting.PerfectTime) {
					base.State = NoteState.Perfect;
				} else if (base.BeatOffset < Stage.TheStageSetting.MissTime) {
					base.State = NoteState.Good;
				} else {
					base.State = NoteState.Miss;
				}
				break;
		}
		
		if (prevState != base.State) {
			Ani.SetTrigger(base.State.ToString());
		}

	}


	void TransformUpdate () {
		float prevZ = transform.localPosition.z;
		float lifeTime = (base.State == NoteState.None ? Mathf.Min(Stage.TheStageSetting.MissTime, Stage.Time - this.Time) : base.BeatOffset);
		float mDistance = Stage.TheStageSetting.StartMoveDistance;

		// Pos
		Vector3 pos = Stage.TheStageSetting.TrackPos(this.X, this.Y);
		pos.z = lifeTime * -Stage.TheStageSetting.NoteSpeed;
		pos.y = Mathf.Abs(pos.z) < mDistance ?
			Mathf.Lerp(0f, pos.y, (mDistance - Mathf.Abs(pos.z)) / mDistance) : 
			0f;

		transform.position = pos;
		
		if (pos.z > 0 != prevZ > 0) {
			SetLayer();
		}
		base.ShadowTF.position = new Vector3(pos.x, Stage.TheStageSetting.TrackBackPosY, pos.z);

		// Rot
		Vector3 angle = Quaternion.Lerp(
			Quaternion.Euler(0f, 0f, 0f),
			Quaternion.Euler(0f, 0f, 90f),
			Mathf.Abs(lifeTime) * Stage.TheStageSetting.NoteSpeed / Stage.TheStageSetting.StartRotDistance
		).eulerAngles;
		transform.rotation = Quaternion.Euler(angle);
		base.ShadowTF.rotation = Quaternion.Euler(90f, 0f, 0f);
		
	}







}
}