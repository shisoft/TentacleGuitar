namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;


[RequireComponent(typeof(AudioSource))]

public class StageMusic : MonoBehaviour {


	public static StageMusic Main = null;

	public AudioClip Clip {
		get {
			return audioSource.clip;
		}
		set {
			audioSource.clip = value;
			isReady = false;
			length = value ? value.length : 0f;
			Time = 0f;
		}
	}
	public float Length {
		get {
			return length > 0f ? length : Clip ? Clip.length : 0f;
		}
	}
	public bool IsReady {
		get {
			if (!isReady) {
				isReady = Clip && Clip.loadState == AudioDataLoadState.Loaded;
			}
			return isReady;
		}
	}
	public bool IsPlaying {
		get {
			return isPlaying;
		}
		set {
			isPlaying = value;
		}
	}
	public float Time {
		get {
			return audioSource.time;
		}
		set {
			audioSource.time = Mathf.Clamp(value, 0f, Length - 0.1f);
			ResetInvoke();
		}
	}
	public float Pitch {
		get {
			return audioSource.pitch;
		}
		set {
			audioSource.pitch = value;
			ResetInvoke();
		}
	}

	private AudioSource audioSource {
		get {
			if (!_audioSource) {
				_audioSource = GetComponent<AudioSource>();
			}
			return _audioSource;
		}
	}
	private AudioSource _audioSource = null;
	private float length = 0f;
	private bool isPlaying = false;
	private bool isReady = false;



	#region ------- Mono --------


	void Awake () {
		if (StageMusic.Main) {
			Debug.LogWarning("Can NOT have muti StageMusic in one scene.");
			DestroyImmediate(this, false);
		} else {
			StageMusic.Main = this;
		}
	}



	#endregion



	#region -------- public --------


	public void Clear () {
		Clip = null;
		Time = 0f;
		Pitch = 1f;
		length = 0f;
		IsPlaying = false;
		isReady = false;
		CancelInvoke();
	}


	public void Play () {
		if (IsReady && !IsPlaying) {
			IsPlaying = true;
			audioSource.Play();
			ResetInvoke();
		}
	}


	public void Pause () {
		if (IsReady && IsPlaying) {
			IsPlaying = false;
			audioSource.Pause();
			ResetInvoke();
		}
	}


	public void Stop () {
		if (IsReady) {
			Pause();
			Time = 0f;
		}
	}


	public void StopToEnd () {
		if (IsReady) {
			Pause();
			Time = Length - 0.1f - 1f / Clip.frequency;
		}
	}


	public void PlayPause () {
		if (IsPlaying) {
			Pause();
		} else {
			Play();
		}
	}


	public void FastForward (float time) {
		Time = Time + time;
	}


	public void Rewind (float time) {
		Time = Time - time;
	}



	#endregion



	#region -------- Logic --------


	private void ResetInvoke () {
		CancelInvoke();
		if (IsPlaying) {
			if (Pitch > 0f) {
				Invoke("StopToEnd", Mathf.Max((Length - Time) / Pitch - 0.1f, 0.1f));
			} else if (Pitch < 0f) {
				Invoke("Stop", Mathf.Max(Time / -Pitch - 0.1f, 0.1f));
			}
		}
	}



	#endregion


}
}