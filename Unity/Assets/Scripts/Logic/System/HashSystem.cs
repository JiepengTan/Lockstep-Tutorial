using System.Collections.Generic;
using Lockstep.Collision2D;
using Lockstep.Math;
using Lockstep.Util;
using Lockstep.Game;

namespace Lockstep.Game {
    public class HashSystem : BaseSystem {
        public override void DoUpdate(LFloat deltaTime){
            //_commonStateService.Hash = GetHash(_gameStateService);
        }

        //{string.Format("{0:yyyyMMddHHmmss}", DateTime.Now)}_
        public static int GetHash(IGameStateService _gameStateService){
            var allEnemies = _gameStateService.GetEnemies();
            var allPlayers = _gameStateService.GetPlayers();
            int hash = 1;
            int idx = 0;
            foreach (var entity in allPlayers) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            foreach (var entity in allEnemies) {
                hash += entity.curHealth.GetHash() * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash() * PrimerLUT.GetPrimer(idx++);
            }

            return hash;
        }
    }
}