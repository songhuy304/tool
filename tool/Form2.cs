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
using WindowsInput;
using WindowsInput.Native;

namespace tool

{
    public partial class Form2 : Form
    {
        private Dictionary<string, Point> viberPoints = new Dictionary<string, Point>();
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



        const int MOUSEEVENTF_LEFTDOWN = 0x02;
        const int MOUSEEVENTF_LEFTUP = 0x04;

        private const int SW_RESTORE = 9;
        public Form2()
        {
            InitializeComponent();
        }

        public void ClickAtPosition(int x, int y)
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
                return;
            }
            var viberCapture = new ViberCapture();



            IntPtr hWnd = viber.MainWindowHandle;
            var sim = new InputSimulator();


            ShowWindow(hWnd, SW_RESTORE);
            Thread.Sleep(1000);
            SetForegroundWindow(hWnd);
            Thread.Sleep(1000);

            var searchPos = viberPoints["SearchBox"];
            ClickAtPosition(searchPos.X, searchPos.Y);
            Thread.Sleep(600 + 100);

            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
            Thread.Sleep(700);
            sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
            Thread.Sleep(700);

            sim.Keyboard.TextEntry("0367002204");
            Thread.Sleep(700);

            string contactItem = @"Images\test.png";
            Point? foundPoint2 = await Task.Run(() => viberCapture.FindTemplatePositionInViber(contactItem));
            if (foundPoint2.HasValue)
            {
                viberPoints["Contact"] = new Point(foundPoint2.Value.X + 50, foundPoint2.Value.Y + 10);
                MessageBox.Show("tim thay");

            }
            else
            {
                MessageBox.Show("Không tìm thấy đủ điểm, vui lòng kiểm tra lại ảnh mẫu.");
            }
            Thread.Sleep(700);


         


        }




        private async void button2_Click(object sender, EventArgs e)
        {

            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber. Vui lòng mở Viber trước.");
                return ;
            }

            var viberCapture = new ViberCapture();
            string inputSearch = @"Images\img-1.png";
            string contactItem = @"Images\contact.png";
            string textBox = @"Images\textBox.png";

            // Tìm các vị trí nút
            Point? foundPoint = await Task.Run(() => viberCapture.FindTemplatePositionInViber(inputSearch));
            //Point? foundPoint3 = await Task.Run(() => viberCapture.FindTemplatePositionInViber(textBox));


            if (foundPoint.HasValue)
            {
                // Lưu vào biến toàn cục
                viberPoints["SearchBox"] = new Point(foundPoint.Value.X + 50, foundPoint.Value.Y + 10);
                //viberPoints["Contact"] = new Point(foundPoint2.Value.X + 50, foundPoint2.Value.Y + 10);
                //viberPoints["MessageBox"] = new Point(foundPoint3.Value.X + 100, foundPoint3.Value.Y + 10);

            }
            else
            {
                MessageBox.Show("Không tìm thấy đủ điểm, vui lòng kiểm tra lại ảnh mẫu.");
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
    }
}