using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace VKInvite
{
    public class JsonError
    {
        public error error { get; set; }
       
    
    }
    public class error
    {
       public int error_code{get;set;}
       public string error_msg { get; set; }
       public string captcha_sid { get; set; }
       public string captcha_img { get; set; }
    }
}
