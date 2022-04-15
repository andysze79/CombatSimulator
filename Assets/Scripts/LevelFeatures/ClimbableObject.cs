using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClimbableObject : MonoBehaviour
{
    public Vector3 LandingPointOffset;
    public Transform testObj;
    public Vector3 testInput;
    [Header("Debug")]
    public bool m_ViewClimbPoint;
    public bool m_ViewLandPoint;
    private BoxCollider boxCol;
    private GameObject debugSphere;
    
    private Vector3 pos;
    private void Awake()
    {
        boxCol = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (testObj == null) return;

        GetClimbPoint(testObj.position);
        GetLandPoint();
        MoveClimbPoint(testInput, 1);
    }
    public Vector3 GetClimbPoint(Vector3 colPos) {
        colPos.y = boxCol.transform.position.y;
        var dist = Vector3.Distance(colPos, boxCol.transform.position);
        var vec1 = (colPos - boxCol.transform.position).normalized;
        var vec2 = boxCol.transform.forward;
        var angle = (Mathf.PI / 180) * Vector3.Angle(vec1, vec2);
        pos = boxCol.transform.position + boxCol.transform.forward * (Mathf.Clamp(Mathf.Cos(angle) * dist, -boxCol.size.z / 2, boxCol.size.z / 2));
        pos.y += boxCol.size.y / 2;

        if(m_ViewClimbPoint) DrawSphere(pos);

        return pos;
    }    
    public Vector3 MoveClimbPoint(Vector3 controllerInput, float dist)
    {
        Debug.DrawRay(pos, controllerInput.normalized, Color.red);
        
        var vec1 = controllerInput;
        var vec2 = boxCol.transform.forward;
        var angle = (Mathf.PI / 180) * Vector3.Angle(vec1, vec2);
        //dist = Mathf.Clamp(dist + Vector3.Distance(pos, boxCol.transform.position), -boxCol.size.z / 2, boxCol.size.z / 2) - Vector3.Distance(pos, boxCol.transform.position); 
        var nextPos = pos + boxCol.transform.forward * (Mathf.Cos(angle)/Mathf.Abs(Mathf.Cos(angle)) * dist);
        nextPos = GetClimbPoint(nextPos);

        return nextPos;
    }
    public Vector3 GetLandPoint() {
        var landingPoint = pos + transform.right * LandingPointOffset.z + transform.up * LandingPointOffset.y;
        if (m_ViewLandPoint) DrawSphere(landingPoint);
        return landingPoint;
    }
    private void DrawSphere(Vector3 pos) {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale *= .3f;
        sphere.transform.position = pos;
        if (debugSphere != null) Destroy(debugSphere);
        debugSphere = sphere;
        //Destroy(debugSphere, 3);
    }
    private void OnDrawGizmos()
    {
        if (boxCol == null) boxCol = GetComponent<BoxCollider>();

        Gizmos.color = new Color(1f,1f,1f);        
        Gizmos.DrawLine(
            new Vector3(boxCol.transform.position.x, boxCol.transform.position.y + boxCol.size.y / 2, boxCol.transform.position.z) - boxCol.transform.forward * boxCol.size.z / 2,
            new Vector3(boxCol.transform.position.x, boxCol.transform.position.y + boxCol.size.y / 2, boxCol.transform.position.z) + boxCol.transform.forward * boxCol.size.z / 2
            );

        Gizmos.color = new Color(0f, 0f, 1f);
        Gizmos.DrawLine(
            new Vector3(boxCol.transform.position.x, boxCol.transform.position.y + boxCol.size.y / 2, boxCol.transform.position.z) - boxCol.transform.forward * boxCol.size.z / 2 + transform.right * LandingPointOffset.z + transform.up * LandingPointOffset.y,
            new Vector3(boxCol.transform.position.x, boxCol.transform.position.y + boxCol.size.y / 2, boxCol.transform.position.z) + boxCol.transform.forward * boxCol.size.z / 2 + transform.right * LandingPointOffset.z + transform.up * LandingPointOffset.y
            );
    }
}
