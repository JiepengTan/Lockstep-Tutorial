using System;

namespace Lockstep.Game {
    public interface IEventRegisterService : IService {

        void RegisterEvent(object obj);
        void UnRegisterEvent(object obj);

        void RegisterEvent<TEnum, TDelegate>(string prefix, int ignorePrefixLen,
            Action<TEnum, TDelegate> callBack, object obj)
            where TDelegate : Delegate
            where TEnum : struct;
    }
}