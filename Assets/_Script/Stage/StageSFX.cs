namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class StageSFX : MonoBehaviour {


	private AudioSource audioSource {
		get {
			if (!_audioSource) {
				_audioSource = GetComponent<AudioSource>();
			}
			return _audioSource;
		}
	}
	private AudioSource _audioSource = null;









}
}