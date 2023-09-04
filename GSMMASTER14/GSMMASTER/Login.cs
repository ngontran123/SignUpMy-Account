using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;
using System.Configuration;
using System.Xml;
using GSMMASTER.Services;
using GSMMASTER.LoginAccount;
using System.Net.Http;

namespace GSMMASTER
{
    public partial class Login : Form
    {
        private static Login instance_login;

        private bool is_remember;
        private string original_pass;
        private string token;
        private bool is_show_pass = false;
        private int key_back = 0;
        public Form1 instance = Form1.ReturnInstance();
        public Form1 form;

        public static Login ReturnLoginInstance()
        {
            return instance_login;
        }
        public Login()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.c3Tek;
            instance_login = this;
            this.saveAccount.Text = "Nhớ tài khoản";
            this.eyeCheckBox.Checked = false;
            convertFont();
            updateLoginSetting();
            userInfo();
        }
        private void convertFont()
        {
            string fontFilePath = Path.Combine(Path.GetTempPath(), "BauhausBold.ttf");
            File.WriteAllBytes(fontFilePath, Properties.Resources.BauhausBold);
            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddFontFile(fontFilePath);
            loginLabel.Font = new Font(pfc.Families[0], 20);
        }
        private void userInfo()
        {
            InforSettingSection inforConfig = (InforSettingSection)ConfigurationManager.GetSection("infoSet");
            saveAccount.Checked = is_remember;
            if (is_remember == true)
            {
                usernameTxtBox.Text = inforConfig.UserName;
                original_pass = inforConfig.Password;
                addUpdateSetting("username", usernameTxtBox.Text);
                addUpdateSetting("password", original_pass);
                char blackDot = '\u25CF';
                int pass_length = original_pass.Length;
                string pass_dot = "";
                for (int i = 0; i < pass_length; i++)
                {
                    pass_dot += blackDot;
                }
                passwordTxtBox.Text = pass_dot;
            }
            else
            {
                usernameTxtBox.Text = "";
                passwordTxtBox.Text = "";
            }
        }
        private void updateLoginSetting()
        {
            InforSettingSection inforConfig = (InforSettingSection)ConfigurationManager.GetSection("infoSet");
            is_remember = inforConfig.Is_Remember;
        }
        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LoginButton_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton.PerformClick();
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (is_remember)
            {
                addUpdateSetting("username", usernameTxtBox.Text);
                addUpdateSetting("password", original_pass);
            }
            this.LoginButton.Enabled = false;
            string email = usernameTxtBox.Text;
            string password = original_pass;
            User user = new User(email, password);
            Dictionary<string, string> user_dict = new Dictionary<string, string>
            {
                {"email",user.email },
                {"password",user.password }
            };

