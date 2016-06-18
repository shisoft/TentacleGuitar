namespace TentacleGuitarUnity {
using UnityEngine;
using System.Collections;
using System.IO;
using NAudio.Wave;


public class AudioConverter {


	public static bool MP3toWAV (string mp3Path) {
		string wavPath = Path.ChangeExtension(mp3Path, ".wav");
		return MP3toWAV(mp3Path, wavPath);
	}


	public static bool MP3toWAV (string mp3Path, string WavPath) {
		try {
			using (Mp3FileReader mp3 = new Mp3FileReader(mp3Path)) {
				using (WaveStream ws = WaveFormatConversionStream.CreatePcmStream(mp3)) {
					WaveFileWriter.CreateWaveFile(WavPath, ws);
				}
			}
			return true;
		} catch (System.Exception) {
			return false;
		}
	}



}
}