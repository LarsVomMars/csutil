using System;
using System.IO;
using System.Linq;

namespace csutil
{
    public class Backup
    {
        public static void BackupFile(string sourceFile, string backupDir, string rename = "", DateTime? older = null)
        {
            ValidateBackupDir(backupDir);
            var name = new FileInfo(sourceFile).Name;
            if (!string.IsNullOrEmpty(rename)) name = rename;
            var backupFile = Path.Combine(backupDir, name);
            if (File.Exists(backupFile)) File.Delete(backupFile);
            File.Copy(sourceFile, backupFile);
            DeleteOlderFiles(backupDir, older ?? DateTime.Now.AddMonths(-1));
        }

        public static void CreateBackupFile(string name, string content, string backupDir, DateTime? older = null)
        {
            ValidateBackupDir(backupDir);
            var backupFile = Path.Combine(backupDir, name);
            if (File.Exists(backupFile)) File.Delete(backupFile);
            File.WriteAllText(backupFile, content);
            DeleteOlderFiles(backupDir, older ?? DateTime.Now.AddMonths(-1));
        }

        private static void ValidateBackupDir(string backupDir)
        {
            if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
        }

        private static void DeleteOlderFiles(string backupDir, DateTime older)
        {
            Directory.GetFiles(backupDir)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTimeUtc < older)
                .ToList()
                .ForEach(f => f.Delete());
        }

        public static string GetTimeStampCSV() => $"{DateTimeOffset.Now.ToUnixTimeSeconds()}.csv";

        public static void DeleteBackupFile(string name, string backupDir)
        {
            var backupFile = Path.Combine(backupDir, name);
            if (File.Exists(backupFile)) File.Delete(backupFile);
        }
    }
}