using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnter : StateMachineBehaviour
{
	public SoundEventType sound;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		sound.Play(animator.transform.position);
	}
}
