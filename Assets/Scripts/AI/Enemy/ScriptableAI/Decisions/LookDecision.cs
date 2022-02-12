using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Decisions/Look")]
public class LookDecision : Decision
{
    public enum Type { Parallel, Cone}
    public Type m_CurrentType = Type.Parallel;
    public bool m_ShowRay;
    public Color RayColor;
    public override bool Decide(StateController controller)
    {
        bool targetvisible;
        
        switch (m_CurrentType)
        {
            case Type.Parallel:
                targetvisible = ParallelLook(controller);
                break;
            case Type.Cone:
                targetvisible = ConeLook(controller);
                break;
            default:
                targetvisible = ParallelLook(controller);
                break;
        }
        return targetvisible;
    }
    private bool ParallelLook(StateController controller) {
        RaycastHit hit;

        if (m_ShowRay) Debug.DrawRay(controller.eyes.position - controller.eyes.right * controller.enemyStats.m_LookSphereCastRadius / 2, controller.eyes.forward * controller.enemyStats.m_LookRange, Color.green);
        if (m_ShowRay) Debug.DrawRay(controller.eyes.position + controller.eyes.right * controller.enemyStats.m_LookSphereCastRadius / 2, controller.eyes.forward * controller.enemyStats.m_LookRange, Color.green);

        if (Physics.SphereCast(
            controller.eyes.position, 
            controller.enemyStats.m_LookSphereCastRadius, 
            controller.eyes.forward, 
            out hit, 
            controller.enemyStats.m_LookRange)
            && hit.collider.CompareTag("Player"))
        {
            controller.chaseTarget = hit.transform;
            return true;
        }
        else
            return false;
    }
    private bool ConeLook(StateController controller)
    {
        RaycastHit hit;
        float angleX = controller.enemyStats.m_LookConeAngleX;
        float angleY = controller.enemyStats.m_LookConeAngleY;
        int precision = controller.enemyStats.m_LookPrecision;

        for (int i = -(int)angleY; i < angleY; i += precision)
        {
            for (int j = -(int)angleX; j < angleX; j += precision)
            {
                var pos = controller.eyes.position;
                
                var dir = Quaternion.AngleAxis(i, Vector3.up) * controller.transform.forward;
                dir = Quaternion.AngleAxis(j, controller.transform.right) * dir;

                if (Physics.Raycast(pos, dir, out hit, controller.enemyStats.m_LookRange))
                {
                    if (m_ShowRay) Debug.DrawLine(pos, hit.point, RayColor);

                    if (hit.collider.CompareTag("Player"))
                    {
                        controller.chaseTarget = hit.transform;
                        return true;
                    }
                }
                else {
                    if (m_ShowRay) Debug.DrawLine(pos, pos + dir * controller.enemyStats.m_LookRange, RayColor);                    
                }
            }
        }
        
        return false;
    }
}

