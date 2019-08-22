using System;
using System.Collections.Generic;
using System.Reflection;
using Lockstep.Logic;
using Lockstep.Math;
using Lockstep.Util;
using UnityEngine;

namespace LockstepTutorial {
    [Serializable]
    public class EntityConfig {
        public virtual object Entity { get; }
        public string prefabPath;

        public void CopyTo(object dst){
            if (Entity.GetType() != dst.GetType()) {
                return;
            }

            FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields) {
                var type = field.FieldType;
                if (typeof(BaseComponent).IsAssignableFrom(type)
                    || typeof(CRigidbody).IsAssignableFrom(type)
                    || typeof(Transform).IsAssignableFrom(type)
                ) {
                    CopyTo(field.GetValue(dst), field.GetValue(Entity));
                }
                else {
                    field.SetValue(dst, field.GetValue(Entity));
                }
            }
        }

        void CopyTo(object dst, object src){
            if (src.GetType() != dst.GetType()) {
                return;
            }

            FieldInfo[] fields = dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var field in fields) {
                var type = field.FieldType;
                field.SetValue(dst, field.GetValue(src));
            }
        }
    }

    [Serializable]
    public class EnemyConfig : EntityConfig {
        public override object Entity => entity;
        public Enemy entity = new Enemy();
    }

    [Serializable]
    public class PlayerConfig : EntityConfig {
        public override object Entity => entity;
        public Player entity = new Player();
    }

    [CreateAssetMenu(menuName = "GameConfig")]
    public class GameConfig : ScriptableObject {
        public List<EnemyConfig> enemies = new List<EnemyConfig>();
        public List<PlayerConfig> player = new List<PlayerConfig>();

        public EnemyConfig GetEnemyConfig(int id){
            return enemies[id];
        }

        public PlayerConfig GetPlayerConfig(int id){
            return player[id];
        }
    }
}