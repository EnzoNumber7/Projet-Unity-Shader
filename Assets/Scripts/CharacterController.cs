using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private InputManager m_inputManager;

    public delegate void Attack();
    public event Attack m_Attack;

    void Awake()
    {
        m_inputManager = new InputManager();
        m_inputManager.Player.Attack.started += ctx => OnAttackEvent();
    }

    private void OnEnable()
    {
        m_inputManager.Enable();
        
    }

    private void OnDisable()
    {
        m_inputManager.Disable();
    }

    public Vector2 GetMoveDirection()
    {
        return m_inputManager.Player.Movement.ReadValue<Vector2>();
    }

    public Vector3 GetForwardMovement()
    {
        Vector2 moveDir = GetMoveDirection();
        Vector3 fixedForward = Vector3.forward;
        fixedForward.y = 0f;
        fixedForward = Vector3.Normalize(fixedForward);
        Vector3 upMovement = fixedForward * moveDir.y;
        return upMovement;
    }
    public Vector3 GetRightMovement()
    {
        Vector2 moveDir = GetMoveDirection();
        Vector3 fixedForward = Vector3.forward;
        fixedForward.y = 0f;
        fixedForward = Vector3.Normalize(fixedForward);
        Vector3 fixedRight = Quaternion.Euler(new Vector3(0, 90, 0)) * fixedForward;
        Vector3 rightMovement = fixedRight * moveDir.x;
        return rightMovement;
    }
    public Vector3 GetHeadingDirection()
    {
        Vector3 rightMovement = GetRightMovement();
        Vector3 upMovement = GetForwardMovement();
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        return heading;

    }

    private void OnAttackEvent()
    {
        m_Attack.Invoke();
    }
}
