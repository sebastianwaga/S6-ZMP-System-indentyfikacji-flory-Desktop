using System;
using System.IO;

namespace VirtualHerbarium.AdminPanel.Services
{
    public static class LocalDatabasePath
    {
        public static string GetDatabasePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var folder = Path.Combine(appData, "VirtualHerbariumAdmin");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var dbPath = Path.Combine(folder, "offline.db");

            if (!File.Exists(dbPath))
            {
                using var fs = File.Create(dbPath);
            }

            return dbPath;
        }
    }
}
