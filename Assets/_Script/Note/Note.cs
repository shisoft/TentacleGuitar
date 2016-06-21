namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;
using PathologicalGames;


public enum NoteState {
	None = 0,
	Miss = 1,
	Good = 2,
	Perfect = 3,
	Holding = 4,
}


/// <summary>
/// 这个音符类指的是显示层的音符，逻辑层要控制这个类，不要依赖这个类。
/// Note的子类都是注册在对象池里的。
/// 不要把 MonoBehaviour 的 Message 函数写进这个类里。
/// </summary>

[RequireComponent(typeof(Animator))]

public class Note : MonoBehaviour {


	#region -------- Var --------


	public int X {
		get {
			return x;
		}
		set {
			x = Mathf.Clamp(value, 0, 23);
		}
	}

	public int Y {
		get {
			return y;
		}
		set {
			y = Mathf.Clamp(value, 0, 5);
			SetColor();
		}
	}

	public float Time {
		get {
			return time;
		}
		set {
			time = Mathf.Max(value, 0f);
		}
	}

	public NoteInfo.NoteType Type {
		get {
			return type;
		}
		set {
			type = value;
		}
	}

	public NoteState State {
		get {
			return state;
		}
		set {
			state = value;
		}
	}

	public float BeatOffset {
		get {
			return beatOffset;
		}
		set {
			beatOffset = value;
		}
	}

	public SpriteRenderer[] AllRenderer {
		get {
			if (allRenderer == null) {
				allRenderer = GetComponentsInChildren<SpriteRenderer>(true);
			}
			return allRenderer;
		}
	}

	public Animator Ani {
		get {
			if (!ani) {
				ani = GetComponent<Animator>();
			}
			return ani;
		}
	}

	public Transform ShadowTF {
		get {
			return shadowTF;
		}
	}

	public bool IsReady {
		get {
			return isReady;
		}
		set {
			if (isReady != value) {
				for (int i = 0; i < AllRenderer.Length; i++) {
					AllRenderer[i].enabled = value;
				}
			}
			isReady = value;
		}
	}

	public NoteInfo NoteInfo {
		get {
			return new NoteInfo(X, Y, Time, Type);
		}
		set {
			X = value.X;
			Y = value.Y;
			Time = value.Time;
			Type = value.Type;
		}
	}



	private int x;		// 0 ~ 23  对应 1 ~ 24 品
	private int y;		// 0 ~ 5 对应 最上面 ~ 最下面的琴弦
	private float time;	// 单位秒 音符在音乐里的对应时间
	private NoteInfo.NoteType type; // 音符的类型，点击/长按 等
	private NoteState state; // 音符当前的状态，例如 还没被击打（none），被击打为完美（perfect），没打到（miss）
	private float beatOffset; // 玩家打击的时差，正好打成perfect时为0，提前0.1s点击音符时为-0.1
	[SerializeField]
	private SpriteRenderer[] ColorChangeSRs;	// 需要根据String改变颜色的SR
	[SerializeField]
	private Transform shadowTF;
	private SpriteRenderer[] allRenderer = null;	// 所有和这个音符相关的 SpriteRenderer
	private Animator ani = null;
	private bool isReady = true;



	#endregion



	#region -------- Puclic --------


	public NoteState Beat (float time) {
		if (State == NoteState.None) {
			BeatOffset = time - this.Time;
			StateUpdate();
			return State;
		} else {
			return NoteState.None;
		}
	}


	public void SetLayer () {
		SetLayer(Stage.Time > Time);
	}


	public void SetLayer (bool front) {
		for (int i = 0; i < AllRenderer.Length; i++) {
			AllRenderer[i].sortingLayerName = front ? "NoteFront" : "NoteBack";
		}
	}


	public void SetColor (int stringID = -1) {
		if (stringID == -1) {
			stringID = Y;
		}
		stringID = Mathf.Clamp(stringID, 0, 5);
		if (Stage.Main) {
			Color color = Stage.TheStageSetting.NoteColors[stringID];
			for (int i = 0; i < ColorChangeSRs.Length; i++) {
				ColorChangeSRs[i].color = color;
			}
		}
	}


