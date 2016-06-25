namespace TentacleGuitarUnity {

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathologicalGames;
using CodeStage.AntiCheat.ObscuredTypes;

//http://tentacleguitar.azurewebsites.net/

public class Stage : MonoBehaviour {



	#region -------- Param --------

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

	public static SpawnPool SongCardPool {
		get {
			if (!songCardPool) {
				songCardPool = PoolManager.Pools["SongCard"];
			}
			return songCardPool;
		}
	}
	private static SpawnPool songCardPool = null;

	// Quick Cache
	public static float Time {
		get {
			return GamePlaying ? StageMusic.Main.Time : UnityEngine.Time.time;
		}
	}
	public static string CurrentSongID = "";

	// Data Stuff
	[SerializeField]
	private StageSetting stageSetting;

	// Logic
	public static bool Loading = false;
	public static bool GamePlaying = false;
	public static bool IsSignedIn = false;
	public static Dictionary<string, SongCard> SongCards = new Dictionary<string, SongCard>();
	public static bool NoSongIsDownLoading {
		get {
			bool flag = true;
			foreach (KeyValuePair<string, SongCard> sc in SongCards) {
				if (sc.Value && sc.Value.IsDownloading) {
					flag = false;
					break;
				}
			}
			return flag;
		}
	}

	// Lerp Animation 
	private Vector3[] StringAimPos = new Vector3[6];
	private float StringShackRant = 0.8f;
	private Vector3 CameraAimPos;
	private Quaternion CameraAimRot;
	private float CameraMoveRant = 0.1f;
	private float CameraRotRant = 0.1f;
	private Color[] StringAimColors = new Color[6];
	private Color[] TrackHightLightAimColors = new Color[24];
	private Color[] FretWireLightAimColors = new Color[25];
	private float StringColorRant = 0.1f;
	private float TrackHightLightColorRant = 0.4f;
	private float FretWireLightColorRant = 0.1f;
	private int ShowingScore = 0;

	#endregion


	#region -------- Mono --------


	void Awake () {
		Main = this;
		TheStageSetting.UserName = ObscuredPrefs.GetString("UserName", "");
		TheStageSetting.SpeedScale = PlayerPrefs.GetInt("SpeedScale", 4);
		InitAimTransform();
		for (int i = 0; i < 6; i++) {
			Main.StringAimPos[i] = TheStageSetting.StringTFs[i].position;
		}
		TheStageSetting.AutoPlay = true;
		StageScore.Close();
		StageMusic.StopToEndCallback += this.OnMusicEnd;
		BeatMapManager.StageAwake();
		InputManager.StageAwake();
	}


	void Start () {
		ObscuredString token = ObscuredPrefs.GetString("LoginToken", "");
		if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(TheStageSetting.UserName)) {
			int year = ObscuredPrefs.GetInt("TokenYear", 0);
			int day = ObscuredPrefs.GetInt("TokenDay", 0);
			int time = (System.DateTime.Now.Year - year) * 365 + System.DateTime.Now.Day - day;
			if (time < 15) {
				IsSignedIn = true;
				StartCoroutine(NetworkManager.TryLoadSongListInfo());
			}
		}
	}


	void Update () {


		///*
		if (GamePlaying && StageMicrophone.IsReady) {
				float[] f = StageMicrophone.GetData(StageMicrophone.SamplePosition() - StageMicrophone.sampleRate, StageMicrophone.sampleRate);
			for (int i = 0; i < f.Length - 1; i++) {
				Debug.DrawLine(
					new Vector3((float)(i-1), f[i] * 10000f, 0f),
					new Vector3((float)i, f[i+1] * 10000f, 0f),
					Color.red
				);
			}
				StageMicrophone.Main.Source.time = Mathf.Max(((float)StageMicrophone.SamplePosition() / StageMicrophone.sampleRate) - 0.02f, 0f);
		}
		//*/



		// Ani

		TheStageSetting.MenuAni.SetBool("GamePlaying", GamePlaying);
		TheStageSetting.MainBtnAni.SetBool("SignedIn", IsSignedIn);

		// Menu Random Note

		if (!GamePlaying) {
			RandomAddNote(
				Time + TheStageSetting.ShowNoteTime,
				0, 24, 0, 6, 0f, 0.9f
			);
		}
		
		// Aim Movement

		int cameraID = (int)TheStageSetting.TrackID(TheStageSetting.CameraPos).x;
		for (int i = 0; i < TheStageSetting.FretSigns.Length; i++) {
			TheStageSetting.FretSigns[i].color = Color.Lerp(
				TheStageSetting.FretSigns[i].color,
				GamePlaying && Mathf.Abs(cameraID - i) < 4 ? TheStageSetting.FretSignColor : Color.clear,
				0.1f
			);
		}

		ShowingScore = StageScore.CurrentScore >= StageScore.FullScore ?
			(int)StageScore.FullScore :
			(int)Mathf.Lerp(
				ShowingScore,
				(GamePlaying ? StageScore.CurrentScore : 0),
				0.9f
			);
		TheStageSetting.ScoreTXT = ShowingScore;
		
		TheStageSetting.TitleBGM.volume = Mathf.Lerp(
			TheStageSetting.TitleBGM.volume, 
			GamePlaying ? 0f: 1f, 
			0.02f
		);

		if (GamePlaying && TheStageSetting.TitleBGM.volume < 0.01f) {
			TheStageSetting.TitleBGM.Pause();
		}

		TheStageSetting.TrackBackSR.color = Color.Lerp(
			TheStageSetting.TrackBackSR.color,
			GamePlaying ? TheStageSetting.TrackBackColor : Color.clear,
			0.01f
		);

		TheStageSetting.GuitarBody.color = TheStageSetting.GuitarHead.color = Color.Lerp(
			TheStageSetting.GuitarBody.color,
			GamePlaying ? Color.clear : TheStageSetting.GuitarColor,
			0.01f
		); 

		TheStageSetting.MainCamera.localPosition = Vector3.Lerp(
			TheStageSetting.MainCamera.localPosition,
			GamePlaying ? TheStageSetting.CameraPlayingPos : TheStageSetting.CameraMenuPos,
			0.16f
		);

		TheStageSetting.MainCamera.localRotation = Quaternion.Lerp(
			TheStageSetting.MainCamera.localRotation,
			GamePlaying ? TheStageSetting.CameraPlayingRot : TheStageSetting.CameraMenuRot,
			0.16f
		);

		TheStageSetting.CameraPos = Vector3.Lerp(TheStageSetting.CameraPos, CameraAimPos, CameraMoveRant);
		TheStageSetting.CameraRot = Quaternion.Lerp(TheStageSetting.CameraRot, CameraAimRot, CameraRotRant);
		
		for (int i = 0; i < 6; i++) {
			TheStageSetting.Strings[i].color = Color.Lerp(TheStageSetting.Strings[i].color, StringAimColors[i], StringColorRant);
			TheStageSetting.StringLights[i].color = Color.Lerp(TheStageSetting.Strings[i].color, StringAimColors[i], StringColorRant);
		}
		
		for (int i = 0; i < 24; i++) {
			TheStageSetting.TrackHighLights[i].color = Color.Lerp(TheStageSetting.TrackHighLights[i].color, TrackHightLightAimColors[i],TrackHightLightColorRant);
		}
		
		for (int i = 0; i < 6; i++) {
			Vector3 pos = StringAimPos[i] + (StringAimPos[i] - TheStageSetting.StringTFs[i].position) * StringShackRant;
			TheStageSetting.StringTFs[i].position = pos;
		}
		
		for (int i = 0; i < 25; i++) {
			TheStageSetting.FretWireLights[i].color = Color.Lerp(TheStageSetting.FretWireLights[i].color, FretWireLightAimColors[i], FretWireLightColorRant);
		}

		// Msg
		InputManager.StageUpdate();

	}


	void FixedUpdate () {
		BeatMapManager.StageFixedUpdate();
		InputManager.StageFixedUpdate();
	}


	void LateUpdate () {
		BeatMapManager.StageLateUpdate();
	}


	#endregion


	#region -------- UI ---------


	public void StartLogin () {
		if (!Loading && Main && !GamePlaying) {
			Main.StopAllCoroutines();
			Loading = true;
			TheStageSetting.LoginUIInteractable = false;
			StartCoroutine(NetworkManager.TryLogin(
				TheStageSetting.UserName,
				TheStageSetting.Password
			));
		} else {
			Debug.LogWarning("Loading Now. Can not start login.");
		}
	}


	public void GotoSignUp () {
		if (!Loading && !GamePlaying) {
			Application.OpenURL(TheStageSetting.SignupURL);
		}
	}


	public void StartSignOut () {
		if (!Loading && Main && !GamePlaying) {
			Main.StopAllCoroutines();
			TheStageSetting.LoginUIInteractable = true;
			IsSignedIn = false;
		} else {
			Debug.LogWarning("Loading Now. Can not start logout.");
		}
	}


	public void SetSpeedScale () {
		TheStageSetting.SpeedScale = TheStageSetting.SpeedScale >= 8 ? 1 : TheStageSetting.SpeedScale + 1;
	}


	public static void TryStartSong (string id) {
		if (!Main || Loading) { 
			return; 
		}
		if (NetworkManager.SongIsDownLoaded(id)) {
			StartGame(
				NetworkManager.GetSongLocalPath(id),
				NetworkManager.GetBeatMapLocalPath(id)
			);
			CurrentSongID = id;
		} else if (Main) {
			if (SongCards.ContainsKey(id) && SongCards[id] && !SongCards[id].IsDownloading) {
				SongCards[id].IsDownloading = true;
				Main.StartCoroutine(NetworkManager.TryDownloadSong(id));
			}
		}
	}


	

	


	#endregion


	#region -------- Public --------


	#region --- Game ---


	public static void StartGame (string songPath, string beatMapPath) {
		// Start Load
		if (!Loading && Main) {
			if (NoSongIsDownLoading) {
				InitAimTransform();
				Main.StopAllCoroutines();
				Main.StartCoroutine(Main.LoadLevel(songPath, beatMapPath));
			} else {
				Debug.LogWarning("Can not start a level now! Song(s) is loading...");
			}
		} else {
			Debug.LogWarning("Can not start a level now! Another level is loading...");
		}
		
	}


	public static void StopGame () {
		if (Loading || !Main) {
			return;
		}
		StageMusic.Main.Clear();
		StageMicrophone.MicrophoneStop();
		BeatMapManager.ClearBeatMap();
		GamePlaying = false;
		TheStageSetting.TitleBGM.UnPause();
		InitAimTransform();
		RemoveAllNotes();
		TheStageSetting.AutoPlay = true;
		StageScore.Close();
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
	public static void MoveCamera (int trackID, float rant = 0.1f) {
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
	public static void MoveCameraHeight (float y, float rant = 0.1f) {
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
	public static void MoveCamera (Vector3 pos, float rant = 0.1f) {
		if (!Main) {
			return;
		}
		Main.CameraMoveRant = Mathf.Clamp01(rant);
		Main.CameraAimPos = pos;
	}

	/// <summary>
	/// 旋转摄影机
	/// </summary>
	/// <param name="rot"> 角度，欧拉角 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void RotCamera (Vector3 rot, float rant = 0.1f) {
		if (!Main) {
			return;
		}
		rot = new Vector3(
			Mathf.Clamp(rot.x, -26f, 16f),
			Mathf.Clamp(rot.y, -26f, 26f),
			Mathf.Clamp(rot.z, -6f, 6f)
		);
		Main.CameraRotRant = Mathf.Clamp01(rant);
		Main.CameraAimRot = Quaternion.Euler(rot);
	}


	#endregion


	#region --- Note ---


	public static void RandomAddNote (float time, int minFret, int maxFret, int minString, int maxString, float minType, float maxType) {

		time = Mathf.Max(time, 0f);
		minFret = Mathf.Clamp(minFret, 0, 24);
		maxFret = Mathf.Clamp(maxFret, 0, 24);
		minString = Mathf.Clamp(minString, 0, 6);
		maxString = Mathf.Clamp(maxString, 0, 6);
		minType = Mathf.Clamp(minType, 0f, 3.99f);
		maxType = Mathf.Clamp(maxType, 0f, 3.99f);

		Stage.AddNote(new NoteInfo(
			Random.Range(minFret, maxFret),
			Random.Range(minString, maxString),
			time,
			(NoteInfo.NoteType)Random.Range(minType, maxType)
		));

	}



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


	public static void RemoveAllNotes (NoteState state) {
		int len = NotePool.Count;
		for (int i = 0; i < len; i++) {
			Note note = NotePool[i].GetComponent<Note>();
			if (note && note.State == state) {
				NotePool.DespawnToDefault(NotePool[i]);
				i--;
				len--;
			}
		}
	}


	#endregion


	#region --- Player Input ---


	public static void Beat (int fret, int stringID, float time = -1f) {
		int len = NotePool.Count;
		for (int i = 0; i < len; i++) {
			Note note = NotePool[i].GetComponent<Note>();
			if (note) {
				if (note.Y != stringID) {
					continue;
				}
				if (note.X != fret && note.Type != NoteInfo.NoteType.Zero) {
					continue;
				}
				if (time < 0f) {
					time = StageMusic.Main.Time;
				}
				if (note.Time > time + TheStageSetting.MissTime) {
					break;
				} else {
					NoteState state = note.Beat(time);
					if (state != NoteState.None && state != NoteState.Holding) {
						StageScore.AddScore(state);
						break;
					}
				}
			}
		}
	}


	public static void Hold (int fret, int stringID) {



	}


	#endregion


	#region --- String ---

	/// <summary>
	/// 让指定编号的琴弦发光（默认颜色）或暗淡
	/// </summary>
	/// <param name="stringID"> 琴弦编号 0 - 5 对应 最上面 - 最下面 </param>
	/// <param name="light"> 发光还是暗淡 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetStringLight (int stringID, bool light, bool blink = false, float rant = 0.1f) {
		SetStringLight(stringID, light ? TheStageSetting.StringColors[stringID] : TheStageSetting.StringDarkColor, blink, rant);
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
	public static void SetStringLight (int stringID, float r, float g, float b, float a, bool blink = false, float rant = 0.1f) {
		SetStringLight(stringID, new Color(r, g, b, a), blink, rant);
	}

	/// <summary>
	/// 和上面的函数一样，只是把rgba改成直接用Unity的Color类
	/// </summary>
	/// <param name="stringID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public static void SetStringLight (int stringID, Color color, bool blink = false, float rant = 0.1f) {
		if (!Main) {
			return;
		}
		Main.StringColorRant = Mathf.Clamp01(rant);
		stringID = Mathf.Clamp(stringID, 0, 5);
		if (blink) {
			TheStageSetting.Strings[stringID].color = color;
			TheStageSetting.StringLights[stringID].color = color;
		} else {
			Main.StringAimColors[stringID] = color;
		}
		
	}


	public static void ShackString (int stringID, float shackScale, float rant = 0.9f) {
		if (!Main) {
			return;
		}
		Main.StringShackRant = Mathf.Clamp01(rant);
		stringID = Mathf.Clamp(stringID, 0, 5);
		Vector3 pos = Main.StringAimPos[stringID];
		pos.y += shackScale;
		TheStageSetting.StringTFs[stringID].position = pos;
	}


	#endregion


	#region --- Track ---

	/// <summary>
	/// 控制指定轨道是否加亮，亮光颜色为默认颜色
	/// </summary>
	/// <param name="trackID"> 轨道的ID，0-23 对应 1-24 品 </param>
	/// <param name="light"> 是否加亮 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetTrackHightLight (int trackID, bool light, bool blink = false, float rant = 0.1f) {
		SetTrackHightLight(trackID, light ? TheStageSetting.TrackHightLightColor : Color.clear, blink, rant);
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
	public static void SetTrackHightLight (int trackID, float r, float g, float b, float a, bool blink = false, float rant = 0.1f) {
		SetTrackHightLight(trackID, new Color(r, g, b, a), blink, rant);
	}

	/// <summary>
	/// 同上，把rgba改成Unity的Color
	/// </summary>
	/// <param name="trackID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public static void SetTrackHightLight (int trackID, Color color, bool blink = false, float rant = 0.1f) {
		if (!Main) {
			return;
		}
		Main.TrackHightLightColorRant = Mathf.Clamp01(rant);
		trackID = Mathf.Clamp(trackID, 0, 23);
		if (blink) {
			TheStageSetting.TrackHighLights[trackID].color = color;
		} else {
			Main.TrackHightLightAimColors[trackID] = color;
		}
		
	}


	#endregion


	#region --- FretWireLight ---


	/// <summary>
	/// 让指定编号的品弦发光（默认颜色）或暗淡
	/// </summary>
	/// <param name="stringID"> 品弦编号 0 - 24 对应 最左边 - 最右边 </param>
	/// <param name="light"> 发光还是暗淡 </param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetFretWireLight (int fretWireID, bool light, bool blink = false, float rant = 0.1f) {
		SetFretWireLight(fretWireID, light ? TheStageSetting.FretWireLightBloomColor : TheStageSetting.FretWireLightNormalColor, blink, rant);
	}


	/// <summary>
	/// 通过rbga数值调整指定编号的品弦的颜色
	/// </summary>
	/// <param name="stringID"> 品弦编号（同上） </param>
	/// <param name="r"> 该颜色的红色通道，下面同理 </param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <param name="a"></param>
	/// <param name="rant"> 缓动率 </param>
	public static void SetFretWireLight (int fretWireID, float r, float g, float b, float a, bool blink = false, float rant = 0.1f) {
		SetFretWireLight(fretWireID, new Color(r, g, b, a), blink, rant);
	}


	/// <summary>
	/// 和上面的函数一样，只是把rgba改成直接用Unity的Color类
	/// </summary>
	/// <param name="stringID"></param>
	/// <param name="color"></param>
	/// <param name="rant"></param>
	public static void SetFretWireLight (int fretWireID, Color color, bool blink = false, float rant = 0.1f) {
		if (!Main) {
			return;
		}
		Main.FretWireLightColorRant = Mathf.Clamp01(rant);
		fretWireID = Mathf.Clamp(fretWireID, 0, 24);
		if (blink) {
			TheStageSetting.FretWireLights[fretWireID].color = color;
			TheStageSetting.FretWireLights[fretWireID].color = color;
		} else {
			Main.FretWireLightAimColors[fretWireID] = color;
		}
	}



	#endregion


	#region --- NetWork ---


	public static void LoginDone (bool success, bool cleanPassword = false, string message = "") {
		Loading = false;
		if (success) {
			IsSignedIn = true;
			ObscuredPrefs.SetString("UserName", TheStageSetting.UserName);
			ObscuredPrefs.SetInt("TokenYear", System.DateTime.Now.Year);
			ObscuredPrefs.SetInt("TokenDay", System.DateTime.Now.DayOfYear);
			Main.StartCoroutine(NetworkManager.TryLoadSongListInfo());
		} else {
			Debug.LogError(message);
			TheStageSetting.LoginUIInteractable = true;
			if (Main) {
				Main.StopAllCoroutines();
			}
		}
	}




	public static void LoadSongListDone (bool success, string message = "") {
		Loading = false;
		SongCardPool.DespawnAllToDefault();
		SongCards.Clear();
		if (success) {
			List<Assets.ExternalCode.Models.Music> infos = NetworkManager.SongList;
			for (int i = 0; infos != null && i < infos.Count; i++) {
				Transform tf = SongCardPool.SpawnToDefault("SongCard");
				tf.localScale = Vector3.one;
				SongCard sc = tf.GetComponent<SongCard>();
				if (sc) {
					sc.Init(
						infos[i].Id.ToString(),
						infos[i].Title,
						infos[i].Level,
						NetworkManager.SongIsDownLoaded(infos[i].Id.ToString())
					);
					SongCards.Add(infos[i].Id.ToString(), sc);
				}
			}
		} else {
			Debug.LogError(message);
			if (Main) {
				Main.StopAllCoroutines();
			}
		}
	}


	public static void DownLoadDone (bool success, string id, string message = "") {
		Loading = false;
		if (SongCards.ContainsKey(id) && SongCards[id]) {
			SongCards[id].IsDownloading = false;
		}
		if (success) {
			if (SongCards.ContainsKey(id)) {
				SongCards[id].IsDownloaded = true;
			}
		} else {
			Debug.LogError(message);
		}
	}


	public static void SetDownLoadProgress (string id, float progress) {
		if (SongCards.ContainsKey(id) && SongCards[id] != null) {
			SongCards[id].Progress = progress;
		}
	}


	#endregion


	#endregion



	#region -------- Logic --------


	private IEnumerator LoadLevel (string songPath, string beatMapPath) {

		Loading = true;

		StageMusic.Main.Stop();

		RemoveAllNotes(NoteState.None);
		StageScore.Close();

		HoldOn.Show();
		HoldOn.SetProgress(0f / 2f);

		// Load Music
		yield return LoadSong(songPath);
		HoldOn.SetProgress(1f / 2f);

		// Load Beat Map
		yield return LoadBeatMap(beatMapPath);
		HoldOn.SetProgress(2f / 2f);

		HoldOn.Hide();
		Resources.UnloadUnusedAssets();

		TheStageSetting.AutoPlay = TheStageSetting.AutoPlayIsOn;

		GamePlaying = true;

		yield return new WaitForSeconds(0.5f);
		RemoveAllNotes();
		yield return new WaitForSeconds(0.5f);


		// Ready -- Go --
		
		StageMusic.Main.Play();
		StageMicrophone.MicrophoneStart(Mathf.CeilToInt(StageMusic.Main.Length));

		Loading = false;
		yield return null;
	}



	private IEnumerator LoadSong (string path) {

		// Check
		if (!File.Exists(path)) {
			yield return LoadLevelFail("SongFile Path is NOT exists! " + path);
		}

		
		///*Mp3 --> Wav
		//#if UNITY_STANDALONE || UNITY_EDITOR

		if (Path.GetExtension(path) == ".mp3") {
			string wavPath = Path.ChangeExtension(path, ".wav");
			bool success = true;
			if (!File.Exists(wavPath)) {
				success = AudioConverter.MP3toWAV(path);
			}
			if (success) {
				path = Path.ChangeExtension(path, ".wav");
			} else {
				yield return LoadLevelFail("Failed to Convert .wav file from: " + path);
			}
		}

		//#endif
		//*/

		// Load
		StageMusic.Main.Clear();
		WWW _www = new WWW(FileUtility.GetFileURL(path));
		yield return _www;
		if (_www.error == null) {
			AudioClip _songClip = _www.GetAudioClipCompressed(false);
			_songClip.name = Path.GetFileNameWithoutExtension(path);
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
				yield return LoadLevelFail("SongFile loading out of time! " + path);
			}
		} else {
			_www.Dispose();
			yield return LoadLevelFail("SongFile can NOT be load! " + path);
		}
		

		yield return null;
	}



	private IEnumerator LoadBeatMap (string path) {
		// Check
		if (!File.Exists(path)) {
			yield return LoadLevelFail("BeatMapFile Path is NOT exists! " + path);
		}

		// Load
		BeatMapManager.ClearBeatMap();
		bool success = BeatMapManager.LoadBeatMap(path);
		if (!success) {
			yield return LoadLevelFail("Failed to load beatMap! " + path);
		}
		StageScore.Open(BeatMapManager.GetNoteSum());

		yield return null;
	}



	private IEnumerator LoadLevelFail (string msg) {
		HoldOn.Hide();
		Debug.LogError(msg);
		StopAllCoroutines();
		Loading = false;
		GamePlaying = false;
		StageMusic.Main.Clear();
		BeatMapManager.ClearBeatMap();
		Resources.UnloadUnusedAssets();
		yield return null;
	}


	private static void SortNotePool () {
		NotePool.Sort(NoteTimeSorter.TheNoteTimeSorter);
	}


	private void OnMusicEnd () {
		RemoveAllNotes();
		TheStageSetting.TitleBGM.UnPause();
		TheStageSetting.AutoPlayTagOn = TheStageSetting.AutoPlay;
		if (TheStageSetting.AutoPlay) {
			NetworkManager.TryUploadResult(CurrentSongID, (int)StageScore.CurrentScore, StageScore.MaxCombo);
		}
		TheStageSetting.AutoPlay = true;
		TheStageSetting.ResultScore = (int)StageScore.CurrentScore;
		TheStageSetting.ResultPerfect = StageScore.PerfectNum;
		TheStageSetting.ResultGood = StageScore.GoodNum;
		TheStageSetting.ResultMiss = StageScore.MissNum;
		TheStageSetting.ResultMaxCombo = StageScore.MaxCombo;
		GamePlaying = false;
	}


	private static void InitAimTransform () {
		Main.CameraAimPos = TheStageSetting.DefaultCameraPos;
		Main.CameraAimRot = TheStageSetting.DefaultCameraRot;
		TheStageSetting.StringColors.CopyTo(Main.StringAimColors, 0);
		Main.TrackHightLightAimColors.Initialize();
		for (int i = 0; i < 24; i++) {
			SetTrackHightLight(i, false);
		}
		for (int i = 0; i < 25; i++) {
			SetFretWireLight(i, false);
		}
		
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