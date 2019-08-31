#define DEBUG_EVENT_TRIGGER
#if UNITY_EDITOR || DEBUG_EVENT_TRIGGER
#define _DEBUG_EVENT_TRIGGER
#endif
using System.Collections.Generic;

namespace Lockstep {
    public delegate void GlobalEventHandler(object param);

    public delegate void NetMsgHandler(object param);

    public partial class EventHelper {
        private static Dictionary<int, List<GlobalEventHandler>> allListeners =
            new Dictionary<int, List<GlobalEventHandler>>();

        private static Queue<MsgInfo> allPendingMsgs = new Queue<MsgInfo>();
        private static Queue<ListenerInfo> allPendingListeners = new Queue<ListenerInfo>();
        private static Queue<EEvent> allNeedRemoveTypes = new Queue<EEvent>();

        private static bool IsTriggingEvent;

        public static void RemoveAllListener(EEvent type){
            if (IsTriggingEvent) {
                allNeedRemoveTypes.Enqueue(type);
                return;
            }

            allListeners.Remove((int) type);
        }

        public static void AddListener(EEvent type, GlobalEventHandler listener){
            if (IsTriggingEvent) {
                allPendingListeners.Enqueue(new ListenerInfo(true, type, listener));
                return;
            }

            var itype = (int) type;
            if (allListeners.TryGetValue(itype, out var tmplst)) {
                tmplst.Add(listener);
            }
            else {
                var lst = new List<GlobalEventHandler>();
                lst.Add(listener);
                allListeners.Add(itype, lst);
            }
        }

        public static void RemoveListener(EEvent type, GlobalEventHandler listener){
            if (IsTriggingEvent) {
                allPendingListeners.Enqueue(new ListenerInfo(false, type, listener));
                return;
            }

            var itype = (int) type;
            if (allListeners.TryGetValue(itype, out var tmplst)) {
                if (tmplst.Remove(listener)) {
                    if (tmplst.Count == 0) {
                        allListeners.Remove(itype);
                    }

                    return;
                }
            }

            //Debug.LogError("Try remove a not exist listner " + type);
        }

        public static void Trigger(EEvent type, object param = null){
            if (IsTriggingEvent) {
                allPendingMsgs.Enqueue(new MsgInfo(type, param));
                return;
            }

            var itype = (int) type;
            if (allListeners.TryGetValue(itype, out var tmplst)) {
                IsTriggingEvent = true;
                foreach (var listener in tmplst.ToArray()) { //TODO 替换成其他更好的方式 避免gc
                    listener?.Invoke(param);
                }
            }

            IsTriggingEvent = false;
            while (allPendingListeners.Count > 0) {
                var msgInfo = allPendingListeners.Dequeue();
                if (msgInfo.isRegister) {
                    AddListener(msgInfo.type, msgInfo.param);
                }
                else {
                    RemoveListener(msgInfo.type, msgInfo.param);
                }
            }

            while (allNeedRemoveTypes.Count > 0) {
                var rmType = allNeedRemoveTypes.Dequeue();
                RemoveAllListener(rmType);
            }

            while (allPendingMsgs.Count > 0) {
                var msgInfo = allPendingMsgs.Dequeue();
                Trigger(msgInfo.type, msgInfo.param);
            }
        }

        public struct MsgInfo {
            public EEvent type;
            public object param;

            public MsgInfo(EEvent type, object param){
                this.type = type;
                this.param = param;
            }
        }

        public struct ListenerInfo {
            public bool isRegister;
            public EEvent type;
            public GlobalEventHandler param;

            public ListenerInfo(bool isRegister, EEvent type, GlobalEventHandler param){
                this.isRegister = isRegister;
                this.type = type;
                this.param = param;
            }
        }
    }
}