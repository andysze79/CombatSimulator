using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/State")]
public class State : ScriptableObject
{
    public enum AnimationTriggerName { 
        Idle, Run, Attack, Hit, Death, None
    }    
    public Action[] actions;
    public Transition[] transitions;
    public AnimationTriggerName playThisAnimation;
    public Color sceneGizmoColor = Color.grey;
    public void EnterState(StateController controller) {
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Initialize(controller);
        }
    }
    public void ExitState(StateController controller) { }
    public void UpdateState(StateController controller) {
        DoActions(controller);
        CheckTransitions(controller);
    }
    private void DoActions(StateController controller) {        
        for (int i = 0; i < actions.Length; i++)
        {
            actions[i].Act(controller);
        }
    }
    private void CheckTransitions(StateController controller) {
        bool decisionResult = false;
        for (int i = 0; i < transitions.Length; i++)
        {
            for (int j = 0; j < transitions[i].decision.Length; j++)
            {
                decisionResult = transitions[i].decision[j].Decide(controller);

                if (decisionResult && transitions[i].logic == TransitionLogic.Or)
                    break;
                if (!decisionResult && transitions[i].logic == TransitionLogic.And)
                    break;
            }

            if (decisionResult)
                controller.TransitionToState(transitions[i].trueState);
            else
                controller.TransitionToState(transitions[i].falseState);
        }
    }
}
