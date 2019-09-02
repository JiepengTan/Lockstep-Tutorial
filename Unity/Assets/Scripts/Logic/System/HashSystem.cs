using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.Util;
using Lockstep.Game;

namespace Lockstep.Game {
    public class HashSystem : BaseSystem {
        public List<Enemy> AllEnemies => _gameStateService.GetEnemies();
        public List<Player> AllPlayers => _gameStateService.GetPlayers();

        //{string.Format("{0:yyyyMMddHHmmss}", DateTime.Now)}_
        public int GetHash(){
            int hash = 1;
            int idx = 0;
            foreach (var entity in AllPlayers) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            foreach (var entity in AllEnemies) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            return hash;
        }
    }
}