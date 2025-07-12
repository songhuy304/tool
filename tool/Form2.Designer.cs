namespace tool
{
    partial class Form2
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.stt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_addfile = new System.Windows.Forms.Button();
            this.btn_add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.btn_luudanhsachchuagui = new System.Windows.Forms.Button();
            this.btn_xoatin = new System.Windows.Forms.Button();
            this.txt_tinnhan = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_delay = new System.Windows.Forms.NumericUpDown();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_delay)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.stt,
            this.phone,
            this.status});
            this.dataGridView1.Location = new System.Drawing.Point(415, 120);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(344, 301);
            this.dataGridView1.TabIndex = 19;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // stt
            // 
            this.stt.HeaderText = "STT";
            this.stt.Name = "stt";
            // 
            // phone
            // 
            this.phone.HeaderText = "Phone Number";
            this.phone.Name = "phone";
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            // 
            // btn_addfile
            // 
            this.btn_addfile.Location = new System.Drawing.Point(464, 73);
            this.btn_addfile.Name = "btn_addfile";
            this.btn_addfile.Size = new System.Drawing.Size(75, 21);
            this.btn_addfile.TabIndex = 18;
            this.btn_addfile.Text = "Choose file";
            this.btn_addfile.UseVisualStyleBackColor = true;
            this.btn_addfile.Click += new System.EventHandler(this.btn_addfile_Click);
            // 
            // btn_add
            // 
            this.btn_add.Location = new System.Drawing.Point(606, 31);
            this.btn_add.Name = "btn_add";
            this.btn_add.Size = new System.Drawing.Size(75, 21);
            this.btn_add.TabIndex = 17;
            this.btn_add.Text = "Add";
            this.btn_add.UseVisualStyleBackColor = true;
            this.btn_add.Click += new System.EventHandler(this.btn_add_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(420, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Phone";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Location = new System.Drawing.Point(464, 31);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(136, 20);
            this.txtPhoneNumber.TabIndex = 15;
            this.txtPhoneNumber.TextChanged += new System.EventHandler(this.txtPhoneNumber_TextChanged);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(42, 398);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(141, 23);
            this.btnSendMessage.TabIndex = 14;
            this.btnSendMessage.Text = "Send Message";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btn_luudanhsachchuagui
            // 
            this.btn_luudanhsachchuagui.Location = new System.Drawing.Point(545, 73);
            this.btn_luudanhsachchuagui.Name = "btn_luudanhsachchuagui";
            this.btn_luudanhsachchuagui.Size = new System.Drawing.Size(136, 21);
            this.btn_luudanhsachchuagui.TabIndex = 23;
            this.btn_luudanhsachchuagui.Text = "Lưu danh sách chưa gửi";
            this.btn_luudanhsachchuagui.UseVisualStyleBackColor = true;
            this.btn_luudanhsachchuagui.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_xoatin
            // 
            this.btn_xoatin.BackColor = System.Drawing.Color.IndianRed;
            this.btn_xoatin.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_xoatin.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_xoatin.Location = new System.Drawing.Point(687, 30);
            this.btn_xoatin.Name = "btn_xoatin";
            this.btn_xoatin.Size = new System.Drawing.Size(75, 21);
            this.btn_xoatin.TabIndex = 22;
            this.btn_xoatin.Text = "Remove";
            this.btn_xoatin.UseVisualStyleBackColor = false;
            this.btn_xoatin.Click += new System.EventHandler(this.btn_xoatin_Click);
            // 
            // txt_tinnhan
            // 
            this.txt_tinnhan.Location = new System.Drawing.Point(40, 249);
            this.txt_tinnhan.Name = "txt_tinnhan";
            this.txt_tinnhan.Size = new System.Drawing.Size(287, 96);
            this.txt_tinnhan.TabIndex = 21;
            this.txt_tinnhan.Text = "";
            this.txt_tinnhan.TextChanged += new System.EventHandler(this.txt_tinnhan_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 232);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Nội dung tin nhắn";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(205, 398);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(122, 23);
            this.button2.TabIndex = 24;
            this.button2.Text = "Kết nối";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Time out";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(82, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(136, 20);
            this.textBox1.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Delay ( ms )";
            // 
            // txt_delay
            // 
            this.txt_delay.Location = new System.Drawing.Point(82, 70);
            this.txt_delay.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.txt_delay.Minimum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.txt_delay.Name = "txt_delay";
            this.txt_delay.Size = new System.Drawing.Size(136, 20);
            this.txt_delay.TabIndex = 30;
            this.txt_delay.Value = new decimal(new int[] {
            700,
            0,
            0,
            0});
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.IndianRed;
            this.listBox1.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.listBox1.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "*** LƯU Ý",
            "",
            "- Cần ấn nút kết nối trước mới ấn chạy",
            "- Cần để trạng thái light mode , scale 100%",
            "- Cần xóa thanh search số điện thoại trước khi kết nối",
            "- Ấn nút ESC trên bàn phím để tạm dừng tool"});
            this.listBox1.Location = new System.Drawing.Point(40, 143);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(287, 82);
            this.listBox1.TabIndex = 31;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txt_delay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btn_addfile);
            this.Controls.Add(this.btn_add);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPhoneNumber);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btn_luudanhsachchuagui);
            this.Controls.Add(this.btn_xoatin);
            this.Controls.Add(this.txt_tinnhan);
            this.Controls.Add(this.label2);
            this.Name = "Form2";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txt_delay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn stt;
        private System.Windows.Forms.DataGridViewTextBoxColumn phone;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.Button btn_addfile;
        private System.Windows.Forms.Button btn_add;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.Button btn_luudanhsachchuagui;
        private System.Windows.Forms.Button btn_xoatin;
        private System.Windows.Forms.RichTextBox txt_tinnhan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown txt_delay;
        private System.Windows.Forms.ListBox listBox1;
    }
}