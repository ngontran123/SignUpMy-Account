using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSMMASTER.ToolsForm
{
    public partial class ReceiveSmS : Form
    {
        public DataGridViewRow row = new DataGridViewRow();
        Form1 instance_gsm = Form1.ReturnInstance();
        public static ReceiveSmS instance;
        private string phone;
        public static ReceiveSmS ReturnInstance()
        {
            return instance;
        }
        public ReceiveSmS()
        {
            InitializeComponent();
        }
        public ReceiveSmS(string res,DataGridViewRow row)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.c3Tek;
            this.portInfo.Text = res;
            this.portInfo.ReadOnly = true;
            this.row = row;
            instance = this;
            phone = row.Cells["sdt"].Value.ToString();
        }
        public void updateDataGrid(string phone)
        {
            List<string> res = new List<string>();
            this.receiveGSM.Rows.Clear();
            this.receiveGSM.Refresh();
            foreach(string mess in this.instance_gsm.List_Receive)
            { List<string> data = new List<string>();
                string p = "";
                if (mess.Contains("~"))
                {
                    data = mess.Split('~').ToList();
                    p = data[data.Count - 3];
                }
                else
                {
                    data = mess.Split('@').ToList();
                    p = data[3];
                    p = p.Remove(0, 2).Insert(0, "0");
                }
                if (phone==p)
                {
                    res.Add(mess);
                }
            }
           foreach(string p in res)
            {
               var index = this.receiveGSM.Rows.Add();
                DataGridViewRow row = this.receiveGSM.Rows[index];
                List<string> data = new List<string>();
                if(p.Contains("~"))
                {
                    data = p.Split('~').ToList();
                }
                else
                {
                    data = p.Split('@').ToList();
                }
                row.Cells["from"].Value = data[0];
                row.Cells["date"].Value = data[2];
                row.Cells["msg"].Value = data[1];
            }
        }
        private void dataGSM_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.receiveGSM.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1;
        }

        private void receiveGSM_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void reloadbtn_Click(object sender, EventArgs e)
        {
            updateDataGrid(phone);
        }

        private void ReceiveSmS_Load(object sender, EventArgs e)
        {
            updateDataGrid(phone);

        }

        private void receiveGSM_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                DateTime n1 = DateTime.ParseExact(e.CellValue1.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime n2 = DateTime.ParseExact(e.CellValue2.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                e.SortResult = n1.CompareTo(n2);
                e.Handled = true;
            }
        }
    }
}
