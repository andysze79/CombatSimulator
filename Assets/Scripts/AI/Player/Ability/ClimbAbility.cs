using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
[CreateAssetMenu(menuName = "CombatSimulator/ Player/ Ability/ ClimbAbility")]
public class ClimbAbility : Ability
{
    public TriggerBase m_EdgeDetectTrigger;
    [Header("Input Settings")]
    public float ActiveInputValue;
    public float ActiveInputMagnitude = .165f;
    public float ActiveInputAngle = 10;
    [Header("Mounting Settings")]
    public float MountSpeed;
    public float CornerMountSpeed;
    public float FacingCliffDuration = .3f;
    [Header("Climbing Settings")]
    public float StartTraverseDistanceBetweenCurrentToEnd = .5f;
    public float LandingDuration;
    public float ClimbDistance;
    public float ClimbDuration;    
    public float ClimbUpDistance;
    public Vector3 offset;
    [Header("Debug Settings")]
    public bool m_DebugMode = false;

    [Header("Hidden Variables")]
    private CharacterController player;
    private ReferenceKeeper referenceKeeper;
    private TriggerBase edgeDetectTrigger;
    private ClimbableObject edgeBoxData;
    private bool isClimbing;
    private bool canLerp;
    [ReadOnly] [SerializeField] private bool changeEdgeBox;
    [ReadOnly] [SerializeField] private BoxCollider edgeBox;
    [ReadOnly] [SerializeField] private List<Collider> edgeDetectTriggers = new List<Collider>();
    [ReadOnly] public Vector3 ControllerSpeed;
    [ReadOnly] public Vector3 JumpSpeed;
    private float InputMagnitude;
    private Tween mountingTween;
    private Tween movingTween;

    private void OnDisable()
    {
        ClearTargetEdgeBox();
        changeEdgeBox = false;   
    }
    public override void StartAbility(ReferenceKeeper playerRef)
    {
        if(referenceKeeper == null) referenceKeeper = playerRef;        
        if(player == null) player = playerRef.PlayerData.CharacterController;        
        if (edgeDetectTrigger == null) GenerateTrigger(playerRef);

        edgeDetectTrigger.TriggerEnter += Climbing;
        edgeDetectTrigger.TriggerExit += DisMont;
        UserControllerGetter.Instance.JumpDownDelegate += ClimbJump;
        playerRef.PlayerLogic.DeactivateRotate();

        edgeDetectTrigger.enabled = true;
    }
    public override void EndAbility(ReferenceKeeper playerRef)
    {
        edgeDetectTrigger.TriggerEnter -= Climbing;
        edgeDetectTrigger.TriggerExit -= DisMont;
        UserControllerGetter.Instance.JumpDownDelegate -= ClimbJump;
        ClearTargetEdgeBox();
    }
    public override void DoAbility(ReferenceKeeper playerRef)
    {
        ControllerSpeed = referenceKeeper.PlayerData.CharacterController.velocity;

        if (edgeBox == null) return;
        if (!canLerp) return;
        if (changeEdgeBox) return;

        CheckClimbMoving(playerRef);
        CheckLanding(playerRef);
        Debug.DrawRay(playerRef.PlayerData.CharacterController.transform.position, playerRef.PlayerData.CharacterController.transform.up * ClimbUpDistance, Color.green);
    }
    private void Climbing(Collider col)
    {
        if (changeEdgeBox) return;

        // Check if detect another edgebox
        if (edgeBox != null && edgeBox != col) {
            changeEdgeBox = true;

            // Stop the moving coroutine for previos edge ,and start monting to another edge
            movingTween?.Kill();
        }
        isClimbing = true;

        edgeDetectTriggers.Add(col);

        player.enabled = false;

        referenceKeeper.PlayerLogic.StopCoroutine(referenceKeeper.PlayerLogic.jumpProcess);

        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.isClimbingName, true);

        //MontToEdgePoint(col, MountSpeed);

