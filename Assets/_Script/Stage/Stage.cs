namespace TentacleGuitarUnity {

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

	public static SpawnPool NotePool {
		get {
			if(!notePool){
				notePool = PoolManager.Pools["Note"];
			}
			return notePool;
		}
	}
	private static SpawnPool notePool = null;

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
	private static bool LoadingLevel = false;

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
		for (int i = 0; i < 24; i++) {
			SetTrackHightLight(i, false);
		}
		StageMusic.StopToEndCallback += this.CleanNoteOnEnd;
		BeatMapManager.StageAwake();
	}


	void Update () {
		
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


	#region --- Game ---


	public static void StartGame (SongInitInfo info) {
		// Start Load
		if(!LoadingLevel || !Main){
			Main.StopAllCoroutines();
			Main.StartCoroutine(Main.LoadLevel(info));
		} else {
			Debug.LogWarning("Can not start a level now! Another level is loading...");
		}
		
	}


	public static void StopGame () {
		StageMusic.Main.Clear();
		BeatMapManager.ClearBeatMap();
	}


	public static void PlayPauseGame () {
		StageMusic.Main.PlayPause();
	}

	public static void PauseGame () {
		StageMusic.Main.Pause();
	}


	public static void UnPauseGame () {
		StageMusic.Main.Play();
	}


	#endregion


	#region --- Camera ---

	/// <summary>
	/// 移动摄影机到指定轨道
	/// </summary>
	/// <param name="trackID"> 轨道的ID 0 - 23 对应 1 - 24 品 </param>
	/// <param name="rant"> 缓动率，1表示一下移动过去，0表示不动 </param>
	public static void MoveCamera (int trackID, float rant = -1f) {
		if (!Main) {
			return;
		}
		trackID = Mathf.Clamp(trackID, 2, 21);
		MoveCamera(new Vector2(TheStageSetting.TrackPos(trackID).x, Main.CameraAimPos.y), rant);
	}

	/// <summary>
	/// 移动摄影机的高度
	/// </summary>
	/// <param name="y"> 高度数值，单位unit </param>
	/// <param name="rant"> 缓动率 </param>
	public static void MoveCameraHeight (float y, float rant = -1f) {
		if (!Main) {
			return;
		}
		y = Mathf.Clamp(y, 0f, 5f);
		MoveCamera(new Vector3(Main.CameraAimPos.x, y, Main.CameraAimPos.z), rant);
	}

	/// <summary>
	/// 移动摄影机到指定世界坐标
	/// </summary>
	/// <param name="pos"> 坐标 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void MoveCamera (Vector3 pos, float rant = -1f) {
		if (!Main) {
			return;
		}
		if (rant > 0f) {
			Main.CameraMoveRant = Mathf.Clamp01(rant);
		}
		Main.CameraAimPos = pos;
	}

	/// <summary>
	/// 旋转摄影机
	/// </summary>
	/// <param name="rot"> 角度，欧拉角 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void RotCamera (Vector3 rot, float rant = -1f) {
		if (!Main) {
			return;
		}
		rot = new Vector3(
			Mathf.Clamp(rot.x, -26f, 16f),
			Mathf.Clamp(rot.y, -26f, 26f),
			Mathf.Clamp(rot.z, -6f, 6f)
		);
		if (rant > 0f) {
			Main.CameraRotRant = Mathf.Clamp01(rant);
		}
		Main.CameraAimRot = Quaternion.Euler(rot);
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
	public static void AddNote (NoteInfo info) {
		Transform tf = NotePool.SpawnToDefault("Note");
		Note n = tf.GetComponent<Note>();
		if (n) {
			n.NoteInfo = info;
		}
		SortNotePool();
	}


	/// <summary>
	/// 瞬间清除台面上的所有音符
	/// </summary>
	public static void RemoveAllNotes () {
		NotePool.DespawnAllToDefault();
	}



	public static void Beat (int fret, int stringID, float Time) {
		int len = NotePool.Count;
		for (int i = 0; i < len; i++) {
			Note note = NotePool[i].GetComponent<Note>();
			if (note) {
				if (note.Time > Time + TheStageSetting.MissTime) {
					break;
				} else {
					NoteState state = note.Beat(Time);
					if (state != NoteState.None && state != NoteState.Holding) {
						StageScore.AddScore(state);
						break;
					}
				}
			}
		}
	}


	public static void Hold (int fret, int stringID, float Time) {



	}



	#endregion


	#region --- String ---

	/// <summary>
	/// 让指定编号的琴弦发光（默认颜色）或暗淡
	/// </summary>
	/// <param name="stringID"> 琴弦编号 0 - 5 对应 最上面 - 最下面 </param>
	/// <param name="light"> 发光还是暗淡 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetStringLight (int stringID, bool light, float rant = -1) {
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
	public static void SetStringLight (int stringID, float r, float g, float b, float a, float rant = -1f) {
		SetStringLight(stringID, new Color(r, g, b, a), rant);
	}

	/// <summary>
	/// 和上面的函数一样，只是把rgba改成直接用Unity的Color类
	/// </summary>
	/// <param name="stringID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public static void SetStringLight (int stringID, Color color, float rant = -1f) {
		if (!Main) {
			return;
		}
		if (rant > 0) {
			Main.StringColorRant = Mathf.Clamp01(rant);
		}
		stringID = Mathf.Clamp(stringID, 0, 5);
		Main.StringAimColors[stringID] = color;
	}


	#endregion


	#region --- Track ---

	/// <summary>
	/// 控制指定轨道是否加亮，亮光颜色为默认颜色
	/// </summary>
	/// <param name="trackID"> 轨道的ID，0-23 对应 1-24 品 </param>
	/// <param name="light"> 是否加亮 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetTrackHightLight (int trackID, bool light, float rant = -1f) {
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
	public static void SetTrackHightLight (int trackID, float r, float g, float b, float a, float rant = -1f) {
		SetTrackHightLight(trackID, new Color(r, g, b, a), rant);
	}

	/// <summary>
	/// 同上，把rgba改成Unity的Color
	/// </summary>
	/// <param name="trackID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public static void SetTrackHightLight (int trackID, Color color, float rant = -1f) {
		if (!Main) {
			return;
		}
		if (rant > 0) {
			Main.TrackHightLightColorRant = Mathf.Clamp01(rant);
		}
		trackID = Mathf.Clamp(trackID, 0, 23);
		Main.TrackHightLightAimColors[trackID] = color;
	}


	#endregion


	#endregion



	#region -------- Logic --------


	private IEnumerator LoadLevel (SongInitInfo info) {

		LoadingLevel = true;

		StageMusic.Main.Stop();

		RemoveAllNotes();
		StageScore.Clear();

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
		StageScore.Init(BeatMapManager.GetNoteSum());

		yield return null;
	}



	private IEnumerator LoadLevelFail (string msg) {
		HoldOn.Hide();
		Debug.LogError(msg);
		StopAllCoroutines();
		LoadingLevel = false;
		StageMusic.Main.Clear();
		BeatMapManager.ClearBeatMap();
		Resources.UnloadUnusedAssets();
		yield return null;
	}


	private static void SortNotePool () {
		NotePool.Sort(NoteTimeSorter.TheNoteTimeSorter);
	}


	private void CleanNoteOnEnd () {
		RemoveAllNotes();
	}

	#endregion




}



public class NoteTimeSorter : IComparer<Transform> {

	public static NoteTimeSorter TheNoteTimeSorter {
		get {
			if (theNoteTimeSorter == null) {
				theNoteTimeSorter = new NoteTimeSorter();
			}
			return theNoteTimeSorter;
		}
	}
	private static NoteTimeSorter theNoteTimeSorter = null;


	public int Compare (Transform x, Transform y) {
		Note noteX = x.GetComponent<Note>();
		if (noteX) {
			Note noteY = y.GetComponent<Note>();
			if (noteY) {
				return noteX.Time.CompareTo(noteY.Time);
			} else {
				return 0;
			}
		} else {
			return 0;
		}
	}


}





}