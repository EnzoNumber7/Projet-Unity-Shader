using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int m_speed;
    [SerializeField] private GameObject m_attackCollider;
    [SerializeField] private Animator m_animator;

    private Rigidbody rb;
    private CharacterController m_controller;
    private Vector2 m_moveDir;

    [SerializeField] private float m_attackCd;
    [SerializeField] private float m_waitForAttackAnim;
    private bool m_canAttack;

    void Start()
    {
        m_attackCollider.SetActive(false);
        m_canAttack = true;

        rb = GetComponent<Rigidbody>();
        m_controller = GetComponent<CharacterController>();

        m_controller.m_Attack += Attack;
    }

    void FixedUpdate()
    {
        Vector2 movementDirection = m_controller.GetMoveDirection();

        if (movementDirection != Vector2.zero)
        {
            m_animator.SetBool("move", true);
            Move();
            SpeedControl();
        }
        else
            m_animator.SetBool("move", false);
    }

    private void Move()
    {
        Vector3 heading = m_controller.GetHeadingDirection();
        rb.transform.rotation = Quaternion.RotateTowards(Quaternion.LookRotation(rb.transform.forward), Quaternion.LookRotation(heading), 720 * Time.deltaTime);

        Vector3 rightMovement = m_controller.GetRightMovement();
        Vector3 upMovement = m_controller.GetForwardMovement();
        Vector3 moveForce = Vector3.Normalize(rightMovement + upMovement);

        rb.AddForce(moveForce * m_speed * 5f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > m_speed)
        {
            Vector3 limitedVel = flatVel.normalized * m_speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Attack()
    {
        if (!m_canAttack) return;
        
        m_animator.SetTrigger("attack");

        StartCoroutine(ActivateCollider());
    }

    private IEnumerator AttackCoolDown()
    {

        yield return new WaitForSeconds(m_attackCd);

        m_attackCollider.SetActive(false);
        m_canAttack = true;
    }

    private IEnumerator ActivateCollider()
    {
        m_canAttack = false;

        yield return new WaitForSeconds(m_waitForAttackAnim);

        m_attackCollider.SetActive(true);
        StartCoroutine(DeactivateCollider());
        StartCoroutine(AttackCoolDown());
    }

    private IEnumerator DeactivateCollider()
    {

        yield return new WaitForSeconds(m_waitForAttackAnim);

        m_attackCollider.SetActive(false);
    }
}
