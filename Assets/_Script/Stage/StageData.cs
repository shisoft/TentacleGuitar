namespace TentacleGuitarUnity {

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;



[System.Serializable]
public class StageSetting {

	
	#region --- Guitar ---


	public SpriteRenderer GuitarBody {
		get {
			return guitarBody;
		}
	}

	public SpriteRenderer GuitarHead {
		get {
			return guitarHead;
		}
	}

	public Color GuitarColor {
		get {
			return guitarColor;
		}
	}


	#endregion


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
			speedScaleTXT.text = "Speed x " + speedScale.ToString();
			PlayerPrefs.SetInt("SpeedScale", speedScale);
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
			return 12f / NoteSpeed;
		}
	}	  // 提前多少秒显示音符，位于临界点附近时会淡入淡出

	public Color[] NoteColors {
		get {
			return noteColors;
		}
	}	// 音符 发光时的颜色，0是最上面的。

	
	#endregion


	#region --- String ---

	public Transform[] StringTFs {
		get {
			return stringTFs;
		}
	}	 // 琴弦的TF

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

	public SpriteRenderer TrackBackSR {
		get {
			return trackBackSR;
		}
	}

	public Color TrackBackColor {
		get {
			return trackBackColor;
		}
	}

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


	#region --- Fret ---

	public SpriteRenderer[] FretWireLights {
		get {
			return fretWireLights;
		}
	}

	public Color FretWireLightNormalColor {
		get {
			return fretWireLightNormalColor;
		}
	}

	public Color FretWireLightBloomColor {
		get {
			return fretWireLightBloomColor;
		}
	}

	public TextMesh[] FretSigns {
		get {
			return fretSigns;
		}
	}

	public Color FretSignColor {
		get {
			return fretSignColor;
		}
	}


	#endregion


	#region --- Camera ---


	public Transform MainCameraHolder {
		get {
			return mainCameraHolder;
		}
	}		// 摄影机容器

	public Transform MainCamera {
		get {
			if (!mainCamera) {
				mainCamera = Camera.main.transform;
			}
			return mainCamera;
		}
	}		// 摄影机

	public Vector3 CameraPos {
		get {
			return MainCameraHolder.position;
		}
		set {
			MainCameraHolder.position = value;
			trackBack.position = new Vector3(value.x, trackBack.position.y, trackBack.position.z);
		}
	}	 // 摄影机的位置

	public Quaternion CameraRot {
		get {
			return MainCameraHolder.rotation;
		}
		set {
			MainCameraHolder.rotation = value;
		}
	}	 // 摄影机的角度

	public Vector3 DefaultCameraPos {
		get {
			return new Vector3(4.6f, 0f, 0f);
		}
	}

	public Quaternion DefaultCameraRot {
		get {
			return Quaternion.identity;
		}
	}

	public Vector3 CameraPlayingPos {
		get {
			return cameraPlayingPos;
		}
	}

	public Vector3 CameraMenuPos {
		get {
			return cameraMenuPos;
		}
	}

	public Quaternion CameraPlayingRot {
		get {
			return Quaternion.Euler(cameraPlayingRot);
		}
	}

	public Quaternion CameraMenuRot {
		get {
			return Quaternion.Euler(cameraMenuRot);
		}
	}


	#endregion


	#region --- Game ---

	public AudioSource TitleBGM {
		get {
			return titleBGM;
		}
	}

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

	public bool AutoPlay {
		get {
			return autoPlay;
		}
		set {
			autoPlay = value;
		}
	}	// 是否自动演示

	public string SignupURL {
		get {
			return signupURL;
		}
	}


	#endregion


	#region --- UI ---

	public bool AutoPlayIsOn {
		get {
			return autoPlayTG.isOn;
		}
	}

	public int ScoreTXT {
		set {
			scoreTXT.text = value.ToString("0000000");
		}
	}

	public Animator MenuAni {
		get {
			return menuAni;
		}
	}

	public Animator MainBtnAni {
		get {
			return mainBtnAni;
		}
	}

	public ObscuredString UserName {
		get {
			return userNameField.text;
		}
		set {
			userNameField.text = value;
		}
	}

	public ObscuredString Password {
		get {
			return passwordField.text;
		}
	}

	public bool LoginUIInteractable {
		set {
			for (int i = 0; i < loginUIStuff.Length; i++) {
				loginUIStuff[i].interactable = value;
			}
		}
	}

	public bool AutoPlayTagOn {
		set {
			autoPlayTag.gameObject.SetActive(value);
		}
	}

	public int ResultScore {
		set {
			resultScore.text = value.ToString("0000000");
		}
	}

	public int ResultPerfect {
		set {
			resultPerfect.text = value.ToString("00");
		}
	}

	public int ResultGood {
		set {
			resultGood.text = value.ToString("00");
		}
	}

	public int ResultMiss {
		set {
			resultMiss.text = value.ToString("00");
		}
	}

	public int ResultMaxCombo {
		set {
			resultMaxCombo.text = value.ToString("00");
		}
	}



	#endregion


	// Guitar
	[SerializeField]
	private SpriteRenderer guitarBody;
	[SerializeField]
	private SpriteRenderer guitarHead;
	[SerializeField]
	private Color guitarColor;


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
	private Color[] noteColors;	


	// String
	[Space(20f)]
	[SerializeField]
	private Color stringDarkColor;
	[SerializeField]
	private Transform[] stringTFs;
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
	private Color trackBackColor;
	[SerializeField]
	private SpriteRenderer trackBackSR;
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


	// Fret
	[Space(20f)]
	[SerializeField]
	private Color fretWireLightNormalColor;
	[SerializeField]
	private Color fretWireLightBloomColor;
	[SerializeField]
	private Color fretSignColor;
	[SerializeField]
	private SpriteRenderer[] fretWireLights;
	[SerializeField]
	private TextMesh[] fretSigns;

	// Camera
	[Space(20f)]
	[SerializeField]
	private Transform mainCameraHolder;
	private Transform mainCamera;
	[SerializeField]
	private Vector3 cameraPlayingPos;
	[SerializeField]
	private Vector3 cameraMenuPos;
	[SerializeField]
	private Vector3 cameraPlayingRot;
	[SerializeField]
	private Vector3 cameraMenuRot;


	// Game
	[Space(20f)]
	[SerializeField]
	private AudioSource titleBGM;
	[SerializeField]
	private float musicLoadingMaxTime = 20f;
	[SerializeField]
	private float missTime;
	[SerializeField]
	private float perfectTime;
	[SerializeField]
	private bool autoPlay = false;
	[SerializeField]
	private string signupURL = "";


	// UI
	[Space(20f)]
	[SerializeField]
	private Toggle autoPlayTG;
	[SerializeField]
	private Text speedScaleTXT;
	[SerializeField]
	private Text scoreTXT;
	[SerializeField]
	private Animator menuAni;
	[SerializeField]
	private Animator mainBtnAni;
	[SerializeField]
	private InputField userNameField;
	[SerializeField]
	private InputField passwordField;
	[SerializeField]
	private Selectable[] loginUIStuff;
	[SerializeField]
	private Text resultScore;
	[SerializeField]
	private Text resultPerfect;
	[SerializeField]
	private Text resultGood;
	[SerializeField]
	private Text resultMiss;
	[SerializeField]
	private Text resultMaxCombo;
	[SerializeField]
	private Transform autoPlayTag;

	
	public Vector3 TrackPos (int trackID = 0, int stringID = 0) {
		return new Vector3(
			tracksPivot.x + trackID * tracksGap.x,
			tracksPivot.y + stringID * tracksGap.y,
			tracksPivot.z
		);
	}



	public Vector2 TrackID (Vector3 pos) {
		return new Vector2(
			(pos.x - tracksPivot.x + tracksGap.x * 0.5f) / tracksGap.x,
			(pos.y - tracksPivot.y + tracksGap.y) / tracksGap.y
		);
	}


}





}