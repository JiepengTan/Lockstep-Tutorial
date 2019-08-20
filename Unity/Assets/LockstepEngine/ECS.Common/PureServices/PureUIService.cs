using System;
using System.Reflection;

namespace Lockstep.Game {
    public class PureUIService : PureBaseService, IUIService {
        public T GetIService<T>() where T : IService{
            return GetService<T>();
        }

        public   bool IsDebugMode { get; set; }
        public void RegisterAssembly(Assembly uiAssembly){ }
        public void OpenWindow(string dir, EWindowDepth dep, UICallback callback = null){ }
        public void OpenWindow(WindowCreateInfo info, UICallback callback = null){ }
        public void CloseWindow(string dir){ }
        public void CloseWindow(object window){ }
        public void ShowDialog(string title, string body, Action<bool> resultCallback){ }
    }
}