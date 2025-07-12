using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace tool
{
    public class PointData
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point ToPoint() => new Point(X, Y);
        public static PointData FromPoint(Point p) => new PointData { X = p.X, Y = p.Y };
    }

    public class AppSettings
    {
        public Dictionary<string, PointData> Points { get; set; } = new Dictionary<string, PointData>();
        public int Delay { get; set; } = 0;
        public int Timeout { get; set; } = 0;
    }

    internal class JsonSetting
    {
        private static readonly string saveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "viber_config.json");

        public static void SaveSettings(AppSettings settings)
        {
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(saveFilePath, json);
        }

        public static AppSettings LoadSettings()
        {
            if (!File.Exists(saveFilePath))
                return new AppSettings();

            string json = File.ReadAllText(saveFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json);
        }
    }
}
