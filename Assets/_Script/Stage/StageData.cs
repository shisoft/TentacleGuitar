namespace TentacleGuitarUnity {

using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum StagePlayMod {
	Auto = 0,
	Mouse = 1,
	RealGuitar = 2,
}
	

[System.Serializable]
public class StageSetting {


	#region --- Note ---


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

	public float ShowNoteTime {
		get {
			return showNoteTime;
		}
	}	  // 提前多少秒显示音符，位于临界点附近时会淡入淡出

	public Color[] NoteColors {
		get {
			return noteColors;
		}
	}	// 音符 发光时的颜色，0是最上面的。

	
	#endregion


	#region --- String ---


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


	#endregion


	#region --- Track ---


	public SpriteRenderer[] TrackHighLights {
		get {
			return trackHighLights;
		}
	}		// 轨道高亮的渲染器

	public Color TrackHightLightColor {
		get {
			return trackHightLightColor;
		}
	}	  // 轨道高亮时的颜色

	public float TrackBackPosY {
		get {
			return trackBack.position.y;
		}
	}   // 轨道底盘的Y坐标


	#endregion


	#region --- Camera ---


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
			trackBack.position = new Vector3(value.x, trackBack.position.y, trackBack.position.z);
		}
	}	 // 摄影机的位置

	public Quaternion CameraRot {
		get {
			return MainCamera.rotation;
		}
		set {
			MainCamera.rotation = value;
		}
	}	 // 摄影机的角度


	#endregion


	#region --- Game Setting ---

	public float MusicLoadingMaxTime {
		get {
			return musicLoadingMaxTime;
		}
	}	 // 等待音乐载准备的时间（s），超出这个时间则宣告音乐载入失败。不要用代码修改。

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
	}	// 游戏模式，自动演示 / 鼠标 / 乐器 / 无

	#endregion


	// Note
	[Space(20f)]
	[SerializeField]
	[Range(1f, 8f)]
	private int speedScale = 4;	
	[SerializeField]
	private float noteSpeed = 3f;	//音符下落速度，程序内部使用，用Editor的Inspector里修改，不要用代码修改
	[SerializeField]
	private float startRotDistance = 8f;
	[SerializeField]
	private float startMoveDistance = 4f;
	[SerializeField]
	private float showNoteTime = 3f;
	[SerializeField]
	private Color[] noteColors;	

	// String
	[Space(20f)]
	[SerializeField]
	private Color stringDarkColor;	
	[SerializeField]
	private Color[] stringColors;
	[SerializeField]
	private SpriteRenderer[] strings;
	[SerializeField]
	private SpriteRenderer[] stringLights;
	

	// Track
	[Space(20f)]
	[SerializeField]
	private Transform trackBack;
	[SerializeField]
	private Vector3 tracksPivot;
	[SerializeField]
	private Vector2 tracksGap;
	[SerializeField]
	private Color trackWireNormalColor;
	[SerializeField]
	private Color trackWireLightColor;
	[SerializeField]
	private Color trackHightLightColor;
	[SerializeField]
	private SpriteRenderer[] trackWires;
	[SerializeField]
	private SpriteRenderer[] trackHighLights;
	

	// Camera
	[Space(20f)]
	[SerializeField]
	private Transform mainCamera;
	

	// GameSetting
	[Space(20f)]
	[SerializeField]
	private float musicLoadingMaxTime = 20f;
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