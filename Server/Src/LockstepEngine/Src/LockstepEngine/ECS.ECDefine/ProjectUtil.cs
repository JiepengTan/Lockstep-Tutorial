using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lockstep.Util {
    public class ProjectUtil {
        public static void Log(object obj){
            Console.WriteLine(obj.ToString());
        }

        public static void Log(string format, params object[] par){
            Console.WriteLine(string.Format(format, par));
        }

        public static void LogError(object obj){
            Console.WriteLine(obj.ToString());
        }

        public static void LogError(string format, params object[] par){
            Console.WriteLine(string.Format(format, par));
        }

        public static void UpdateProjectFile(string projectPath, string relDir, string dstDir){
            List<string> allLines = new List<string>();
            var projectText = File.ReadAllLines(projectPath);
            int startLine = -1;
            string prefix = "     ";
            string posfix = " \"/>";
            var tag = "<Compile Include=\"" + relDir.Replace("/", "\\");
            for (int i = 0; i < projectText.Length; i++) {
                if (projectText[i].Contains(tag)) {
                    if (startLine == -1) {
                        startLine = i;
                    }
                }
                else {
                    allLines.Add(projectText[i]);
                }
            }

            List<string> newInsert = new List<string>();
            if (startLine != -1) {
                PathUtil.Walk(dstDir, "*.cs", (path) => {
                    newInsert.Add(prefix
                                  + path.Replace(dstDir, tag).Replace("/", "\\")
                                  + posfix
                    );
                });
                allLines.InsertRange(startLine, newInsert);
            }
            else {
                newInsert.Add("  <ItemGroup>");
                PathUtil.Walk(dstDir, "*.cs", (path) => {
                    newInsert.Add(prefix
                                  + path.Replace(dstDir, tag).Replace("/", "\\")
                                  + posfix
                    );
                });
                newInsert.Add("  </ItemGroup>");
                var insrtIdx = 0;
                for (int i = 0; i < allLines.Count; i++) {
                    if (allLines[i].Contains("</ItemGroup>")) {
                        insrtIdx = i + 1;
                        break;
                    }
                }

                allLines.InsertRange(insrtIdx, newInsert);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var line in allLines) {
                sb.AppendLine(line);
            }

            File.WriteAllText(projectPath, sb.ToString());
        }
    }
}