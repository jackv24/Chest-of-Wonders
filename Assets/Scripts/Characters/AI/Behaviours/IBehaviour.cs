using System.Collections;
using System.Collections.Generic;

namespace BehaviourTreeCustom
{
    public enum Result
    {
        Success,
        Failure,
        Pending
    }

    public interface IBehaviour
    {
        Result Execute(AIAgent agent);
    }
}
