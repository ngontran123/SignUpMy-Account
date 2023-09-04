using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMMASTER.LoginAccount
{
    public class ResponseUser
    {
        [JsonProperty("success")]
        public string Success { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public Info Data { get; set; }
    }
    public class Info
    {
        [JsonProperty("auth_data")]
        public Token AuthData { get; set; }
        [JsonProperty("user_data")]
        public UserInfo UserData { get; set; }
    }
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string Expire { get; set; }
    }
    public class UserInfo
    {
        [JsonProperty("fullname")]
        public string Fullname { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("role")]
        public string Role { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("balance")]
        public string Balance { get; set; }
    }

}
