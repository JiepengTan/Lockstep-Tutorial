using System;
using System.IO;
using Lockstep.Logging;

namespace Lockstep.Util {
    public class Utils {
        public static void StartServices(){
            LTime.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            LTime.DoUpdate();
            CoroutineHelper.DoUpdate();
        }

        public static void ExecuteCmd(string shellName, string workingDir){
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = shellName;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = workingDir;
            process.Start();
            Debug.Log($"ExeCmd {process.StartInfo.FileName} {shellName}    ##workingDir={process.StartInfo.WorkingDirectory} ");
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Debug.Log(output);
        }
    }
}