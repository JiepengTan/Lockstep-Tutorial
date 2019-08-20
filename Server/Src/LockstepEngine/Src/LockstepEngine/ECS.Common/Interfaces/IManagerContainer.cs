using System.Collections.Generic;
using System.Linq;

namespace Lockstep.Game
{
    public interface IManagerContainer {
        T GetManager<T>() where T : BaseService;
    }
}