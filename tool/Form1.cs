using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const int SW_RESTORE = 9;

        Dictionary<string, Point> viberPoints = new Dictionary<string, Point>();


        private void ClickAtPosition(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, UIntPtr.Zero);
        }


        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber. Vui lòng mở Viber trước.");
                return ;
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


            IntPtr hWnd = viber.MainWindowHandle;
            GetWindowRect(hWnd, out RECT rect);

            int w = rect.Right - rect.Left;
            int h = rect.Bottom - rect.Top;

            viberPoints["SearchBox"] = new Point(rect.Left + 120, rect.Top + 50);
            //viberPoints["Contact"] = new Point(viberPoints["SearchBox"].X, viberPoints["SearchBox"].Y + 170);
            viberPoints["Contact"] = new Point(viberPoints["SearchBox"].X, viberPoints["SearchBox"].Y + 140);
            viberPoints["MessageBox"] = new Point(rect.Left + (w / 2), rect.Bottom - 50);

            var sim = new InputSimulator();

            ShowWindow(hWnd, SW_RESTORE);
            Thread.Sleep((int)txt_delay.Value);
            SetForegroundWindow(hWnd);
            Thread.Sleep((int)txt_delay.Value);

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (GetForegroundWindow() != hWnd)
                {
                    MessageBox.Show("Cửa sổ bị mất hoạt động", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                try
                {
                    string sdt = dataGridView1.Rows[i].Cells[1].Value?.ToString();
                    if (string.IsNullOrEmpty(sdt)) continue;

                    // Tô màu dòng đang gửi
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;

                    // 1️⃣ Click Search Box + Gõ SĐT
                    var searchPos = viberPoints["SearchBox"];
                    ClickAtPosition(searchPos.X, searchPos.Y);
                    Thread.Sleep((int)txt_delay.Value + 100);

                    // Xóa nội dung search cũ
                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                    Thread.Sleep((int)txt_delay.Value);
                    sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                    Thread.Sleep((int)txt_delay.Value);

                    sim.Keyboard.TextEntry(sdt);
                    Thread.Sleep((int)txt_delay.Value + 500);

                    // 2️⃣ Click Contact
                    var contactPos = viberPoints["Contact"];
                    ClickAtPosition(contactPos.X, contactPos.Y);
                    Thread.Sleep((int)txt_delay.Value + 200);

                    // 3️⃣ Click Message Box + Gõ + Gửi
                    var msgPos = viberPoints["MessageBox"];
                    ClickAtPosition(msgPos.X, msgPos.Y);
                    Thread.Sleep((int)txt_delay.Value);

                    sim.Keyboard.TextEntry(tinNhanGui);
                    Thread.Sleep((int)txt_delay.Value);
                    sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);

                    // Cập nhật trạng thái + màu thành công
                    dataGridView1.Rows[i].Cells[2].Value = "Đã gửi";
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                catch (Exception ex)
                {
                    dataGridView1.Rows[i].Cells[2].Value = "Lỗi";
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                }

                await Task.Delay(1000); // Giãn cách giữa 2 tin nhắn
            }

            MessageBox.Show("Đã gửi hết!");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string sdt = dataGridView1.Rows[e.RowIndex].Cells[1].Value?.ToString();
                txt_sdt.Text = sdt;
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            string sdt = txt_sdt.Text.Trim();

            if (!string.IsNullOrEmpty(sdt))
            {
                // Lấy số thứ tự tự tăng
                int sttValue = dataGridView1.Rows.Count + 1;

                // Thêm dòng mới
                dataGridView1.Rows.Add(sttValue, sdt, "-");

                // Xóa TextBox sau khi thêm
                txt_sdt.Clear();
            }
            else
            {
                MessageBox.Show("not null");
            }
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
                        dataGridView1.Rows.Add(currentSTT,  sdt, "-");
                        currentSTT++;
                    }
                }

                MessageBox.Show("Đã thêm số điện thoại từ file!");
            }
        }

        private void txt_tinnhan_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_xoatin_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    dataGridView1.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng để xóa!");
            }
        }
    }


}

