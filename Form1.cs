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
        bool captchaEntered=true;
        public Form1(string vkAccessToken, string vkUserId)
        {
            InitializeComponent();
            VkAccessToken = vkAccessToken;
            VkUserId = vkUserId;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Captcha = new captcha();
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
           // richTextBox1.AppendText(output.ToString());
            var tmp = (JsonError)JsonConvert.DeserializeObject(output.ToString(), typeof(JsonError));
            if (tmp.error != null)
            {
                if (tmp.error.error_code == 14)
                {
                    waitCaptcha = true;
                    button4.Enabled = true;
                    textBox2.Enabled = true;
                    Captcha.lastUid = uid;
                    Captcha.sid = tmp.error.captcha_sid;
                    Captcha.imgUrl = tmp.error.captcha_img;
                    MessageBox.Show("Введите капчу и нажмите ок, чтобы продолжить");
                    pictureBox2.Show();
                    pictureBox2.Load(Captcha.imgUrl);
                }
                if (tmp.error.error_code == 15)
                {
                    MessageBox.Show("Вы не можете пригласить данного пользователя, так как он запретил эту возможность.");
                }
            }
            response.Close();
        }

       
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            textBox2.Enabled = false;
            if (textBox2.Text != "")
            {
                captchaEntered = true;
                Captcha.key = textBox2.Text;
                pictureBox2.Hide();
                textBox2.Text = "";
                InviteFriend(Captcha.lastUid,textBox1.Text);
            }
            
            
        }

        private void backgroundInviter_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}

