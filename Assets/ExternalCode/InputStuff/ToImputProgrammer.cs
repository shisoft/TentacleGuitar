using UnityEngine;
using System.Collections;

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

		// ----- Your Code Here -----

	}


	/// <summary>
	/// 画面更新时调用一次，主线程
	/// 暂停时也会持续调用，谱面尚未加载时也会持续调用，下同
	/// 画面更新 是指 Unity显示层（以玩家眼中的游戏画面为准）更新，距离上次调用的时间为 UnityEngine.Time.deltaTime
	/// 当前歌曲播放的进度（秒）为 TentacleGuitarUnity.Stage.Time，暂停时持续刷新同一个秒数
	/// 请适当 cache 节约性能
	/// </summary>
	public static void StageUpdate () {

		// ----- Your Code Here -----

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
