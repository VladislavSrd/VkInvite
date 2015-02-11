using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Runtime.InteropServices;

namespace VKInvite
{
    public partial class authForm : Form
    {
        private string VkAccessToken;
        private string VkUserId;
        int appId = 4775436;
        private const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


        public authForm()
        {
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
            InitializeComponent();
        }


        private void authForm_Load(object sender, EventArgs e)
        {
            const string vkUri = "https://oauth.vk.com/authorize?client_id=4775436&scope=262144&" +
                           "redirect_uri=http://oauth.vk.com/blank.html" + "&display=touch&response_type=token";
            Uri requestUri = new Uri(vkUri);
            webBrowser1.Navigate(requestUri);
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().Contains(@"https://oauth.vk.com/blank.html#"))
            {
                GetParams();
                Form1 form = new Form1(VkAccessToken,VkUserId);
                form.Show();
                this.Hide();
            } 
        }

        private void GetParams()
        {
            string accessStr = webBrowser1.Url.AbsoluteUri.Replace('#','?');
            Uri accessUri = new Uri(accessStr);
            VkAccessToken = HttpUtility.ParseQueryString(accessUri.Query).Get(0);
            VkUserId = HttpUtility.ParseQueryString(accessUri.Query).Get(2);
        }
    }
}
