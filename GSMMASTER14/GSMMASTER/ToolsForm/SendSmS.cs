using GSMMASTER.GSM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSMMASTER.ToolsForm
{
    public partial class SendSmS : Form
    {
        public Form1 instance = Form1.ReturnInstance();
        private DataGridViewRow row = new DataGridViewRow();
        WaitFormFunc waitForm=new WaitFormFunc();
        public SendSmS()
        {
            InitializeComponent();

        }
        public SendSmS(string info,DataGridViewRow row)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.c3Tek;
            this.simInfoLabel.Text = info;
            this.row = row;
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sendbtn_Click(object sender, EventArgs e)
        { string message = "";
            string phone = this.sendphone.Text;
            string content = this.content.Text;
            string port = row.Cells[1].Value.ToString();
            if (string.IsNullOrEmpty(phone) || phone.Length < 10 || phone.Length > 11)
            {
                MessageBox.Show("Số điện thoại không hợp lệ");
                return;
            }
            else if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Nội dung không được để trống");
                return;
            }
            Task.Run(async () =>
            {
                try
                {  
                 
                    if(!string.IsNullOrEmpty(phone)&&!string.IsNullOrEmpty(content))
                    {
                        this.instance.BeginInvoke(new MethodInvoker(() =>
                    {
                        waitForm.Load(this);
                    }));
                        GsmObject gsm = this.instance.gsmOject.Find(g => g.Port == port);
                        if(gsm!=null)
                        {
                            await gsm.SendSms(phone, content);
                        }
                        await Task.Delay(500);
                        if (!string.IsNullOrEmpty(gsm.readPortSmS))
                        {
                            message = gsm.readPortSmS;
                        }
                        gsm = null;
                        this.instance.BeginInvoke(new MethodInvoker(() =>
                        {
                            waitForm.Close();
                        }));
                        if (!message.Equals("Gửi SMS thành công"))
                        {
                            MessageBox.Show(message);
                            return;
                        }

                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    }
            
            });
           
        }
    }
}
