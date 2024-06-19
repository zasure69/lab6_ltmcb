using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Security.Policy;

namespace Lab6
{
    public partial class Server : Form
    {
        public class Player
        {
            public int numCorrect {  get; set; }
            public int numWrong { get; set; }
            public Player()
            {
                numCorrect = 0;
                numWrong = 0;
            }
        
        }
        public Server()
        {
            InitializeComponent();
            clients = new List<TcpClient>();
            player = new Dictionary<string, Player>();
        }

        TcpListener server;
        List<TcpClient> clients;
        Dictionary<string, Player> player;
        int numberToGuess;
        int numberOfRounds = 1;
        int numberOfPlayers = 0;
        bool isServerRun = true;

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            server.BeginAcceptTcpClient(AcceptClient, null);
            AppendMessage("Khởi động server.");
        }
        private void AcceptClient(IAsyncResult ar)
        {
            if (isServerRun)
            {
                TcpClient client = server.EndAcceptTcpClient(ar);
                clients.Add(client);
                AppendMessage("Người chơi đã kết nối.");
                numberOfPlayers++;
                server.BeginAcceptTcpClient(AcceptClient, null);
                AppendPlayer(numberOfPlayers.ToString());
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[256];
                stream.BeginRead(buffer, 0, buffer.Length, ReceiveData, new Tuple<TcpClient, byte[]>(client, buffer));
            }
            
        }

        private void ReceiveData(IAsyncResult ar)
        {
            var state = (Tuple<TcpClient, byte[]>)ar.AsyncState;
            TcpClient client = state.Item1;
            byte[] buffer = state.Item2;

            try
            {
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.EndRead(ar);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                ProcessMessage(message);

                stream.BeginRead(buffer, 0, buffer.Length, ReceiveData, state);
            }
            catch (Exception ex)
            {
                AppendMessage("Lỗi: " + ex.Message);
                clients.Remove(client);
            }
        }

        private void ProcessMessage(string message)
        {
            string[] parts = message.Split(':');
            string playerName = parts[0];
            int guess = int.Parse(parts[1]);

            if (!player.ContainsKey(playerName))
            {
                player[playerName] = new Player();
            }

            string response = "";
            if (guess < numberToGuess)
            {
                response = "Số đoán thấp hơn số cần tìm!";
                BroadcastMessage(playerName + " đã đoán " + guess + ": " + response);
                AppendMessage(playerName + " đã đoán " + guess + ": " + response);
                player[playerName].numWrong++;
            }
            else if (guess > numberToGuess)
            {
                response = "Số đoán cao hơn số cần tìm!";
                BroadcastMessage(playerName + " đã đoán " + guess + ": " + response);
                AppendMessage(playerName + " đã đoán " + guess + ": " + response);
                player[playerName].numWrong++;
            }
            else
            {
                response = "Chính xác!";
                BroadcastMessage(playerName + " đã đoán " + guess + ": " + response);
                AppendMessage(playerName + " đã đoán " + guess + ": " + response);
                numberOfRounds++;
                player[playerName].numCorrect++;
                if (numberOfRounds > int.Parse(tbRound.Text))
                {
                    EndGame();
                    return;
                }
                AppendRound(numberOfRounds.ToString());
                numberToGuess = new Random().Next(1, 101);
                AppendGuessNumber(numberToGuess.ToString());
                BroadcastMessage("Vòng " + numberOfRounds.ToString() + " bắt đầu. " +
                    "Phạm vi số cần đoán " + tbA.Text + " - " + tbB.Text);
                AppendMessage("Vòng " + numberOfRounds.ToString() + " bắt đầu. " +
                    "Phạm vi số cần đoán " + tbA.Text + " - " + tbB.Text);
            }

            
        }

        private void EndGame()
        {
            string winner = "";
            int correct = -1;
            int wrong = 100;
            foreach (var p in player)
            {
                if (p.Value.numCorrect > correct)
                {
                    correct = p.Value.numCorrect;
                    wrong = p.Value.numWrong;
                    winner = p.Key;
                } else if (p.Value.numCorrect == correct)
                {
                    if (p.Value.numWrong < wrong)
                    {
                        correct = p.Value.numCorrect;
                        wrong = p.Value.numWrong;
                        winner = p.Key;
                    } else if (p.Value.numWrong == wrong)
                    {
                        correct = p.Value.numCorrect;
                        wrong = p.Value.numWrong;
                        winner += ", " + p.Key;
                    }
                }
            }
            BroadcastMessage("Trò chơi kết thúc. Người chiến thắng " +  winner + ". Ứng dụng sẽ đóng trong vòng 10 giây nữa.");
            AppendMessage("Trò chơi kết thúc. Người chiến thắng " + winner);

            SendGameHistoryToWebsite();
            
            foreach (var client in clients)
            {
                if (client != null) client.Close();
            }
            isServerRun = false;
            if (server != null) server.Stop();
            Thread.Sleep(5000);
            Application.Exit();
        }

