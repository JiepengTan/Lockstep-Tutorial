using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public class ManagerContainer : IManagerContainer {
        private Dictionary<string, BaseService> _name2Mgr = new Dictionary<string, BaseService>();
        public List<BaseService> AllMgrs = new List<BaseService>();

        public void RegisterManager(BaseService service){
            var name = service.GetType().Name;
            if (_name2Mgr.ContainsKey(name)) {
                //Debug.LogError($"Duplicate Register manager {name} type:{service?.GetType().ToString() ?? ""}");
                return;
            }

            _name2Mgr.Add(name, service);
            AllMgrs.Add(service);
        }

        public T GetManager<T>() where T : BaseService{
            if (_name2Mgr.TryGetValue(typeof(T).Name, out var val)) {
                return val as T;
            }

            return null;
        }

        public void Foreach(Action<BaseService> func){
            foreach (var mgr in AllMgrs) {
                func(mgr);
            }
        }
    }
}