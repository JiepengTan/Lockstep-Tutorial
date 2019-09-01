using System;
using System.Collections.Generic;
using System.Reflection;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Common;


namespace Lockstep.ECS {

    public interface IEntitySetter { }

    [System.Serializable]
    public class BaseEntitySetter : BaseFormater,IEntitySetter {

        private static Dictionary<Type, int> type2Idx = new Dictionary<Type, int>();
        private static Dictionary<string, int> _name2Idx;

        public static void UpdateEntityConfigLUT(Dictionary<string, int> name2Idx){
            _name2Idx = name2Idx;
        }

        public virtual void SetComponentsTo(object targetEntity){
#if false
            var allMemberInfos = this.GetType().GetPublicMemberInfos();
            foreach (var memberInfo in allMemberInfos) {
                int index = 0;
                var memType = memberInfo.type;
                if (type2Idx.TryGetValue(memType, out int qidx)) {
                    index = qidx;
                }
                else {
                    if (_name2Idx.TryGetValue(memType.Name, out int nidx)) {
                        index = nidx;
                        type2Idx.Add(memType, nidx);
                    }
                    else {
                        Lockstep.Logging.Debug.LogError("Do not have type" + memType.Name.ToString());
                        return;
                    }
                }
                
                IComponent srcComp = memberInfo.GetValue(this) as IComponent;
                if (targetEntity.HasComponent(index)) {
                    IComponent dstComp = targetEntity.GetComponent(index);
                    srcComp.CopyPublicMemberValues((object) dstComp);
                }
                else {
                    IComponent dstComp = targetEntity.CreateComponent(index, srcComp.GetType());
                    srcComp.CopyPublicMemberValues((object) dstComp);
                    targetEntity.AddComponent(index, dstComp);
                }
            }
#endif
        }
    }
}