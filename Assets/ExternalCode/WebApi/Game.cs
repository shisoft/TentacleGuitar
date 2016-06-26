using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
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
        public static Task<List<Music>> GetMusicsAsync()
        {
            return Task.Factory.StartNew(()=>
            {
                var args = new Dictionary<string, string>();
                var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/GetMusics", args);
                return JsonConvert.DeserializeObject<List<Music>>(ret);
            });
        }

        /// <summary>
        /// 提交成绩
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <param name="UserToken">用户Token</param>
        /// <param name="Score">分数</param>
        public static Task SubmitScoreAsync(Guid MusicId, string UserToken, long Score)
        {
            return Task.Factory.StartNew(()=>
            {
                var args = new Dictionary<string, string>();
                args.Add("Id", MusicId.ToString());
                args.Add("Token", UserToken);
                args.Add("Score", Score.ToString());
                var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/SignIn", args);
            });
        }

        /// <summary>
        /// 获取伴奏文件字节集（MP3文件）
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <returns>该曲目的字节集</returns>
        public static Task<byte[]> GetInstrumentAsync(Guid MusicId)
        {
            return Task.Factory.StartNew(()=>
            {
                var args = new Dictionary<string, string>();
                args.Add("Id", MusicId.ToString());
                return HttpHelper.Blob("http://tentacleguitar.azurewebsites.net/GetInstrument", args);
            });
        }

        /// <summary>
        /// 获取谱面
        /// </summary>
        /// <param name="MusicId">曲目ID</param>
        /// <returns>返回谱面的文本形式</returns>
        public static Task<string> GetTabularStringAsync(Guid MusicId)
        {
            return Task.Factory.StartNew(()=>
            {
                var args = new Dictionary<string, string>();
                args.Add("Id", MusicId.ToString());
                return HttpHelper.Post("http://tentacleguitar.azurewebsites.net/GetTabular", args);
            });
        }
        
        /// <summary>
        /// 将谱面文本转换为Tabular对象
        /// </summary>
        /// <param name="tabularStr"></param>
        /// <returns></returns>
		public static Tabular ParseTabularAsync(string tabularStr)
        {
			return JsonConvert.DeserializeObject<Tabular>(tabularStr);
        }
    }
}
