using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKInvite
{
    public class captcha
    {
        public string sid { set; get; }
        public string imgUrl { set; get; }
        public string key { set; get; }
        public int lastUid { set; get; } 
    }
}
