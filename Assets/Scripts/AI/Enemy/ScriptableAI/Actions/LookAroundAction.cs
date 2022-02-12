using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAround")]
public class LookAroundAction : Action
{
    public AnimationCurve Movement;
    public override void Initialize(StateController controller)
    {
        controller.LookAroundStep = 0;
        SetDatas(controller);
    }
    public override void Act(StateController controller)
    {
        Rotate(controller);        
    }
    private void SetDatas(StateController controller) {
        var range = controller.GetCurrentWayPointInfo().LookAroundRange;

        controller.rot = controller.transform.eulerAngles;
        controller.LookAroundFrom = new Vector3(controller.rot.x, range[0], controller.rot.z);
        controller.LookAroundTo = new Vector3(controller.rot.x, range[1], controller.rot.z);
    }
    private void Rotate(StateController controller) {

        controller.LookAroundStep += Time.deltaTime * controller.enemyStats.m_LookAroundSpeed;        

        if(controller.LookAroundStep < 1) 
            controller.transform.eulerAngles = Vector3.Lerp(controller.rot, controller.LookAroundTo, Movement.Evaluate(controller.LookAroundStep));        
        else
            controller.transform.eulerAngles = Vector3.Lerp(controller.LookAroundFrom, controller.LookAroundTo, Movement.Evaluate(controller.LookAroundStep));        
    }

    public override void OnExitState(StateController controller)
    {
        
    }
}
