namespace TentacleGuitarUnity {

using UnityEngine;
using System.Collections;
using System.IO;
using PathologicalGames;



public class Stage : MonoBehaviour {

	// Instance
	public static Stage Main;
	public static StageSetting TheStageSetting;

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
	

	#region -------- Mono --------


	void Awake () {
		Main = this;
		TheStageSetting = stageSetting;
		BeatMapManager.StageAwake();
	}



	void Start () {
		///*
		StartLevel(new SongInitInfo(
			Application.dataPath + @"\Ccna — Aquarius.mp3",
			Application.dataPath + @"\Ccna — Aquarius.mp3",// Test
			StagePlayMod.Auto,
			true
		));
		doTest = true;
		//*/
	}


	bool doTest = false;
	void Update () {
		if (doTest)
			AddNote(new TapNoteInfo(Random.Range(0, 24), Random.Range(0, 6), Time + 3f));
		
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


	public void MoveCamera (int trackID, float rant = 0.8f) {
		MoveCamera(new Vector2(stageSetting.TrackPos(trackID).x, stageSetting.CameraPos.y));
	}


	public void MoveCamera (Vector2 pos, float rant = 0.8f) {



	}


	public void MoveCameraHeight (float z, float rant = 0.8f) {



	}


	public void RotCamera (Vector2 rot, float rant = 0.8f) {



	}


	#endregion


	#region --- Note ---


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


	public void RemoveAllNotes () {
		PoolManager.Pools["Note"].DespawnAllToDefault();
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