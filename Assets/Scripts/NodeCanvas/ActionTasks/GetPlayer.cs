using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

namespace NodeCanvas.Tasks.Actions
{
    public class GetPlayer : ActionTask
    {
        public BBParameter<GameObject> storeValue;

        private GameObject player;

        protected override string OnInit()
        {
            if(!player)
            {
                player = GameObject.FindWithTag("Player");
            }

            return base.OnInit();
        }

        protected override void OnExecute()
        {
            if(player && !storeValue.isNone)
            {
                storeValue.value = player;
            }

            EndAction(true);
        }
    }
}
