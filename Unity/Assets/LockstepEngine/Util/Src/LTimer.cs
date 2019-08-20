using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Util;


namespace Lockstep.Util {
    public static class LTimer {
        public delegate void DoneHandler(bool isSuccessful);
        /// Event, which is invoked every second
        public static event Action<long> OnTickPerSeconds;
        public static long CurrentTick { get; private set; }

        private static List<Action> _mainThreadActions;
        private static readonly object _mainThreadLock = new object();

        public static void DoStart(){
            _mainThreadActions = new List<Action>();
            CoroutineHelper.StartCoroutine(StartTicker());
        }

        public static void DoUpdate(){
            if (_mainThreadActions.Count > 0) {
                lock (_mainThreadLock) {
                    foreach (var actions in _mainThreadActions) {
                        actions.Invoke();
                    }

                    _mainThreadActions.Clear();
                }
            }
        }

        /// <summary>
        ///     Waits while condition is false
        ///     If timed out, callback will be invoked with false
        /// </summary>
        public static void WaitUntil(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds){
            CoroutineHelper.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds, true));
        }

        /// <summary>
        ///     Waits while condition is true
        ///     If timed out, callback will be invoked with false
        /// </summary>
        public static void WaitWhile(Func<bool> condiction, DoneHandler doneCallback, float timeoutSeconds){
            CoroutineHelper.StartCoroutine(WaitWhileTrueCoroutine(condiction, doneCallback, timeoutSeconds));
        }

        private static IEnumerator WaitWhileTrueCoroutine(Func<bool> condition, DoneHandler callback,
            float timeoutSeconds, bool reverseCondition = false){
            while ((timeoutSeconds > 0) && (condition.Invoke() == !reverseCondition)) {
                timeoutSeconds -= LTime.deltaTime;
                yield return null;
            }

            callback.Invoke(timeoutSeconds > 0);
        }

        public static void AfterSeconds(float time, Action callback){
            CoroutineHelper.StartCoroutine(StartWaitingSeconds(time, callback));
        }

        public static void ExecuteOnMainThread(Action action){
            OnMainThread(action);
        }

        public static void OnMainThread(Action action){
            lock (_mainThreadLock) {
                _mainThreadActions.Add(action);
            }
        }

        private static IEnumerator StartWaitingSeconds(float time, Action callback){
            yield return new WaitForSeconds(time);
            callback.Invoke();
        }

        private static IEnumerator StartTicker(){
            CurrentTick = 0;
            while (true) {
                yield return new WaitForSeconds(1);
                CurrentTick++;
                try {
                    if (OnTickPerSeconds != null)
                        OnTickPerSeconds.Invoke(CurrentTick);
                }
                catch (Exception e) {
                    Debug.LogError(e);
                }
            }
        }
    }
}