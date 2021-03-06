using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class LifeformAnimation : MonoBehaviour
{
    private const string ParamIsMoving = "IsMoving";
    private const string ParamIsDead = "IsDead";
    private const string ParamIsInteracting = "IsInteracting";
    private const string ParamIsSleeping = "IsSleeping";
    private const string ParamMovementMultiplier = "MovementMultiplier";
    private const string ParamVelocityX = "VelocityX";
    private const string ParamVelocityY = "VelocityY";

    [SerializeField]
    private LifeformStateMachine m_StateMachine;
    [SerializeField]
    private NavMeshAgent m_NavAgent;
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private Vector2 m_Velocity;
    [SerializeField]
    private float m_SmoothFactor = 0.15f;
    [SerializeField]
    private LifeformFocus m_Focus;

    private Avatar m_Avatar;
    private Vector2 m_SmoothDeltaPos;

    private Lifeform GetLifeform() => m_StateMachine.GetStateComponent();

    private void HandleStateChange(Lifeform lf, IState<Lifeform> state)
    {
        bool isInteracting = false;
        bool isDead = false;
        bool isSleeping = false;

        if(state is LifeformDeadState)
          isDead = true;
        else if(state is LifeformSleepState)
          isSleeping = true;
        else if(state is LifeformEatState)
          isInteracting = true;
        else if(state is LifeformBreedState)
          isInteracting = true;

        m_Animator.SetBool(ParamIsDead, isDead);
        m_Animator.SetBool(ParamIsInteracting, isInteracting);
        m_Animator.SetBool(ParamIsSleeping, isSleeping);

        // Reset the multiplier if it was zeroed from OnAnimationComplete
        SetMovementMultiplier(lf.Genetics.GetMoveRate());
    }

    private void SetMovementMultiplier(float multiplier)
    {
        m_Animator.SetFloat(ParamMovementMultiplier, multiplier);
    }

    private void CreateAvatar()
    {
        GameObject root = m_Animator.gameObject;
        Avatar a = AvatarBuilder.BuildGenericAvatar(root, ""); 
        a.name = "LifeformAvatar";

        m_Avatar = a;
        m_Animator.avatar = m_Avatar;
    }

    private void Start()
    {
        if(m_Animator == null)
          m_Animator = GetComponent<Animator>();
        if(m_Focus == null)
          m_Focus = GetComponent<LifeformFocus>();
        if(m_Avatar == null)
          CreateAvatar();

        // Don't update automatically, rely on this behaviour's movement from Update()
        m_NavAgent.updatePosition = false;

        // Set the lifeform movement rate from genetics to the animatior
        Lifeform lf = GetLifeform();
        SetMovementMultiplier(lf.Genetics.GetMoveRate());

        // Bind state change callback
        m_StateMachine.OnStateChange += HandleStateChange;
    }

    private void Update()
    {
        Lifeform lf = GetLifeform();
        Transform lf_trans = lf.gameObject.transform;
        Vector3 worldDeltaPos = m_NavAgent.nextPosition - lf_trans.position;
        
        float dx = Vector3.Dot(lf_trans.right, worldDeltaPos);
        float dy = Vector3.Dot(lf_trans.forward, worldDeltaPos);
        Vector2 deltaPos = new Vector2(dx, dy);

        float smooth = Mathf.Min(1.0f, Time.deltaTime / m_SmoothFactor);
        m_SmoothDeltaPos = Vector2.Lerp(m_SmoothDeltaPos, deltaPos, smooth);

        if(Time.deltaTime > 1e-5f)
          m_Velocity = m_SmoothDeltaPos / Time.deltaTime;

        bool shouldMove = m_Velocity.magnitude > 0.01f
          && m_NavAgent.remainingDistance > m_NavAgent.radius;
        
        m_Animator.SetBool(ParamIsMoving, shouldMove);
        m_Animator.SetFloat(ParamVelocityX, m_Velocity.x);
        m_Animator.SetFloat(ParamVelocityY, m_Velocity.y);
        m_Focus.SetLookAtTarget(m_NavAgent.steeringTarget + lf_trans.forward);
    }

    private void OnAnimatorMove()
    {
        Lifeform lf = GetLifeform();
        Transform lf_trans = lf.gameObject.transform;
        lf_trans.position = m_NavAgent.nextPosition + m_Animator.deltaPosition;
        lf_trans.forward = m_Animator.deltaRotation * lf_trans.forward;

        m_Animator.speed = m_Velocity.magnitude * 0.75f;
    }
}
