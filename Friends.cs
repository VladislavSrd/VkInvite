using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace VKInvite
{
    [JsonObject]
    public class Friends
    {
        [JsonProperty("response")]
        public List<Friend> friendList { get; set; }
    }
    public class Friend
    {
        [JsonProperty("uid")]
        public int Id { get; set; }
        [JsonProperty("first_name")]
        public string firstName { get; set; }
        [JsonProperty("last_name")]
        public string lastName { get; set; }
    }
}
