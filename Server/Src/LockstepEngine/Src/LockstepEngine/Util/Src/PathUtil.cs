using System;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Lockstep.Util {
    
    public static partial class PathUtil {
        // 遍历所选目录或文件，递归
        public static void Walk(string path, string exts, System.Action<string> callback, bool _is_save_assets = false,
            bool _is_all_directories = true){
            bool isAll = string.IsNullOrEmpty(exts) || exts == "*" || exts == "*.*";
            string[] extList = exts.Replace("*", "").Split('|');

            if (Directory.Exists(path)) {
                // 如果选择的是文件夹
                SearchOption searchOption =
                    _is_all_directories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                string[] files = Directory.GetFiles(path, "*.*", searchOption).Where(file => {
                    if (isAll)
                        return true;
                    foreach (var ext in extList) {
                        if (file.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                            return true;
                        }
                    }

                    return false;
                }).ToArray();

                foreach (var item in files) {
                    if (callback != null) {
                        callback(item);
                    }
                }

                if (_is_save_assets) {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
                }
            }
            else {
                if (isAll) {
                    if (callback != null) {
                        callback(path);
                    }
                }
                else {
                    // 如果选择的是文件
                    foreach (var ext in extList) {
                        if (path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) {
                            if (callback != null) {
                                callback(path);
                            }
                        }
                    }
                }

                if (_is_save_assets) {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
#endif
                }
            }
        }
    }
}