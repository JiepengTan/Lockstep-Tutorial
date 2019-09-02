using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.Util;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
#if UNITY_5_3_OR_NEWER
using UnityEditor;
using Lockstep.Game;

#endif
namespace Lockstep.CodeGenerator {
    public class Define {
#if !UNITY_5_3_OR_NEWER
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory + RelDir;
#else
        public static string BaseDirectory => Application.dataPath + RelPath;
#endif
        public static string RelPath = "";
    }
}