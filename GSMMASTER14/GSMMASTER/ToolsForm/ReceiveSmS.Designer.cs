namespace GSMMASTER.ToolsForm
{
    partial class ReceiveSmS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.portInfo = new Guna.UI2.WinForms.Guna2TextBox();
            this.reloadbtn = new Guna.UI2.WinForms.Guna2Button();
            this.receiveGSM = new Guna.UI2.WinForms.Guna2DataGridView();
            this.stt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.from = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guna2AnimateWindow1 = new Guna.UI2.WinForms.Guna2AnimateWindow(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.receiveGSM)).BeginInit();
            this.SuspendLayout();
            // 
            // portInfo
            // 
            this.portInfo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.portInfo.DefaultText = "";
            this.portInfo.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.portInfo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.portInfo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.portInfo.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.portInfo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.portInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.portInfo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.portInfo.Location = new System.Drawing.Point(12, 25);
            this.portInfo.Name = "portInfo";
            this.portInfo.PasswordChar = '\0';
            this.portInfo.PlaceholderText = "";
            this.portInfo.SelectedText = "";
            this.portInfo.Size = new System.Drawing.Size(263, 36);
            this.portInfo.TabIndex = 0;
            // 
            // reloadbtn
            // 
            this.reloadbtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.reloadbtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.reloadbtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.reloadbtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.reloadbtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.reloadbtn.ForeColor = System.Drawing.Color.White;
            this.reloadbtn.Location = new System.Drawing.Point(308, 25);
            this.reloadbtn.Name = "reloadbtn";
            this.reloadbtn.Size = new System.Drawing.Size(120, 36);
            this.reloadbtn.TabIndex = 1;
            this.reloadbtn.Text = "Tải lại";
            this.reloadbtn.Click += new System.EventHandler(this.reloadbtn_Click);
            // 
            // receiveGSM
            // 
            this.receiveGSM.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.receiveGSM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.receiveGSM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.receiveGSM.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.receiveGSM.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.receiveGSM.ColumnHeadersHeight = 15;
            this.receiveGSM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.receiveGSM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stt,
            this.from,
            this.date,
            this.msg});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.receiveGSM.DefaultCellStyle = dataGridViewCellStyle3;
            this.receiveGSM.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.receiveGSM.Location = new System.Drawing.Point(4, 67);
            this.receiveGSM.Name = "receiveGSM";
            this.receiveGSM.RowHeadersVisible = false;
            this.receiveGSM.Size = new System.Drawing.Size(641, 388);
            this.receiveGSM.TabIndex = 6;
            this.receiveGSM.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.receiveGSM.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.receiveGSM.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.receiveGSM.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.receiveGSM.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.receiveGSM.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.receiveGSM.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.receiveGSM.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.receiveGSM.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.receiveGSM.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiveGSM.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.receiveGSM.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.receiveGSM.ThemeStyle.HeaderStyle.Height = 15;
            this.receiveGSM.ThemeStyle.ReadOnly = false;
            this.receiveGSM.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.receiveGSM.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.receiveGSM.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.receiveGSM.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.receiveGSM.ThemeStyle.RowsStyle.Height = 22;
            this.receiveGSM.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.receiveGSM.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.receiveGSM.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.receiveGSM_CellContentClick);
            this.receiveGSM.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGSM_RowPostPaint);
            this.receiveGSM.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.receiveGSM_SortCompare);
            // 
            // stt
            // 
            this.stt.HeaderText = "STT";
            this.stt.Name = "stt";
            this.stt.ReadOnly = true;
            this.stt.Width = 51;
            // 
            // from
            // 
            this.from.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.from.HeaderText = "From";
            this.from.Name = "from";
            this.from.ReadOnly = true;
            this.from.Width = 53;
            // 
            // date
            // 
            this.date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.date.HeaderText = "Date";
            this.date.Name = "date";
            this.date.ReadOnly = true;
            this.date.Width = 53;
            // 
            // msg
            // 
            this.msg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.msg.HeaderText = "Message";
            this.msg.Name = "msg";
            this.msg.ReadOnly = true;
            this.msg.Width = 73;
            // 
            // ReceiveSmS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 456);
            this.Controls.Add(this.receiveGSM);
            this.Controls.Add(this.reloadbtn);
            this.Controls.Add(this.portInfo);
            this.Name = "ReceiveSmS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReceiveSmS";
            this.Load += new System.EventHandler(this.ReceiveSmS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.receiveGSM)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox portInfo;
        private Guna.UI2.WinForms.Guna2Button reloadbtn;
        public Guna.UI2.WinForms.Guna2DataGridView receiveGSM;
        private System.Windows.Forms.DataGridViewTextBoxColumn stt;
        private System.Windows.Forms.DataGridViewTextBoxColumn from;
        private System.Windows.Forms.DataGridViewTextBoxColumn date;
        private System.Windows.Forms.DataGridViewTextBoxColumn msg;
        private Guna.UI2.WinForms.Guna2AnimateWindow guna2AnimateWindow1;
    }
}