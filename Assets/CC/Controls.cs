using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] Inputs _controls;

    private bool _characterEnabled;

    public delegate void ComboAttackEvent();
    public event ComboAttackEvent OnComboAttack;

    public delegate void RangeAttackEvent();
    public event RangeAttackEvent OnRangeAttack;

    public delegate void GroundAttackEvent();
    public event GroundAttackEvent OnGroundAttack;

    public delegate void DashEvent();
    public event DashEvent OnDash;

    void Awake()
    {
        _controls = new Inputs();

        _characterEnabled = true;
    }

    private void OnEnable()
    {
        _controls.Enable();
        if (_characterEnabled)
            BindCharacterEvents();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        _controls.Enable();
        if (_characterEnabled)
            UnbindCharacterEvents();
    }

    public void EnableCharacterControls()
    {
        if (_characterEnabled) return;
        _characterEnabled = true;
        BindCharacterEvents();

        _controls.Player.Enable();
    }

    public void DisableCharacterControls()
    {
        if (!_characterEnabled) return;
        _characterEnabled = false;
        UnbindCharacterEvents();

        _controls.Player.Disable();
    }

    #region Public Readers
    public Vector2 GetMoveDirection()
    {
        return _controls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return _controls.Player.Look.ReadValue<Vector2>();
    }

    #endregion

    private void BindCharacterEvents()
    {
        _controls.Player.ComboAttack.performed += ctx => { ComboAttackPerformed(); };
        _controls.Player.RangeAttack.performed += ctx => { RangeAttackPerformed(); };
        _controls.Player.GroundAttack.performed += ctx => { GroundAttackPerformed(); };
        _controls.Player.Dash.performed += ctx => { DashPerformed(); };
    }

    private void UnbindCharacterEvents()
    {
        _controls.Player.ComboAttack.performed -= ctx => { ComboAttackPerformed(); };
        _controls.Player.RangeAttack.performed -= ctx => { RangeAttackPerformed(); };
        _controls.Player.GroundAttack.performed -= ctx => { GroundAttackPerformed(); };
        _controls.Player.Dash.performed -= ctx => { DashPerformed(); };
    }

    private void ComboAttackPerformed() => OnComboAttack?.Invoke();
    private void RangeAttackPerformed() => OnRangeAttack?.Invoke();
    private void GroundAttackPerformed() => OnGroundAttack?.Invoke();
    private void DashPerformed() => OnDash?.Invoke();
}
