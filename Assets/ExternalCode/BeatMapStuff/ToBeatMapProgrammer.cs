using UnityEngine;
using System.Collections;
using TentacleGuitarUnity;

/// <summary>
/// 
/// 谱面管理器 1.0
/// 
/// 游戏主体 与 谱面载入装置 在此对♂接♀
/// 
/// 1、通过已知路径加载谱面文件
/// 2、在适当的时刻添加音符
/// 
/// </summary>
public class BeatMapManager {

	
	#region -------- Logic Message --------


	/// <summary>
	/// 载入一张谱面，主线程
	/// 将产生的异常用 Debug.LogError("") 打印出即可，下同
	/// </summary>
	/// 
	/// <param name="info"> 
	/// 需要载入的文件信息
	/// </param>
	/// 
	/// <returns> 是否成功载入谱面 </returns>
	public static bool LoadBeatMap (SongInitInfo info) {


		// ----- Your Code Here -----




		// 执行到这里时，谱面完成加载
		return true;
	}


	/// <summary>
	/// 清除当前加载的所有信息，主线程
	/// 调用 LoadBeatMap() 前已经调用一次了，无需重复清除
	/// </summary>
	public static void ClearBeatMap () {


		// ----- Your Code Here -----


	}


	#endregion


	#region -------- Mono Message --------


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
	public static void StageLateUpdate () {

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


	#region -------- Request --------


	/// <summary>
	/// 获取当前加载的谱面的音符总数
	/// </summary>
	/// <returns> 音符总数，没加载谱面的话返回0 </returns>
	public static int GetNoteSum () {


		// ----- Your Code Here -----


		return 1000;
	}

	#endregion


	#region -------- Stage API --------


	// 请看 Stage.cs 里的 #region -------- Public -------- (170行附近)
	
	
	#endregion


}
