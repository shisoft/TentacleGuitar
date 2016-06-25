using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using TentacleGuitarUnity;
using Assets.ExternalCode.WebApi;
using Assets.ExternalCode.Models;


/// <summary>
/// 网络管理器
/// 
/// 游戏主体 与 网络管理器 在此对♂接♀
/// 
/// 1、登陆登出等常用指令
/// 2、获取谱面信息，下载谱面
/// 
/// </summary>
public class NetworkManager {



	#region -------- Param --------


	// User Stuff

	/// <summary>
	/// 用户是否登陆
	/// </summary>
	public static bool IsLogedIn {
		get {

			// --------- Your Code Here --------

			return true;
		}
	}

	/// <summary>
	/// 获取当前用户的用户名，未登录的话返回""
	/// </summary>
	public static string CurrentUserName {
		get {

			// --------- Your Code Here --------

			return "";
		}
	}

	/// <summary>
	/// 获取当前用户的登陆令牌（“记住我”功能使用，加密保存至本地，不要直接返回真实密码），未登录的话返回""
	/// </summary>
	public static ObscuredString LoginToken {
		set {
			ObscuredPrefs.SetString("LoginToken", value);
		}
		get {
			return ObscuredPrefs.GetString("LoginToken", "");
		}
	}


	// Limit Times

	/// <summary>
	/// 等待登陆时间，单位秒。点击登陆后，若超过这个时间则宣告登陆失败。
	/// </summary>
	public static int LoginLimitTime {
		get {

			// -------- Your Code Here --------

			return 40;
		}
	}

	/// <summary>
	/// 等待登出时间，单位秒。同上
	/// </summary>
	public static int LogoutLimitTime {
		get {

			// -------- Your Code Here --------

			return 40;
		}
	}

	/// <summary>
	/// 等待加载歌曲信息的时间，单位秒。同上
	/// </summary>
	public static int LoadSongListLimitTime {
		get {

			// -------- Your Code Here --------

			return 40;
		}
	}


	// SongList Stuff


	/// <summary>
	/// 获取歌曲列表的歌曲总数，包含下载的和未下载的
	/// 这步要获取在本地的Cache，不要访问服务器
	/// </summary>
	public static List<Music> SongList = new List<Music>();
	
	#endregion



	#region -------- Server Logic --------


	#region --- Try ---


	/// <summary>
	/// 尝试登陆
	/// </summary>
	/// <param name="name"> 用户名 </param>
	/// <param name="passWord"> 密码或登陆令牌 </param>
	/// <param name="useRealPassWord"> 若为true 则参数passWord是真实密码，否则为“记住我”功能使用的登陆令牌，这个令牌我会加密保存在本地 </param>
	public static IEnumerator TryLogin (ObscuredString name, ObscuredString passWord) {

		//string token = Account.SignIn(name, passWord);

		var t = Account.SignInAsync(name, passWord);

		while (!t.IsCompleted) {
			yield return new WaitForSeconds(0.01f);
		}
		
		string token = t.Result;

		if (string.IsNullOrEmpty(token)) {
			Stage.LoginDone(false, false, "Fail to Sign In.");
		} else {
			LoginToken = token;
			Stage.LoginDone(true);
		}

	}


	/// <summary>
	/// 尝试加载歌曲列表的信息
	/// </summary>
	public static IEnumerator TryLoadSongListInfo () {
		SongList.Clear();

		var t = Game.GetMusicsAsync();

		while (!t.IsCompleted) {
			yield return new WaitForSeconds(0.01f);
		}

		SongList = t.Result;

		Stage.LoadSongListDone(true);
	}


	/// <summary>
	/// 尝试下载列表中的一首歌
	/// </summary>
	public static IEnumerator TryDownloadSong (string id) {

		Stage.SetDownLoadProgress(id, 0.2f);

		var t0 = Game.GetInstrumentAsync(new System.Guid(id));
		while (!t0.IsCompleted) {
			yield return new WaitForSeconds(0.01f);
		}
		byte[] b = t0.Result;


		FileUtility.ByteToFile(
			b,
			Application.persistentDataPath + "/" + id + "/Music.mp3"
		);


		Stage.SetDownLoadProgress(id, 0.8f);


		var t1 = Game.GetTabularStringAsync(new System.Guid(id));
		while (!t1.IsCompleted) {
			yield return new WaitForSeconds(0.01f);
		}
		FileUtility.WriteText(
			t1.Result, 
			Application.persistentDataPath + "/" + id + "/BeatMap.json"
		);


		Stage.SetDownLoadProgress(id, 1f);
		Stage.DownLoadDone(true, id);
		
		yield return null;
	}


	/// <summary>
	/// 上传玩家成绩
	/// </summary>
	/// <param name="songID">歌曲ID</param>
	/// <param name="score">分数</param>
	/// <param name="maxCombo">最大连击数</param>
	public static void TryUploadResult (string songID, int score, int maxCombo) {
		Game.SubmitScoreAsync(new System.Guid(songID), LoginToken, score);
	}


	#endregion


	#region --- Cancel ---



	/// <summary>
	/// 尝试取消下载某首歌
	/// </summary>
	/// <param name="id"></param>
	public static void CancelDownloadSongImmediate (string id) {

		// --------- Your Code Here --------



	}


	#endregion


	#endregion



	#region -------- Local Logic --------




	/// <summary>
	/// 查看列表里的某首歌是否已经下载完毕 或 曾经下载过
	/// </summary>
	/// <param name="id">  </param>
	/// <returns>  </returns>
	public static bool SongIsDownLoaded (string id) {


		// -------- Your Code Here --------


		//<Test>
		bool d = Stage.SongCards.ContainsKey(id) && Stage.SongCards[id].IsDownloaded;
		//</Test>

		return d;
	}


	/// <summary>
	/// 获取列表里的某首歌音乐文件在本地的完整路径
	/// 游戏主体可以保证
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static string GetSongLocalPath (string id) {

		// -------- Your Code Here --------

		return System.IO.Path.Combine(Application.dataPath, "Ccna — Aquarius.wav");
	}


	/// <summary>
	/// 获取列表里的某首歌谱面文件在本地的完整路径
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public static string GetBeatMapLocalPath (string id) {

        // -------- Your Code Here --------


        return System.IO.Path.Combine(Application.dataPath, "ReadMe.txt");
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



	#region -------- Stage API --------


	// 完成登陆时调用 Stage.LoginDone (bool 是否登陆成功, bool 是否清空输入框里的密码 = false, string 错误信息 = "") 确保此时可以正常使用 #region Logic 里的函数

	// 完成登出时调用 Stage.LogoutDone (bool 是否登出成功, bool 是否忘记密码 = false, string 错误信息 = "") 确保此时已经没有 CurrentUser 了

	// 完成加载歌曲信息时调用  Stage.LoadSongListDone (bool 是否加载成功, string 错误信息 = "") 确保此时能够正常访问 #region SongListLogic 里的函数

	// 完成下载歌曲后调用 Stage.DownLoadDone(bool 是否加载成功, string 歌曲ID, string 错误信息 = "")


	#endregion
	


}
