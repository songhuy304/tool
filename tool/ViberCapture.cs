using OpenCvSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp.Extensions;
public class ViberCapture
{
    // Cấu trúc và API Windows cần thiết
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("dwmapi.dll")]
    private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);

    private const int SW_RESTORE = 9;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;



    // Hàm chính để chụp và lưu ảnh Viber
    public Bitmap CaptureViberToMemory()
    {
        try
        {
            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber. Vui lòng mở Viber trước.");
                return null;  
            }

            IntPtr hWnd = viber.MainWindowHandle;
            ShowWindow(hWnd, SW_RESTORE);
            Thread.Sleep(500);
            SetForegroundWindow(hWnd);
            Thread.Sleep(500);

            RECT rect;
            DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT)));

            var bmp = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size);
            }

            return bmp; // Không lưu, chỉ trả về ảnh
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi chụp Viber: {ex.Message}");
            return null;
        }
    }

    public System.Drawing.Point? FindTemplatePositionInViber(string templateImagePath)
    {
        try
        {
            // 1. Chụp màn hình Viber
            Bitmap viberBmp = CaptureViberToMemory();
            if (viberBmp == null)
            {
                return null;
            }

            // 2. Lấy tọa độ màn hình của cửa sổ Viber
            var viber = Process.GetProcessesByName("Viber").FirstOrDefault();
            if (viber == null || viber.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Không tìm thấy cửa sổ Viber.");
                return null;
            }

            RECT rect;
            DwmGetWindowAttribute(viber.MainWindowHandle, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT)));
            int windowX = rect.Left;
            int windowY = rect.Top;

            // 3. Chuyển Bitmap => Mat
            Mat viberImage;
            using (MemoryStream ms = new MemoryStream())
            {
                viberBmp.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                viberImage = Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
            }

            // 4. Đọc template
            using (Mat template = Cv2.ImRead(templateImagePath, ImreadModes.Color))
            {
                if (template.Empty() || viberImage.Empty())
                {
                    return null;
                }

                // 5. Template Matching
                Mat result = new Mat();
                Cv2.MatchTemplate(viberImage, template, result, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);

                if (maxVal < 0.8)
                {
                    return null;
                }

                // 6. Tính tọa độ thực tế trên màn hình
                int absoluteX = windowX + maxLoc.X;
                int absoluteY = windowY + maxLoc.Y;

                return new System.Drawing.Point(absoluteX, absoluteY);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi tìm ảnh mẫu: {ex.Message}");
            return null;
        }
    }


    // Cách sử dụng:



}