using System.Collections.Generic;
using System.Linq;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PingMono : UnityEngine.MonoBehaviour {
        private void OnGUI(){
            GUI.Label(new Rect(0, 0, 100, 100), $"!!Ping: {SimulatorService.Instance.PingVal}ms");
        }
    }
}