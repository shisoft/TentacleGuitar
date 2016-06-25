using UnityEngine;
using System.Collections.Generic;
using TentacleGuitarUnity;
using AssemblyCSharp;

/// <summary>
/// 
/// 乐器输入管理器 1.0
/// 
/// 游戏主体 与 乐器输入装置 在此对♂接♀
/// 
/// 1、在玩家用乐器输入信号时，将信息专递给游戏主体
/// 
/// </summary>
public class InputManager {


	#region -------- Mono Message ---------


	/// <summary>
	/// 当前场景的逻辑层准备就绪时调用一次，主线程。
	/// Unity的Scene载入时才会触发这个函数，在同一个Scene里 重新载入歌曲和谱面 不会再次调用这个函数
	/// 可以保证此时 Stage、StageMusic、PoolManager 等类已经 Awake，详情查看 Edit -> Project Setting -> Script Execution Order
	/// </summary>
	public static void StageAwake () {
		NotesMap.init ();
	}


	private static int getNote(double freq){
		double lowerF = 0, higherF = 0;
		int lowerI = 0, higherI = 0;
		for (int i = 0; i < NotesMap.freqs.Count; i++){
			var f = NotesMap.freqs [i];
			if (f < freq && f > lowerF) {
				lowerF = f;
				lowerI = i;
			} else if (f > freq) {
				higherF = f;
				higherI = i;
				break;
			}
		}
		double lowerD = freq - lowerF;
		double higherD = higherF - freq;
		int freqIdx;
		if (lowerD < higherD) {
			freqIdx = lowerI;
		} else {
			freqIdx = higherI;
		}
		return freqIdx;
	}

	public static List<InNote> getNotes (){
		int bins = 8192;
		float[] spectrum = StageMicrophone.GetSpectrumData (bins);
		if (spectrum != null) {
			double threshold = 0.001;
			Dictionary<int, InNote> notes = new Dictionary<int, InNote> ();
			for (int i = 0; i < spectrum.Length; i++) {
				double sAmp = spectrum [i];
				if (sAmp > threshold) {
					var frequency = i * StageMicrophone.sampleRate / bins;
					if (frequency > 70 && frequency < 1200) {
						int noteIdx = getNote (frequency);
						if (notes.ContainsKey (noteIdx)) {
							notes[noteIdx].Amp += sAmp;
						} else {
							notes.Add (noteIdx, new InNote(noteIdx, sAmp));
						}
					}
					i++;
				}
			}
			List<InNote> noteList = new List<InNote> ();
			foreach (InNote note in notes.Values) {
				noteList.Add (note);
			}
			noteList.Sort ((a, b) => a.Amp.CompareTo(b.Amp));
			int maxNotesInChord = 6;
			if (noteList.Count > maxNotesInChord) {
				noteList.RemoveRange (maxNotesInChord, noteList.Count - maxNotesInChord);
			}
			return noteList;
		}
		return null;
	}

	/// <summary>
	/// 画面更新时调用一次，主线程
	/// 暂停时也会持续调用，谱面尚未加载时也会持续调用，下同
	/// 画面更新 是指 Unity显示层（以玩家眼中的游戏画面为准）更新，距离上次调用的时间为 UnityEngine.Time.deltaTime
	/// 当前歌曲播放的进度（秒）为 TentacleGuitarUnity.Stage.Time，暂停时持续刷新同一个秒数
	/// 请适当 cache 节约性能
	/// </summary>
	public static void StageUpdate () {
		var noteList = getNotes();
		if (noteList != null) {
			var strNoteList = new List<string> (); 
			foreach (InNote note in noteList) {
				strNoteList.Add (NotesMap.notes[note.Id]);
			}
			Debug.Log (string.Join(" ", strNoteList.ToArray()));
		}
	}


		/// <summary>
		/// Unity物理层、逻辑层更新时调用一次
		/// 多次调用的时间间隔相等，大约为0.02s，具体时间用 UnityEngine.Time.deltaTime 可以获取 
		/// 主线程假死会使时间间隔变大，此时 deltaTime 有一定几率依旧显示正常
		/// </summary>
		public static void StageFixedUpdate () {

			// ----- Your Code Here -----

		}


	#endregion


		#region -------- Stage API ---------


		// 用 Stage.Beat(int, int, float) 单次击打一下某个位置，

		// 用 StageInput.Hold(int, int) 按住某个位置一帧，确保在 StageUpdate 函数里调用

		// 用 StageMicrophone.SamplePosition() 获取当前麦克风正在录制的位置，单位Sample（秒x44100)

		// 用 StageMicrophone.GetData() 获取一段已经录制的音频，这个音频是声音最原始的震动，采样率44100次每秒。


		#endregion


	}
