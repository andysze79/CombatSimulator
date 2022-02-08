using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "PluggableAI/Actions/LookAround")]
public class LookAroundAction : Action
{
    public AnimationCurve Movement;
    private Vector3 from;
    private Vector3 to;
    private Vector3 rot;
    private float step;
    public override void Initialize(StateController controller)
    {
        step = 0;
        SetDatas(controller);
    }
    public override void Act(StateController controller)
    {
        Rotate(controller);        
    }
    private void SetDatas(StateController controller) {
        var range = controller.GetCurrentWayPointIndo().LookAroundRange;

        rot = controller.transform.eulerAngles;
        from = new Vector3(rot.x, range[0], rot.z);
        to = new Vector3(rot.x, range[1], rot.z);
    }
    private void Rotate(StateController controller) {

        step += Time.deltaTime * controller.enemyStats.m_LookAroundSpeed;        

        if(step < 1) 
            controller.transform.eulerAngles = Vector3.Lerp(rot, to, Movement.Evaluate(step));        
        else
            controller.transform.eulerAngles = Vector3.Lerp(from, to, Movement.Evaluate(step));        
    }

    
}
