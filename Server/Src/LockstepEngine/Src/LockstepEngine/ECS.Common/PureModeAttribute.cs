namespace Lockstep {
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true)]
    public class PureModeAttribute : System.Attribute {
        private EPureModeType _pureMode;
        public EPureModeType Type => _pureMode;

        public PureModeAttribute(EPureModeType pureMode){
            this._pureMode = pureMode;
        }
    }
}