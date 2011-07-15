namespace SRS_Server.Net
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.rtbMessageLog = new System.Windows.Forms.RichTextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnKickUsers = new System.Windows.Forms.Button();
            this.lblUsersOnline = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtServerIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.txtCommandLine = new System.Windows.Forms.TextBox();
            this.notifier = new System.Windows.Forms.NotifyIcon(this.components);
            this.NotifierContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.slblErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnTutors = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.NotifierContext.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbMessageLog
            // 
            this.rtbMessageLog.Location = new System.Drawing.Point(6, 14);
            this.rtbMessageLog.Name = "rtbMessageLog";
            this.rtbMessageLog.Size = new System.Drawing.Size(401, 273);
            this.rtbMessageLog.TabIndex = 0;
            this.rtbMessageLog.Text = "";
            this.rtbMessageLog.TextChanged += new System.EventHandler(this.rtbMessageLog_TextChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 137);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(89, 23);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start Server";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server Status:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTutors);
            this.groupBox1.Controls.Add(this.btnKickUsers);
            this.groupBox1.Controls.Add(this.lblUsersOnline);
            this.groupBox1.Controls.Add(this.txtServerPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtServerIP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.lblServerStatus);
            this.groupBox1.Controls.Add(this.btnStart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 209);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server ";
            // 
            // btnKickUsers
            // 
            this.btnKickUsers.Enabled = false;
            this.btnKickUsers.Location = new System.Drawing.Point(12, 180);
            this.btnKickUsers.Name = "btnKickUsers";
            this.btnKickUsers.Size = new System.Drawing.Size(89, 23);
            this.btnKickUsers.TabIndex = 10;
            this.btnKickUsers.Text = "Kick Users";
            this.btnKickUsers.UseVisualStyleBackColor = true;
            this.btnKickUsers.Click += new System.EventHandler(this.btnKickUsers_Click);
            // 
            // lblUsersOnline
            // 
            this.lblUsersOnline.AutoSize = true;
            this.lblUsersOnline.Location = new System.Drawing.Point(11, 112);
            this.lblUsersOnline.Name = "lblUsersOnline";
            this.lblUsersOnline.Size = new System.Drawing.Size(79, 13);
            this.lblUsersOnline.TabIndex = 9;
            this.lblUsersOnline.Text = "Users Online: 0";
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(79, 79);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.ReadOnly = true;
            this.txtServerPort.Size = new System.Drawing.Size(153, 20);
            this.txtServerPort.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Server Port:";
            // 
            // txtServerIP
            // 
            this.txtServerIP.Location = new System.Drawing.Point(79, 50);
            this.txtServerIP.Name = "txtServerIP";
            this.txtServerIP.ReadOnly = true;
            this.txtServerIP.Size = new System.Drawing.Size(153, 20);
            this.txtServerIP.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Server IP:";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(112, 137);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(89, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.Location = new System.Drawing.Point(76, 26);
            this.lblServerStatus.Name = "lblServerStatus";
            this.lblServerStatus.Size = new System.Drawing.Size(37, 13);
            this.lblServerStatus.TabIndex = 3;
            this.lblServerStatus.Text = "Offline";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSendMsg);
            this.groupBox2.Controls.Add(this.rtbMessageLog);
            this.groupBox2.Controls.Add(this.txtCommandLine);
            this.groupBox2.Location = new System.Drawing.Point(256, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(414, 319);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server Messages";
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Location = new System.Drawing.Point(344, 291);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(63, 23);
            this.btnSendMsg.TabIndex = 9;
            this.btnSendMsg.Text = "Send";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // txtCommandLine
            // 
            this.txtCommandLine.Location = new System.Drawing.Point(6, 293);
            this.txtCommandLine.Name = "txtCommandLine";
            this.txtCommandLine.Size = new System.Drawing.Size(333, 20);
            this.txtCommandLine.TabIndex = 8;
            // 
            // notifier
            // 
            this.notifier.ContextMenuStrip = this.NotifierContext;
            this.notifier.Icon = ((System.Drawing.Icon)(resources.GetObject("notifier.Icon")));
            this.notifier.Text = "Student Response Server";
            this.notifier.Visible = true;
            this.notifier.DoubleClick += new System.EventHandler(this.notifier_DoubleClick);
            // 
            // NotifierContext
            // 
            this.NotifierContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.NotifierContext.Name = "NotifierContext";
            this.NotifierContext.Size = new System.Drawing.Size(104, 26);
            this.NotifierContext.Text = "Close Server";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slblErrors});
            this.statusStrip1.Location = new System.Drawing.Point(0, 332);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(675, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // slblErrors
            // 
            this.slblErrors.Name = "slblErrors";
            this.slblErrors.Size = new System.Drawing.Size(0, 17);
            // 
            // btnTutors
            // 
            this.btnTutors.Location = new System.Drawing.Point(112, 180);
            this.btnTutors.Name = "btnTutors";
            this.btnTutors.Size = new System.Drawing.Size(89, 23);
            this.btnTutors.TabIndex = 11;
            this.btnTutors.Text = "Manage Tutors";
            this.btnTutors.UseVisualStyleBackColor = true;
            this.btnTutors.Click += new System.EventHandler(this.btnTutors_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 354);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximumSize = new System.Drawing.Size(691, 392);
            this.MinimumSize = new System.Drawing.Size(691, 392);
            this.Name = "frmMain";
            this.Text = "SRS Server";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.NotifierContext.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMessageLog;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblUsersOnline;
        private System.Windows.Forms.TextBox txtCommandLine;
        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.Button btnKickUsers;
        private System.Windows.Forms.NotifyIcon notifier;
        private System.Windows.Forms.ContextMenuStrip NotifierContext;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slblErrors;
        private System.Windows.Forms.Button btnTutors;
    }
}

