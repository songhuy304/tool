using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tool.Properties;
using WindowsInput;
using WindowsInput.Native;

namespace tool
{

    public partial class Form2 : Form
    {
        private Dictionary<string, Point> viberPoints = new Dictionary<string, Point>();
        private CancellationTokenSource cancellationTokenSource;
        private bool isProcessing = false;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int SW_RESTORE = 9;
        private const int WM_HOTKEY = 0x0312;
        private const int VK_ESCAPE = 0x1B;

        public Form2()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.KeyDown += Form2_KeyDown;

            // Đăng ký hotkey ESC global
            RegisterHotKey(this.Handle, 1, 0, VK_ESCAPE);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                if (m.WParam.ToInt32() == 1) // ID của hotkey ESC
                {
                    if (isProcessing)
                    {
                        StopProcessing();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && isProcessing)
            {
                StopProcessing();
            }
        }

        private void StopProcessing()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                MessageBox.Show("Đã dừng gửi tin nhắn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void ClickAtPosition(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }


        // Hàm trả về mảng string chứa tất cả các dòng từ RichTextBox
        public string[] GetLinesFromRichTextBox(RichTextBox rtb)
        {
            return rtb.Lines; 
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            // Nếu đang xử lý thì dừng
            if (isProcessing)
            {
                StopProcessing();
                return;
            }

            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber. Vui lòng mở Viber trước.");
                return;
            }

            string tinNhanGui = txt_tinnhan.Text.Trim();
            if (string.IsNullOrEmpty(tinNhanGui))
            {
                MessageBox.Show("Vui lòng nhập nội dung tin nhắn!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Danh sách số điện thoại trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bắt đầu xử lý
            isProcessing = true;
            cancellationTokenSource = new CancellationTokenSource();

            // Thay đổi text button
            btnSendMessage.Text = "Dừng (ESC)";
            btnSendMessage.BackColor = Color.Red;

            try
            {
                await ProcessSendMessages(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Đã bị hủy
            }
            finally
            {
                // Kết thúc xử lý
                isProcessing = false;
                btnSendMessage.Text = "Gửi tin nhắn";
                btnSendMessage.BackColor = SystemColors.Control;

                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
        }

        private async Task ProcessSendMessages(CancellationToken cancellationToken)
        {
            var viberCapture = new ViberCapture();
            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            IntPtr hWnd = viber.MainWindowHandle;
            var sim = new InputSimulator();
            string tinNhanGui = txt_tinnhan.Text.Trim();

            ShowWindow(hWnd, SW_RESTORE);
            await Task.Delay(1000, cancellationToken);
            SetForegroundWindow(hWnd);
            await Task.Delay(1000, cancellationToken);

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                // Kiểm tra nếu bị hủy
                cancellationToken.ThrowIfCancellationRequested();

                if (GetForegroundWindow() != hWnd)
                {
                    MessageBox.Show("Cửa sổ bị mất hoạt động", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    string sdt = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                    if (string.IsNullOrEmpty(sdt)) continue;

                    // Tô màu dòng đang gửi
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;

                    var searchPos = viberPoints["SearchBox"];
                    ClickAtPosition(searchPos.X, searchPos.Y);
                    await Task.Delay(600 + 100, cancellationToken);

                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                    await Task.Delay(700, cancellationToken);
                    sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    await Task.Delay(700, cancellationToken);

                    sim.Keyboard.TextEntry(sdt);
                    await Task.Delay(700, cancellationToken);

                    string contact = @"Images\test.png";
                    string da_chat = @"Images\da_chat.png";
                    Point? foundPoint = await Task.Run(() => viberCapture.FindTemplatePositionInViber(contact), cancellationToken);
                    Point? clickPoint = null;

                    if (foundPoint.HasValue)
                    {
                        clickPoint = new Point(foundPoint.Value.X + 50, foundPoint.Value.Y + 20);
                    }
                    else
                    {
                        Point? pointDaChat = await Task.Run(() => viberCapture.FindTemplatePositionInViber(da_chat), cancellationToken);
                        if (pointDaChat.HasValue)
                        {
                            clickPoint = new Point(pointDaChat.Value.X + 50, pointDaChat.Value.Y + 70);
                        }
                    }

                    // ❌ Nếu cả 2 ảnh đều không tìm thấy → bỏ qua item này
                    if (!clickPoint.HasValue)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = "Không tìm thấy liên hệ";
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        continue; // ➤ Chuyển sang item kế tiếp
                    }

                    // Nếu có tọa độ cần click → thực hiện
                    await Task.Delay(700, cancellationToken);
                    ClickAtPosition(clickPoint.Value.X, clickPoint.Value.Y);

                    await Task.Delay(700, cancellationToken);

                    string inputText = @"Images\input_text.png";
                    Point? foundPoint2 = await Task.Run(() => viberCapture.FindTemplatePositionInViber(inputText), cancellationToken);

                    if (foundPoint2.HasValue)
                    {
                        viberPoints["input_Text"] = new Point(foundPoint2.Value.X + 140, foundPoint2.Value.Y + 20);
                        var inputPoint = viberPoints["input_Text"];
                        await Task.Delay(700, cancellationToken);
                        ClickAtPosition(inputPoint.X, inputPoint.Y);

                        sim.Keyboard.TextEntry(tinNhanGui);
                        await Task.Delay(600, cancellationToken);
                        sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                        dataGridView1.Rows[i].Cells[2].Value = "Đã gửi";
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[2].Value = "Không tìm thấy ô chat";
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        continue;
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    dataGridView1.Rows[i].Cells[2].Value = "Lỗi";
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }

                await Task.Delay(2000, cancellationToken); // Giãn cách giữa 2 tin nhắn
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                MessageBox.Show("Đã gửi hết!");
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Hủy đăng ký hotkey khi đóng form
            UnregisterHotKey(this.Handle, 1);
            base.OnFormClosed(e);
        }



        private async void button2_Click(object sender, EventArgs e)
        {

            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber. Vui lòng mở Viber trước.");
                return;
            }


            var viberCapture = new ViberCapture();
            string inputSearch = @"Images\img-1.png";


            Point? foundPoint = await Task.Run(() => viberCapture.FindTemplatePositionInViber(inputSearch));

            if (foundPoint.HasValue)
            {
                viberPoints["SearchBox"] = new Point(foundPoint.Value.X + 50, foundPoint.Value.Y + 10);
                MessageBox.Show("Kết nối thành công");

            }
            else
            {
                MessageBox.Show("Không tìm thấy, hãy thoát ra màn hình viber chính");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_addfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] lines = System.IO.File.ReadAllLines(ofd.FileName);

                int currentSTT = dataGridView1.Rows.Count + 1;

                foreach (string line in lines)
                {
                    string sdt = line.Trim();
                    if (!string.IsNullOrEmpty(sdt))
                    {
                        dataGridView1.Rows.Add(currentSTT, sdt, "-");
                        currentSTT++;
                    }
                }

                MessageBox.Show("Đã thêm số điện thoại từ file!");
            }
        }

        private void txtPhoneNumber_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string sdt = txtPhoneNumber.Text.Trim();

            if (!string.IsNullOrEmpty(sdt))
            {
                int sttValue = dataGridView1.Rows.Count + 1;

                // Thêm dòng mới
                dataGridView1.Rows.Add(sttValue, sdt, "-");

                txtPhoneNumber.Clear();
            }
            else
            {
                MessageBox.Show("not null");
            }
        }

        private void txt_tinnhan_TextChanged(object sender, EventArgs e)
        {

        }
    }
}