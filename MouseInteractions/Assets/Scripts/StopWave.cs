using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWave : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
          animator.SetBool("Idle", true);

    }
}
