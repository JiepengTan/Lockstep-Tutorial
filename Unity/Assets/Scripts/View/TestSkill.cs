using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Game;
using UnityEngine;

public class TestSkill : MonoBehaviour
{
    public Skill skill = new Skill() ;

    public SkillBoxConfig config;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int skillIdx;

    private void OnDrawGizmos(){
        if(config == null) return;
        if (skillIdx<0 || skillIdx > config.skillInfos.Count) {
            return;
        }
        config.CheckInit();
        if (skill != null) {
            skill.SkillInfo = config.skillInfos[skillIdx];
        }
        skill.OnDrawGizmos();
    }
}
