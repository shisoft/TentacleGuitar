namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;

public class HoldNote : Note {



	public HoldNoteInfo NoteInfo {
		get {
			return new HoldNoteInfo(base.X, base.Y, base.Time, this.Length);
		}
		set {
			base.X = value.X;
			base.Y = value.Y;
			base.Time = value.Time;
			this.Length = value.Length;
		}
	}

	public float Length {
		get {
			return length;
		}
		set {
			length = Mathf.Max(0f, value);
		}
	}


	private float length;


	void OnEnable () {
		base.SetColor();
		base.SetLayer();
	}




}
}