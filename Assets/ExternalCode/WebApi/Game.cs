using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.ExternalCode.Models;
using Newtonsoft.Json;
using TentacleGuitar.Tabular;

namespace Assets.ExternalCode.WebApi
{
    public static class Game
    {
        /// <summary>
        /// 获取曲目列表
        /// </summary>
        /// <returns>返回含有Music对象的List</returns>
        public static List<Music> GetMusics()
        {
            var args = new Dictionary<string, string>();
            var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/GetMusics", args);
            return JsonConvert.DeserializeObject<List<Music>>(ret);
        }

        /// <summary>
        /// 提交成绩
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <param name="UserToken">用户Token</param>
        /// <param name="Score">分数</param>
        public static void SubmitScore(Guid MusicId, string UserToken, long Score)
        {
            var args = new Dictionary<string, string>();
            args.Add("Id", MusicId.ToString());
            args.Add("Token", UserToken);
            args.Add("Score", Score.ToString());
            var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/SignIn", args);
        }

        /// <summary>
        /// 获取伴奏文件字节集（MP3文件）
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <returns>该曲目的字节集</returns>
        public static byte[] GetInstrument(Guid MusicId)
        {
            var args = new Dictionary<string, string>();
            args.Add("Id", MusicId.ToString());
            return HttpHelper.Blob("http://tentacleguitar.azurewebsites.net/GetInstrument", args);
        }

        /// <summary>
        /// 获取谱面
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <returns>返回Tabular对象</returns>
        public static Tabular GetTabular(Guid MusicId)
        {
            var args = new Dictionary<string, string>();
            args.Add("Id", MusicId.ToString());
            var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/GetTabular", args);
            return JsonConvert.DeserializeObject<Tabular>(ret);
        }
    }
}
