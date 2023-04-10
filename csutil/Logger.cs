using System;
using System.IO;

namespace csutil
{
    public class Logger
    {
        private readonly LoggerSettings _settings;

        public Logger() => _settings = new LoggerSettings();
        public Logger(LoggerSettings settings) => _settings = settings ?? new LoggerSettings();

        public Logger(bool logToFile = false, bool logToConsole = true, bool addTimestamp = true,
            string timestampFormat = "u", bool singleLogFile = false, string logDirectory = @".\logs\",
            string logName = "log", LoggerColors infoColors = null, LoggerColors debugColors = null,
            LoggerColors warnColors = null, LoggerColors errorColors = null, bool useColors = true)
        {
            _settings = new LoggerSettings(logToFile, logToConsole, addTimestamp, timestampFormat, singleLogFile,
                logDirectory, logName, infoColors, debugColors, warnColors, errorColors, useColors);
        }

        protected void Log(string prefix, string data, LoggerColors colors = null)
        {
            var now = DateTime.Now;
            var content = $"[{prefix}]: {data}{Environment.NewLine}";

            if (_settings.AddTimestamp) content = $"[{now.ToString(_settings.TimestampFormat)}] " + content;

            if (_settings.LogToConsole)
            {
                var currentForegroundColor = Console.ForegroundColor;
                var currentBackgroundColor = Console.BackgroundColor;

                if (_settings.UseColors)
                {
                    if (colors == null) colors = new LoggerColors();
                    Console.ForegroundColor = colors.FG;
                    Console.BackgroundColor = colors.BG;
                }

                Console.Write($"{content}");

                if (_settings.UseColors)
                {
                    Console.ForegroundColor = currentForegroundColor;
                    Console.BackgroundColor = currentBackgroundColor;
                }
            }

            if (_settings.LogToFile)
            {
                if (!Directory.Exists(_settings.LogDirectory)) Directory.CreateDirectory(_settings.LogDirectory);
                string logPath;

                if (_settings.SingleLogFile)
                {
                    logPath = Path.Combine(_settings.LogDirectory, $"{_settings.LogName}.txt");
                }
                else
                {
                    var year = now.Year.ToString();
                    var month = now.Month.ToString();
                    var day = now.Day.ToString();

                    var logDir = Path.Combine(_settings.LogDirectory, year, month, day);
                    if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                    logPath = Path.Combine(logDir, $"{_settings.LogName}.txt");
                }

                if (File.Exists(logPath)) File.AppendAllText(logPath, content);
                else File.WriteAllText(logPath, content);
            }
        }

        public void Info(object data) => Log("INFO", data.ToString(), _settings.InfoColors);

        public void Debug(object data) => Log("DBG", data.ToString(), _settings.DebugColors);

        public void Warn(object data) => Log("WARN", data.ToString(), _settings.WarnColors);

        public void Error(object data) => Log("ERR", data.ToString(), _settings.ErrorColors);
    }

    public class LoggerSettings
    {
        public LoggerSettings(
            bool logToFile = false, bool logToConsole = true, bool addTimestamp = true, string timestampFormat = "u",
            bool singleLogFile = true, string logDirectory = @".\logs\",
            string logName = "log", LoggerColors infoColors = null, LoggerColors debugColors = null,
            LoggerColors warnColors = null, LoggerColors errorColors = null, bool useColors = true
        )
        {
            LogToFile = logToFile;
            LogToConsole = logToConsole;
            AddTimestamp = addTimestamp;
            TimestampFormat = timestampFormat;
            SingleLogFile = singleLogFile;
            LogDirectory = logDirectory;
            LogName = logName;
            InfoColors = infoColors ?? new LoggerColors();
            DebugColors = debugColors ?? new LoggerColors();
            WarnColors = warnColors ?? new LoggerColors(ConsoleColor.DarkYellow);
            ErrorColors = errorColors ?? new LoggerColors(ConsoleColor.Red);
            UseColors = useColors;
        }

        public bool LogToFile { get; }
        public bool LogToConsole { get; }
        public bool AddTimestamp { get; }
        public bool SingleLogFile { get; }
        public bool UseColors { get; }

        public string TimestampFormat { get; }
        public string LogDirectory { get; }
        public string LogName { get; }

        public LoggerColors InfoColors { get; }
        public LoggerColors DebugColors { get; }
        public LoggerColors WarnColors { get; }
        public LoggerColors ErrorColors { get; }
    }

    public class LoggerColors
    {
        public LoggerColors(ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            FG = fg;
            BG = bg;
        }

        public ConsoleColor FG { get; }
        public ConsoleColor BG { get; }
    }
}