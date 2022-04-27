using System;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class LifeformFocus: MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private Transform m_LifeformHead;
    [SerializeField]
    private float m_LookAtCoolTime = 0.2f;
    [SerializeField]
    private float m_LookAtHeatTime = 0.2f;
    [SerializeField]
    private float m_RotationSpeed = 6.28f;

    private Vector3 m_LookAtTarget;
    private Vector3 m_LookAt;
    private float m_LookAtWeight = 0f;
    private bool m_Looking = true;

    private void Start()
    {
        if(m_Animator == null)
          m_Animator = GetComponent<Animator>();

        if(m_LifeformHead == null)
          throw new NullReferenceException(nameof(m_LifeformHead));

        m_LookAtTarget = m_LifeformHead.position + transform.forward;
        m_LookAt = m_LookAtTarget;
    }

    private void OnAnimatorIK()
    {
        m_LookAtTarget.y = m_LifeformHead.position.y;

        Vector3 curDir = m_LookAt - m_LifeformHead.position;
        Vector3 nextDir = m_LookAtTarget - m_LifeformHead.position;

        curDir = Vector3.RotateTowards(curDir, nextDir, m_RotationSpeed * Time.deltaTime, float.PositiveInfinity);
        m_LookAt = m_LifeformHead.position + curDir;

        float lookAtTargetWeight = m_Looking ? 1f : 0f;
        float blendTime = lookAtTargetWeight > m_LookAtWeight
            ? m_LookAtHeatTime
            : m_LookAtCoolTime;
        
        m_LookAtWeight = Mathf.MoveTowards(m_LookAtWeight, lookAtTargetWeight, Time.deltaTime / blendTime);

        m_Animator.SetLookAtWeight(m_LookAtWeight, 0.2f, 0.5f, 0.7f, 0.5f);
        m_Animator.SetLookAtPosition(m_LookAt);
    }

    public void SetLookAtTarget(Vector3 targetPos)
    {
        m_LookAtTarget = targetPos;
    }
}
