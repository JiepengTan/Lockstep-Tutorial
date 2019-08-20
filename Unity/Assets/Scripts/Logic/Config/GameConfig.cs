using System;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace LockstepTutorial {
    [Serializable]
    public class EnemyConfig {
        public string prefabPath;
        [Header("Health")] public int startingHealth = 100; 
        public int currentHealth;  
        public LFloat sinkSpeed;   
        public int scoreValue = 10;
        public AudioClip deathClip;

        [Header("Attack")] public LFloat timeBetweenAttacks;
        public int attackDamage = 10;
    }

    [Serializable]
    public class PlayerConfig {
        public string prefabPath;
        [Header("Health")] public int startingHealth = 100;              
        public int currentHealth;                                     
        public AudioClip deathClip;                                      
        public LFloat flashSpeed;                                        
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f); 

        [Header("Attack")] public int damagePerShot = 20;
        public LFloat timeBetweenBullets;                
        public LFloat range;                             
        public string faceLightName;                      
        public string attackTransName;

        [Header("movement")] public LFloat speed;
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