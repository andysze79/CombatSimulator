using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EnemyVisualDebugger))]
public class VisualDebugger : Editor
{
    private void OnSceneGUI()
    {
        EnemyVisualDebugger enemyVisual = (EnemyVisualDebugger)target;
        //var enemyStateController = enemyVisual.EnemyStateController;
        //var pos = enemyStateController.eyes.position;
        //Handles.color = enemyVisual.m_LookConeColor;
        //Handles.DrawSphere(
        //    0, 
        //    pos, 
        //    enemyStateController.transform.rotation, 
        //    enemyStateController.enemyStats.m_LookRange);

        //var EnemyStateController = enemyVisual.EnemyStateController;
        //var EnemyData = enemyVisual.EnemyData;

        //RaycastHit hit;
        //float angleX = EnemyData.m_LookConeAngleX;
        //float angleY = EnemyData.m_LookConeAngleY;
        //int precision = EnemyData.m_LookPrecision;

        //for (int i = -(int)angleY; i < angleY; i += precision)
        //{
        //    for (int j = -(int)angleX; j < angleX; j += precision)
        //    {
        //        var dir = Quaternion.AngleAxis(i, Vector3.up) * EnemyStateController.transform.forward;
        //        dir = Quaternion.AngleAxis(j, EnemyStateController.transform.right) * dir;

                

        //        if (Physics.Raycast(pos, dir, out hit, EnemyData.m_LookRange))
        //        {
        //            if (hit.collider.CompareTag("Player"))
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
