namespace TentacleGuitarUnity {

using UnityEngine;
using System.Collections;
using System.IO;
using PathologicalGames;



public class Stage : MonoBehaviour {

	// Instance
	public static Stage Main;
	public static StageSetting TheStageSetting {
		get {
			return Main.stageSetting;
		}
	}

	// Quick Cache
	public static float Time {
		get {
			return StageMusic.Main.Time;
		}
	}

	// Data Stuff
	[SerializeField]
	private StageSetting stageSetting;

	// Logic
	private bool LoadingLevel = false;

	// Lerp Animation 
	private Vector3 CameraAimPos;
	private Quaternion CameraAimRot;
	private float CameraMoveRant = 0.1f;
	private float CameraRotRant = 0.1f;
	private Color[] StringAimColors = new Color[6];
	private Color[] TrackHightLightAimColors = new Color[24];
	private float StringColorRant = 0.1f;
	private float TrackHightLightColorRant = 0.4f;

	#region -------- Mono --------


	void Awake () {
		Main = this;
		CameraAimPos = TheStageSetting.CameraPos;
		CameraAimRot = TheStageSetting.CameraRot;
		TheStageSetting.StringColors.CopyTo(StringAimColors, 0);
		TrackHightLightAimColors.Initialize();
		BeatMapManager.StageAwake();
	}



	void Start () {

		///* TestOnly Test Only
		StartLevel(new SongInitInfo(
			Application.dataPath + @"\Ccna — Aquarius.wav",// 音乐文件地址
			Application.dataPath + @"\Ccna — Aquarius.wav",// 谱面文件地址
			StagePlayMod.Auto,
			true
		));
		doTest = true;
		//*/

	}



	bool doTest = false;
	int currentTestTruck = 0;
	float currentTestAngle = 0f;
	void Update () {
		
		if (doTest) {

			if (Time + TheStageSetting.ShowNoteTime < StageMusic.Main.Length) {
				AddNote(new TapNoteInfo(Random.Range(0, 24), Random.Range(0, 6), Time + TheStageSetting.ShowNoteTime));
			}
			
			if (Input.GetKeyDown(KeyCode.A)) {
				currentTestTruck = Mathf.Clamp(currentTestTruck - 1, 2, 21);
				MoveCamera(currentTestTruck, 0.1f);
			}

			if (Input.GetKeyDown(KeyCode.D)) {
				currentTestTruck = Mathf.Clamp(currentTestTruck + 1, 2, 21);
				MoveCamera(currentTestTruck, 0.1f);
			}

			if (Input.GetKeyDown(KeyCode.E)) {
				currentTestAngle += 10f;
				currentTestAngle = Mathf.Clamp(currentTestAngle + 10f, -30f, 30f);
				RotCamera(new Vector3(0f, currentTestAngle, 0f), 0.1f);
			}

			if (Input.GetKeyDown(KeyCode.Q)) {
				currentTestAngle = Mathf.Clamp(currentTestAngle - 10f, -30f, 30f);
				RotCamera(new Vector3(0f, currentTestAngle, 0f), 0.1f);
			}

			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				SetStringLight(0, !Input.GetKey(KeyCode.Z));
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				SetTrackHightLight(1, !Input.GetKey(KeyCode.Z));
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)) {
				SetTrackHightLight(2, !Input.GetKey(KeyCode.Z));
			}
			if (Input.GetKeyDown(KeyCode.Alpha4)) {
				SetTrackHightLight(3, !Input.GetKey(KeyCode.Z));
			}
			if (Input.GetKeyDown(KeyCode.Alpha5)) {
				SetTrackHightLight(4, !Input.GetKey(KeyCode.Z));
			}
			if (Input.GetKeyDown(KeyCode.Alpha6)) {
				SetTrackHightLight(5, !Input.GetKey(KeyCode.Z));
			}

		}
			
