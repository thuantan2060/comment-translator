using EnvDTE;
using EnvDTE80;
using System;
using System.Runtime.InteropServices;

namespace CommentTranslator.Util
{
    internal class VsOutputLogger
    {
        private static Lazy<Action<string>> _Logger = new Lazy<Action<string>>(() => GetWindow().OutputString);

        private static Action<string> Logger
        {
            get { return _Logger.Value; }
        }

        public static void SetLogger(Action<string> logger)
        {
            _Logger = new Lazy<Action<string>>(() => logger);
        }

        public static void WriteLn(string format, params object[] args)
        {
            var message = string.Format(format, args);
            WriteLn(message);
        }

        public static void WriteLn(string message)
        {
            Logger(message + Environment.NewLine);
        }

        public static void Write(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Write(message);
        }

        public static void Write(string message)
        {
            Logger(message);
        }

        private static OutputWindowPane GetWindow()
        {
            var dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
            return dte.ToolWindows.OutputWindow.ActivePane;
        }
    }
}
