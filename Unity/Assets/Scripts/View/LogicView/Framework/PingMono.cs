using System.Collections.Generic;
using System.Linq;
using Lockstep.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PingMono : UnityEngine.MonoBehaviour {
       [SerializeField] private int PingVal;
        private void OnGUI(){
            PingVal = SimulatorService.Instance.PingVal;
            GUI.Label(new Rect(0, 0, 100, 100), $"Ping: {PingVal}ms Dealy: {SimulatorService.Instance.DelayVal}ms ");
        }
    }
}