        // Jump from the ground and start climbing
        if(edgeDetectTriggers.Count < 1)
            MontToEdgePoint(col, MountSpeed);
        // Climbing from one edge to another edge
        else
            MontToEdgePoint(col, CornerMountSpeed);
    }
    private void MontToEdgePoint(Collider col, float mountSpeed) {
        mountingTween?.Kill();

        Debug.Log("mont on " + col.name);

        edgeBox = col.GetComponent<BoxCollider>();
        edgeBoxData = col.GetComponent<ClimbableObject>();

        var lookAtEdge = Quaternion.LookRotation(edgeBox.transform.right, Vector3.up);
        player.transform.DOLocalRotateQuaternion(lookAtEdge, FacingCliffDuration);

        var climbingPoint = edgeBoxData.GetClimbPoint(player.transform.position);
        var playerClimbingPoint = edgeDetectTrigger.transform.localPosition;
        var target = climbingPoint - edgeBox.transform.up * playerClimbingPoint.y + edgeBox.transform.right * playerClimbingPoint.z;
        var duration = Vector3.Distance(player.transform.position, target) / mountSpeed;

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
        var playerPos = playerRef.PlayerData.CharacterController.transform.position;
              
        // Find the next climb point
        var target = GetNextClimbPoint(playerRef, ClimbDistance);

        // Check if there are obstacles on the way to the next climb point
        var checkHitWall = CheckHitWall(
            playerPos,
            (target - playerPos).normalized,
            playerRef.PlayerData.CharacterController.radius,
            ClimbDistance,
            CombateSimulator.GameManager.Instance.m_GlobalVariables.WallLayer);

        // if there are obstacles, move myself to the closest point that won't penetrate the obstacle.
        if (checkHitWall.Item1) {
            edgeBoxData.GetClimbPoint(playerPos);

            if (checkHitWall.Item2 < .1f) return;
            else {
                target = GetNextClimbPoint(playerRef, checkHitWall.Item2); 
            }
        }
        
        canLerp = false;

        if (Vector3.Distance(player.transform.position, target) < StartTraverseDistanceBetweenCurrentToEnd && edgeDetectTriggers.Count > 1)
        { 
            Debug.Log("Reach To End");
            
            var anotherTriggerIndex = edgeDetectTriggers.IndexOf(edgeBox) + 1;
            anotherTriggerIndex = (anotherTriggerIndex < edgeDetectTriggers.Count)? anotherTriggerIndex : 0;
            
            MontToEdgePoint(edgeDetectTriggers[anotherTriggerIndex], CornerMountSpeed);
            return;
        }

        movingTween = player.transform.DOMove(target, ClimbDuration);
        movingTween.onComplete += OnTweenComplete;
    }
    private Vector3 GetNextClimbPoint(ReferenceKeeper playerRef, float climbDistance) {
        var nextPos = edgeBoxData.MoveClimbPoint(
            new Vector3(playerRef.PlayerLogic.moveValueRaw.x, 0, playerRef.PlayerLogic.moveValueRaw.z),
            climbDistance
            );
        var playerClimbingPoint = edgeDetectTrigger.transform.localPosition;
        var target = nextPos - edgeBox.transform.up * playerClimbingPoint.y + edgeBox.transform.right * playerClimbingPoint.z;
        return target;
    }
    private void CheckLanding(ReferenceKeeper playerRef) {
        var input = GetHorizontalInputVector(playerRef);
        var angle = Vector3.Angle(input, -edgeBox.transform.right);

        InputMagnitude = playerRef.PlayerLogic.moveValueRaw.magnitude;
        if (playerRef.PlayerLogic.moveValueRaw.magnitude < ActiveInputMagnitude || angle > ActiveInputAngle) return;
        
        var landingPoint = edgeBoxData.GetLandPoint();    

        canLerp = false;
        EndAbility(playerRef);

        // Waiting for crounch up animation
        Sequence landingSeq = DOTween.Sequence();
        landingSeq.PrependInterval(LandingDuration);
        var movingforward = player.transform.DOMove(landingPoint, .3f);
        movingforward.onComplete += DisMont;
        landingSeq.Append(movingforward);
        
        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.CrounchUpName);

        referenceKeeper.PlayerLogic.stopCheckGrounded = true;
    }
    private (bool,float) CheckHitWall(Vector3 pos, Vector3 dir, float playerRadius, float moveDist, LayerMask layer) {
        float dist = 0;
        Ray ray = new Ray();
        ray.origin = pos;
        ray.direction = dir;
        RaycastHit hitInfo;
        var result = Physics.Raycast(ray, out hitInfo, playerRadius + moveDist, layer);
        if(result) Debug.DrawRay(pos, dir * (playerRadius + moveDist), Color.red);

        dist = Vector3.Distance(hitInfo.point, pos) - playerRadius;
        return (result, dist);
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
        isClimbing = false;
        referenceKeeper.PlayerLogic.stopCheckGrounded = false;
        DisMont(edgeBox);
    }
    private void DisMont(Collider col) {
        edgeDetectTriggers.Remove(col);

        if (edgeBox != null && edgeBox != col) return;
        if (changeEdgeBox) return;
        canLerp = false;
        player.enabled = true;

        referenceKeeper.AnimationPlayer.PlayAnimation(referenceKeeper.PlayerData.isClimbingName, false);        
    }
    private void ClimbJump()
    {
        if (!isClimbing) return; 
        DisMont();
        Debug.Log("Climb Jump");

        // ISSUE HERE FIX IT!
        referenceKeeper.PlayerLogic.Jump(ClimbUpDistance, true);
    }
    private void ClearTargetEdgeBox() {
        edgeBox = null;
        edgeBoxData = null;
        isClimbing = false;
        edgeDetectTriggers.Clear();
    }    
}
