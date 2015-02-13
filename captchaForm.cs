using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKInvite
{
    public partial class captchaForm : Form
    {
        public captcha Captcha { set; get; }
        public captchaForm(captcha captcha)
        {
            Captcha = captcha;
            InitializeComponent();
            AcceptButton = button4;
            button4.DialogResult = DialogResult.OK;
            pictureBox2.Load(Captcha.imgUrl);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Captcha.key = textBox2.Text;
            this.Dispose();
        }
    }
}
