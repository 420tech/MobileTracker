using System;
using System.IO;

namespace MobileTracker.Services
{
    internal static class EnvLoader
    {
        /// <summary>
        /// Walk up from the current base directory and try to find a .env file.
        /// If found, parse simple KEY=VALUE pairs (supports quoted values) and set
        /// them into Environment variables if they are not already set.
        /// </summary>
        public static void LoadDotEnvIfExists()
        {
            try
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory ?? ".");
                while (dir != null)
                {
                    var envFile = Path.Combine(dir.FullName, ".env");
                    if (File.Exists(envFile))
                    {
                        ParseAndSet(envFile);
                        return;
                    }

                    dir = dir.Parent;
                }
            }
            catch
            {
                // Swallow any exceptions - this helper is best-effort only
            }
        }

        private static void ParseAndSet(string path)
        {
            foreach (var raw in File.ReadAllLines(path))
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                var idx = line.IndexOf('=');
                if (idx <= 0)
                    continue;

                var key = line.Substring(0, idx).Trim();
                var val = line.Substring(idx + 1).Trim();

                // Remove optional surrounding quotes
                if ((val.StartsWith("\"") && val.EndsWith("\"")) || (val.StartsWith("'") && val.EndsWith("'")))
                {
                    val = val.Substring(1, val.Length - 2);
                }

                // Only set if not already present
                if (Environment.GetEnvironmentVariable(key) == null)
                {
                    Environment.SetEnvironmentVariable(key, val);
                }
            }
        }
    }
}
