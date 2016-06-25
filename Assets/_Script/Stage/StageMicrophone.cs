namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class StageMicrophone : MonoBehaviour {


	public static StageMicrophone Main;

	public AudioSource Source {
		get {
			if (!source) {
				source = GetComponent<AudioSource>();
			}
			return source;
		}
	}

	public static bool IsReady {
		get {
			return Main && Main.Source.clip && Main.Source.clip.loadState == AudioDataLoadState.Loaded;
		}
	}

	public static int SampleLength {
		get {
			if (IsReady) {
				return Main.Source.clip.samples;
			} else {
				return 0;
			}
		}
	}

	private AudioSource source;




	void Awake () {
		Main = this;
	}



	public static void MicrophoneStart (int length) {
		if (!Main)
			return;
		Main.Source.clip = Microphone.Start(null, true, length, 44100);
		Main.Source.loop = true;
		while (!(Microphone.GetPosition(null) > 0)) { }
		Main.Source.Play(); 
	}


	public static void MicrophoneStop () {
		if (!Main)
			return;
		Microphone.End(null);
		Main.Source.clip = null;
		Main.Source.Stop();
	}


	public static int SamplePosition () {
		if (!Microphone.IsRecording(null)) {
			return 0;
		}
		return Microphone.GetPosition(null);
	}

	
	public static float[] GetData (int samplePosition, int sampleLength) {
		if (!Main || !IsReady) {
			return null;
		}
		samplePosition = (int)Mathf.Repeat(samplePosition, StageMicrophone.SampleLength);
		if (samplePosition + sampleLength < StageMicrophone.SampleLength) {
			float[] f = new float[sampleLength];
			Main.Source.clip.GetData(f, samplePosition);
			return f;
		} else {
			List<float> fList = new List<float>();
			
			float[] f = new float[StageMicrophone.SampleLength - samplePosition];
			Main.Source.clip.GetData(f, samplePosition);
			fList.AddRange(f);
			
			int len = sampleLength - (StageMicrophone.SampleLength - samplePosition);
			if(len > 0){
				f = new float[len];
				Main.Source.clip.GetData(f, 0);
				fList.AddRange(f);
			}
			
			return fList.ToArray();
		}
	}


	public static float[] GetData (int sampleLength) {
		return GetData(SamplePosition(), sampleLength);
	}




}

}
