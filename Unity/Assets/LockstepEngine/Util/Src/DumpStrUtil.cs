using System.Collections;
using System.Collections.Generic;
using System.Text;
using Lockstep.Game;

namespace Lockstep.Util {
    public class DumpStrUtil {
        public static void DumpList<T>(List<T> lst, StringBuilder sb, string prefix) where T : IDumpStr{
            sb.AppendLine(prefix + typeof(T).Name + ".Count" + ":" + lst.Count.ToString());
            for (int i = 0; i < lst.Count; i++) {
                sb.AppendLine(prefix + i + ":");
                lst[i]?.DumpStr(sb, "\t" + prefix);
            }
        }

        public static void DumpList<T>(T[] lst, StringBuilder sb, string prefix) where T : IDumpStr{
            sb.AppendLine(prefix + typeof(T).Name + ".Count" + ":" + lst.Length.ToString());
            for (int i = 0; i < lst.Length; i++) {
                sb.AppendLine(prefix + i + ":");
                lst[i]?.DumpStr(sb, "\t" + prefix);
            }
        }
        public static void DumpList(string name,IList lst, StringBuilder sb, string prefix){
            sb.AppendLine(prefix + name + " Count" + ":" + lst.Count.ToString());
            sb.Append("[");
            for (int i = 0; i < lst.Count; i++) {
                sb.Append( i + ":" + lst[i].ToString());
            }
            sb.Append("]");
        }
    }
}