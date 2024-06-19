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
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using static System.Net.WebRequestMethods;

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
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "i6kYEHNlCjQqX9GAqeZYk8bTYSJ0yU4oS788QVPe",
            BasePath = "https://test-df66b-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };

        IFirebaseClient client;
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
            client = new FireSharp.FirebaseClient(config);
            if (client != null)
            {
                MessageBox.Show("Connection is established!");
            }


            var values = new Dictionary<string, string>
                {
                    { "history", history },
                };

                SetResponse response = await client.SetAsync("History/",values);
                MessageBox.Show("Data is inserted!");


                

            
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

