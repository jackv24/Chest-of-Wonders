using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
    [Name("Check Trigger Detector")]
    [Category("Physics")]
    public class CheckTriggerDetector : ConditionTask<TriggerDetector>
    {
        protected override bool OnCheck()
        {
            return agent.InsideCount > 0;
        }
    }
}
