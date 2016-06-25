using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

namespace Assets.ExternalCode.WebApi
{
    public static class Account
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Username">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>成功返回用户Token，失败返回null</returns>
        public static Task<string> SignInAsync(string Username, string Password)
        {
            return Task.Factory.StartNew(()=>
            {
                var args = new Dictionary<string, string>();
                args.Add("Username", Username);
                args.Add("Password", Password);
                var ret = HttpHelper.Post("http://tentacleguitar.azurewebsites.net/SignIn", args);
                if (ret == "Access Denied")
                    return null;
                else
                    return ret;
            });
        }
    }
}
