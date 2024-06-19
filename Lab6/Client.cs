using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab6
{
    public partial class Client : Form
    {
        TcpClient client;
        NetworkStream stream;
        System.Windows.Forms.Timer countdownTimer;
        int countdown;
        Thread listenThread;
        string a, b;
        List<string> historyGuess = new List<string>();
        Thread autoGuessThread;
        bool stopAutoGuess = false;
        public Client()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();
                AppendMessage("Kết nối đến server.");

                listenThread = new Thread(ListenForMessages);
                listenThread.Start();
            }
            catch (Exception ex)
            {
                AppendMessage("Lỗi: " + ex.Message);
            }
        }

        private void ListenForMessages()
        {
            try
            {
                byte[] buffer = new byte[256];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (message.Contains("Chính xác!") && message.Contains(tbName.Text))
                    {
                        AppendPoint(10);
                    }
                    else if (message.Contains(tbName.Text))
                    {
                        AppendPoint(-10);
                    }
                    if (message.Contains("bắt đầu"))
                    {
                        historyGuess.Clear();
                        countdownTimer.Stop();
                        lblCountdown.Text = "0 giây";
                        countdown = 0;
                        var list = message.Split(' ');
                        a = list[list.Length - 3];
                        b = list[list.Length - 1];
                    }
                    AppendMessage(message);
                }
            }
            catch (Exception ex)
            {
                AppendMessage("Lỗi ListenMessage: " + ex.Message);
            }
        }

        private void CloseConnection()
        {
            autoGuessThread.Abort();
            listenThread.Abort();
            client.Close();
            Application.Exit();
        }

        private void AppendPoint(int Point)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(AppendPoint), new object[] { Point });
            }
            else
            {
                if (Point < 0)
                {
                    if (int.Parse(tbPoint.Text) >= 10)
                    {
                        tbPoint.Text = (int.Parse(tbPoint.Text) + Point).ToString();
                    }
                    else
                    {
                        tbPoint.Text = "0";
                    }
                } else
                {
                    tbPoint.Text = (int.Parse(tbPoint.Text) + Point).ToString();
                }
                
            }
        }

        private void btnGuess_Click(object sender, EventArgs e)
        {
            if (countdown > 0)
            {
                AppendMessage("Vui lòng đợi " + countdown + " giây để tiếp tục trả lời.");
                return;
            }

            string playerName = tbName.Text;
            string guess = tbAnswer.Text;
            historyGuess.Add(guess);
            string message = playerName + ":" + guess;
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            tbAnswer.Text = "";
            AppendMessage("Số đã đoán: " + guess);
            StartCountdown();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdown--;
            this.Invoke(new Action(() =>
            {
                lblCountdown.Text = countdown + " giây";
            }));
            
            
            if (countdown == 0)
            {
                countdownTimer.Stop();
                this.Invoke(new Action(() =>
                {
                    lblCountdown.Text = "0 giây";
                }));
                
            }
        }

        private void StartCountdown()
        {
            countdown = 6;
            countdownTimer.Start();
        }

        private void AppendMessage(string message)
        {
            
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendMessage), new object[] { message });
            }
            else
            {
                rtbSVnoti.AppendText(message + "\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (autoGuessThread == null || !autoGuessThread.IsAlive)
            {
                stopAutoGuess = false;
                autoGuessThread = new Thread(AutoGuess);
                autoGuessThread.Start();
            }
            
        }

        private void AutoGuess()
        {
            while (!stopAutoGuess)
            {
                if (countdown == 0)
                {
                    string playerName = tbName.Text;
                    int rand = new Random().Next(int.Parse(a), int.Parse(b) + 1);
                    while (historyGuess.Contains(rand.ToString()))
                    {
                        rand = new Random().Next(int.Parse(a), int.Parse(b) + 1);
                    }
                    
                    string guess = rand.ToString();
                    historyGuess.Add(guess);
                    string message = playerName + ":" + guess;
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    AppendMessage("Số đã đoán: " + guess);
                    this.Invoke(new Action(() =>
                    {
                        StartCountdown();  // Bắt đầu đếm ngược sau khi gửi dự đoán
                    }));
                }
            }
        }

        private void StopAutoGuess()
        {
            stopAutoGuess = true;
            if (autoGuessThread != null && autoGuessThread.IsAlive)
            {
                autoGuessThread.Join(); // Chờ thread dừng lại
            }
        }

        private void SaveGameHistory()
        {
            string history = "Game History:\n";
            this.Invoke(new Action(() =>
            {
                history += rtbSVnoti.Text;
            }));
            System.IO.File.WriteAllText("history.txt", history);
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopAutoGuess();
            if (stream != null) stream.Close();
            if (client != null) client.Close();
            SaveGameHistory();
        }
    }
}