		// Aim Movement
		TheStageSetting.CameraPos = Vector3.Lerp(TheStageSetting.CameraPos, CameraAimPos, CameraMoveRant);
		TheStageSetting.CameraRot = Quaternion.Lerp(TheStageSetting.CameraRot, CameraAimRot, CameraRotRant);
		for (int i = 0; i < 6; i++) {
			TheStageSetting.Strings[i].color = Color.Lerp(TheStageSetting.Strings[i].color, StringAimColors[i], StringColorRant);
			TheStageSetting.StringLights[i].color = Color.Lerp(TheStageSetting.Strings[i].color, StringAimColors[i], StringColorRant);
		}
		for (int i = 0; i < 24; i++) {
			TheStageSetting.TrackHighLights[i].color = Color.Lerp(TheStageSetting.TrackHighLights[i].color, TrackHightLightAimColors[i],TrackHightLightColorRant);
		}

	}


	void FixedUpdate () {
		BeatMapManager.StageFixedUpdate();
	}


	void LateUpdate () {
		BeatMapManager.StageLateUpdate();
	}



	#endregion



	#region -------- Public --------

	
	public void StartLevel (SongInitInfo info) {
		// Start Load
		if(!LoadingLevel){
			StopAllCoroutines();
			StartCoroutine(LoadLevel(info));
		} else {
			Debug.LogWarning("Can not start a level now! Another level is loading...");
		}
		
	}


	#region --- Camera ---

	/// <summary>
	/// 移动摄影机到指定轨道
	/// </summary>
	/// <param name="trackID"> 轨道的ID 0 - 23 对应 1 - 24 品 </param>
	/// <param name="rant"> 缓动率，1表示一下移动过去，0表示不动 </param>
	public void MoveCamera (int trackID, float rant = -1f) {
		trackID = Mathf.Clamp(trackID, 2, 21);
		MoveCamera(new Vector2(stageSetting.TrackPos(trackID).x, CameraAimPos.y), rant);
	}

	/// <summary>
	/// 移动摄影机的高度
	/// </summary>
	/// <param name="y"> 高度数值，单位unit </param>
	/// <param name="rant"> 缓动率 </param>
	public void MoveCameraHeight (float y, float rant = -1f) {
		y = Mathf.Clamp(y, 0f, 5f);
		MoveCamera(new Vector3(CameraAimPos.x, y, CameraAimPos.z), rant);
	}

	/// <summary>
	/// 移动摄影机到指定世界坐标
	/// </summary>
	/// <param name="pos"> 坐标 </param>
	/// <param name="rant"> 缓动率 </param>
	public void MoveCamera (Vector3 pos, float rant = -1f) {
		if (rant > 0f) {
			CameraMoveRant = Mathf.Clamp01(rant);
		}
		CameraAimPos = pos;
	}

	/// <summary>
	/// 旋转摄影机
	/// </summary>
	/// <param name="rot"> 角度，欧拉角 </param>
	/// <param name="rant"> 缓动率 </param>
	public void RotCamera (Vector3 rot, float rant = -1f) {
		rot = new Vector3(
			Mathf.Clamp(rot.x, -26f, 16f),
			Mathf.Clamp(rot.y, -26f, 26f),
			Mathf.Clamp(rot.z, -6f, 6f)
		);
		if (rant > 0f) {
			CameraRotRant = Mathf.Clamp01(rant);
		}
		CameraAimRot = Quaternion.Euler(rot);
	}


	#endregion


	#region --- Note ---


	/// <summary>
	/// 添加一个音符到场景中，主线程
	/// 调用的太早的话Stage.Main会为空，请使用 region -> MonoMessage 里提供的函数调用
	/// 注意：你可以在任意时间Add任意Note到Stage里，即便是在初始化时就把所有Note都Add进去，Stage也会正常运转
	/// 但是Add太多Note会影响性能，所以请在Note出现前几秒把它Add进Stage
	/// 你不需要清理过期的Note
	/// </summary>
	/// <param name="info">
	/// 这个音符的信息
	/// </param>
	public void AddNote (TapNoteInfo info) {
		Transform tf = PoolManager.Pools["Note"].SpawnToDefault("TapNote");
		TapNote tn = tf.GetComponent<TapNote>();
		if (tn) {
			tn.NoteInfo = info;
		}
	}


	public void AddNote (HoldNoteInfo info) {
		Transform tf = PoolManager.Pools["Note"].SpawnToDefault("HoldNote");
		HoldNote hn = tf.GetComponent<HoldNote>();
		if (hn) {
			hn.NoteInfo = info;
		}
	}

	/// <summary>
	/// 瞬间清除台面上的所有音符
	/// </summary>
	public void RemoveAllNotes () {
		PoolManager.Pools["Note"].DespawnAllToDefault();
	}


	#endregion


	#region --- String ---

	/// <summary>
	/// 让指定编号的琴弦发光（默认颜色）或暗淡
	/// </summary>
	/// <param name="stringID"> 琴弦编号 0 - 5 对应 最上面 - 最下面 </param>
	/// <param name="light"> 发光还是暗淡 </param>
	/// <param name="rant"> 缓动率 </param>
	public void SetStringLight (int stringID, bool light, float rant = -1) {
		SetStringLight(stringID, light ? TheStageSetting.StringColors[stringID] : TheStageSetting.StringDarkColor, rant);
	}

	/// <summary>
	/// 通过rbga数值调整指定编号的琴弦的颜色
	/// </summary>
	/// <param name="stringID"> 琴弦编号（同上） </param>
	/// <param name="r"> 该颜色的红色通道，下面同理 </param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <param name="a"></param>
	/// <param name="rant"> 缓动率 </param>
	public void SetStringLight (int stringID, float r, float g, float b, float a, float rant = -1f) {
		SetStringLight(stringID, new Color(r, g, b, a), rant);
	}

	/// <summary>
	/// 和上面的函数一样，只是把rgba改成直接用Unity的Color类
	/// </summary>
	/// <param name="stringID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public void SetStringLight (int stringID, Color color, float rant = -1f) {
		if (rant > 0) {
			StringColorRant = Mathf.Clamp01(rant);
		}
		stringID = Mathf.Clamp(stringID, 0, 5);
		StringAimColors[stringID] = color;
	}


	#endregion


	#region --- Track ---

	/// <summary>
	/// 控制指定轨道是否加亮，亮光颜色为默认颜色
	/// </summary>
	/// <param name="trackID"> 轨道的ID，0-23 对应 1-24 品 </param>
	/// <param name="light"> 是否加亮 </param>
	/// <param name="rant"> 缓动率 </param>
	public void SetTrackHightLight (int trackID, bool light, float rant = -1f) {
		SetTrackHightLight(trackID, light ? TheStageSetting.TrackHightLightColor : Color.clear, rant);
	}

	/// <summary>
	/// 同上，把默认颜色改成rgba控制的颜色
	/// </summary>
	/// <param name="trackID"></param>
	/// <param name="r"></param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <param name="a"></param>
	/// <param name="rant"></param>
	public void SetTrackHightLight (int trackID, float r, float g, float b, float a, float rant = -1f) {
		SetTrackHightLight(trackID, new Color(r, g, b, a), rant);
	}

	/// <summary>
	/// 同上，把rgba改成Unity的Color
	/// </summary>
	/// <param name="trackID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public void SetTrackHightLight (int trackID, Color color, float rant = -1f) {
		if (rant > 0) {
			TrackHightLightColorRant = Mathf.Clamp01(rant);
		}
		trackID = Mathf.Clamp(trackID, 0, 23);
		TrackHightLightAimColors[trackID] = color;
	}


	#endregion


	#endregion



	#region -------- Logic --------


	private IEnumerator LoadLevel (SongInitInfo info) {

		LoadingLevel = true;

		StageMusic.Main.Stop();

		RemoveAllNotes();
		
		HoldOn.Show();
		HoldOn.SetProgress(0f / 2f);

		// Load Music
		yield return LoadSong(info);
		HoldOn.SetProgress(1f / 2f);

		// Load Beat Map
		yield return LoadBeatMap(info);
		HoldOn.SetProgress(2f / 2f);

		HoldOn.Hide();
		Resources.UnloadUnusedAssets();

		TheStageSetting.PlayMod = info.PlayMode;

		if (info.PlayAfterLoad) {
			StageMusic.Main.Play();
		}

		LoadingLevel = false;

		yield return null;
	}



	private IEnumerator LoadSong (SongInitInfo info) {

		// Check
		if (!File.Exists(info.SongFilePath)) {
			yield return LoadLevelFail("SongFile Path is NOT exists! " + info.SongFilePath);
		}

		// Mp3 --> Wav
		if (Path.GetExtension(info.SongFilePath) == ".mp3") {
			string wavPath = Path.ChangeExtension(info.SongFilePath, ".wav");
			bool success = true;
			if (!File.Exists(wavPath)) {
				success = AudioConverter.MP3toWAV(info.SongFilePath);
			}
			if (success) {
				info.SongFilePath = Path.ChangeExtension(info.SongFilePath, ".wav");
			} else {
				yield return LoadLevelFail("Failed to Convert .wav file from: " + info.SongFilePath);
			}
		}

		// Load
		StageMusic.Main.Clear();
		WWW _www = new WWW(@"file:///" + info.SongFilePath);
		yield return _www;
		if (_www.error == null) {
			AudioClip _songClip = _www.GetAudioClipCompressed(false);
			_songClip.name = Path.GetFileNameWithoutExtension(info.SongFilePath);
			// Wait Song Load
			StageMusic.Main.Clip = _songClip;
			float waitTime = stageSetting.MusicLoadingMaxTime;
			while (!StageMusic.Main.IsReady && waitTime > 0f) {
				waitTime -= 0.01f;
				yield return new WaitForSeconds(0.01f);
			}
			_www.Dispose();
			if (waitTime <= 0f) {
				StageMusic.Main.Clip = null;
				yield return LoadLevelFail("SongFile loading out of time! " + info.SongFilePath);
			}
		} else {
			_www.Dispose();
			yield return LoadLevelFail("SongFile can NOT be load! " + info.SongFilePath);
		}
		

		yield return null;
	}



	private IEnumerator LoadBeatMap (SongInitInfo info) {
		// Check
		if (!File.Exists(info.BeatMapFilePath)) {
			yield return LoadLevelFail("BeatMapFile Path is NOT exists! " + info.BeatMapFilePath);
		}

		// Load
		BeatMapManager.ClearBeatMap();
		bool success = BeatMapManager.LoadBeatMap(info);
		if (!success) {
			yield return LoadLevelFail("Failed to load beatMap! " + info.BeatMapFilePath);
		}

		yield return null;
	}



	private IEnumerator LoadLevelFail (string msg) {
		HoldOn.Hide();
		Debug.LogError(msg);
		StopAllCoroutines();
		LoadingLevel = false;
		Resources.UnloadUnusedAssets();
		yield return null;
	}



	#endregion



}
}