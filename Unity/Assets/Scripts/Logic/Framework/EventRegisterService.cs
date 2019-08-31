using System;
using System.Reflection;

using UnityEngine;

namespace Lockstep.Game {
    public class EventRegisterService : IEventRegisterService {
        public static T CreateDelegateFromMethodInfo<T>(System.Object instance, MethodInfo method) where T : Delegate{
            return Delegate.CreateDelegate(typeof(T), instance, method) as T;
        }

        public void UnRegisterEvent(object obj){
            RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length,
                EventHelper.RemoveListener, obj);
        }

        public void RegisterEvent(object obj){
            RegisterEvent<EEvent, GlobalEventHandler>("OnEvent_", "OnEvent_".Length, EventHelper.AddListener,
                obj);
        }


        public void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack, object obj)
            where TDelegate : Delegate
            where TEnum : struct{
            if (callBack == null) return;
            var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                                   BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods) {
                var methodName = method.Name;
                if (methodName.StartsWith(prefix)) {
                    var eventTypeStr = methodName.Substring(ignorePrefixLen);
                    if (Enum.TryParse(eventTypeStr, out TEnum eType)) {
                        try {
                            var handler = CreateDelegateFromMethodInfo<TDelegate>(obj, method);
                            callBack(eType, handler);
                        }
                        catch (Exception e) {
                            Debug.LogError("methodName " + methodName);
                            throw;
                        }
                    }
                }
            }
        }
    }
}