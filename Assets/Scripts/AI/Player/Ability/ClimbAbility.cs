using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
[CreateAssetMenu(menuName = "CombatSimulator/ Player/ Ability/ ClimbAbility")]
public class ClimbAbility : Ability
{
    public TriggerBase m_EdgeDetectTrigger;
    public float ActiveInputValue;
    public float ActiveInputMagnitude = .165f;
    public float ActiveInputAngle = 10;
    public float MountSpeed;
    public float FacingCliffDuration = .3f;
    public float LandingDuration;
    public float ClimbDistance;
    public float ClimbDuration;
    public Vector3 offset;
    public float InputMagnitude;
    private CharacterController player;
    private TriggerBase edgeDetectTrigger;
    private bool canLerp;
    public BoxCollider edgeBox;
    private ClimbableObject edgeBoxData;
    public bool changeEdgeBox;
    private Tween mountingTween;
    private Tween movingTween;
    private void OnDisable()
    {
        ClearTargetEdgeBox();
        changeEdgeBox = false;   
    }
    public override void StartAbility(ReferenceKeeper playerRef)
    {
        if(player == null) player = playerRef.PlayerData.CharacterController;        
        if (edgeDetectTrigger == null) GenerateTrigger(playerRef);

        edgeDetectTrigger.TriggerEnter += Climbing;
        edgeDetectTrigger.TriggerExit += DisMont;
        UserControllerGetter.Instance.JumpDownDelegate += DisMont;
        playerRef.PlayerLogic.DeactivateRotate();

        edgeDetectTrigger.enabled = true;
    }

    public override void EndAbility(ReferenceKeeper playerRef)
    {
        edgeDetectTrigger.TriggerEnter -= Climbing;
        edgeDetectTrigger.TriggerExit -= DisMont;
        UserControllerGetter.Instance.JumpDownDelegate -= DisMont;
        ClearTargetEdgeBox();
    }
    
    public override void DoAbility(ReferenceKeeper playerRef)
    {
        if (edgeBox == null) return;
        if (!canLerp) return;
        if (changeEdgeBox) return;

        CheckClimbMoving(playerRef);
        CheckLanding(playerRef);
    }
    private void Climbing(Collider col)
    {
        if (changeEdgeBox) return;

        // Check if detect another edgebox
        if (edgeBox != null && edgeBox != col) {
            changeEdgeBox = true;
            // Stop moving from previos edge to start monting to another edge
            movingTween?.Kill();
        }

        player.enabled = false;

        player.Move(Vector3.zero);

        Debug.Log("mont on " + col.name);

        edgeBox = col.GetComponent<BoxCollider>();
        edgeBoxData = col.GetComponent<ClimbableObject>();

        var lookAtEdge = Quaternion.LookRotation(edgeBox.transform.right, Vector3.up);
        player.transform.DOLocalRotateQuaternion(lookAtEdge, FacingCliffDuration);

        var climbingPoint = edgeBoxData.GetClimbPoint(player.transform.position);
        var playerClimbingPoint = edgeDetectTrigger.transform.localPosition;
        var target = climbingPoint - edgeBox.transform.up * playerClimbingPoint.y + edgeBox.transform.right * playerClimbingPoint.z;
        var duration = Vector3.Distance(player.transform.position, target) / MountSpeed;

        mountingTween = player.transform.DOMove(target, duration);
        mountingTween.onComplete += OnTweenComplete;
        mountingTween.onComplete += OnFinishedMoving;
    }
    private void CheckClimbMoving(ReferenceKeeper playerRef)
    {        
        var input = GetHorizontalInputVector(playerRef);
        var angleRight = Vector3.Angle(input, edgeBox.transform.forward);
        var angleLeft = Vector3.Angle(input, -edgeBox.transform.forward);

        if (playerRef.PlayerLogic.moveValueRaw.magnitude < ActiveInputMagnitude) return;
        if (angleRight > ActiveInputAngle && angleLeft > ActiveInputAngle) return;

        if (!canLerp) return;

        Debug.Log("moving on " + edgeBoxData.name);

        var nextPos = edgeBoxData.MoveClimbPoint(
            new Vector3(playerRef.PlayerLogic.moveValueRaw.x, 0, playerRef.PlayerLogic.moveValueRaw.z),
            ClimbDistance
            );

        canLerp = false;
        var playerClimbingPoint = edgeDetectTrigger.transform.localPosition;
        var target = nextPos - edgeBox.transform.up * playerClimbingPoint.y + edgeBox.transform.right * playerClimbingPoint.z;
        movingTween = player.transform.DOMove(target, ClimbDuration);
        movingTween.onComplete += OnTweenComplete;
    }    
    private void CheckLanding(ReferenceKeeper playerRef) {
        var input = GetHorizontalInputVector(playerRef);
        var angle = Vector3.Angle(input, -edgeBox.transform.right);

        InputMagnitude = playerRef.PlayerLogic.moveValueRaw.magnitude;
        if (playerRef.PlayerLogic.moveValueRaw.magnitude < ActiveInputMagnitude || angle > ActiveInputAngle) return;
        
        var landingPoint = edgeBoxData.GetLandPoint();

        canLerp = false;
        EndAbility(playerRef);

        Sequence landingSeq = DOTween.Sequence();
        var dist1 = Vector3.Distance(player.transform.position ,new Vector3(player.transform.position.x, landingPoint.y, player.transform.position.z));
        var dist2 = Vector3.Distance(new Vector3(player.transform.position.x, landingPoint.y, player.transform.position.z), landingPoint);

        //Moving Up
        landingSeq.Append(player.transform.DOMove(
            new Vector3(player.transform.position.x, landingPoint.y, player.transform.position.z), LandingDuration * (dist1 / (dist1+dist2))));
        //Moving Forward
        var movingforward = player.transform.DOMove(landingPoint, LandingDuration * (dist2 / (dist1 + dist2)));
        movingforward.onComplete += DisMont;
        landingSeq.Append(movingforward);
    }
    private Vector3 GetHorizontalInputVector(ReferenceKeeper playerRef)
    {
        var input = playerRef.PlayerLogic.moveValueRaw;
        input.y = 0;
        input = input.normalized;
        Debug.DrawRay(edgeDetectTrigger.transform.position + Vector3.up * .5f, input, Color.blue);
        return input;
    }
    private void GenerateTrigger(ReferenceKeeper playerRef)
    {
        edgeDetectTrigger = Instantiate(m_EdgeDetectTrigger, player.transform);
        edgeDetectTrigger.transform.localPosition += offset;
    }
    private void OnTweenComplete() { 
        canLerp = true;     
    }
    private void OnFinishedMoving() {
        if (changeEdgeBox) changeEdgeBox = false;
    }
    private void DisMont()
    {
        DisMont(edgeBox);
    }
    private void DisMont(Collider col) {
        if (edgeBox != null && edgeBox != col) return;
        if (changeEdgeBox) return;
        canLerp = false;
        player.enabled = true;        
    }
    private void ClearTargetEdgeBox() {
        edgeBox = null;
        edgeBoxData = null;
    }
    
}
