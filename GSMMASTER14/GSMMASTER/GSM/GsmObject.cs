using GsmComm.PduConverter;
using GsmComm.PduConverter.SmartMessaging;
using GSMMASTER.MVTAccount;
using GSMMASTER.Services;
using GSMMASTER.ToolsForm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Util;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static GSMMASTER.Form1;
using static System.Net.Mime.MediaTypeNames;

namespace GSMMASTER.GSM
{
    public class GsmObject
    {
        public string data = "";
        public SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        public SerialPort sp = new SerialPort();
        private string port;
        private string network;
        private string phone;
        private string status;
        private string _tkc;
        private string expiration_date;
        private string ht;
        private string tkkm;
        private string serial_sim;
        private string imei;
        private string msg;
        private bool _isSIM;
        private bool isreadport;
        private bool _isPhone;
        private bool _isInfor;
        private string text = "";
        public string readPortSmS = "";
        private bool check_11_dig = false;
        private bool CheckSimReadyHasResult = false;
        private bool CarrierHasResult = false;
        private string _otp;
        private object lockReadPort = new object();
        private SemaphoreSlim lockWritePort = new SemaphoreSlim(1,5);
        private SemaphoreSlim lockChangeImeiPort = new SemaphoreSlim(1, 1);
        private SemaphoreSlim lockSMSPort = new SemaphoreSlim(1, 1);
        private SemaphoreSlim lockRegisterPort=new SemaphoreSlim(10);
        private SemaphoreSlim registerSemaphore = new SemaphoreSlim(1);
        private DateTime lastReportPhone = DateTime.MinValue;
        private DateTime lastReportInfo = DateTime.MinValue;
        private DateTime lastReportNetwork = DateTime.MinValue;
        private DateTime lastReportSerialData = DateTime.MinValue;
        private DateTime lastSmSReport = DateTime.MinValue;
        private DateTime timeOutExist = DateTime.Now;
        private DateTime lastSerialSimReport = DateTime.MinValue;
        private DateTime lastReportImei = DateTime.MinValue;
        private string transaction_id;
        private string time_process;
        private string note;
        private string status_my;
        public string sms;
        public string send_from;
        public string receive_date;
        private string otp_receive_times;
        public DataGridViewRow rowGSMSelect = new DataGridViewRow();
        public Form1 instance = Form1.ReturnInstance();
        public ReceiveSmS instance_sms = ReceiveSmS.ReturnInstance();
        public Setting instance_setting = Setting.ReturnInstance();
        private string current_sms;
        private int oldindexsms;
        private List<SmsPdu> _smsPduList = new List<SmsPdu>();
        private bool isFailSendSMS;
        private bool smsSuccess;
        private bool delallsms = true;
        private bool isTKKM;
        private DateTime lastTKKMReport = DateTime.MinValue;
        private bool stop_loop = false;
        public string phone_temp = "";
        public string phone_secondary_temp = "";
        private CancellationTokenSource cts;
        private bool running_loop=true;
        private bool isSerial;
        private bool is_imei;
        private bool is_change_imei = false;
        private static Dictionary<string, SemaphoreSlim> instanceSemaphores = new Dictionary<string, SemaphoreSlim>();
        public RealPhoneImei phone_imei = new RealPhoneImei();
        public Task task { get; private set; }
        public GsmObject(string port, DataGridViewRow row)
        { 
            this.rowGSMSelect = row;
            this.Port = port;
            this.NetWork = "";
            this.Phone = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Expiration_Date = "";
            this.Msg = this.loadMsg("Đang lấy thông tin");
            this.Serial_Sim = "";
            this.HT = "";
            this.TKKM = "";
            this.Imei = "";
            this.Transaction_Id = "";
            this.TimeProcess = "";
            this.StatusMy = "";
            this.Note = "";
            this.Otp_Receive = "";
            
            cts = new CancellationTokenSource();
            this.sp = new SerialPort()
            {
                PortName = port,
                BaudRate = int.Parse(instance.baudrate),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.RequestToSend,
                NewLine = "\r\n",
                WriteTimeout = 1000,
                ReadTimeout = 1000
            };
       
                this.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                this.task = Task.Run(() => this.Work(), cts.Token);
        }
        public GsmObject(string port)
        {
            this.rowGSMSelect = instance.newRow();
            this.Port = port;
            this.NetWork = "";
            this.Phone = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Expiration_Date = "";
            this.Msg = this.loadMsg("Đang lấy thông tin");
            this.HT = "";
            this.Serial_Sim = "";
            this.TKKM = "";
            this.Imei = "";
        }
        public string Port
        {
            get => this.port;
            set
            {
                this.port = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "port", value);
            }
        }
        public string Otp_Receive
        {
            get => this.otp_receive_times;
            set
            {
                this.otp_receive_times = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "otp_receive", value);
            }
        }
        public string NetWork
        {
            get => this.network;
            set
            {
                this.network = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "network", value);
            }
        }
        public string Phone
        {
            get => this.phone;
            set
            {
                this.phone = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "sdt", value);
            }
        }
        public string TKC
        {
            get => this._tkc;
            set
            {
                this._tkc = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "tkc", value);
            }
        }
        public string Expiration_Date
        {
            get => this.expiration_date;
            set
            {
                this.expiration_date = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "hsd", value);
            }
        }
        public string Status
        {
            get => this.status;
            set
            {
                this.status = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "status", value);
            }
        }
        public string HT
        {
            get => this.ht;
            set
            {
                this.ht = value;
            }
        }
        public string TKKM
        {
            get => this.tkkm;
            set
            {
                this.tkkm = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "tkkm", value);
            }
        }

        public string Msg
        {
            get => this.msg;
            set
            {
                this.msg = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "msg", value);
            }
        }
        
        public string Serial_Sim
        {
            get => this.serial_sim;
            set
            {
                this.serial_sim = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "iccid", value);
            }
        }
        public string Imei
        {
            get => this.imei;
            set
            {
                this.imei = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "imei", value);
            }
        }
        public string Otp
        {
            get => this._otp;
            set
            {
                this._otp = value;
                if (string.IsNullOrEmpty(this._otp))
                {
                    return;
                }
            }
        }
        public string SMS
        {
            get => this.sms;
            set
            {
                this.sms = value;
            }
        }
        public string Transaction_Id
        {
            get => this.transaction_id;
            set
            {
                this.transaction_id = value;
            }
        }
        public string TimeProcess
        {
            get => this.time_process;
            set
            {
                this.time_process = value;
            }
        }
        public string StatusMy
        {
            get => this.status_my;
            set
            {
                this.status_my = value;
            }
        }
        public string Note
        {
            get => this.note;
            set
            {
                this.note = value;
                UpdateGUI.ChangeRow(rowGSMSelect, "note", value);
            }
        }
        public string Send_From
        {
            get => this.send_from;
            set
            {
                this.send_from = value;
            }
        }
        public string Receive_Date
        {
            get => this.receive_date;
            set
            {
                this.receive_date = value;
            }
        }
       
        public string loadMsg(string msg)
        {
            this.Msg = "";
            string res = "[" + DateTime.Now + "] " + msg;
            return res;
        }
        public DataGridViewRow selectRowSMS()
        {
            DataGridViewRow row = new DataGridViewRow();
            try
            {
                foreach (DataGridViewRow r in instance_sms.receiveGSM.Rows)
                {
                    if (r.Cells["port"].Value.ToString().Equals(this.Port))
                    {
                        row = r;
                        break;
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            return row;
        }

        public void reset(string port, DataGridViewRow row)
        {
            this.rowGSMSelect = row;
            this.Port = port;
            this.NetWork = "";
            this.Phone = "";
            this.Status = "Waiting";
            this.TKC = "";
            this.Expiration_Date = "";
            this.Msg = this.loadMsg("Đang lấy thông tin");
            this.HT = "";
            this.TKKM = "";
            this.Otp = "";
            this.Serial_Sim = "";
            this.Transaction_Id = "";
            this.TimeProcess = "";
            this.StatusMy = "";
            this.Note = "";
            this.CarrierHasResult = false;
            cts.Cancel();
            this.sp.BaudRate = int.Parse(this.instance.baudrate);
            this.sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            this.task = Task.Run(() => this.Work(), cts.Token);
        }
        public void stopPort()
        {
            running_loop = false;
        }
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (!Monitor.TryEnter(this.lockReadPort, 2000))
            {
                return;
            }
            try
            {  
                this.data += this.sp.ReadExisting();
                if (this.data.EndsWith("\n") || this.data.EndsWith("\r"))
                {
                    this.lastReportSerialData = DateTime.Now;
                    if (this.data.Contains("+CUSD:"))
                    {
                        if (this.data.EndsWith("OK\r\n"))
                        {
                            this.HandleSerialData(data);
                            data = "";
                        }
                    }
                    else
                    {
                        this.HandleSerialData(data);
                        data = "";
                    }
                }

            }
            catch (Exception er)
            {
                Console.WriteLine(er);
            }
            finally
            {
                Monitor.Exit(this.lockReadPort);
            }
        }
        public DataGridViewRow GSMSelectRow()
        {
            DataGridViewRow row = new DataGridViewRow();
            try
            {
                foreach (DataGridViewRow r in this.instance.dataGSM.Rows)
                {
                    if (r.Cells["port"].Value.ToString().Equals(this.Port))
                    {
                        row = r;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return row;
        }
        public async Task sendAT(string command)
        {
            await this.lockWritePort.WaitAsync();
            try
            {
                this.sp.WriteLine(command);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                this.lockWritePort.Release();
            }
        }
        public bool TryOpenPort()
        {
            try
            {
                if (!sp.IsOpen)
                {
                    this.sp.Open();
                }
                this.sp.DiscardInBuffer();
                this.sp.DiscardOutBuffer();
                return this.sp.IsOpen;
            }
            catch (Exception e)
            {

                return false;
            }
        }
        public async Task CheckSimReady()
        {

            await this.semaphoreSlim.WaitAsync();
            try
            {
                this.CheckSimReadyHasResult = false;
                this._isSIM = true;
                await this.sendAT("AT+CPIN?");
                await this.sendAT("AT+CGMI?");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task RunUSSD(string command)
        {
            try
            {
                await this.sendAT("AT+CMGF=0");
                await this.sendAT("AT+CUSD=2");
                await this.sendAT("AT+CUSD=1,\"" + command + "\",15\r");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task LockSms(int timeout)
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
        public async Task<bool> SendSms(string phone, string msg)
        {
            this.readPortSmS = "Chờ tác vụ";
            bool res = false;
            await this.lockWritePort.WaitAsync();
            try
            {
                this.readPortSmS = "Delay SmS";
                await this.instance.Lock(2);
                try
                {
                    this.isFailSendSMS = true;
                    this.readPortSmS = "Đang gửi tin nhắn SMS";
                    this.isreadport = true;
                    this.sp.Write("AT+CMGF=1" + Environment.NewLine);
                    this.sp.Write("AT+CMGS=\"" + phone + "\"\n");
                    this.sp.Write(msg);
                    this.sp.Write(new byte[1] { 26 }, 0, 1);
                    for (int timeWait = 0; timeWait < 25000 & this.isreadport; timeWait += 100)
                        await Task.Delay(100);
                    if (!this.isreadport && !this.isFailSendSMS)
                    {
                        res = true;
                    }
                    else
                    {
                        this.readPortSmS = "Gửi thất bại";
                    }
                }
                catch (Exception er)
                {
                    this.readPortSmS = "Gửi thất bại";
                }
                this.isreadport = false;
                this.phone = null;
                this.msg = null;
            }
            finally
            {
                this.lockWritePort.Release();
            }
            return res;
        }

        public async Task<int> changeImei(string imei)
        { 
            int res = -1;
            await lockChangeImeiPort.WaitAsync();
            try
            {  
                res = 0;
                is_change_imei = true;
                await this.instance.Lock(2);
                await this.sendAT($"AT+EGMR=1,7,\"{imei}\"");
                for(int i=0;(i<10000 && is_change_imei==true);i+=100)
                {
                    await Task.Delay(100);
                }
                if(is_change_imei==false)
                {
                    res = 1;
                }
                is_change_imei = false;
                return res;
            }
            catch(Exception er)
            {
                Console.WriteLine(er.Message);
            }
            finally
            {
                this.lockChangeImeiPort.Release();   
            }
            return res;
        }
        public string ConvertToDecimal(string input)
        {
            int value = Convert.ToInt32(input);
            return value.ToString("C2").Replace("$", "");
        }
        public string otpFilter(string txt1, string txt2)
        {
            string str = "";
            if ((txt2 == "MyViettel" && txt1.Contains("OTP")) || (txt2 == "VTMONEY" && txt1.Contains("OTP")))
            {
                str = new Regex("OTP\\s(\\d{4}).*").Match(txt1).Groups[1].ToString();
            }
            else
            {
                for (int i = 8; i > 3; i--)
                {
                    Match match = new Regex("(\\d{i})".Replace("i", i.ToString())).Match(txt1);
                    if (match.Groups.Count > 1)
                    {
                        str = match.Groups[1].ToString();
                        break;
                    }
                }
            }
            return str;
        }
        public void deleteMessageByIndex()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.sendAT(string.Format("AT+CMGD={0}\r", this.oldindexsms));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

       public string parseStringDigit(string iccid)
        {
            if(string.IsNullOrEmpty(iccid))
            {
                return "";
            }
            Regex reg = new Regex("[^0-9]");
            string res = reg.Replace(iccid, string.Empty);
            return res;
        }
        public async Task<bool> RegisterHandling(string phone,int carrier)
        {
          
            try
            {
                int receive_time = 0;
                int send_otp = 0;
                int otp_receive_time = 0;
                bool stop_receive = false;
                this.Otp_Receive = otp_receive_time.ToString();
                phone_temp = phone;
                DateTime time_out = DateTime.Now;
                this.Otp = "";
                if (this.instance.phoneCreated.Contains(phone))
                {
                    this.Status = "Thành công";
                    this.Msg = this.loadMsg("Sim này đã tạo My* thành công");
                    this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                    {
                        {
                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.LightGreen;
                        }
                    }));
                    return false;
                }
                string token = Environment.GetEnvironmentVariable("TOKEN");
                string network = this.NetWork;
                this.Msg = this.loadMsg("Đang tiến hành lấy Otp");
                this.Status = "Đang khởi tạo";
                this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                {
                    {
                        this.rowGSMSelect.Cells["status"].Style.BackColor = Color.CadetBlue;
                    }
                }));
                string password = this.instance.mypassword.Text;
                RegisterMVP regis = new RegisterMVP(phone, password, carrier);
                int first_place = regis.phone.IndexOf("0");
                string phone_push = regis.phone.Remove(first_place, 1).Insert(first_place, "84");
                var input_data = new Dictionary<string, string>()
            {
                { "phone", phone_push},
                { "password", regis.password },
                { "carrier", regis.carrier.ToString()},
            };
             ResponseMVTRegister otpVal = await TelecomService.PostApiMVT<ResponseMVTRegister>("create-transaction", new FormUrlEncodedContent(input_data), token);
             await Task.Delay(1000); 
                try
                {
                    if (otpVal != null)
                    {
                        if (!string.IsNullOrEmpty(otpVal.Message) && otpVal.Message == "Tạo giao dịch thành công")
                        {
                        DateTime port_now = DateTime.Now;
                        loop:
                           
                            if (DateTime.Now.Subtract(time_out).TotalMinutes>13&&otp_receive_time>0)
                            {    
                                    this.Status = "Thất bại";
                                    this.Msg = "Quá thời gian xử lý.";
                                    this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                                    }));
                                otpVal = null;
                                return false;
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
                                    try
                                    {
                                        this.Msg = this.loadMsg("Giao dịch thất bại");
                                        this.Status = "Thất bại";
                                        this.Note = status.Data[0].Note;
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                                        }));
                                    }
                                    catch (Exception er)
                                    {
                                        Console.WriteLine(er.Message);
                                    }
                                        send_otp = 1;
                                   
                                    otpVal = null;
                                    status = null;
                                        return false;
                                    }
                                
                                else if (status_id.Equals("2"))
                                {       
                                            try
                                            {
                                               
                                            this.Msg = this.loadMsg("Giao dịch thành công");
                                            this.Status = "Thành công";
                                            this.Note= status.Data[0].Note;
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.LightGreen;
                                        }));
                                            }
                                            catch (Exception er)
                                            {
                                                Console.WriteLine(er.Message);
                                            }
                                    this.instance.phoneCreated.Add(phone);
                                        send_otp = 1;
                                    status = null;
                                    otpVal = null;
                                        return true;
                                    }
                                }
                           
                                if (send_otp == 0)
                                {
                                    if (!string.IsNullOrEmpty(this.Otp))
                                    {
                                        string sms_received = this.Otp;
                                        string[] val = sms_received.Split('~');
                                        string from = val[0];
                                        string content = val[1];
                                        string telco_received_at = val[2];
                                      if(!content.Contains("OTP"))
                                    {
                                        this.Msg = loadMsg("Nhận Otp thất bại");
                                        this.Status = "Thất bại";
                                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                        {
                                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                                        }));
                                        otpVal = null;
                                        status = null;
                                        return false;
                                    }
                                    PushSmS sms = new PushSmS(phone, from, content, telco_received_at);
                                    var push_data = new Dictionary<string, string>()
                                {
                                { "phone", sms.phone },
                                { "from", sms.from },
                                { "content", sms.content },
                                { "telco_received_at", sms.telco_received_at}
                                };
                                 ResponseSmSPush smsrep = await TelecomService.PostApiMVT<ResponseSmSPush>("add-sms", new FormUrlEncodedContent(push_data), token);
                                    if (smsrep != null)
                                    {    
                                        otp_receive_time++;
                                        this.Otp_Receive = otp_receive_time.ToString();
                                        if (smsrep.Message.Equals("Nhận thành công"))
                                            {
                                                
                                                this.Msg = this.loadMsg("Đã gửi Otp thành công");
                                                this.Otp = "";
                                                
                                            }
                                            else
                                            {
                                                    send_otp = 1;                                                
                                                    this.Msg = this.loadMsg(smsrep.Message);
                                                    this.Status = "Thất bại";
                                                   this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                            {
                                                this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                                            }));
                                           otpVal = null;
                                            smsrep = null;
                                           return false;
                                            }
                                        smsrep = null;
                                        }
                                    }
                                    else if (DateTime.Now.Subtract(port_now).TotalMinutes > 10 && otp_receive_time == 0)
                                    {
                                     this.Msg = this.loadMsg("Lỗi không nhận đc Otp");
                                     this.Status = "Thất bại";
                                     this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                                    {
                                        this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                                    }));
                                        send_otp = 1;
                                    otpVal = null;
                                    return false;
                                    }
                                }
                            await Task.Delay(5000);
                            goto loop;
                        }
                     else if(otpVal.Success=="false")
                        {
                            this.Msg = this.loadMsg(otpVal.Message);
                            this.Status = "Thất bại";
                            this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                            {
                                this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                            }));
                            otpVal = null;
                            return false;
                        }
                    }
                    else
                    {
                        this.Msg = this.loadMsg("Khởi tạo giao dịch thất bại");
                        this.Status = "Thất bại";
                        this.instance.dataGSM.BeginInvoke(new MethodInvoker(() =>
                        {
                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.IndianRed;
                        }));
                        return false;
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
          return true;
        }

        public void ReadSmsInbox()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.sendAT("AT+CMGF=0");
                    await this.sendAT(string.Format("AT+CMGR={0}",this.oldindexsms));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }
        public void HandleSerialData(string input)
        {
            try
            {
                this.lastReportSerialData = DateTime.Now;
                if (input.Contains("+CMGS: ") && this.isreadport && this.isFailSendSMS)
                {
                    this.isreadport = false;
                    this.isFailSendSMS = false;
                    this.readPortSmS = "Gửi SMS thành công";
                }
                else if (input.Contains("CMS ERROR") && this.isreadport && this.isFailSendSMS)
                {

                    this.isreadport = false;
                    this.isFailSendSMS = true;
                    this.readPortSmS = "Gửi SMS thất bại";
                }
                if (input.Contains("+CPIN: READY") && !this._isSIM)
                {
                    this.Status = "Sẵn sàng";
                    this._isSIM = true;
                    this.CheckSimReadyHasResult = true;
                    this.Msg = loadMsg("Sim đã sẵn sàng");
                    return;
                }
                else if (input.Contains("+CPIN: READY") && this._isSIM)
                {
                    this.timeOutExist = DateTime.Now;
                }
                if (input.Contains("+CPIN: NOT READY") || (input.Contains("+CME ERROR: 10") && !input.Contains("+CME ERROR: 100")))
                {
                    this._isSIM = true;
                    this.CheckSimReadyHasResult = false;
                    this.CarrierHasResult = false;
                    this.lastReportInfo = DateTime.MinValue;
                    this.lastReportNetwork = DateTime.MinValue;
                    this.lastReportPhone = DateTime.MinValue;
                    this.Phone = "";
                    this.NetWork = "";
                    this.Expiration_Date = "";
                    this.Status = "No Sim";
                    this.HT = "";
                    this.Msg = loadMsg("Không nhận dạng được SIM");
                    this.TKC = "";
                    this.TKKM = "";
                }
                if(input.Trim().Length==2 && input.Trim()=="OK" && is_change_imei==true)
                {
                    
                    is_change_imei = false;
                    this.Imei = "";
                }
                if(input.Replace("OK","").Trim().Length==15)
                {
                    this.Imei = input.Replace("OK", "");
                }
                if (input.Contains("+ICCID:"))
                {
                    string[] iccid_values = input.Replace("OK","").Replace("\n","").Replace("\r","").Replace("f","").Split(':');
                    string iccid = iccid_values[1];
                    if(!string.IsNullOrEmpty(iccid))
                    {
                        this.Serial_Sim =parseStringDigit(iccid);
                    }
                }
             
                if (input.Contains("+COPS:"))
                {
                    string str = input.Replace(" ", "").Replace("\n\r", "");

                    this.NetWork = !str.ToUpper().Contains("VIETTEL") ? (!str.ToUpper().Contains("MOBIFONE") ? (!str.ToUpper().Contains("VINAPHONE") ? (!str.ToUpper().Contains("VIETNAMOBILE") ? (!str.ToUpper().Contains("VNSKY")? "Sim tạm thời bị khóa hoặc không có sóng":"VNSKY"):"VIETNAMOBILE") : "VINAPHONE") : "MOBIFONE") : "VIETTEL";
                    if (this.NetWork == "Sim tạm thời bị khóa hoặc không có sóng")
                    {
                        this.Msg = loadMsg("Không đọc được nhà mạng của Sim");
                        this.Status = "No Carrier";
                    }
                }
                if (input.Contains("+CUSD:"))
                {   
                    if (this._isPhone)
                    {
                       
                        this._isPhone = false;
                        if (this.NetWork == "VIETTEL")
                        {
                            string phone_reg = input.Replace(" ", "").Replace("\n", ",").Replace("\r","").Replace("\t","");
                            string[] phone_split = phone_reg.Split(',');
                            phone_split = phone_split.Where(x => !string.IsNullOrWhiteSpace(x)&&!string.IsNullOrEmpty(x)).ToArray();
                            string text_clone = "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"";
                            if (phone_split[1] == text_clone)
                            {
                                this.text = "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"";
                            }
                            if (check_11_dig)
                            {
                                Regex reg = new Regex("[^0-9]");
                                string[] newip = phone_split[1].Split(':');
                                string phone_eleven = reg.Replace(newip[0], "");
                                this.Phone = phone_eleven;
                            }
                            else
                            {
                                Match match = new Regex(".*?" + char.ConvertFromUtf32(34) + "(\\d+).*?:([0-9\\.]+)d[a-zA-Z0?,:]+(\\d+\\/\\d+\\/\\d+).*").Match(phone_reg);
                                this.Phone = "0" + match.Groups[1].ToString().Substring(2);
                                if (phone_split[1].Contains("."))
                                {
                                    string[] tkc = phone_split[1].Split(':');
                                    Regex reg_digit= new Regex(@"(\d+|\d+\.\d+)d");
                                    Match match_digit = reg_digit.Match(tkc[1]);
                                    if(match_digit.Success)
                                    {
                                        this.TKC = match_digit.Value.Replace("d","");
                                    }
                                }
                                else
                                {
                                    Regex reg_digit = new Regex(@"(\d+|\d+\.\d+)d");
                                    Match match_digit = reg_digit.Match(phone_split[3]);
                                    if (match_digit.Success)
                                    {
                                        this.TKC = match_digit.Value.Replace("d","");
                                    }
                                }
                               
                                this.Expiration_Date = match.Groups[3].ToString();
                            }
                        }
                        else if (this.NetWork == "VINAPHONE")
                        {
                            MessageBox.Show(input);
                            this.Phone = "0" + new Regex(".*(\\d{9}).*").Match(input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "")).Groups[1].ToString();
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expiration_Date = "";
                        }
                        else if (this.NetWork == "MOBIFONE" || this.NetWork=="VNSKY")
                        {
                            string[] phones = input.Split(',');
                            Regex reg_phone = new Regex(@"\b84\d+\b");
                            string phone_value = phones[1].Replace("\"","");
                            Match phone_match = reg_phone.Match(phone_value);
                            string phone = "";
                            if(phone_match.Success)
                            {
                                string phone_temp_value = phone_match.Value;
                                phone = "0" + phone_temp_value.Substring(2);
                            }
                            if (!string.IsNullOrEmpty(phone))
                            {
                                this.Phone = phone;
                            }
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expiration_Date = "";
                        }
                        else if (this.NetWork == "VIETNAMOBILE")
                        {
                            this.Phone = "0" + new Regex(".*(\\d{11}).*").Match(input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "")).Groups[1].ToString().Substring(2);
                            phone_secondary_temp = this.Phone;
                            this.TKC = "";
                            this.Expiration_Date = "";
                        }
                        if (this.Phone != "" && this.Phone != "Loading" && (this.Phone.Length == 10 || this.Phone.Length == 11))
                        {   
                            this.CarrierHasResult = true;
                            this.Msg = loadMsg(this.Port + " đã nhận dạng thành công: SĐT:" + this.Phone + " Network:" + this.NetWork);
                        }
                        else
                        {
                            this.Phone = "Loading";
                            this.Msg = loadMsg(this.Port + " không nhận dạng được số điện thoại.");
                        }
                    }
                    if (this._isInfor)
                    {
                        this._isInfor = false;
                        if (this.NetWork == "MOBIFONE"|| this.NetWork=="VNSKY")
                        {   
                            if (input.Contains("VNSKY"))
                            {
                                this.NetWork = "VNSKY";
                            }
                            Match match = new Regex("TKC (\\d+) d.* ([\\d+\\/\\d+\\/\\d+]+)").Match(input.Replace("\r\n", "").Replace("\n", "").Replace("-", "/"));
                            this.TKC =match.Groups[1].ToString();
                            this.Expiration_Date = match.Groups[2].ToString();
                        }
                        else if (this.NetWork == "VINAPHONE")
                        {
                            Match match = new Regex("=(\\d+) VND.* ([\\d+\\/\\d+\\/\\d+]+)").Match(input.Replace("\r\n", "").Replace("\n", ""));
                            this.TKC =match.Groups[1].ToString();
                            string date = match.Groups[0].ToString();
                            string[] dates = date.Split('.');
                            string[] expiration_date = dates[0].Split(',');
                            string expire = expiration_date[1].Replace("HSD", "");
                            this.Expiration_Date = expire;
                        }
                        else if (this.NetWork == "VIETTEL")
                        {
                            string input_refact = input.Replace(" ", "").Replace("\r\n", "").Replace("\n", "");
                            Match match = new Regex(".*?" + char.ConvertFromUtf32(34) + "(\\d+).*?:([0-9\\.]+)d[a-zA-Z0?,:]+(\\d+\\/\\d+\\/\\d+).*").Match(input_refact);
                            this.Phone = "0" + match.Groups[1].ToString().Substring(2);
                            this.TKC = match.Groups[2].ToString();
                            this.Expiration_Date = match.Groups[3].ToString();
                        }
                        else if(this.NetWork == "VIETNAMOBILE")
                        {   
                            string modified_str = input.Replace("\n", " ");
                            string[] split_values = modified_str.Split(' ');
                            string tkc = "";
                            foreach(string value in split_values)
                            {       
                                if(value.Contains("d"))
                                {    
                                    tkc = value;
                                    break;
                                }
                            }
                            Regex reg = new Regex("[^0-9.]");
                            tkc = reg.Replace(tkc,"");
                            this.TKC = tkc;
                        }
                       
                    }
                    if (this.isTKKM)
                    {
                        this.isTKKM = false;
                        Regex reg = new Regex(@"(\d+|\d+\.\d+)d");
                        Match match = reg.Match(input);
                        if (match.Success)
                        {
                            string tkkm = match.Value.Replace("d", "");
                            this.TKKM = tkkm;
                        }
                    
                    }
                }
                if (input.Contains("+CMGR: "))
                {   
                    this.current_sms = input;
                    this.smsSuccess = false;
                    if (this.current_sms.Replace("\n", "").Replace("\r", "").EndsWith("OK"))
                    {
                        input = this.current_sms;
                        this.current_sms = string.Empty;
                        this.smsSuccess = true;
                    }
                }
                else if (!string.IsNullOrEmpty(this.current_sms) && !this.smsSuccess)
                {
                    this.current_sms += input;
                    if (input.Replace("\n", "").Replace("\r", "").EndsWith("OK"))
                    {                                                    
                        input = this.current_sms;
                        this.current_sms = string.Empty;
                        this.smsSuccess = true;
                    }
                }
                if(input.Contains("+CMGR: ") && this.smsSuccess)
                {
                    this.deleteMessageByIndex();
                    int start_index_1 = input.IndexOf("+CMGR: ") + 7;
                    int start_index_2 = input.IndexOf("\r\n", start_index_1) + 2;
                    int num = input.IndexOf("\r\n", start_index_2 + 1);
                    if (start_index_2 == -1 || num == -1 || num <= start_index_2)
                    {
                        return;
                    }
                    string pdu = input.Substring(start_index_2, num - start_index_2);
                    string txt1 = string.Empty;
                    string txt2 = string.Empty;
                    string txt3 = string.Empty;
                    SmsDeliverPdu smsDeliverPdu = (SmsDeliverPdu)IncomingSmsPdu.Decode(pdu, true);
                    string otp = smsDeliverPdu.UserDataText;
                    if (otp.Contains("OTP"))
                    {
                        txt1 = smsDeliverPdu.UserDataText;
                        txt2 = smsDeliverPdu.OriginatingAddress;
                        txt3 = string.Format("{0:dd/MM/yyyy}", smsDeliverPdu.SCTimestamp.ToDateTime()) + " " + string.Format("{0:HH:mm:ss}", smsDeliverPdu.SCTimestamp.ToDateTime());
                        Regex regex = new Regex("^[^a-zA-Z0-9]*");
                        string txt1_1 = regex.Replace(txt1, "");
                        int first_place = this.phone_temp.IndexOf("0");
                        string phone_push = this.phone_temp.Remove(first_place, 1).Insert(first_place, "84");
                        string com = this.Port;
                        string otp_number = otpFilter(txt1_1, txt2);
                        string re = txt2 + "~" + txt1_1 + "~" + txt3 + "~" +phone_push+"~"+com+"~"+otp_number;
                        string re_ver1 = txt2 + "@" + txt1_1 + "@" + txt3 + "@" +phone_push+"@"+com+"@"+otp_number;
                        this.Otp = re;
                        this.instance.addNewMessageRow(re);
                        instance.List_Receive.Add(re_ver1);
                        return;
                    }
                    else
                    {
                        txt1 = smsDeliverPdu.UserDataText;
                        txt2 = smsDeliverPdu.OriginatingAddress;
                        txt3 = string.Format("{0:dd/MM/yyyy}", smsDeliverPdu.SCTimestamp.ToDateTime()) + " " + string.Format("{0:HH:mm:ss}", smsDeliverPdu.SCTimestamp.ToDateTime());
                        Regex regex = new Regex("^[^a-zA-Z0-9]*");
                        string txt1_1 = regex.Replace(txt1, "");
                        if(string.IsNullOrEmpty(this.Phone))
                        {
                            return;
                        }
                        if(txt2.Equals("123"))
                        {
                            return;
                        }
                        int first_place = this.Phone.IndexOf("0");
                        string phone_push = this.Phone.Remove(first_place, 1).Insert(first_place, "84");
                        string com = this.Port;
                        string re = txt2 + "@" + txt1_1 + "@" + txt3 + "@" + phone_push+"@"+com;
                        string re_ver1 = txt2 + "@" + txt1_1 + "@" + txt3 + "@" + phone_push+"@"+com;
                        this.instance.addNewMessageRow(re);
                        instance.List_Receive.Add(re_ver1);
                    }
                }
                if (!input.Contains("+CMTI: \"SM\","))
                {
                    return;
                 
                }
                this.oldindexsms = int.Parse((input.Split('\n')).FirstOrDefault((y => y.Contains("+CMTI: \"SM\","))).Replace("+CMTI: \"SM\",", ""));
                this.ReadSmsInbox();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public async Task DeleteMessageByIndex(int index)
        {
            try
            {
                await this.sendAT(string.Format("AT+CMGD={0}\r", index));
                await this.sendAT("AT+CMGD=1,4");
                await this.sendAT("AT+CNMI=1,1");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        public async Task DeleteAllSmsInbox()
        {
            try
            {
                await this.sendAT("AT+CPMS=\"SM\",\"SM\",\"SM\"");
                await Task.Delay(100);
                await this.sendAT("AT+CMGF=1");
                await Task.Delay(100);
                await this.sendAT("AT+CMGD=1,4");
                await Task.Delay(100);
                await this.sendAT("AT+CNMI=1,1");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
        }
        public Task Work()
        {

            this.delallsms = true;
            return Task.Run(async () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    if (this.rowGSMSelect == null)
                    {
                        this.Msg = loadMsg("Port " + this.Port + " không thể kết nối");
                    }
                    else
                    {
                        this._isSIM = false;
                        this.Status = "Try Open Port";
                        this.Msg = this.Port + " đang mở cổng";
                        bool try_open_port = this.TryOpenPort();
                        if (!try_open_port)
                        {

                            this.Status = "Port Close";
                            this.Msg = this.Port + " đã đóng";

                        }
                        else
                        {

                            try { 
                            while (true)
                            {
                                try
                                {

                                    if (!this._isSIM)
                                    {
                                        await CheckSimReady();
                                        for (int msWait = 0; !this.CheckSimReadyHasResult && msWait < 3000; msWait += 100)
                                        {
                                            await Task.Delay(100);
                                        }
                                    }
                                    else
                                    {
                                        if (DateTime.Now.Subtract(this.timeOutExist).TotalSeconds > 2)
                                        {

                                            this.Port = port;
                                            this.NetWork = "";
                                            this.Phone = "";
                                            this.Status = "Try Open Port";
                                            this.TKC = "";
                                            this.Expiration_Date = "";
                                            this.Msg = port + " đang mở cổng";
                                            this.HT = "";
                                            this.Serial_Sim = "";
                                            this.Imei = "";
                                            this.TKKM = "";
                                            this.Otp = "";
                                            this.Transaction_Id = "";
                                            this.TimeProcess = "";
                                            this.StatusMy = "";
                                            this.Note = "";
                                            this.Otp_Receive = "";
                                            this.rowGSMSelect.Cells["status"].Style.BackColor = Color.White;
                                            this.delallsms = true;
                                            this.CarrierHasResult = false;
                                            await Task.Delay(1000);
                                        }
                                        if (this.delallsms)
                                        {
                                            await this.DeleteAllSmsInbox();
                                            await Task.Delay(100);
                                            this.delallsms = false;
                                        }
                                        await this.sendAT("AT+CPIN?");
                                        await Task.Delay(100);
                                
                                        if ((this.NetWork == "" || this.NetWork == "Sim tạm thời bị khóa hoặc không có sóng") && DateTime.Now.Subtract(this.lastReportNetwork).TotalSeconds > 5.0)
                                        {
                                            this.lastReportNetwork = DateTime.Now;
                                            await this.sendAT("AT+COPS?");
                                            await Task.Delay(100);
                                        }
                                            if (string.IsNullOrEmpty(this.Imei) &&(!string.IsNullOrEmpty(this.NetWork) && this.NetWork!= "Sim tạm thời bị khóa hoặc không có sóng") && DateTime.Now.Subtract(this.lastReportImei).TotalSeconds>5.0)
                                            {
                                                this.lastReportImei = DateTime.Now;
                                                this.is_imei = true;
                                                await this.sendAT("AT+GSN");
                                                await Task.Delay(100);
                                            }
                                            if (string.IsNullOrEmpty(this.Serial_Sim) && this.NetWork != "" && DateTime.Now.Subtract(this.lastSerialSimReport).TotalSeconds > 5.0)
                                        {
                                            this.lastSerialSimReport = DateTime.Now;
                                            await this.sendAT("AT+ICCID?");
                                            await Task.Delay(100);
                                        }
                                        if (!this.CarrierHasResult && this.NetWork != "" && this.NetWork != "Sim tạm thời bị khóa hoặc không có sóng" && DateTime.Now.Subtract(this.lastReportPhone).TotalSeconds > 10)
                                        {
                                            this.Status = "Sẵn sàng";
                                            this.lastReportPhone = DateTime.Now;
                                            this.Phone = "Loading";
                                            this._isPhone = true;
                                            this.Msg = loadMsg(this.Port + " bắt đầu nhận dạng SĐT");
                                            if (this.NetWork == "VIETTEL" && text != "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"")
                                            {
                                                await this.RunUSSD("*101#");
                                            }
                                            else if (this.NetWork == "VIETTEL" && text == "\"YeucaucuaQuykhachkhongduocdapungtaithoidiemnay\"")
                                            {
                                                check_11_dig = true;
                                                await this.RunUSSD("*098#");
                                            }
                                            else if (this.NetWork == "VINAPHONE")
                                            {
                                                await this.RunUSSD("*110#");
                                            }
                                            else if (this.NetWork == "MOBIFONE" || this.NetWork == "VNSKY")
                                            {
                                                await this.RunUSSD("*0#");
                                            }
                                            else if (this.NetWork == "VIETNAMOBILE")
                                            {
                                                await this.RunUSSD("*123#");
                                            }
                                            for (int wait = 0; !this.CarrierHasResult && wait < 1000; wait += 100)
                                            {
                                                await Task.Delay(100);
                                            }
                                        }
                                        if ((this.TKC == "" || this.TKC == "Lấy lại TKC" || this.Expiration_Date == "") && this.NetWork != "" && this.NetWork != "Sim tạm thời bị khóa hoặc không có sóng" && DateTime.Now.Subtract(this.lastReportInfo).TotalSeconds > 10 && this.Phone != "" & this.Phone != "Loading")
                                        {
                                            this.lastReportInfo = DateTime.Now;
                                            this._isInfor = true;
                                            if (this.NetWork == "MOBIFONE" || this.NetWork == "VINAPHONE" || this.NetWork == "VIETNAMOBILE" || this.NetWork == "VIETTEL" || this.NetWork == "VNSKY")
                                            {
                                                await this.RunUSSD("*101#");
                                            }

                                            await Task.Delay(100);
                                        }
                                        if (!string.IsNullOrEmpty(this.TKC) && !string.IsNullOrEmpty(this.Expiration_Date) && !string.IsNullOrEmpty(this.NetWork) && !string.IsNullOrEmpty(this.Phone) && this.Phone != "Loading" && DateTime.Now.Subtract(this.lastTKKMReport).TotalSeconds > 10)
                                        {
                                            this.lastTKKMReport = DateTime.Now;
                                            this.isTKKM = true;
                                            if (this.NetWork == "VIETTEL")
                                            {
                                                await this.RunUSSD("*102#");
                                            }
                                            await Task.Delay(100);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                            catch(Exception er)
                            {
                                Console.WriteLine(er.Message);
                            }

                        }
                    }
                }
            });
        }
    }
}
