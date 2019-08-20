using UnityEngine;
using Random = UnityEngine.Random;

namespace Lockstep.Collision2D {
    public class RandomMove : MonoBehaviour {
        public Vector3 targetPos = new Vector3();
        public float halfworldSize;
        public float spd = 3;
        public float updateInterval = 50;
        private float timer = 0;
        public bool isNeedRotate = false;

        public float rotateSpd = 60;
        void Start(){
            spd = spd * Random.Range(1.0f, 2.0f);
            rotateSpd = rotateSpd * Random.Range(1.0f, 2.0f);
            UpdateTargetPos();
        }

        void UpdateTargetPos(){
            targetPos = new Vector3(Random.Range(-halfworldSize, halfworldSize), 0,
                Random.Range(-halfworldSize, halfworldSize));
        }

        private void Update(){
            timer += Time.deltaTime;
            if (timer > updateInterval) {
                timer = 0;
                UpdateTargetPos();
            }

            if ((transform.position - targetPos).sqrMagnitude < 1) {
                UpdateTargetPos();
            }

            if (isNeedRotate) {
                var deg = transform.localRotation.eulerAngles.y;
                transform.localRotation = Quaternion.Euler(0,deg + Time.deltaTime * rotateSpd,0);
            }

            transform.position += (targetPos - transform.position).normalized * Time.deltaTime * spd;
        }
    }
}