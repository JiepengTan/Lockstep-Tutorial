using Lockstep.Collision2D;
using Lockstep.Logic;
using Lockstep.Math;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace LockstepTutorial {

    public class InputMono : UnityEngine.MonoBehaviour {
        private static bool IsReplay => GameManager.Instance.IsReplay;
        [HideInInspector] public int floorMask;
        public float camRayLength = 100;

        public bool hasHitFloor;
        public LVector2 mousePos;
        public LVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        void Start(){
            floorMask = LayerMask.GetMask("Floor");
        }

        public void Update(){
            if (!IsReplay) {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                inputUV = new LVector2(h.ToLFloat(), v.ToLFloat());

                isInputFire = Input.GetButton("Fire1");
                hasHitFloor = Input.GetMouseButtonDown(1);
                if (hasHitFloor) {
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit floorHit;
                    if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
                        mousePos = floorHit.point.ToLVector2XZ();
                    }
                }

                skillId = -1;
                for (int i = 0; i < 6; i++) {
                    if (Input.GetKey(KeyCode.Keypad1 + i)) {
                        skillId = i;
                    }
                }

                isSpeedUp = Input.GetKeyDown(KeyCode.Space);
                GameManager.CurGameInput =  new PlayerInput() {
                    mousePos = mousePos,
                    inputUV = inputUV,
                    isInputFire = isInputFire,
                    skillId = skillId,
                    isSpeedUp = isSpeedUp,
                };
                
            }
        }
    }
}