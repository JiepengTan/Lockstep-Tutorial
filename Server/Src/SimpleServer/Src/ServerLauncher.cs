using System;
using System.Threading;
using Lockstep.Logging;
using Lockstep.Network;
using Lockstep.Util;

namespace Lockstep.FakeServer{
    public class ServerLauncher {
        private static Server server;

        public static void Main(){
            //let async functions call in this thread  
            OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(contex);
            Debug.Log("Main start");
            Utils.StartServices();
            try {
                DoAwake();
                while (true) {
                    try {
                        Thread.Sleep(3);
                        contex.Update();
                        server.Update();
                    }
                    catch (ThreadAbortException e) {
                        return;
                    }
                    catch (Exception e) {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (ThreadAbortException e) {
                return;
            }
            catch (Exception e) {
                Log.Error(e.ToString());
            }
        }

        static void DoAwake(){
            server = new Server();
            server.Start();
        }
    }
}