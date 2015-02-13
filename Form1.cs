using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace VKInvite
{
    public partial class Form1 : Form
    {
        bool waitCaptcha = false;
        private string VkAccessToken;
        private string VkUserId;
        Friends friends;
        captcha Captcha;
        public Form1(string vkAccessToken, string vkUserId)
        {
            InitializeComponent();
            VkAccessToken = vkAccessToken;
            VkUserId = vkUserId;
            Captcha = new captcha();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            friends = GetFriends();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                InviteFriend(friends.friendList[listView1.SelectedIndices[0]].Id, textBox1.Text);
            }
        }

        private Friends GetFriends()
        {
            Uri getFriendsApiUrl = new Uri(string.Format("https://api.vk.com/method/friends.get?user_id={0}&order=hints&fields=nickname", VkUserId));
            var request = (HttpWebRequest)WebRequest.Create(getFriendsApiUrl);

            request.Method = "GET";
            request.Accept = "application/json";

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            var tmp = (Friends)JsonConvert.DeserializeObject(output.ToString(), typeof(Friends));
            response.Close();
            return tmp;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            if (friends.friendList.Count != 0)
            {
                foreach (var friend in friends.friendList)
                {
                    var item = new ListViewItem(friend.firstName + " " + friend.lastName);
                    listView1.Items.Add(item);
                }
                label1.Text = friends.friendList.Count.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var friend in friends.friendList)
            {
                InviteFriend(friend.Id, textBox1.Text);
                Thread.Sleep(200);
            }
        }

        void InviteFriend(int uid, string group_id)
        {
            string inviteStr;
            if (waitCaptcha == true)
                inviteStr = string.Format(@"https://api.vk.com/method/groups.invite?group_id={0}&user_id={1}&access_token={2}&captcha_sid={3}&captcha_key={4}", group_id, uid, VkAccessToken,Captcha.sid,Captcha.key);
            else
                inviteStr = string.Format(@"https://api.vk.com/method/groups.invite?group_id={0}&user_id={1}&access_token={2}", group_id, uid, VkAccessToken);
            waitCaptcha = false;
            Uri inviteUrl = new Uri(inviteStr);
            var request = (HttpWebRequest)WebRequest.Create(inviteUrl);

            request.Method = "GET";
            request.Accept = "application/json";


            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            if (output.ToString() == "{\"response\":1}")
                listView1.Items[friends.friendList.FindIndex(p => p.Id == uid)].ForeColor = Color.ForestGreen;
            else
            {
                var tmp = (JsonError)JsonConvert.DeserializeObject(output.ToString(), typeof(JsonError));
                if (tmp.error != null)
                {
                    if (tmp.error.error_code == 14)
                    {
                        waitCaptcha = true;
                        Captcha.lastUid = uid;
                        Captcha.sid = tmp.error.captcha_sid;
                        Captcha.imgUrl = tmp.error.captcha_img;
                        captchaForm captchaForm = new captchaForm(Captcha);
                        captchaForm.ShowDialog();
                        if (captchaForm.DialogResult == DialogResult.OK)
                        {
                            Captcha.key = captchaForm.Captcha.key;
                            InviteFriend(Captcha.lastUid, textBox1.Text);
                        }
                    }
                    if (tmp.error.error_code == 15)
                    {
                        listView1.Items[friends.friendList.FindIndex(p => p.Id == uid)].ForeColor = Color.DarkMagenta;
                    }
                    
                }
            }
            response.Close();
        }

    }
}

