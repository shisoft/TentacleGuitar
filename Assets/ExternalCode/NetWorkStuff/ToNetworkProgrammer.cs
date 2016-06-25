using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using TentacleGuitarUnity;
using Assets.ExternalCode.WebApi;
using Assets.ExternalCode.Models;
using System.IO;


/// <summary>
/// 网络管理器
/// 
/// 游戏主体 与 网络管理器 在此对♂接♀
/// 
/// 1、登陆登出等常用指令
/// 2、获取谱面信息，下载谱面
/// 
/// </summary>
public class NetworkManager
{



    #region -------- Param --------




    /// <summary>
    /// 获取当前用户的登陆令牌（“记住我”功能使用，加密保存至本地，不要直接返回真实密码），未登录的话返回""
    /// </summary>
    public static ObscuredString LoginToken
    {
        set
        {
            ObscuredPrefs.SetString("LoginToken", value);
        }
        get
        {
            return ObscuredPrefs.GetString("LoginToken", "");
        }
    }




    /// <summary>
    /// 获取歌曲列表的歌曲总数，包含下载的和未下载的
    /// 这步要获取在本地的Cache，不要访问服务器
    /// </summary>
    public static List<Music> SongList = new List<Music>();


    #endregion



    #region -------- Server Logic --------


    /// <summary>
    /// 尝试登陆
    /// </summary>
    /// <param name="name"> 用户名 </param>
    /// <param name="passWord"> 密码或登陆令牌 </param>
    /// <param name="useRealPassWord"> 若为true 则参数passWord是真实密码，否则为“记住我”功能使用的登陆令牌，这个令牌我会加密保存在本地 </param>
    public static IEnumerator TryLogin(ObscuredString name, ObscuredString passWord)
    {

        //string token = Account.SignIn(name, passWord);

        var t = Account.SignInAsync(name, passWord);

        while (!t.IsCompleted)
        {
            yield return new WaitForSeconds(0.01f);
        }

        string token = t.Result;

        if (string.IsNullOrEmpty(token))
        {
            Stage.LoginDone(false, false, "Fail to Sign In.");
        }
        else
        {
            LoginToken = token;
            Stage.LoginDone(true);
        }

    }


    /// <summary>
    /// 尝试加载歌曲列表的信息
    /// </summary>
    public static IEnumerator TryLoadSongListInfo()
    {
        SongList.Clear();

        var t = Game.GetMusicsAsync();

        while (!t.IsCompleted)
        {
            yield return new WaitForSeconds(0.01f);
        }

        SongList = t.Result;

        Stage.LoadSongListDone(true);
    }


    /// <summary>
    /// 尝试下载列表中的一首歌
    /// </summary>
	public static IEnumerator TryDownloadSong (string id) 
	{

		Stage.SetDownLoadProgress(id, 0.2f);

		var t0 = Game.GetInstrumentAsync(new System.Guid(id));
        while (!t0.IsCompleted)
        {
            yield return new WaitForSeconds(0.01f);
        }
        byte[] b = t0.Result;


        FileUtility.ByteToFile(
            b,
            Application.persistentDataPath + "/" + id + "/Music.mp3"
        );


        Stage.SetDownLoadProgress(id, 0.8f);


        var t1 = Game.GetTabularStringAsync(new System.Guid(id));
        while (!t1.IsCompleted)
        {
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
    public static void TryUploadResult(string songID, int score, int maxCombo)
    {
        Game.SubmitScoreAsync(new System.Guid(songID), LoginToken, score);
    }


    #endregion



    #region -------- Local Logic --------




    /// <summary>
    /// 查看列表里的某首歌是否已经下载完毕 或 曾经下载过
    /// </summary>
    /// <param name="id">  </param>
    /// <returns>  </returns>
    public static bool SongIsDownLoaded(string id)
    {
		return File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, id), "Music.mp3")) &&
			File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, id), "BeatMap.json"));
    }


    /// <summary>
    /// 获取列表里的某首歌音乐文件在本地的完整路径
    /// 游戏主体可以保证
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetSongLocalPath(string id)
    {
		return Path.Combine(Path.Combine(Application.persistentDataPath, id), "Music.mp3");
    }


    /// <summary>
    /// 获取列表里的某首歌谱面文件在本地的完整路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetBeatMapLocalPath(string id)
    {
		return Path.Combine(Path.Combine(Application.persistentDataPath, id), "BeatMap.json");
    }


    #endregion





}
