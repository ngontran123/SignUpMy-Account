namespace GSMMASTER.ToolsForm
{
    partial class SendSmS
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
            this.simInfoLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sendphone = new Guna.UI2.WinForms.Guna2TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.content = new Guna.UI2.WinForms.Guna2TextBox();
            this.sendbtn = new Guna.UI2.WinForms.Guna2Button();
            this.closebtn = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // simInfoLabel
            // 
            this.simInfoLabel.AutoSize = true;
            this.simInfoLabel.Location = new System.Drawing.Point(213, 21);
            this.simInfoLabel.Name = "simInfoLabel";
            this.simInfoLabel.Size = new System.Drawing.Size(35, 13);
            this.simInfoLabel.TabIndex = 0;
            this.simInfoLabel.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Gửi tới:";
            // 
            // sendphone
            // 
            this.sendphone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.sendphone.DefaultText = "";
            this.sendphone.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.sendphone.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.sendphone.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.sendphone.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.sendphone.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.sendphone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.sendphone.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.sendphone.Location = new System.Drawing.Point(117, 76);
            this.sendphone.Name = "sendphone";
            this.sendphone.PasswordChar = '\0';
            this.sendphone.PlaceholderText = "";
            this.sendphone.SelectedText = "";
            this.sendphone.Size = new System.Drawing.Size(302, 31);
            this.sendphone.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nội dung";
            // 
            // content
            // 
            this.content.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.content.DefaultText = "";
            this.content.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.content.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.content.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.content.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.content.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.content.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.content.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.content.Location = new System.Drawing.Point(117, 182);
            this.content.Multiline = true;
            this.content.Name = "content";
            this.content.PasswordChar = '\0';
            this.content.PlaceholderText = "";
            this.content.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.content.SelectedText = "";
            this.content.Size = new System.Drawing.Size(406, 192);
            this.content.TabIndex = 4;
            // 
            // sendbtn
            // 
            this.sendbtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.sendbtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.sendbtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.sendbtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.sendbtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.sendbtn.ForeColor = System.Drawing.Color.White;
            this.sendbtn.Location = new System.Drawing.Point(117, 386);
            this.sendbtn.Name = "sendbtn";
            this.sendbtn.Size = new System.Drawing.Size(131, 34);
            this.sendbtn.TabIndex = 5;
            this.sendbtn.Text = "Gửi";
            this.sendbtn.Click += new System.EventHandler(this.sendbtn_Click);
            // 
            // closebtn
            // 
            this.closebtn.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.closebtn.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.closebtn.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.closebtn.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.closebtn.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.closebtn.ForeColor = System.Drawing.Color.White;
            this.closebtn.Location = new System.Drawing.Point(362, 386);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(131, 34);
            this.closebtn.TabIndex = 6;
            this.closebtn.Text = "Hủy";
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // SendSmS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 443);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.sendbtn);
            this.Controls.Add(this.content);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sendphone);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.simInfoLabel);
            this.Name = "SendSmS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SendSmS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label simInfoLabel;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox sendphone;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2TextBox content;
        private Guna.UI2.WinForms.Guna2Button sendbtn;
        private Guna.UI2.WinForms.Guna2Button closebtn;
    }
}