	public void Despawn () {
		PoolManager.Pools["Note"].DespawnToDefault(this.transform);
	}


	public void Clear () {
		X = 0;
		Y = 0;
		Time = 0f;
		State = NoteState.None;
		BeatOffset = 0f;
		SetLayer();
		SetColor();
		IsReady = false;
	}


	#endregion



	#region -------- Mono --------


	void OnEnable () {
		SetColor();
		SetLayer();
		BeatOffset = -Stage.TheStageSetting.MissTime - 1f;
		State = NoteState.None;
		IsReady = false;
		TapNoteUpdate();
	}


	void OnDisable () {
		Clear();
	}



	void Update () {

		IsReady = true;

		StateUpdate();

		switch (Type) {
			default:
			case NoteInfo.NoteType.Tap:
				TapNoteUpdate();
				break;
			case NoteInfo.NoteType.Zero:
				ZeroNoteUpdate();
				break;
			case NoteInfo.NoteType.Hold:
				HoldNoteUpdate();
				break;
			case NoteInfo.NoteType.Slide:
				SlideNoteUpdate();
				break;
		}
		



	}


	#endregion



	#region -------- Logic --------



	void StateUpdate () {
		//      -missTime    -perTime     perTime   missTime
		//  none    |     good   |    per    |   good  |    miss
		if (State != NoteState.None) {
			return;
		}
		NoteState prevState = State;
		switch (Stage.TheStageSetting.PlayMod) {
			default:
			case StagePlayMod.Auto:
				BeatOffset = 0f;
				State = Stage.Time > this.Time ? NoteState.Perfect : NoteState.None;
				if (prevState != State) {
					StageScore.AddScore(State);
				}
				break;
			case StagePlayMod.MouseAndKeyboard:
			case StagePlayMod.RealGuitar:
				if (StageMusic.Main.Time - Time > Stage.TheStageSetting.MissTime) {
					BeatOffset = Stage.TheStageSetting.MissTime;
					State = NoteState.Miss;
					if (prevState != State) {
						StageScore.AddScore(State);
					}
					break;
				}
				if (BeatOffset < -Stage.TheStageSetting.MissTime) {
					State = NoteState.None;
				} else if (BeatOffset < -Stage.TheStageSetting.PerfectTime) {
					State = NoteState.Good;
				} else if (BeatOffset < Stage.TheStageSetting.PerfectTime) {
					State = NoteState.Perfect;
				} else if (BeatOffset < Stage.TheStageSetting.MissTime) {
					State = NoteState.Good;
				} else {
					State = NoteState.Miss;
				}
				break;
		}

		if (prevState != State) {
			Ani.SetTrigger(State.ToString());
		}

	}


	void TapNoteUpdate () {

		float prevZ = transform.localPosition.z;
		float mDistance = Stage.TheStageSetting.StartMoveDistance;
		float speed = Stage.TheStageSetting.NoteSpeed;
		float lifeTime = State == NoteState.None ? Stage.Time - this.Time : BeatOffset;
		if (lifeTime > 0f) {
			lifeTime *= 0.1f / Stage.TheStageSetting.SpeedScale;
		}

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
		
		// Rot
		Vector3 angle = Quaternion.Lerp(
			Quaternion.Euler(0f, 0f, 0f),
			Quaternion.Euler(0f, 0f, 90f),
			Mathf.Abs(lifeTime) * speed / Stage.TheStageSetting.StartRotDistance
		).eulerAngles;
		transform.rotation = Quaternion.Euler(angle);
		
		// Scale
		transform.localScale = Vector3.one * Mathf.Clamp01((Stage.TheStageSetting.ShowNoteTime * speed - Mathf.Abs(pos.z)) * 1f);
		
		// Shadow
		ShadowTF.position = new Vector3(pos.x, Stage.TheStageSetting.TrackBackPosY, pos.z);
		ShadowTF.rotation = Quaternion.Euler(90f, 0f, 0f);

	}


	void ZeroNoteUpdate () {






	}


	void HoldNoteUpdate () {






	}


	void SlideNoteUpdate () {






	}



	#endregion


}
}