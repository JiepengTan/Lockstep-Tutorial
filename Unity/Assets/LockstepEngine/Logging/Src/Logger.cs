using System;
using System.Diagnostics;
using System.Text;

namespace Lockstep.Logging {
    public static class Logger {
        public static LogSeverity LogSeverityLevel =
            LogSeverity.Info | LogSeverity.Warn | LogSeverity.Error | LogSeverity.Exception;

        public static event EventHandler<LogEventArgs> OnMessage = DefaultServerLogHandler;
        public static Action<bool, string> OnAssert;

        public static void SetLogAllSeverities(){
            LogSeverityLevel = LogSeverity.Trace | LogSeverity.Info | LogSeverity.Warn | LogSeverity.Error |
                               LogSeverity.Exception;
        }

        public static void Err(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Error, message, args);
        }

        public static void Warn(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Warn, message, args);
        }

        public static void Info(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Info, message, args);
        }

        public static void Trace(object sender, string message, params object[] args){
            LogMessage(sender, LogSeverity.Trace, message, args);
        }

        public static void Assert(object sender, bool val, string message){
            if (!val) {
                LogMessage(sender, LogSeverity.Error, "AssertFailed!!! " + message);
            }
        }

        private static void LogMessage(object sender, LogSeverity sev, string format, params object[] args){
            if (OnMessage != null && (LogSeverityLevel & sev) != 0) {
                var message = (args != null && args.Length > 0) ? string.Format(format, args) : format;
                OnMessage.Invoke(sender, new LogEventArgs(sev, message));
            }
        }
        
        static StringBuilder _logBuffer = new StringBuilder(); 
        public static void DefaultServerLogHandler(object sernder, LogEventArgs logArgs){
            if ((LogSeverity.Error & logArgs.LogSeverity) != 0
                || (LogSeverity.Exception & logArgs.LogSeverity) != 0
            ) {
                StackTrace st = new StackTrace(true);
                StackFrame[] sf = st.GetFrames();
                for (int i = 4; i < sf.Length; ++i) {
                    var frame = sf[i];
                    _logBuffer.AppendLine(frame.GetMethod().DeclaringType.FullName + "::" + frame.GetMethod().Name +
                                  " Line=" + frame.GetFileLineNumber());
                }
            }

            Console.WriteLine(logArgs.Message);
            if (_logBuffer.Length != 0) {
                Console.WriteLine(_logBuffer.ToString());
                _logBuffer.Length = 0;
                _logBuffer.Clear();
            }
        }
    }
}