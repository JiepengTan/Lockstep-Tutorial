using System;

namespace Lockstep.Logging {
    public class BaseLogger {
        protected DebugInstance Debug;

        public void SetLogger(DebugInstance logger){
            this.Debug = logger;
        }

        protected void Log(string format, params object[] args){
            Debug?.Log(format, args);
        }

        protected void LogFormat(string format, params object[] args){
            Debug?.LogFormat(format, args);
        }

        protected void LogError(string format, params object[] args){
            Debug?.LogError(format, args);
        }

        protected void LogError(Exception e){
            Debug?.LogError(e);
        }

        protected void LogErrorFormat(string format, params object[] args){
            Debug?.LogErrorFormat(format, args);
        }

        protected void Assert(bool val, string msg = ""){
            Debug?.Assert(val, msg);
        }
    }
}