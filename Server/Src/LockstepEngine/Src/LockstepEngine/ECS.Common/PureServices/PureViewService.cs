
using Lockstep.Math;

namespace Lockstep.Game {
    public class PureViewService : PureBaseService, IService {
        public void BindView(IEntity entity, short assetId, LVector2 createPos, int deg = 0){ }
        public void DeleteView(uint entityId){ }
        public void RebindView(IEntity entity){ }
        public void RebindAllEntities(){ }
    }
}