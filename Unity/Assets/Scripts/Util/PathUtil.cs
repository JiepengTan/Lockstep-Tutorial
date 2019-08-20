using System.IO;
using UnityEngine;

namespace Lockstep.Util {
    public static partial class PathUtil {
        public static string GetUnityPath(string path){
            return Path.Combine(Application.dataPath, path);
        }
    }
}