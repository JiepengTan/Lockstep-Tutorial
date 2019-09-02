using System.Collections.Generic;
using System.Linq;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PingMono : UnityEngine.MonoBehaviour {
        private float _guiTimer;
        public List<float> delays => SimulatorService.Delays;

        private void Update(){
            if (delays == null) return;
            _guiTimer += Time.deltaTime;
            if (_guiTimer > 0.5f) {
                _guiTimer = 0;
                SimulatorService.PingVal = (int) (delays.Sum() * 1000 / LMath.Max(delays.Count, 1));
                delays.Clear();
            }
        }

        private void OnGUI(){
            GUI.Label(new Rect(0, 0, 100, 100), $"!!Ping: {SimulatorService.PingVal}ms");
        }
    }
}