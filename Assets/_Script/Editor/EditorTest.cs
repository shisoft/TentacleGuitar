namespace TentacleGuitarUnity {
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using JsonOrg;
using System.Text;

public class EditorTest {

	[Test]
	public void Test_0 () {
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		bool t = AudioConverter.MP3toWAV(@"D:\Mine\Unity3D\Project-Focusy\Assets\InsaneTechniques.mp3", @"D:\Test.wav");
		sw.Stop();
		Debug.Log(t + " " + sw.Elapsed.Seconds);

	}




}
}