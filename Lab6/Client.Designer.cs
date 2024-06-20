namespace Lab6
{
    partial class Client
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
            this.rtbSVnoti = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAnswer = new System.Windows.Forms.TextBox();
            this.btnGuess = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCountdown = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPoint = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtbSVnoti
            // 
            this.rtbSVnoti.Location = new System.Drawing.Point(58, 54);
            this.rtbSVnoti.Name = "rtbSVnoti";
            this.rtbSVnoti.Size = new System.Drawing.Size(575, 389);
            this.rtbSVnoti.TabIndex = 0;
            this.rtbSVnoti.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Thông báo của server";
            // 
            // tbAnswer
            // 
            this.tbAnswer.Location = new System.Drawing.Point(59, 566);
            this.tbAnswer.Name = "tbAnswer";
            this.tbAnswer.Size = new System.Drawing.Size(574, 22);
            this.tbAnswer.TabIndex = 2;
            // 
            // btnGuess
            // 
            this.btnGuess.Location = new System.Drawing.Point(505, 518);
            this.btnGuess.Name = "btnGuess";
            this.btnGuess.Size = new System.Drawing.Size(128, 33);
            this.btnGuess.TabIndex = 3;
            this.btnGuess.Text = "Đoán";
            this.btnGuess.UseVisualStyleBackColor = true;
            this.btnGuess.Click += new System.EventHandler(this.btnGuess_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 547);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Đáp án";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 464);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tên";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(58, 483);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(304, 22);
            this.tbName.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(283, 612);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(128, 33);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 518);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Đếm ngược lần đoán kế tiếp";
            // 
            // lblCountdown
            // 
            this.lblCountdown.AutoSize = true;
            this.lblCountdown.Location = new System.Drawing.Point(251, 518);
            this.lblCountdown.Name = "lblCountdown";
            this.lblCountdown.Size = new System.Drawing.Size(43, 16);
            this.lblCountdown.TabIndex = 10;
            this.lblCountdown.Text = "0 giây";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(368, 464);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "Điểm";
            // 
            // tbPoint
            // 
            this.tbPoint.Location = new System.Drawing.Point(368, 483);
            this.tbPoint.Name = "tbPoint";
            this.tbPoint.ReadOnly = true;
            this.tbPoint.Size = new System.Drawing.Size(265, 22);
            this.tbPoint.TabIndex = 11;
            this.tbPoint.Text = "0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(368, 518);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 33);
            this.button1.TabIndex = 13;
            this.button1.Text = "Tự động chơi";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 671);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbPoint);
            this.Controls.Add(this.lblCountdown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGuess);
            this.Controls.Add(this.tbAnswer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbSVnoti);
            this.Name = "Client";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbSVnoti;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAnswer;
        private System.Windows.Forms.Button btnGuess;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCountdown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbPoint;
        private System.Windows.Forms.Button button1;
    }
}