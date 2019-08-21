using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Logic {
    [CreateAssetMenu(menuName = "SkillInfo")]
    public class SkillBoxConfig : UnityEngine.ScriptableObject {
        public List<SkillInfo> skillInfos = new List<SkillInfo>();
    }
}