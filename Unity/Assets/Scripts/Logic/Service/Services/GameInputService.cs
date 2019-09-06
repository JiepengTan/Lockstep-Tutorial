using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Logging;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.Game {
    public class GameInputService : IInputService {
        public static PlayerInput CurGameInput = new PlayerInput();

        public void Execute(InputCmd cmd, object entity){
            var input = new Deserializer(cmd.content).Parse<PlayerInput>();
            var playerInput = entity as PlayerInput;
            playerInput.mousePos = input.mousePos;
            playerInput.inputUV = input.inputUV;
            playerInput.isInputFire = input.isInputFire;
            playerInput.skillId = input.skillId;
            playerInput.isSpeedUp = input.isSpeedUp;
            //Debug.Log("InputUV  " + input.inputUV);
        }

        public List<InputCmd> GetInputCmds(){
            if (CurGameInput.Equals(PlayerInput.Empty)) {
                return null;
            }

            return new List<InputCmd>() {
                new InputCmd() {
                    content = CurGameInput.ToBytes()
                }
            };
        }
    }
}