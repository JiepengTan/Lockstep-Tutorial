using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Logic {
    [CreateAssetMenu(menuName = "SkillInfo")]
    public class SkillBoxConfig : UnityEngine.ScriptableObject {
        public List<SkillInfo> skillInfos = new List<SkillInfo>();

        private bool hasInit = false;

        public void CheckInit(){
            if(hasInit) return;
            hasInit = true;
            foreach (var info in skillInfos) {
                info.DoInit();
            }
        }
    }
}