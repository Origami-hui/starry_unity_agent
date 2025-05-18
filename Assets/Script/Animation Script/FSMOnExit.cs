using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnExit : StateMachineBehaviour
{

    public string[] onExitMessage;

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var e in onExitMessage)
        {
            animator.gameObject.SendMessage(e);
        }
    }

}
