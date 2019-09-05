using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.Game {
    public class HashHelper : BaseSimulatorHelper {
        private INetworkService _networkService;
        private IFrameBuffer _cmdBuffer;

        private List<int> _allHashCodes = new List<int>();
        private int _firstHashTick = 0;
        Dictionary<int, int> _tick2Hash = new Dictionary<int, int>();

        public HashHelper(IServiceContainer serviceContainer, World world, INetworkService networkService,
            IFrameBuffer cmdBuffer) : base(serviceContainer, world){
            this._networkService = networkService;
            this._cmdBuffer = cmdBuffer;
        }


        public void CheckAndSendHashCodes(){
            //only sends the hashCodes whose FrameInputs was checked
            if (_cmdBuffer.NextTickToCheck > _firstHashTick) {
                var count = LMath.Min(_allHashCodes.Count, (int) (_cmdBuffer.NextTickToCheck - _firstHashTick),
                    (480 / 4));
                if (count > 0) {
                    _networkService.SendHashCodes(_firstHashTick, _allHashCodes, 0, count);
                    _firstHashTick = _firstHashTick + count;
                    _allHashCodes.RemoveRange(0, count);
                }
            }
        }

        public bool TryGetValue(int tick, out int hash){
            return _tick2Hash.TryGetValue(tick, out hash);
        }

        public int CalcHash(){
            int idx = 0;
            return CalcHash(ref idx);
        }


        private int CalcHash(ref int idx){
            int hashIdx = 0;
            int hashCode = 0;
            foreach (var svc in _serviceContainer.GetAllServices()) {
                if (svc is IHashCode hashSvc) {
                    hashCode += hashSvc.GetHash(ref hashIdx) * PrimerLUT.GetPrimer(hashIdx++);
                }
            }

            return hashCode;
        }

        public void SetHash(int tick, int hash){
            if (tick < _firstHashTick) {
                return;
            }

            var idx = (int) (tick - _firstHashTick);
            if (_allHashCodes.Count <= idx) {
                for (int i = 0; i < idx + 1; i++) {
                    _allHashCodes.Add(0);
                }
            }

            _allHashCodes[idx] = hash;
            _tick2Hash[Tick] = hash;
        }
    }
}