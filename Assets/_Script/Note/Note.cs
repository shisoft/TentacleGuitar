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


	private int x;		// 0 ~ 23  对应 1 ~ 24 品
	private int y;		// 0 ~ 5 对应 最上面 ~ 最下面的琴弦
	private float time;	// 单位秒 音符在音乐里的对应时间
	private NoteState state; // 音符当前的状态，例如 还没被击打（none），被击打为完美（perfect），没打到（miss）
	private float beatOffset; // 玩家打击的时差，正好打成perfect时为0，提前0.1s点击音符时为-0.1
	[SerializeField]
	private SpriteRenderer[] ColorChangeSRs;	// 需要根据String改变颜色的SR
	[SerializeField]
	private Transform shadowTF;
	private SpriteRenderer[] allRenderer = null;	// 所有和这个音符相关的 SpriteRenderer
	private Animator ani = null;
	private bool isReady = false;


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
			for (int i = 0; i < ColorChangeSRs.Length; i++) {
				ColorChangeSRs[i].color = Stage.TheStageSetting.NoteColors[stringID];
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


}
}