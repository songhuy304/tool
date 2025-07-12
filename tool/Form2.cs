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
        private bool isPaused = false;
        private int currentRowIndex = 0; // Lưu vị trí hiện tại
        private int currentMessageIndex = 0; // Lưu index tin nhắn hiện tại

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

        private int GetDelayValue()
        {
            return (int)txt_delay.Value;
        }

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
                        TogglePauseResume();
                    }
                }
            }
            base.WndProc(ref m);
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && isProcessing)
            {
                TogglePauseResume();
            }
        }

        private async void TogglePauseResume()
        {
            if (isPaused)
            {
                // Tiếp tục
                await ResumeProcessing();
            }
            else
            {
                // Tạm dừng
                PauseProcessing();
            }
        }

        private void PauseProcessing()
        {
            isPaused = true;
            btnSendMessage.Text = "Tiếp tục (ESC)";
            btnSendMessage.BackColor = Color.Orange;
            MessageBox.Show($"Đã tạm dừng tại dòng {currentRowIndex + 1}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task ResumeProcessing()
        {
            isPaused = false;
            btnSendMessage.Text = "Tạm dừng (ESC)";
            btnSendMessage.BackColor = Color.Red;

            // Mở lại cửa sổ Viber và thiết lập lại môi trường
            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber != null && viber.MainWindowHandle != IntPtr.Zero)
            {
                IntPtr hWnd = viber.MainWindowHandle;
                ShowWindow(hWnd, SW_RESTORE);
                await Task.Delay(1000);
                SetForegroundWindow(hWnd);
                await Task.Delay(1000);
            }
        }

        private void StopProcessing()
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
                ResetProcessingState();
                MessageBox.Show("Đã dừng hoàn toàn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetProcessingState()
        {
            currentRowIndex = 0;
            currentMessageIndex = 0;
            isProcessing = false;
            isPaused = false;
            btnSendMessage.Text = "Gửi tin nhắn";
            btnSendMessage.BackColor = SystemColors.Control;
        }

        public void ClickAtPosition(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }

        public string[] GetLinesFromRichTextBox(RichTextBox rtb)
        {
            return rtb.Lines;
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                if (isPaused)
                {
                    // Tiếp tục
                    await ResumeProcessing();
                    return;
                }
                else
                {
                    // Dừng hoàn toàn
                    StopProcessing();
                    return;
                }
            }

            // Bắt đầu mới
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
            isPaused = false;
            currentRowIndex = 0;
            currentMessageIndex = 0;
            cancellationTokenSource = new CancellationTokenSource();

            // Thay đổi text button
            btnSendMessage.Text = "Tạm dừng (ESC)";
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
                ResetProcessingState();

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
            string[] messageLines = GetLinesFromRichTextBox(txt_tinnhan);

            if (messageLines.Length == 0)
            {
                MessageBox.Show("Vui lòng nhập nội dung tin nhắn!");
                return;
            }

            // Thiết lập môi trường Viber ban đầu
            await SetupViberEnvironment(hWnd, cancellationToken);

            // Bắt đầu từ vị trí đã lưu
            for (int i = currentRowIndex; i < dataGridView1.Rows.Count; i++)
            {
                // Cập nhật vị trí hiện tại
                currentRowIndex = i;

                // Chờ khi bị tạm dừng
                while (isPaused && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(100, cancellationToken);
                }

                // Kiểm tra nếu bị hủy
                cancellationToken.ThrowIfCancellationRequested();

                // Sau khi tiếp tục từ pause, thiết lập lại môi trường
                if (i > 0 && GetForegroundWindow() != hWnd)
                {
                    await SetupViberEnvironment(hWnd, cancellationToken);
                }

                // Lấy dòng tin nhắn theo index và tăng index
                string currentMessage = messageLines[currentMessageIndex];
                currentMessageIndex = (currentMessageIndex + 1) % messageLines.Length;

                if (GetForegroundWindow() != hWnd)
                {
                    PauseProcessing();
                    MessageBox.Show("Cửa sổ bị mất hoạt động. Đã tạm dừng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Chờ người dùng tiếp tục
                    while (isPaused && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(100, cancellationToken);
                    }

                    // Khi tiếp tục, thiết lập lại môi trường
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await SetupViberEnvironment(hWnd, cancellationToken);
                    }
                }

                try
                {
                    string sdt = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                    if (string.IsNullOrEmpty(sdt)) continue;

                    // Tô màu dòng đang gửi
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;

                    var searchPos = viberPoints["SearchBox"];
                    ClickAtPosition(searchPos.X, searchPos.Y);
                    await Task.Delay(GetDelayValue(), cancellationToken);

                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                    await Task.Delay(GetDelayValue(), cancellationToken);
                    sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    await Task.Delay(GetDelayValue(), cancellationToken);

                    sim.Keyboard.TextEntry(sdt);
                    await Task.Delay(GetDelayValue(), cancellationToken);

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

                    if (!clickPoint.HasValue)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = "Không tìm thấy liên hệ";
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                        continue;
                    }

                    await Task.Delay(GetDelayValue(), cancellationToken);
                    ClickAtPosition(clickPoint.Value.X, clickPoint.Value.Y);
                    await Task.Delay(GetDelayValue(), cancellationToken);

                    string inputText = @"Images\input_text.png";
                    Point? foundPoint2 = await Task.Run(() => viberCapture.FindTemplatePositionInViber(inputText), cancellationToken);

                    if (foundPoint2.HasValue)
                    {
                        viberPoints["input_Text"] = new Point(foundPoint2.Value.X + 140, foundPoint2.Value.Y + 20);
                        var inputPoint = viberPoints["input_Text"];
                        await Task.Delay(GetDelayValue(), cancellationToken);
                        ClickAtPosition(inputPoint.X, inputPoint.Y);

                        sim.Keyboard.TextEntry(currentMessage);
                        await Task.Delay(GetDelayValue(), cancellationToken);
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

                await Task.Delay(2000, cancellationToken);
            }

            // Hoàn thành
            MessageBox.Show("Đã gửi tin nhắn cho tất cả!", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Hàm thiết lập môi trường Viber
        private async Task SetupViberEnvironment(IntPtr hWnd, CancellationToken cancellationToken)
        {
            ShowWindow(hWnd, SW_RESTORE);
            await Task.Delay(1000, cancellationToken);
            SetForegroundWindow(hWnd);
            await Task.Delay(1000, cancellationToken);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
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
                MessageBox.Show(this, "Kết nối thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        dataGridView1.Rows.Add(currentSTT, sdt, "Chưa gửi");
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
                dataGridView1.Rows.Add(sttValue, sdt, "Chưa gửi");

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

        private void button1_Click(object sender, EventArgs e)
        {
            // Tạo danh sách để lưu các số điện thoại chưa gửi
            List<string> unsentNumbers = new List<string>();

            // Duyệt qua tất cả các dòng trong DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Kiểm tra nếu dòng không phải là dòng mới (chưa có dữ liệu)
                if (!row.IsNewRow)
                {
                    // Lấy giá trị từ cột trạng thái (giả sử cột thứ 3 là cột trạng thái)
                    string status = row.Cells[2].Value?.ToString();

                    // Nếu trạng thái khác "Đã gửi" hoặc chưa có trạng thái
                    if (status != "Đã gửi")
                    {
                        // Lấy số điện thoại từ cột thứ 2 (giả sử)
                        string phoneNumber = row.Cells[1].Value?.ToString();

                        if (!string.IsNullOrEmpty(phoneNumber))
                        {
                            unsentNumbers.Add(phoneNumber);
                        }
                    }
                }
            }

            // Hiển thị kết quả hoặc lưu vào file
            if (unsentNumbers.Count > 0)
            {
                // Ví dụ: Lưu vào file text
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.Title = "Lưu danh sách số chưa gửi";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(saveFileDialog.FileName, unsentNumbers);
                    MessageBox.Show($"Đã lưu {unsentNumbers.Count} số chưa gửi vào file.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Không có số điện thoại nào chưa gửi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btn_xoatin_Click(object sender, EventArgs e)
        {
            // Kiểm tra có dòng nào đang được chọn không
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Xác nhận trước khi xóa
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa dòng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Xóa dòng được chọn
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                        {
                            dataGridView1.Rows.Remove(row);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng muốn xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}