using System;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameUnitService : IService {
        void CreateBullet(LVector2 pos, EDir dir, int type, IEntity owner);
        void CreateEnemy(LVector2 pos, int type);
        void CreateEnemy(LVector2 pos);
        void CreateCamp(LVector2 pos, int type);
        void CreatePlayer(byte actorId, int type);
        void DropItem(LFloat rate);
        void TakeDamage(IEntity bullet, IEntity suffer);
        void DelayCall(LFloat delay, Action callback);
        void Upgrade(IEntity entity);
    }

}