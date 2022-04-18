using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class LifeformAnimation : MonoBehaviour
{
    private const string ParamIsMoving = "IsMoving";
    private const string ParamIsDead = "IsDead";
    private const string ParamIsInteracting = "IsInteracting";

    [SerializeField]
    private LifeformStateMachine m_StateMachine;
    [SerializeField]
    private Animator m_Animator;

    private void HandleStateChange(Lifeform lf, IState<Lifeform> state)
    {
        bool isMoving = false;
        bool isInteracting = false;
        bool isDead = false;

        if(state is LifeformDeadState)
          isDead = true;
        else if(state is LifeformSleepState)
          isDead = true;
        else if(state is LifeformMoveState)
          isMoving = true;
        else if(state is LifeformWanderState)
          isMoving = true;
        else if(state is LifeformEatState)
          isInteracting = true;

        m_Animator.SetBool(ParamIsDead, isDead);
        m_Animator.SetBool(ParamIsMoving, isMoving);
        m_Animator.SetBool(ParamIsInteracting, isInteracting);
    }

    private void Start()
    {
        if(m_Animator == null)
          m_Animator = GetComponent<Animator>();

        m_StateMachine.OnStateChange += HandleStateChange;
    }

    private void Update()
    {
        m_Animator.Update(Time.deltaTime);
    }
}
