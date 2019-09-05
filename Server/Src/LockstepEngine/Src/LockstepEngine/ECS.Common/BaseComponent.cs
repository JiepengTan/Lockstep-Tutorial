using System;
using System.Collections.Generic;
using Lockstep.Serialization;

namespace Lockstep.ECS {
    public interface ICloneable {
        void CopyTo(object comp);
        object Clone();
    }

    public partial class BaseComponent :BaseFormater, ICloneable {
        public virtual void CopyTo(object comp){ }

        public virtual object Clone(){
            return null;
        }
    }

    public class CopyToUnExceptTypeException : Exception {
        public CopyToUnExceptTypeException(string message, string hint = null)
            : base(hint != null ? message + "\n" + hint : message){ }
    }
}