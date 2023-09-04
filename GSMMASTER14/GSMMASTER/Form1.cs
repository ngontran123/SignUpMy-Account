using GSMMASTER.GSM;
using GSMMASTER.MVTAccount;
using GSMMASTER.Services;
using GSMMASTER.ToolsForm;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GSMMASTER
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public List<GsmObject> gsmOject = new List<GsmObject>();
        private bool isPortAvail = false;
        private object lockReader = new object();
        private int new_port_num = -1;
        private int number = 1;
        public string baudrate = "";
        public string checkport = "";
        public string blackListPort = "";
        public string mypassword_set = "";
        public DateTime last_scan_port = DateTime.Now;
        public DateTime rescan_status = DateTime.Now;
        public bool start_scanning = false;
        public List<string> List_Receive = new List<string>();
        public bool isActivated = false;
        public bool openPort = false;
        public List<string> transactionID = new List<string>();
        public bool rescanActivated = false;
        public bool is_load_port_activated = false;
        public System.Timers.Timer time_check_regis = new System.Timers.Timer();
        System.Timers.Timer check_timer = new System.Timers.Timer();
        private SemaphoreSlim registerLock = new SemaphoreSlim(5);
        private SemaphoreSlim checkLock = new SemaphoreSlim(5);
        SemaphoreSlim semaphore_timer = new SemaphoreSlim(15);
        SemaphoreSlim lockSMSPort=new SemaphoreSlim(1,1);
        DateTime lastSmSReport = DateTime.MinValue;
        public object messagelock=new object();
        private int new_num_trans_id = 0;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationTokenSource logout_cts = new CancellationTokenSource();
        public string username = "";
        public string phone_text = "";
        public string email_text = "";
        public System.Timers.Timer time_load_regis_my = new System.Timers.Timer();
        public int is_start_regis = 0;
        public List<string> phoneCreated = new List<string>();
        public Login instance_login = Login.ReturnLoginInstance();
        public List<string> port_setting = new List<string>();
        public List<string> List_Imei = new List<string>();
        public RealPhoneImei phone_imei_ob = new RealPhoneImei();
        public static Form1 ReturnInstance()
        {
            return instance;
        }
        public Form1()
        {
            instance = this;
            InitializeComponent();
            this.Icon = Properties.Resources.c3Tek;
            CheckForIllegalCrossThreadCalls = false;
            UpdateGUI.dataForm += new UIViewRow(UpdateDataGridViewRow);
        }
        public void UpdateDataGridViewRow(DataGridViewRow row, string name, string value)
        {
            try
            {
                if (row == null && instance == null)
                {
                    return;
                }
                this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        row.Cells[name].Value = value;
                    }
                    catch(Exception er)
                    {
                        Console.WriteLine(er.Message);
                    }
                }
                ));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
            
        }
        public void deletePortSetting()
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                XmlNode node = xml.SelectSingleNode("//portSet/port/add[@port_custom]");
                if(node!=null)
                {
                    node.ParentNode.RemoveAll();
                }
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("portSet/port");
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
        }
        public void addPortSetting(int size)
        {
            try
            {
           
               
                   int initial_size = Properties.Settings.Default.PortCustom.Count;
                
                    for (int i = initial_size-1; i < size+(initial_size-1); i++)
                    {
                        Properties.Settings.Default.PortCustom.Add($"Cổng {i+1}");
                        Properties.Settings.Default.Save();
                }
                
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
        }
        public void updatePortSetting(DataGridView datagrid)
        {
            for(int i=0;i<datagrid.RowCount;i++)
            {   DataGridViewRow row= datagrid.Rows[i];
              
                    if (Properties.Settings.Default.PortCustom[i] != null)
                    {
                    try
                    {
                        Properties.Settings.Default.PortCustom[i] = row.Cells["portmanual"].Value.ToString();
                    }
                    catch(Exception er)
                    {
                        Console.WriteLine(er.Message);
                    }
                    }
               
            }
            Properties.Settings.Default.Save();
        }
        
        public void setupPortSetting()
        {
            try
            {
                if (Properties.Settings.Default.PortCustom == null)
                {
                    Properties.Settings.Default.PortCustom = new System.Collections.Specialized.StringCollection();
                }
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
                int size = Properties.Settings.Default.PortCustom.Count;

                if (size < new_port_num)
                {
                    int remain = new_port_num - size;
                    addPortSetting(remain);
                }
                for (int i = 0; i < dataGSM.RowCount; i++)
                {
                    DataGridViewRow row = dataGSM.Rows[i];
                    dataGSM.Invoke(new MethodInvoker(() =>
                    {
                        row.Cells["portmanual"].Value = Properties.Settings.Default.PortCustom[i];
                    }));
                }
        }
        
        private void dataGSM_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGSM.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
        }

        private void dataGSM_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1)
            {
                int n1 = int.Parse(e.CellValue1.ToString().Replace("COM", ""));
                int n2 = int.Parse(e.CellValue2.ToString().Replace("COM", ""));
                e.SortResult = n1.CompareTo(n2);
                e.Handled = true;
            }
        }
        //Ham tao mot line moi trong datagrid
        public DataGridViewRow newRow()
        {
            int rowId = -1;
            try
            {
                this.dataGSM.Invoke(new MethodInvoker(() => rowId = this.dataGSM.Rows.Add()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return this.dataGSM.Rows[rowId];
        }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            updateSetting();
            updateCom();
            updateMyPasswordSetting();
           Assembly assembly=Assembly.GetEntryAssembly();
           Version ver = assembly.GetName().Version;
           List_Imei=List_Imei.Concat(phone_imei_ob.getAllTypeTalco("Apple")).Concat(phone_imei_ob.getAllTypeTalco("Samsung")).Concat(phone_imei_ob.getAllTypeTalco("HTC")).Concat(phone_imei_ob.getAllTypeTalco("Motorola")).ToList();
            this.usernameLabel.Text = $"Tên người dùng: {username}";
            this.phoneLabel.Text = $"Số điện thoại: {phone_text}";
            this.emailLabel.Text = $"Email: {email_text}";
            this.mypassword.Text = mypassword_set;
            this.version.Text = $"Version:{ver.ToString()}";
        }
        //Ham update cai dat
        public void updateSetting()
        {
            InforSettingSection inforConfig = (InforSettingSection)ConfigurationManager.GetSection("infoSet");
            baudrate = inforConfig.Baudrate;
            checkport = inforConfig.Recheck_Port;
        }
        public void updateCom()
        {
            AppSettingSection appConfig = (AppSettingSection)ConfigurationManager.GetSection("appSet");
            blackListPort = "";
            foreach (AppSettingElement ele in appConfig.collect)
            {
                blackListPort += ele.Com + ",";
            }
            if (blackListPort.Length > 0)
            {
                blackListPort = blackListPort.Remove(blackListPort.Length - 1);
            }
        }
        public void updateMyPasswordSetting()
        {
            MyPasswordSettingSection mypassworConfig = (MyPasswordSettingSection)ConfigurationManager.GetSection("mypasswordSet");
            mypassword_set= mypassworConfig.MyPassword;
        }
     
        //Ham xu ly lay thong tin port
        public void PortHandling(object state)
        {
            PortName pn = (PortName)state;
            Task.Run(async () =>
            {
                await checkLock.WaitAsync();
                try
                {
                    string port = pn.port;
                    try
                    {
                        Form1.Reference reference = new Form1.Reference(this.run);
                        this.Invoke(reference, port, pn.k);
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine(er);
                    }
                    port = null;
                    pn = null;
                }
                finally
                {
                    checkLock.Release();
                }
            }
            );

        }

        public void run(string port, DataGridViewRow i)
        {
            try
            {
                Task.Run(() =>
            {
                try
                {
                    GsmObject obj = new GsmObject(port, i);
                    gsmOject.Add(obj);
                }
                catch(Exception er)
                {
                    Console.WriteLine(er.Message);
                }
            });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void removeRow(string phone)
        {
            foreach (DataGridViewRow row in dataGSM.Rows)
            {
                string ph = row.Cells["sdt"].Value.ToString();
                if (ph == phone)
                {
                    this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                        this.dataGSM.Rows.Remove(row);
                    }));
                    break;
                }
            }
        }
        public void runOpen(string port)
        {
            try
            {
                GsmObject obj = new GsmObject(port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public delegate void Reference(string x, DataGridViewRow y);
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            setupPortSetting();
            int row_count = dataGSM.RowCount;
            
            if (row_count < new_port_num)
            {
                return;
            }
            if (is_load_port_activated)
            {
                if (!openPort)
                {
                    Task.Run(() =>
                    {
                    this.checkportsbtn.Enabled = false;
                    for (int i = 0; i < dataGSM.RowCount; i++)
                    {
                        string port_name = dataGSM.Rows[i].Cells[1].Value.ToString();
                        PortName pn = new PortName(port_name, dataGSM.Rows[i]);
                        PortHandling(pn);
                        }
                    }
                    ).ContinueWith(t => { this.checkportsbtn.Enabled = true;});
                    openPort = true;
                }
                else
                {
                    isActivated = false;
                    this.transactionID.Clear();
                    foreach (DataGridViewRow row in dataGSM.Rows)
                    {
                        string port = row.Cells[1].Value.ToString();
                        row.Cells["status"].Style.BackColor = Color.White;
                        GsmObject ob = gsmOject.SingleOrDefault(t => t.Port == port);
                     
                        if (!ob.sp.IsOpen)
                        {
                            ob.sp.Open();
                        }
                        if (ob != null)
                        {
                            ob.reset(port, row);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Phải tải danh sách cổng trước.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {
        }
        public class PortName
        {
            public string port { get; set; }
            public DataGridViewRow k { get; set; }
            public PortName(string p, DataGridViewRow i)
            {
                this.port = p;
                this.k = i;
            }
        }
        public async Task Lock(int timeout)
        {
            await this.lockSMSPort.WaitAsync();
            try
            {
                while (DateTime.Now.Subtract(this.lastSmSReport).TotalSeconds < timeout)
                {
                    await Task.Delay(100);
                }
                this.lastSmSReport = DateTime.Now;
            }
            finally
            {
                this.lockSMSPort.Release();
            }

        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {   
            string[] ports = SerialPort.GetPortNames();
            int port_nums = ports.Length;
            blackListPort = blackListPort.ToUpper();
            string[] values = blackListPort.Split(',');
            if (port_nums == 0)
            {
                MessageBox.Show("Không phát hiện cổng trên thiết bị.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            for (int i = 0; i < ports.Length; i++)
            {
                if (values.Contains(ports[i]))
                {
                    port_nums--;
                }
            }
            num_of_port.Text = "Số cổng hiện đang có:"+port_nums.ToString() + " cổng";
            if (port_nums != new_port_num)
            {
                dataGSM.Rows.Clear();
                dataGSM.Refresh();
                new_port_num = port_nums;
                Task.Run((async () =>
                  {
                      await Task.Delay(1200);
                      for (int i = 0; i < ports.Length; i++)
                      {
                          string port = ports[i];
                          if (!values.Contains(port))
                          {
                              try
                              {
                                  Form1.ReferenceStart reference = new Form1.ReferenceStart(this.runOpen);
                                  this.Invoke(reference, port);
                                  reference = null;
                              }
                              catch (Exception er)
                              {
                                  Console.WriteLine(er);
                              }
                              port = null;
                          }
                          else
                          {
                              foreach (DataGridViewRow row in dataGSM.Rows)
                              {
                                  if (values.Contains(row.Cells["Port"].Value.ToString()))
                                  {
                                      dataGSM.Rows.RemoveAt(row.Index);
                                  }
                              }
                          }
                      }
                      ports = null;
                  }
                  )).ContinueWith(x => is_load_port_activated = true);
            }
        }
        public delegate void ReferenceStart(string port);
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {  if(e.CloseReason==CloseReason.UserClosing)
            {
                bool can_close = true;
                foreach(DataGridViewRow row in dataGSM.Rows)
                {
                    string status = row.Cells["status"].Value.ToString();
                    if(status=="Đang khởi tạo")
                    {
                        can_close = false;
                        break;
                    }
                }
            if(!can_close)
                {
                    MessageBox.Show("Không thể đóng chương trình trong quá trình tạo My*", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                    Environment.Exit(Environment.ExitCode);
                }
            }
        }
        public delegate void RegisterStart(MyRegisObject phoneobj);
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        }
        public void autoScan()
        {
            while (true)
            {
                int count_phone = countValidSim();
                int count_expire = countExpire();
                if (isActivated)
                {
                    if (DateTime.Now.Subtract(last_scan_port).TotalMilliseconds > int.Parse(this.checkport))
                    {
                        foreach (DataGridViewRow row in dataGSM.Rows)
                        {
                            string port = row.Cells[1].Value.ToString();
                            GsmObject ob = gsmOject.SingleOrDefault(t => t.Port == port);
                            ob.reset(port, row);
                        }
                        isActivated = false;
                    }
                }
                else if (count_expire == count_phone && count_phone > 0)
                {
                    this.isActivated = true;
                    this.last_scan_port = DateTime.Now;
                }
            }
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {

        }

        private void dataGSM_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void num_of_port_Click(object sender, EventArgs e)
        {

        }

        private void tinNhắnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGSM.CurrentRow;
                string phone = row.Cells["sdt"].Value.ToString();
                string port = row.Cells[1].Value.ToString();
                string network = row.Cells["network"].Value.ToString();
                if (phone == null)
                {
                    MessageBox.Show("Cổng này chưa có sim hoặc sim chưa có sóng");
                }
                string res = phone + "(" + network + ")" + "-" + port;
                if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(network))
                {
                    ReceiveSmS sms = new ReceiveSmS(res, row);
                    sms.ShowDialog();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        public int countValidSim()
        {
            int count = 0;
            if (dataGSM.RowCount == 0 || dataGSM.RowCount < new_port_num)
            {
                return -1;
            }
            try
            {

                foreach (DataGridViewRow row in dataGSM.Rows)
                {
                    try
                    {
                        if (row.Cells["sdt"].Value.ToString() != "" && row.Cells["sdt"].Value.ToString().Length==10)
                        {
                            count++;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    catch (Exception er)
                    { }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return count;
        }
        public int countExpire()
        {
            int count = 0;
            if (dataGSM.RowCount == 0 || dataGSM.RowCount < new_port_num)
            {
                return -1;
            }
            try
            {
                foreach (DataGridViewRow row in dataGSM.Rows)
                {
                    try
                    {
                        if (row.Cells[8].Value.ToString() != "")
                        {
                            count++;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    catch (Exception er)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return count;
        }
        private void tảiLạiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                Parallel.ForEach(selectedRows, row =>
                {
                    row.Cells["status"].Style.BackColor = Color.White;
                    string port = row.Cells[1].Value.ToString();
                    GsmObject gsm = gsmOject.SingleOrDefault(x => x.Port == port);
                    if (gsm != null)
                    {
                        gsm.reset(port, row);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void copySĐTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                DataGridViewRow row = dataGSM.CurrentRow;
                string phone = row.Cells["sdt"].Value.ToString();
                if (phone == null)
                {
                    MessageBox.Show("Cổng này chưa có sim hoặc sim chưa có sóng.","Copy sđt",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }
                Clipboard.SetDataObject(phone);
                MessageBox.Show("Đã copy số điện thoại.");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void copyDanhSáchCổngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                DataGridViewRow row = dataGSM.CurrentRow;
                string port = row.Cells[1].Value.ToString();
                Clipboard.SetDataObject(port);
                MessageBox.Show("Đã copy port","Copy port",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }

        private void đóngCổngĐãChọnVàXóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGSM.CurrentRow;
                string port = row.Cells[1].Value.ToString();
                if (port == null)
                {
                    return;
                }
                GsmObject ob = gsmOject.SingleOrDefault(x => x.Port == port);
                if (ob != null)
                {
                    ob.Msg = "Port này đã đóng";
                    ob.Status = "Port Closed";
                    ob.Port = ob.Port;
                    ob.NetWork = "";
                    ob.Phone = "";
                    ob.TKC = "";
                    ob.Expiration_Date = "";
                    ob.HT = "";
                    ob.TKKM = "";
                    ob.sp.Close();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        private void guna2ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = @"D:\GSMInfo.xlsx";
                dialog.Title = "Save GSM Sim InFo To Excel";
                dialog.RestoreDirectory = true;
                dialog.DefaultExt = "xlsx";
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                string path = dialog.FileName;
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                while(app.Interactive)
                {
                    try
                    {
                        app.Interactive = false;
                    }
                    catch(Exception er)
                    {

                    }
                }
                try
                {
                    Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                    Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                    app.Visible = true;
                    worksheet = workbook.Sheets["Sheet1"];
                    worksheet = workbook.ActiveSheet;
                    worksheet.Name = "GSM Sim Info";
                    int count = 0;
                    for (int i = 1; i < 15; i++)
                    {  if(i==13)
                        {
                            continue;
                        }
                        worksheet.Cells[1, i] = dataGSM.Columns[i - 1].HeaderText;
                    }
                    for (int i = 0; i < dataGSM.RowCount; i++)
                    {
                        if (!string.IsNullOrEmpty(dataGSM.Rows[i].Cells[5].Value.ToString()) && dataGSM.Rows[i].Cells[5].Value.ToString() != "Loading")
                        {
                            count++;
                            for (int j = 0; j < 14; j++)
                            {
                                if (worksheet != null && dataGSM.Rows[i].Cells[j].Value != null)
                                {  if(j==12)
                                    {
                                        continue;
                                    }
                                    if (j == 0 || j == 6 || j == 5 || j == 10 || j==7|| j == 8 || j == 9)
                                    { if (j == 0)
                                        {
                                            worksheet.Cells[count + 1, j + 1] = count;
                                        }
                                        else
                                        {
                                            worksheet.Cells[count + 1, j + 1].NumberFormat = "@";
                                            worksheet.Cells[count + 1, j + 1] = dataGSM.Rows[i].Cells[j].Value.ToString();
                                        }
                                    }
                                    else
                                    {
                                        worksheet.Cells[count + 1, j + 1] = dataGSM.Rows[i].Cells[j].Value.ToString();
                                    }
                                }
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                        Microsoft.Office.Interop.Excel.Range range = worksheet.Range[$"H1:H{count+1}"];
                        range.WrapText = false;
                    Microsoft.Office.Interop.Excel.Range range_grid = worksheet.Range[$"A1:T{count + 1}"];
                    range_grid.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    range_grid.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;

                    // Apply bold vertical gridlines
                    range_grid.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    range_grid.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                    workbook.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    workbook.Close();
                    app.Quit();
                    
                }
                finally
                {
                    app.Interactive = true;
                }
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
        }

        private void gửiTinNhắnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGSM.CurrentRow;
            string phone = row.Cells["sdt"].Value.ToString();
            string telco = row.Cells["network"].Value.ToString();
            string port = row.Cells[1].Value.ToString();
            if (phone != "" && telco != "" && port != "")
            {
                string res = phone + "(" + telco + ")" + "-" + port;
                SendSmS smsForm = new SendSmS(res, row);
                smsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Cổng này chưa có sim hoặc sim chưa có sóng.");
            }
        }
        public void addNewMessageRow(string res)
        {  
           
                this.messageGrid.BeginInvoke(
                    new Action(() =>
                    {
                        try
                        {
                          
                            List<string> data = new List<string>();
                            if (res.Contains("~"))
                            {
                                data = res.Split('~').ToList();
                            }
                            else
                            {
                                data = res.Split('@').ToList();
                            }

                            try
                            {
                                string com = data[4];
                                if(!com.Contains("COM")) {
                                    return;
                                }
                                int first_index = data[3].IndexOf("0");
                                if(first_index==0)
                                {
                                    data[3] = data[3].Remove(first_index, 1).Insert(first_index, "84");
                                }
                                int index = this.messageGrid.Rows.Add();
                                this.messageGrid.Rows[index].Cells[1].Value = data[4];
                                this.messageGrid.Rows[index].Cells[3].Value = data[3];
                                this.messageGrid.Rows[index].Cells[4].Value = data[2];
                                this.messageGrid.Rows[index].Cells[5].Value = data[1];
                                this.messageGrid.Rows[index].Cells[2].Value = data[0];
                                this.messageGrid.Rows[index].Cells[6].Value = data[5];
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show(er.Message + "here");
                        }
                    }));
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void messageGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.messageGrid.Rows[e.RowIndex].Cells[0].Value = e.RowIndex + 1;
        }

        private void messageGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void guna2Button1_Click_2(object sender, EventArgs e)
        {
            this.messageGrid.BeginInvoke(new MethodInvoker(() =>
        {
            this.messageGrid.Rows.Clear();
            this.messageGrid.Refresh();
        }));
            foreach (string r in List_Receive)
            {
            
                addNewMessageRow(r);
            }
        }

        private void messageGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 2)
            {
                DateTime n1 = DateTime.ParseExact(e.CellValue1.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime n2 = DateTime.ParseExact(e.CellValue2.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                e.SortResult = n1.CompareTo(n2);
                e.Handled = true;
            }
        }

        private async Task RegisterHandling(object state)
        {
           
                try
                {
                    var phoneobj = (MyRegisObject)state;
                    int receive_otp = 0;
                    int send_otp = 0;
                    int otp_receive_time = 0;
                    DateTime time_out = DateTime.Now;
                    GsmObject gsm = gsmOject.SingleOrDefault(x => x.Phone == phoneobj.phone);
                    if (gsm == null)
                    {
                        return;
                    }
                    if (phoneCreated.Contains(phoneobj.phone))
                    {
                        return;
                    }
                    string token = Environment.GetEnvironmentVariable("TOKEN");
                    string network = gsm.NetWork;
                this.BeginInvoke(new MethodInvoker(() =>
                    {
                        gsm.Msg = gsm.loadMsg("Đang tiến hành lấy Otp");
                        gsm.Status = "Đang khởi tạo";
                    }));
                    string password = mypassword.Text;
                    RegisterMVP regis = new RegisterMVP(phoneobj.phone, password, phoneobj.carrier);
                    int first_place = regis.phone.IndexOf("0");
                    string phone_push = regis.phone.Remove(first_place, 1).Insert(first_place, "84");
                    var input_data = new Dictionary<string, string>()
            {
                { "phone", phone_push},
                { "password", regis.password },
                { "carrier", regis.carrier.ToString()},
            };
                    ResponseMVTRegister otpVal = await TelecomService.PostApiMVT<ResponseMVTRegister>("create-transaction", new FormUrlEncodedContent(input_data), token);
                    try
                    {
                        int times = 0;
                        if (otpVal != null)
                        {
                            if (!string.IsNullOrEmpty(otpVal.Message) && otpVal.Message == "Tạo giao dịch thành công")
                            {
                                DateTime port_now = DateTime.Now;
                                transactionID.Add(otpVal.Data.TranSactionId);
                        loop:
                            if (DateTime.Now.Subtract(time_out).TotalMinutes > 2)
                            {
                                if (network == "MOBIFONE"||network=="VNSKY")
                                { this.BeginInvoke(new MethodInvoker(() =>
                                {
                                    try
                                    {
                                        if (gsm != null)
                                        {
                                            gsm.Status = "Thành công";
                                            gsm.Msg = "Đã lấy được trust-device otp.";
                                            gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Green;
                                        }
                                    }
                                    catch(Exception er)
                                    {
                                        Console.WriteLine(er.Message);
                                    }
                                }
                                  ));
                            }
                            else
                            {
                                this.BeginInvoke(new MethodInvoker(() =>
                            {  if (gsm != null)
                                {
                                    gsm.Status = "Thất bại";
                                    gsm.Msg = "Quá thời gian xử lý.";
                                    gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Red;
                                }
                            }));
                            }
                                return;
                                    }
                                    var transaction_id = otpVal.Data.TranSactionId;
                                    var list_transaction_id = new Dictionary<string, string>
                                { { "list_transaction_id[0]",transaction_id} };
                                    ResponseStatusRegister status = await TelecomService.PostApiMVT<ResponseStatusRegister>("list-transaction", new FormUrlEncodedContent(list_transaction_id), token);
                                    if (status != null)
                                    {
                                        string status_id = status.Data[0].Status;
                                        if (status_id.Equals("3"))
                                        {
                                            if (gsm != null)
                                            {
                                        this.BeginInvoke(new MethodInvoker(() =>
                                            {
                                                try
                                                {
                                                    if (gsm != null)
                                                    {
                                                        gsm.Msg = gsm.loadMsg("Giao dịch thất bại");
                                                        gsm.Status = "Thất bại";
                                                        gsm.Transaction_Id = status.Data[0].Transaction_Id;
                                                        gsm.TimeProcess = status.Data[0].Time_Process;
                                                        gsm.StatusMy = status.Data[0].Status;
                                                        gsm.Note = status.Data[0].Note;
                                                        gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Red;
                                                    }
                                                }
                                                catch(Exception er)
                                                {
                                                    Console.WriteLine(er.Message);
                                                }
                                            }));
                                                send_otp = 1;
                                        return;
                                               
                                            }
                                        }
                                        else if (status_id.Equals("2"))
                                        {
                                            if (gsm != null)
                                            {
                                        this.BeginInvoke(new MethodInvoker(() =>
                                            {
                                                try
                                                {
                                                    if (gsm != null)
                                                    {
                                                        gsm.Msg = gsm.loadMsg("Giao dịch thành công");
                                                        gsm.Status = "Thành công";
                                                        gsm.Transaction_Id = status.Data[0].Transaction_Id;
                                                        gsm.TimeProcess = status.Data[0].Time_Process;
                                                        gsm.StatusMy = status.Data[0].Status;
                                                        gsm.Note = status.Data[0].Note;
                                                        gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Green;
                                                    }
                                                }
                                                catch(Exception er)
                                                {
                                                    Console.WriteLine(er.Message);
                                                }
                                            }));
                                                phoneCreated.Add(phoneobj.phone);
                                                send_otp = 1;
                                        return;
                                            }
                                        }
                                    }
                                    if (gsm != null)
                                    {
                                        if (send_otp == 0)
                                        {
                                            if (!string.IsNullOrEmpty(gsm.Otp))
                                            {
                                                otp_receive_time++;
                                        this.BeginInvoke(new MethodInvoker(() =>
                                        {   
                                            gsm.Msg = gsm.loadMsg("Đã nhận được OTP");
                                        }));
                                                string sms_received = gsm.Otp;
                                                string[] val = sms_received.Split('*');
                                                string from = val[0];
                                                string content = val[1];
                                                string telco_received_at = val[2];
                                                PushSmS sms = new PushSmS(phoneobj.phone, from, content, telco_received_at);
                                                var push_data = new Dictionary<string, string>()
                                {
                                { "phone", sms.phone },
                                { "from", sms.from },
                                { "content", sms.content },
                                { "telco_received_at", sms.telco_received_at }
                                };
                                                ResponseSmSPush smsrep = await TelecomService.PostApiMVT<ResponseSmSPush>("add-sms", new FormUrlEncodedContent(push_data), token);
                                                if (smsrep != null)
                                                {
                                                    if (smsrep.Message.Equals("Nhận thành công"))
                                                    {
                                                this.BeginInvoke(new MethodInvoker(() =>
                                                    {
                                                        gsm.Msg = gsm.loadMsg("Đã gửi Otp thành công");
                                                        gsm.Otp = "";
                                                    }));
                                                    }
                                                    else
                                                    {
                                                        send_otp = 1;
                                                this.BeginInvoke(new MethodInvoker(() =>
                                                {
                                                    gsm.Msg = gsm.loadMsg(smsrep.Message);
                                                    gsm.Status = "Thất bại";
                                                    gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Red;
                                                }));
                                                        if (status != null)
                                                        {
                                                    this.BeginInvoke(new MethodInvoker(() =>
                                                        {
                                                            gsm.Transaction_Id = status.Data[0].Transaction_Id;
                                                            gsm.TimeProcess = status.Data[0].Time_Process;
                                                            gsm.StatusMy = status.Data[0].Status;
                                                            gsm.Note = status.Data[0].Note;
                                                        }));
                                                        }
                                                return;
                                                    }
                                                }
                                            }
                                            else if (DateTime.Now.Subtract(port_now).TotalMilliseconds > 90000 && otp_receive_time == 0)
                                            {
                                        if (gsm != null)
                                        {
                                            this.BeginInvoke(new MethodInvoker(() =>
                                                {
                                                    gsm.Msg = gsm.loadMsg("Lỗi không nhận đc Otp");
                                                    gsm.Status = "Thất bại";
                                                    gsm.Transaction_Id = status.Data[0].Transaction_Id;
                                                    gsm.TimeProcess = status.Data[0].Time_Process;
                                                    gsm.StatusMy = status.Data[0].Status;
                                                    gsm.Note = status.Data[0].Note;
                                                    gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Red;
                                                }));
                                        }
                                                gsm = null;
                                                send_otp = 1;
                                        return;
                                            }
                                        }

                                    }
                            await Task.Delay(1000);
                            goto loop;   
                            }
                            else if (!string.IsNullOrEmpty(otpVal.Message) && otpVal.Message != "Tạo giao dịch thành công")
                        {
                            if (gsm != null)
                            {
                                this.BeginInvoke(new MethodInvoker(() =>
                                {
                                    gsm.Msg = gsm.loadMsg("Bị lỗi hoặc đã tạo tài khoản");
                                    gsm.Status = "Thất bại";
                                    gsm.rowGSMSelect.Cells["status"].Style.BackColor = Color.Red;


                                }));
                            }
                            gsm = null;
                        }
                        }
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (Exception er)
                {
                    Console.WriteLine(er.Message);
                }
           
         
            }
        public void addUpdateMyPasswordSetting(string key, string value)
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                xml.SelectSingleNode("//mypasswordSet").Attributes[key].Value = value;
                xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                ConfigurationManager.RefreshSection("mypasswordSet");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public int countClosePort()
        {
            int count = 0;
            foreach(var port in gsmOject)
            {
                if(!port.sp.IsOpen)
                {
                    count++;
                }
            }
            return count;
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            isActivated = false;
            try
            {
                addUpdateMyPasswordSetting("mypassword", this.mypassword.Text);
            }   
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            if (dataGSM.RowCount < new_port_num || dataGSM.RowCount == 0)
            {
                MessageBox.Show("Không có port nào được phát hiện.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(mypassword.Text))
            {
                MessageBox.Show("Mật khẩu tạo My* không được để trống.","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if(mypassword.Text.Length<6||mypassword.Text.Length>20)
            {
                MessageBox.Show("Mật khẩu phải có độ dài từ 6 đến 20 ký tự","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (is_start_regis == 0)
            {
                Image image = Properties.Resources.stop_button__1_;
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.beginBtn.Image = image;
                    this.beginBtn.Text = "Dừng tạo My*";
                    this.mypassword.Enabled = false;
                }));
                is_start_regis = 1;
                time_load_regis_my = new System.Timers.Timer();
                time_load_regis_my.Interval = 3000;
                time_load_regis_my.Elapsed += async(send, ev) =>
                {
                 
                    List<DataGridViewRow> rows = dataGSM.Rows.Cast<DataGridViewRow>().ToList();
                    foreach (var row in rows)
                    {
                        if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()) && row.Cells["sdt"].Value.ToString() != "Loading" && (row.Cells["status"].Value.ToString().Equals("Sẵn sàng") || row.Cells["status"].Value.ToString().Equals("Try Open Port")))
                        {
                            new Task(async () =>
                        {
                            try
                            {
                                string phone = row.Cells["sdt"].Value.ToString();
                                string telco = row.Cells["network"].Value.ToString();
                                string port = row.Cells["port"].Value.ToString();

                                GsmObject gsm = instance.gsmOject.SingleOrDefault(p => p.Port == port);
                                if (gsm != null)
                                {
                                    if (telco == "VIETTEL")
                                    {
                                        int value = await gsm.RegisterHandling(phone, 0) ? 1 : 0;
                                    }
                                    else if (telco == "MOBIFONE" || telco == "VNSKY")
                                    {
                                        int value = await gsm.RegisterHandling(phone, 2) ? 1 : 0;
                                    }
                                    else if (telco == "VINAPHONE")
                                    {
                                        int value = await gsm.RegisterHandling(phone, 1) ? 1 : 0;
                                    }
                                    else if (telco == "VIETNAMOBILE")
                                    {
                                        int value = await gsm.RegisterHandling(phone, 3) ? 1 : 0;
                                    }
                                }
                                gsm = null;
                            }
                            catch (Exception er)
                            {
                                Console.WriteLine(er.Message);
                            }
                        }).Start();
                        }
                        await Task.Delay(100);
                    }
                    };
                    time_load_regis_my.Start();
                }
            else
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        Image image = Properties.Resources.play__1_;
                        this.beginBtn.Image = image;
                        this.beginBtn.Text = "Bắt đầu tạo My*";
                        this.mypassword.Enabled = true;
                    }));
                is_start_regis = 0;
                time_load_regis_my.Stop();
                time_load_regis_my.Dispose();
                }
        }
        private void statusGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
      
        private void tạoTKMyViettelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string phone = "";
            if (string.IsNullOrEmpty(mypassword.Text))
            {
                MessageBox.Show("Mật khẩu tạo My* không được để trống.","Mật khẩu trống",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                Parallel.ForEach(selectedRows, row =>
                {
                    string telco = row.Cells["network"].Value.ToString();
                    if (!string.IsNullOrEmpty(row.Cells["note"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["note"].Value = "";
                        }));
                    }
                    if (telco != "VIETTEL")
                    {
                        MessageBox.Show("Chức năng này chỉ tạo MyViettel.","MyViettel",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                    if (row.Cells["status"].Value.ToString() == "Đang khởi tạo")
                    {
                        MessageBox.Show("Sim này đang đăng ký My*","Đang đăng ký",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                        return;
                    }
                    if (phoneCreated.Contains(row.Cells["sdt"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                        row.Cells["status"].Value = "Thành công";
                        row.Cells["msg"].Value = "Sim này đã tạo My* thành công";
                        row.Cells["status"].Style.BackColor = Color.LightGreen;
                    }));
                        return;
                    }
                    if (telco == "VIETTEL")
                    {
                        if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()) && row.Cells["sdt"].Value.ToString() != "Loading")
                        {
                            phone = row.Cells["sdt"].Value.ToString();
                            createService(phone, 0);
                        }
                    }
                });
        }
        public class MyRegisObject
        {
            public string phone { get; set; }
            public int carrier { get; set; }
            public MyRegisObject(string phone, int carrier)
            {
                this.phone = phone;
                this.carrier= carrier;
            }
        }
        private void delallsmsbtn_Click(object sender, EventArgs e)
        {
            this.messageGrid.BeginInvoke(new MethodInvoker(() =>
            {
                this.messageGrid.Rows.Clear();
                this.messageGrid.Refresh();
            }));
        }
        private void guna2Button1_Click_3(object sender, EventArgs e)
        {
            this.Hide();
            this.instance_login.Show();
        }
        
        private  void tạoTKMyMobifoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string phone = "";
            if (string.IsNullOrEmpty(mypassword.Text))
            {
                MessageBox.Show("Mật khẩu tạo My* không được để trống.", "Mật khẩu trống", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
          
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                Parallel.ForEach(selectedRows, row =>
                {
                    string telco = row.Cells["network"].Value.ToString();
                    if (!string.IsNullOrEmpty(row.Cells["note"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["note"].Value = "";
                        }));
                    }
                    if (telco != "MOBIFONE" && telco!="VNSKY")
                    {
                        MessageBox.Show("Chức năng này chỉ tạo MyMobi.", "MyMobiFone", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                  
                    if (row.Cells["status"].Value.ToString() == "Đang khởi tạo")
                    {
                        MessageBox.Show("Sim này đang đăng ký My*", "Đang đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (phoneCreated.Contains(row.Cells["sdt"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["status"].Value = "Thành công";
                            row.Cells["msg"].Value = "Sim này đã tạo My* thành công";
                            row.Cells["status"].Style.BackColor = Color.LightGreen;
                        }));
                        return;
                    }

                    if (telco == "MOBIFONE" || telco == "VNSKY")
                    {
                        if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()) && row.Cells["sdt"].Value.ToString() != "Loading")
                        {
                            phone = row.Cells["sdt"].Value.ToString();
                            createService(phone, 2);
                        }
                    }

                });
        }
        public void createService(string phone,int carrier)
        {
            GsmObject gsm = instance.gsmOject.Find(p => p.Phone == phone);
            try
            {
                if (gsm != null)
                {
                    new Task(async () => {
                        int value = await gsm.RegisterHandling(phone,carrier) ? 1 : 0;
                        gsm = null;
                    }).Start();
                }
            }
            catch(Exception ex) { Console.WriteLine(ex.Message);}
        }

        private void guna2Button2_Click_1(object sender, EventArgs e)
        {  if(!openPort)
            {
                MessageBox.Show("Lỗi lưu cấu hình","Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            updatePortSetting(dataGSM);
            MessageBox.Show("Đã lưu cấu hình port");
        }

        private void mypassword_TextChanged(object sender, EventArgs e)
        {
            this.mypassword.SelectionStart = this.mypassword.TextLength;
        }

        private void tạoTKMyVinaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string phone = "";
            if (string.IsNullOrEmpty(mypassword.Text))
            {
                MessageBox.Show("Mật khẩu tạo My* không được để trống.", "Mật khẩu trống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                Parallel.ForEach(selectedRows, row =>
                {
                    string telco = row.Cells["network"].Value.ToString();
                    if (!string.IsNullOrEmpty(row.Cells["note"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["note"].Value = "";
                        }));
                    }
                    if (telco != "VINAPHONE")
                    {
                        MessageBox.Show("Chức năng này chỉ tạo MyVina.", "MyVina", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    if (row.Cells["status"].Value.ToString() == "Đang khởi tạo")
                    {
                        MessageBox.Show("Sim này đang đăng ký My*", "Đang đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (phoneCreated.Contains(row.Cells["sdt"].Value.ToString()))
                    {   
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                        row.Cells["status"].Value = "Thành công";
                        row.Cells["msg"].Value = "Sim này đã tạo My* thành công";
                        row.Cells["status"].Style.BackColor = Color.LightGreen;
                    }));
                        return;
                    }

                    if (telco == "VINAPHONE")
                    {
                        if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()) && row.Cells["sdt"].Value.ToString() != "Loading")
                        {
                            phone = row.Cells["sdt"].Value.ToString();
                            createService(phone, 1);
                        }
                    }

                });
        }

        private void tạoTKMyVietnammobileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string phone = "";
            if (string.IsNullOrEmpty(mypassword.Text))
            {
                MessageBox.Show("Mật khẩu tạo My* không được để trống.", "Mật khẩu trống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
          
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                Parallel.ForEach(selectedRows, row =>
                {
                    string telco = row.Cells["network"].Value.ToString();
                    if (!string.IsNullOrEmpty(row.Cells["note"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["note"].Value = "";
                        }));
                    }
                    if (telco != "VIETNAMOBILE")
                    {
                        MessageBox.Show("Chức năng này chỉ tạo MyVietnamobile.", "MyVietnamobile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (row.Cells["status"].Value.ToString() == "Đang khởi tạo")
                    {
                        MessageBox.Show("Sim này đang đăng ký My*", "Đang đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (phoneCreated.Contains(row.Cells["sdt"].Value.ToString()))
                    {
                        this.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            row.Cells["status"].Value = "Thành công";
                            row.Cells["msg"].Value = "Sim này đã tạo My* thành công";
                            row.Cells["status"].Style.BackColor = Color.LightGreen;
                        }));
                        return;
                    }

                    if (telco == "VIETNAMOBILE")
                    {
                        if (!string.IsNullOrEmpty(row.Cells["sdt"].Value.ToString()) && row.Cells["sdt"].Value.ToString() != "Loading")
                        {
                            phone = row.Cells["sdt"].Value.ToString();
                            createService(phone, 3);
                        }
                    }

                });
        }

        private void filter_btn_Click(object sender, EventArgs e)
        {
            this.messageGrid.BeginInvoke(new MethodInvoker(() =>
            {
                string phone_filter = this.filter_txtbox.Text.ToString();
                int first_index = phone_filter.IndexOf("0");
                if(first_index==0)
                {
                    phone_filter= phone_filter.Remove(first_index, 1).Insert(first_index, "84");
                    List<DataGridViewRow> rows = new List<DataGridViewRow>();
                    foreach(DataGridViewRow row in this.messageGrid.Rows)
                    {
                        string to_phone = row.Cells["to"].Value.ToString();
                        if(to_phone.Equals(phone_filter))
                        {
                            rows.Add(row);
                        }
                    }
                    if(rows.Count>0)
                    { this.messageGrid.Rows.Clear();
                        this.messageGrid.Refresh();
                        foreach(var row in rows)
                        {
                            this.messageGrid.Rows.Add(row);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy số điện thoại này hoặc không có tin nhắn gửi tới.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    List<DataGridViewRow> rows = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in this.messageGrid.Rows)
                    {
                        string to_phone = row.Cells["to"].Value.ToString();
                        if (to_phone.Equals(phone_filter))
                        {
                            rows.Add(row);
                        }
                    }
                    if (rows.Count > 0)
                    {
                        this.messageGrid.Rows.Clear();
                        this.messageGrid.Refresh();
                        foreach (var row in rows)
                        {
                            this.messageGrid.Rows.Add(row);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy số điện thoại này hoặc không có tin nhắn gửi tới.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }));
        }

        private void filter_txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                this.filter_btn.PerformClick();
            }
        }

        public async Task ChangeImeiPort()
        {
            try
            {
                List<DataGridViewRow> selectedRows = dataGSM.SelectedRows.Cast<DataGridViewRow>().ToList();
                var tasks = new List<Task>();
                foreach (DataGridViewRow row in selectedRows)
                {
                    string telco = row.Cells["network"].Value.ToString();
                    string imei = row.Cells["imei"].Value.ToString();
                    string phone = row.Cells["sdt"].Value.ToString();
                    if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(imei))
                    {
                        tasks.Add(
                         Task.Factory.StartNew(async () =>
                         {
                             try
                             {
                                 GsmObject gsm = gsmOject.SingleOrDefault(p => p.Phone == phone);
                                 if (gsm != null)
                                 {
                                     Random random = new Random();
                                     int idx = random.Next(1, List_Imei.Count);
                                     int checksum_digit = random.Next(1, 10);
                                     string imei_change = phone_imei_ob.generatePhoneImeiNumber(List_Imei[idx], checksum_digit);
                                     int val = await gsm.changeImei(imei_change);
                                 }
                                 gsm = null;
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine(ex.Message);
                             }
                         })
                        );
                        await Task.Delay(1000);
                    }
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
            }
        }


        private async void imeiBtn_Click(object sender, EventArgs e)
        {
           await ChangeImeiPort();
        }
    }
}