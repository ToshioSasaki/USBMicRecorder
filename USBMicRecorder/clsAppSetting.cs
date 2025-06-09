using System.Text.Json;

namespace USBMicRecorder
{


    /// <summary>
    /// アプリケーションの設定を表すクラスです。
    /// </summary>
    public class AppSettings
    {
        public string LastSaveDirectory { get; set; } = string.Empty;
        public string LastPlayDirectory { get; set; } = string.Empty;
    }

    /// <summary>
    /// アプリケーションの設定を管理するクラスです。
    /// </summary>
    public class SettingsManager
    {
        private static readonly string SettingsPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyMicApp", "settings.json");

        /// <summary>
        /// アプリケーションの設定を読み込みます。
        /// </summary>
        /// <returns></returns>
        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    return settings ?? new AppSettings();
                }
            }
            catch { }
            return new AppSettings();
        }

        /// <summary>
        /// アプリケーションの設定を保存します。
        /// </summary>
        /// <param name="settings"></param>
        public static void Save(AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }

}
