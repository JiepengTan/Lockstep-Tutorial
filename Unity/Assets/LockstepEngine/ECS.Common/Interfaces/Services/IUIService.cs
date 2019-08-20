using System;
using System.Reflection;

namespace Lockstep.Game {
    public enum EWindowDepth {
        Normal,
        Notice,
        Forward,
    }

    public struct WindowCreateInfo {
        public string resDir;
        public EWindowDepth depth;

        public WindowCreateInfo(string dir, EWindowDepth dep){
            this.resDir = dir;
            this.depth = dep;
        }
    }

    public delegate void UICallback(object windowObj);

    public interface IUIService : IService {
        bool IsDebugMode { get; }
        T GetIService<T>() where T : IService;
        void RegisterAssembly(Assembly uiAssembly);
        void OpenWindow(string dir, EWindowDepth dep, UICallback callback = null);
        void OpenWindow(WindowCreateInfo info, UICallback callback = null);
        void CloseWindow(string dir);
        void CloseWindow(object window);
        void ShowDialog(string title, string body, Action<bool> resultCallback);
    }
}