            FormUrlEncodedContent data = new FormUrlEncodedContent(user_dict);
            var token_ob = await TelecomService.GetUserInfoApi<ResponseUser>("login", data);
            if (token_ob != null)
            {
                if (token_ob.Success == "true")
                {
                    string token = "Bearer " + token_ob.Data.AuthData.AccessToken;
                    Environment.SetEnvironmentVariable("TOKEN", token);
                    if (form != null)
                    {
                        form.username = token_ob.Data.UserData.Username;
                        form.phone_text = token_ob.Data.UserData.Phone;
                        form.email_text = token_ob.Data.UserData.Email;
                        form.Show();
                        this.Hide();
                    }
                    else
                    {
                        form = new Form1();
                        form.username = token_ob.Data.UserData.Username;
                        form.phone_text = token_ob.Data.UserData.Phone;
                        form.email_text = token_ob.Data.UserData.Email;
                        form.Show();
                        this.Hide();
                    }
                }

                else
                {
                    MessageBox.Show(token_ob.Message,"Lỗi đăng nhập",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Email hoặc password không chính xác.","Sai thông tin",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            this.LoginButton.Enabled = true;
        }
        private void guna2TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginButton.PerformClick();
            }
            else if (e.KeyData == (Keys.V | Keys.Control))
            {
               if(!is_show_pass)
                {
                    e.Handled = true;
                    original_pass += Clipboard.GetText();
                    string dot = "\u25CF";
                    string black_dot = string.Concat(Enumerable.Repeat(dot.ToString(), original_pass.Length));
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        this.passwordTxtBox.Text = "";
                        this.passwordTxtBox.Text = black_dot;
                        this.passwordTxtBox.SelectionStart = this.passwordTxtBox.Text.Length;

                    }));
                    addUpdateSetting("password", original_pass);
                }
                else
                {
                    e.Handled = true;
                    original_pass += Clipboard.GetText();
                    addUpdateSetting("password", original_pass);
                }
            }
        }
        public void addUpdateSetting(string key, string value)
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xml.SelectSingleNode("//infoSet").Attributes[key].Value = value;
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("//infoSet");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        private void saveAccount_CheckedChanged(object sender, EventArgs e)
        {   
            if (saveAccount.Checked)
            {
                addUpdateSetting("is_remember", "true");
                addUpdateSetting("username", usernameTxtBox.Text);
                addUpdateSetting("password", passwordTxtBox.Text);
            }
            else
            {
                addUpdateSetting("is_remember", "false");
            }
        }

        private void passwordTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsControl(e.KeyChar)) && e.KeyChar != (char)Keys.Back && e.KeyChar!=(char)Keys.Enter && e.KeyChar!=(char)Keys.ControlKey)
            {
                if (!is_show_pass)
                {
                    e.Handled = true;
                    char blackDot = '\u25CF';
                    original_pass += e.KeyChar;
                    passwordTxtBox.Text += blackDot;
                    passwordTxtBox.SelectionStart = passwordTxtBox.Text.Length;
                    if (is_remember)
                    {
                        addUpdateSetting("password", original_pass);
                    }
                }
                else
                {
                    original_pass += e.KeyChar;
                    passwordTxtBox.SelectionStart = passwordTxtBox.Text.Length;
                    if (is_remember)
                    {
                        addUpdateSetting("password", original_pass);
                    }
                }
            }

            else if (e.KeyChar == (char)Keys.Back)
            {

                int original_length = passwordTxtBox.Text.Length;
                BeginInvoke(new Action(() =>
                {
                    int delete_char = original_length - passwordTxtBox.Text.Length;
                    int after_delete_char = original_length - delete_char;
                    if (!string.IsNullOrEmpty(original_pass))
                    {
                        int passfield_txt = passwordTxtBox.Text.Length - 1;

                        for (int i = original_pass.Length - 1; i >= after_delete_char; i--)
                        {
                            original_pass = original_pass.Remove(i);
                        }
                    }
                    addUpdateSetting("password", original_pass);
                }));

            }
        }

        private void usernameTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void usernameTxtBox_Click(object sender, EventArgs e)
        {
        }

        private void passwordTxtBox_Click(object sender, EventArgs e)
        {
            passwordTxtBox.SelectionStart = passwordTxtBox.Text.Length;
        }

        private void usernameTxtBox_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void passwordTxtBox_MouseClick(object sender, MouseEventArgs e)
        {
            passwordTxtBox.SelectionStart = passwordTxtBox.Text.Length;
        }

        private void eyeCheckBtn_Click(object sender, EventArgs e)
        {
            if (!is_show_pass)
            {
                Image backgroundImage = Properties.Resources.eye;
                is_show_pass = true;
                this.eyeCheckBox.Image = backgroundImage;
                this.passwordTxtBox.Text = original_pass;
            }
            else
            {

                Image backgroundImage = Properties.Resources.hide;
                this.eyeCheckBox.Image = backgroundImage;
                is_show_pass = false;
                string hide_pass;
                StringBuilder sb = new StringBuilder();
                foreach (var c in original_pass)
                {
                    char dot = '\u25CF';
                    sb.Append(dot);
                }
                hide_pass = sb.ToString();
                this.passwordTxtBox.Text = hide_pass;
                sb.Clear();
            }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void usernameTxtBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void passwordTxtBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
