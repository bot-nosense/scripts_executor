
namespace ExcuteScripts
{
    partial class Main
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
            this.gb_conn = new System.Windows.Forms.GroupBox();
            this.bt_cof = new System.Windows.Forms.Button();
            this.bt_conn = new System.Windows.Forms.Button();
            this.gb_import = new System.Windows.Forms.GroupBox();
            this.tb_view_ip = new System.Windows.Forms.TextBox();
            this.bt_import = new System.Windows.Forms.Button();
            this.gb_event = new System.Windows.Forms.GroupBox();
            this.bt_log = new System.Windows.Forms.Button();
            this.bt_submit = new System.Windows.Forms.Button();
            this.gb_stt = new System.Windows.Forms.GroupBox();
            this.lb_stt = new System.Windows.Forms.Label();
            this.gb_conn.SuspendLayout();
            this.gb_import.SuspendLayout();
            this.gb_event.SuspendLayout();
            this.gb_stt.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_conn
            // 
            this.gb_conn.Controls.Add(this.bt_cof);
            this.gb_conn.Controls.Add(this.bt_conn);
            this.gb_conn.Location = new System.Drawing.Point(13, 13);
            this.gb_conn.Name = "gb_conn";
            this.gb_conn.Size = new System.Drawing.Size(258, 77);
            this.gb_conn.TabIndex = 0;
            this.gb_conn.TabStop = false;
            this.gb_conn.Text = "Connect Database";
            this.gb_conn.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // bt_cof
            // 
            this.bt_cof.Location = new System.Drawing.Point(133, 19);
            this.bt_cof.Name = "bt_cof";
            this.bt_cof.Size = new System.Drawing.Size(119, 52);
            this.bt_cof.TabIndex = 0;
            this.bt_cof.Text = "Open Config File";
            this.bt_cof.UseVisualStyleBackColor = true;
            this.bt_cof.Click += new System.EventHandler(this.button2_Click);
            // 
            // bt_conn
            // 
            this.bt_conn.Location = new System.Drawing.Point(6, 19);
            this.bt_conn.Name = "bt_conn";
            this.bt_conn.Size = new System.Drawing.Size(121, 52);
            this.bt_conn.TabIndex = 0;
            this.bt_conn.Text = "Connect";
            this.bt_conn.UseVisualStyleBackColor = true;
            // 
            // gb_import
            // 
            this.gb_import.Controls.Add(this.tb_view_ip);
            this.gb_import.Controls.Add(this.bt_import);
            this.gb_import.Location = new System.Drawing.Point(277, 13);
            this.gb_import.Name = "gb_import";
            this.gb_import.Size = new System.Drawing.Size(253, 243);
            this.gb_import.TabIndex = 0;
            this.gb_import.TabStop = false;
            this.gb_import.Text = "Import Files";
            this.gb_import.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // tb_view_ip
            // 
            this.tb_view_ip.Location = new System.Drawing.Point(7, 49);
            this.tb_view_ip.Multiline = true;
            this.tb_view_ip.Name = "tb_view_ip";
            this.tb_view_ip.Size = new System.Drawing.Size(241, 188);
            this.tb_view_ip.TabIndex = 1;
            // 
            // bt_import
            // 
            this.bt_import.Location = new System.Drawing.Point(7, 20);
            this.bt_import.Name = "bt_import";
            this.bt_import.Size = new System.Drawing.Size(240, 23);
            this.bt_import.TabIndex = 0;
            this.bt_import.Text = "Import";
            this.bt_import.UseVisualStyleBackColor = true;
            // 
            // gb_event
            // 
            this.gb_event.Controls.Add(this.bt_log);
            this.gb_event.Controls.Add(this.bt_submit);
            this.gb_event.Location = new System.Drawing.Point(13, 96);
            this.gb_event.Name = "gb_event";
            this.gb_event.Size = new System.Drawing.Size(258, 77);
            this.gb_event.TabIndex = 0;
            this.gb_event.TabStop = false;
            this.gb_event.Text = "Handle";
            this.gb_event.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // bt_log
            // 
            this.bt_log.Location = new System.Drawing.Point(133, 19);
            this.bt_log.Name = "bt_log";
            this.bt_log.Size = new System.Drawing.Size(119, 52);
            this.bt_log.TabIndex = 0;
            this.bt_log.Text = "Open Log File";
            this.bt_log.UseVisualStyleBackColor = true;
            this.bt_log.Click += new System.EventHandler(this.button2_Click);
            // 
            // bt_submit
            // 
            this.bt_submit.Location = new System.Drawing.Point(6, 19);
            this.bt_submit.Name = "bt_submit";
            this.bt_submit.Size = new System.Drawing.Size(121, 52);
            this.bt_submit.TabIndex = 0;
            this.bt_submit.Text = "Submit";
            this.bt_submit.UseVisualStyleBackColor = true;
            // 
            // gb_stt
            // 
            this.gb_stt.Controls.Add(this.lb_stt);
            this.gb_stt.Location = new System.Drawing.Point(13, 179);
            this.gb_stt.Name = "gb_stt";
            this.gb_stt.Size = new System.Drawing.Size(259, 77);
            this.gb_stt.TabIndex = 0;
            this.gb_stt.TabStop = false;
            this.gb_stt.Text = "Status";
            this.gb_stt.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // lb_stt
            // 
            this.lb_stt.AutoSize = true;
            this.lb_stt.Location = new System.Drawing.Point(7, 20);
            this.lb_stt.Name = "lb_stt";
            this.lb_stt.Size = new System.Drawing.Size(0, 13);
            this.lb_stt.TabIndex = 0;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 266);
            this.Controls.Add(this.gb_stt);
            this.Controls.Add(this.gb_event);
            this.Controls.Add(this.gb_import);
            this.Controls.Add(this.gb_conn);
            this.Name = "Main";
            this.Text = "Auto Run Scripts";
            this.Load += new System.EventHandler(this.Main_Load);
            this.gb_conn.ResumeLayout(false);
            this.gb_import.ResumeLayout(false);
            this.gb_import.PerformLayout();
            this.gb_event.ResumeLayout(false);
            this.gb_stt.ResumeLayout(false);
            this.gb_stt.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_conn;
        private System.Windows.Forms.GroupBox gb_import;
        private System.Windows.Forms.GroupBox gb_event;
        private System.Windows.Forms.Button bt_log;
        private System.Windows.Forms.Button bt_submit;
        private System.Windows.Forms.GroupBox gb_stt;
        private System.Windows.Forms.Label lb_stt;
        private System.Windows.Forms.TextBox tb_view_ip;
        private System.Windows.Forms.Button bt_import;
        private System.Windows.Forms.Button bt_cof;
        private System.Windows.Forms.Button bt_conn;
    }
}