        private async void SendGameHistoryToWebsite()
        {
            string history = "Game History:\n";
            this.Invoke(new Action(() =>
            {
                foreach (var item in lvHistory.Items)
                {
                    var list = item.ToString().Split('{');
                    history += list[1].Substring(0, list[1].Length-1) + "\n";
                }
            }));


            using (HttpClient client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = true }))
            {

                var values = new Dictionary<string, string>
                {
                    { "content", history },
                    { "token" , "03AFcWeA565epLP3MoMzPRQMbwfPe8dw3g_umh8oRP2MLSal88t_ozDBCz7QKsg_4r5WCqc9B6XVOKqwWzJHMWEryPm6QTMX6ZbUK6qrh55DTJVK5mzcfLisFb20o6Q66E0tGp5bHU5WAwSAKMvOanR47BS_9CGnbiyz2grePeTign07qRbSpYd4qmUbtrfDwEM1Dyoszkp49UsiVcx5q7QYR8dSw_us3kF9yw36jrVJQATuYZlxFsVbvei60FTV7UZt5S8xFLtRIekpeXTFls_qMJ0Q0dnaWSE2BC1R1T4Gn4GcAIdLOHyvb3SUZ7-vahbR8jkb_HvHgAJ8z9I38xcE0xPUz-qzs38NcQF1WFLgF5cfz3O1CoX73rPBrA3hUYMMnmGjTKMtlP2JkcOWjtldXl27iRNR0K_94yokI30CMQLZlWryyY2VgkDRm4HbTol-XYHqKdNDq1fYxhcsd9jZ8yDKE65WnlPQBs5geXSlVP7VK2WH5Ji7hgeoq-eteP0Fb-iKuFCg_ZyqvCquDm2cIemf69hoU-gQ721JQkrCL6BPI5tMud9TprJudBJnZoJMnfAH_XZf9ifZrMgOH28_P9HCq7WhW2_wfxJjl7zCIdpWuYs9dQ3msKyVVx2DHGocfcI7r8VNHHaVoNluTvdpR4cvPxC7NzrgFOzvL6FXSf78EOZ_bYhz7a83vGroKMcy_X28A8O3CbvCnJpG07wShK2_yYQqnpbnRUApylBhXeWw3hry-xMECLfqfr4yfAGpRXTqFbXJs5YEnPO9fB8MFttRWz-NoRD_GVbbdaRb2kh96h4TLliKNxZt_wKzqgGxfB7o5GCv-3b84bFxEsp8pj2O_L4KOF7WHiVIczBCr1P4dM99JWWXkcs3WCOGJXz5SoM5TUnPy3JWQEm8px_bUEOuSIKZ3j_eQ18HTe__hSV2XyKl2-5wHjBSCfG3YXnhVSMIvDIIabm0lOMD1klig28SWPK-uvsLqJwNlU63Rvl94cFD0K8ZNkyr1BgG7fzQT4juXxw3hxqLo0nyTNE3s4DB7G3rtqi7JSj-6PzUE1_toIrlUYmpHIUk8kPrRMoNx-vUu6eFLHhv6Q3FZeOdv68j1xnhMsGgA05jxcK8IpWAoYHXdN5z-CXPL2bLhqCM9XgCx20R1ttEwxcUPJyoIOl2B-8R_Jy-Z7GpbZTPs_FHNr-SzsjTddCdTS0PGcJbh6jOuVZtBgXSpTCvEwoSKUSbdSmNPKU-KHAz3iKEeG166vdgcJ8vfv_q9-6ZkRfLlI4aDKS7d4MFiEiBGBkFn-jPbuvHoc5BejbywdMB6F7Sx3HrIDNgjVrfCj8T1xzb2ZuU4uYmw6A52IEuSdmYsYF0THS4_O_TQdLxuyvuc8VOXCAPRWHL6BTQhCYGb5nge2B8lSPX_BjIQV4bucSzZLaMoBe973ViDLp33qbOtsYsMhqH6nbadVjOU_xXMHAD1oTn5dGmI-vUylDoJVQY6w5Vk_XG8wsu5POyZtsmzEpnasMQ1OKxftDdYkfSlqdMX9BMG20EHqBXL00MMQV_DKQwHWvddQWYcv38fqnTwR1Wqs2WXkB7amlNEcLZdhKgtWHfXpPe1YkKximWC2gLLdzONz-cTaY_vVxOo_TLL5aYaCSzJu0kRnmgF0IpWygXggbXUBDDgeyJ7dm_-HP_CgCyZ4ZkWEkg"},
                    { "ttl", "1h" }
                };

                var contentToPost = new FormUrlEncodedContent(values);

                HttpResponseMessage response = await client.PostAsync("https://ctxt.io/new", contentToPost);

                string responseText = await response.Content.ReadAsStringAsync();
                // Tìm URL trong phản hồi
                MessageBox.Show(response.RequestMessage.RequestUri.ToString());
                System.IO.File.WriteAllText("content.txt", contentToPost.Headers.ToString());
                System.IO.File.WriteAllText("web.txt", responseText);
                System.IO.File.WriteAllText("webhis.txt", history);
                
            }
        }

        private string ExtractUrlFromResponse(string responseText)
        {
            // Hàm này sẽ trích xuất URL từ phản hồi của https://ctxt.io
            // Giả định rằng phản hồi chứa URL trong một thuộc tính "Location" hoặc một dạng tương tự
            // Cần phải phân tích đúng định dạng phản hồi để lấy URL
            // Đây chỉ là một ví dụ đơn giản, cần phải kiểm tra lại với định dạng thực tế của phản hồi
            // Nếu URL ở cuối response
            int startIndex = responseText.LastIndexOf("https://ctxt.io/");
            if (startIndex != -1)
            {
                string url = responseText.Substring(startIndex);
                int endIndex = url.IndexOf("\""); // Giả sử URL kết thúc với dấu ngoặc kép
                if (endIndex != -1)
                {
                    url = url.Substring(0, endIndex);
                }
                return url;
            }
            return string.Empty;
        }

        private void BroadcastMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in clients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
        }

        private void AppendPlayer(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendPlayer), new object[] { message });
            }
            else
            {
                tbNumPlay.Text = message;
            }
        }

        private void AppendRound(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendRound), new object[] { message });
            }
            else
            {
                textBox1.Text = message;
            }
        }

        private void AppendGuessNumber(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendGuessNumber), new object[] { message });
            }
            else
            {
                tbX.Text = message;
                if (int.Parse(message) - 10 < 0)
                {
                    tbA.Text = "0";
                } else
                {
                    tbA.Text = (int.Parse(message) - 10).ToString();
                }
                tbB.Text = (int.Parse(message) + 10).ToString();
            }
        }

        private void AppendMessage(string message)
        {
            if (isServerRun)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<string>(AppendMessage), new object[] { message });
                }
                else
                {
                    lvHistory.Items.Add(message);
                }
            }
            
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (server != null) server.Stop();
            foreach (var client in clients)
            {
                if (client != null) client.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnStart.Enabled)
            {
                MessageBox.Show("Server chưa khởi động!");
                return;
            }
            else if (tbRound.Text == "")
            {
                MessageBox.Show("Vui lòng điền số vòng chơi.");
                return;
            }
            else if (int.Parse(tbRound.Text) < 0)
            {
                MessageBox.Show("Số vòng chơi tối thiểu là 5.");
                return;
            } 
            else if (int.Parse(tbNumPlay.Text) == 0)
            {
                MessageBox.Show("Chưa có người chơi kết nối vào!");
                return;
            }
            numberToGuess = new Random().Next(1, 101); // Random số từ 1-100
            textBox1.Text = "1";
            tbA.Text = (numberToGuess - 10).ToString();
            tbB.Text = (numberToGuess + 10).ToString();
            tbX.Text = numberToGuess.ToString();
            
            button1.Enabled = false;
            BroadcastMessage("Trò chơi bắt đầu. Phạm vi số cần đoán của vòng 1: " + tbA.Text + " - " + tbB.Text);
            AppendMessage("Trò chơi bắt đầu. Phạm vi số cần đoán của vòng 1: " + tbA.Text + " - " + tbB.Text);
        }
    }
}

