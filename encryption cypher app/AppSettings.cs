using System;
using System.IO;
using System.Text.Json;

namespace encryption_cypher_app
{
    /// <summary>
    /// Persists lightweight user preferences to %AppData%\EncSypher\settings.json.
    /// All methods are null-safe; a missing or corrupt file returns safe defaults.
    /// </summary>
    internal static class AppSettings
    {
        private static readonly string SettingsDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EncSypher");

        private static readonly string SettingsPath =
            Path.Combine(SettingsDir, "settings.json");

        private sealed class SettingsModel
        {
            public string? LastUsedDirectory { get; set; }
        }

        /// <summary>
        /// Returns the last directory the user opened/saved a file in,
        /// or null if none has been recorded or the directory no longer exists.
        /// </summary>
        public static string? GetLastUsedDirectory()
        {
            SettingsModel model = Load();
            if (model.LastUsedDirectory is not null &&
                Directory.Exists(model.LastUsedDirectory))
            {
                return model.LastUsedDirectory;
            }
            return null;
        }

        /// <summary>
        /// Stores a directory path and immediately flushes to disk.
        /// Silently ignores any IO errors (non-critical preference).
        /// </summary>
        public static void SetLastUsedDirectory(string? directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath)) return;

            SettingsModel model = Load();
            model.LastUsedDirectory = directoryPath;
            Save(model);
        }

        private static SettingsModel Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                    return new SettingsModel();

                string json = File.ReadAllText(SettingsPath);
                return JsonSerializer.Deserialize<SettingsModel>(json) ?? new SettingsModel();
            }
            catch
            {
                return new SettingsModel();
            }
        }

        private static void Save(SettingsModel model)
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                string json = JsonSerializer.Serialize(model,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Non-critical — silently swallow
            }
        }
    }
}
