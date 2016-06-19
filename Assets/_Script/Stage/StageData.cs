namespace TentacleGuitarUnity {

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum StagePlayMod {
	None = 0,
	Auto = 1,
	Mouse = 2,
	RealGuitar = 3,
}


[System.Serializable]
public class StageSetting {


	public float NoteSpeed {
		get {
			return SpeedScale * noteSpeed;
		}
	}	   // 获取音符绝对速度的快捷方式 unit / s

	public int SpeedScale {
		get {
			return speedScale;
		}
		set {
			speedScale = Mathf.Clamp(value, 1, 8);
		}
	}	  // 音符下落速度，由玩家按个人喜好自定义

	public float StartRotDistance {
		get {
			return startRotDistance;
		}
	}	  // 音符距离判定线多远(单位unit)开始旋转

	public float StartMoveDistance {
		get {
			return startMoveDistance;
		}
	}   // 音符距离判定线多远(单位unit)开始上下散开

	public float MusicLoadingMaxTime {
		get {
			return musicLoadingMaxTime;
		}
	}	 // 等待音乐载准备的时间（s），超出这个时间则宣告音乐载入失败。不要用代码修改。

	public Color[] NoteColors {
		get {
			return noteColors;
		}
	}	// 音符 发光时的颜色，0是最上面的。

	public Color[] StringColors {
		get {
			return stringColors;
		}
	}	// 琴弦 发光时的颜色，0是最上面的。

	public Color StringDarkColor {
		get {
			return stringDarkColor;
		}
	}	 // 琴弦 暗淡时的颜色

	public Transform NoteHolderTF {
		get {
			return noteHolderTF;
		}
	}	  // 装载音符的容器

	public Transform TrackBack {
		get {
			return trackBack;
		}
	}		// 跟着摄影机走的轨道背景

	public Transform MainCamera {
		get {
			return mainCamera;
		}
	}		// 摄影机容器

	public Vector3 CameraPos {
		get {
			return MainCamera.position;
		}
		set {
			MainCamera.position = value;
		}
	}	 // 摄影机位置

	public float TrackBackPosY {
		get {
			return trackBackPosY;
		}
	}   // 轨道底盘的Y坐标

	public SpriteRenderer[] Strings {
		get {
			return strings;
		}
	}	// 琴弦的渲染器

	public SpriteRenderer[] StringLights {
		get {
			return stringLights;
		}
	}	 // 琴弦光晕的渲染器

	public float MissTime {
		get {
			return missTime;
		}
	}	  // 音符穿过判定线后多少秒miss，介于MissTime 和 PerfectTime 之间算Good

	public float PerfectTime {
		get {
			return perfectTime;
		}
	}	 //音符在前后多少秒内被击打为Perfect 请确保 0 < perfectTime < missTime

	public StagePlayMod PlayMod {
		get {
			return playMod;
		}
		set {
			playMod = value;
		}
	}


	[SerializeField]
	private int speedScale = 4;	
	[SerializeField]
	private float noteSpeed = 0.05f;	//音符下落速度，程序内部使用，用Editor的Inspector里修改，不要用代码修改
	[SerializeField]
	private float startRotDistance = 8f;
	[SerializeField]
	private float startMoveDistance = 4f;
	[SerializeField]
	private float musicLoadingMaxTime = 20f;
	[SerializeField]
	private float trackBackPosY = 0f;
	[SerializeField]
	private Color[] noteColors;	
	[SerializeField]
	private Color[] stringColors;
	[SerializeField]
	private Color stringDarkColor;	
	[SerializeField]
	private Transform noteHolderTF;
	[SerializeField]
	private Transform trackBack;
	[SerializeField]
	private Transform mainCamera;
	[SerializeField]
	private Vector3 tracksPivot;
	[SerializeField]
	private Vector2 tracksGap;
	[SerializeField]
	private SpriteRenderer[] strings;
	[SerializeField]
	private SpriteRenderer[] stringLights;
	[SerializeField]
	private float missTime;
	[SerializeField]
	private float perfectTime;
	[SerializeField]
	private StagePlayMod playMod = StagePlayMod.RealGuitar;


	/// <summary>
	/// 获取轨道的世界位置
	/// </summary>
	/// 
	/// <param name="trackID">
	/// 0 表示 第1品  23 表示 第24品
	/// </param>
	/// 
	/// <param name="stringID">
	/// 0 表示 最上面的琴弦  5 表示最下面的琴弦
	/// </param>
	/// 
	/// <returns>
	/// 获取到的位置
	/// </returns>
	public Vector3 TrackPos (int trackID = 0, int stringID = 0) {
		return new Vector3(
			tracksPivot.x + trackID * tracksGap.x,
			tracksPivot.y + stringID * tracksGap.y,
			tracksPivot.z
		);
	}



}




}