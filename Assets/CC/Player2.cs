using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class Player2 : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] Animator _animator;
    Transform MainCameraTransform;
    [SerializeField] Controls _input;
    public int Speed;

    [Header("Attack")]
    [SerializeField] GameObject _attackCollider;
    bool _isAttack = false;

    [Header("Combo Attack")]
    [SerializeField] float _couldownComboAttack = 1f;
    int _attackNumber = 0;
    float _lastTimeClick = 0;
    float _maxComboDelay = 1f;
    bool _isClick = false;
    [SerializeField] UnityEvent[] _combos;
    [SerializeField] GameObject GroundSlash;
    [SerializeField] GameObject RangeSlash;

    [Header("Range Attack")]
    [SerializeField] float _couldownRangeAttack = 2f;

    [Header("Ground Attack")]
    [SerializeField] float _couldownGroundAttack = 2f;

    [Header("Dash")]
    [SerializeField] UnityEvent _onDash;
    [SerializeField] float _meshDestroyDelay = 3f;
    [SerializeField] Material _mat;
    [SerializeField] SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] float _dashSpeed = 2f;
    [SerializeField] float _dashingTime = 2f;
    [SerializeField] float _dashCouldown = 2f;
    GameObject _clone;
    bool _isDashReturnTriggered = false;
    bool _isDashing = false;
    bool _canDash = true;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        MainCameraTransform = Camera.main.transform;
        _input.OnComboAttack += ComboAttack;
        _input.OnRangeAttack += RangeAttack;
        _input.OnGroundAttack += GroundAttack;
        _input.OnDash += CloneAndDash;
        _attackCollider.SetActive(false);
    }

    void Update()
    {
        if (Time.time - _lastTimeClick > _maxComboDelay)
        {
            if (_isClick && _attackNumber <= 3)
            {
                _lastTimeClick = Time.time;
                _isClick = false;
            }
            else
            {
                if(_attackNumber == 0) return;

                _attackNumber = 0;
                _animator.SetInteger("ComboNumber", _attackNumber);
                _attackCollider.SetActive(false);
                _input.OnComboAttack -= ComboAttack;
                StartCoroutine(WaitCouldownAttack(_couldownComboAttack));
            }
        }
    }

    void FixedUpdate()
    {
        if (_isDashing) return;
        Move();
    }

    void Move()
    {
        Vector3 cameraForward = MainCameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector2 moveDir = _input.GetMoveDirection();
        Vector3 dir = cameraForward * moveDir.y + MainCameraTransform.right * moveDir.x;
        dir.Normalize();

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            Vector3 slopeNormal = hit.normal;
            float slopeAngle = Vector3.Angle(Vector3.up, slopeNormal);

            if (slopeAngle > 0 && slopeAngle < 100f)
            {
                Vector3 forceUp = Vector3.up * Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Speed * 2f;
                _rb.AddForce(forceUp, ForceMode.Force);
            }
        }

        transform.LookAt(transform.position + dir);

        if (dir.magnitude > 0.01)
        {
            _rb.AddForce(dir * Speed * 2f, ForceMode.Force);
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, Speed);
            _animator.SetBool("Moving", true);
        }
        else
        {
            _rb.velocity = Vector3.zero;
            _animator.SetBool("Moving", false);
        }
        _animator.SetFloat("Velocity Z", _rb.velocity.z);
    }
    void ComboAttack()
    {
        if (_isClick) return;

        if(_attackNumber != 0)
        {
            _isClick = true;
        }
        else
        {
            _lastTimeClick = Time.time;
            _input.OnRangeAttack -= RangeAttack;
            _input.OnGroundAttack -= GroundAttack;
            _input.OnDash -= CloneAndDash;
        }

        _attackNumber = Mathf.Clamp(_attackNumber + 1, 0, 3);

        if (_attackNumber == 1)
        {
            _animator.SetTrigger("ComboAttack");
            _attackCollider.SetActive(true);
        }
        _combos[_attackNumber - 1].Invoke();
        _animator.SetInteger("ComboNumber", _attackNumber);
        _isAttack = true;
    }

    void RangeAttack()
    {
        _animator.SetTrigger("RangeAttack");
        _attackCollider.SetActive(true);
        _input.OnComboAttack -= ComboAttack;
        _input.OnRangeAttack -= RangeAttack;
        _input.OnGroundAttack -= GroundAttack;
        _input.OnDash -= CloneAndDash;
        StartCoroutine(WaitInstantiateRangeSlash());
        StartCoroutine(WaitCouldownAttack(_couldownRangeAttack));
    }

    void GroundAttack()
    {
        _animator.SetTrigger("GroundAttack");
        _attackCollider.SetActive(true);
        _input.OnComboAttack -= ComboAttack;
        _input.OnRangeAttack -= RangeAttack;
        _input.OnGroundAttack -= GroundAttack;
        _input.OnDash -= CloneAndDash;
        StartCoroutine(WaitInstantiateGroundSlash());
        StartCoroutine(WaitCouldownAttack(_couldownGroundAttack));
    }

    IEnumerator WaitCouldownAttack(float couldown)
    {
        yield return new WaitForSeconds(couldown);
        _input.OnComboAttack += ComboAttack;
        _input.OnRangeAttack += RangeAttack;
        _input.OnGroundAttack += GroundAttack;
        if (_canDash) {
            _input.OnDash += CloneAndDash;
        }
        _isAttack = false;

    }
    IEnumerator WaitInstantiateGroundSlash()
    {
        yield return new WaitForSeconds(0.5f);
        Instantiate(GroundSlash);
    }

    IEnumerator WaitInstantiateRangeSlash()
    {
        yield return new WaitForSeconds(1.3f);
        Instantiate(RangeSlash);
    }

    void CloneAndDash()
    {
        if (_skinnedMeshRenderer == null) return;

        _canDash = false;

        _clone = new GameObject();
        _clone.transform.SetPositionAndRotation(transform.position, transform.rotation);
        MeshRenderer mr = _clone.AddComponent<MeshRenderer>();
        MeshFilter mf = _clone.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        _skinnedMeshRenderer.BakeMesh(mesh);
        mf.mesh = mesh;

        mr.materials = new Material[] { _mat, _mat, _mat };

        StartCoroutine(Dash());
        StartCoroutine(WaitForDestroy());
    }

    IEnumerator Dash()
    {
        _input.OnDash -= CloneAndDash;
        _input.OnComboAttack -= ComboAttack;
        _input.OnRangeAttack -= RangeAttack;
        _input.OnGroundAttack -= GroundAttack;
        _onDash.Invoke();
        _rb.velocity = Vector3.zero;
        _rb.AddForce(transform.forward * _dashSpeed, ForceMode.VelocityChange);
        _isDashing = true;
        yield return new WaitForSeconds(_dashingTime);
        _isDashing = false;
        _input.OnComboAttack += ComboAttack;
        _input.OnRangeAttack += RangeAttack;
        _input.OnGroundAttack += GroundAttack;
        _input.OnDash += BackToCloneDash;
    }

    void BackToCloneDash()
    {
        transform.position = _clone.transform.position;
        _isDashReturnTriggered = true;
        _input.OnDash -= BackToCloneDash;
        StartCoroutine(WaitCouldownDash());
    }

    IEnumerator WaitCouldownDash()
    {
        if (_isDashReturnTriggered)
        {
            Destroy(_clone);
        }

        yield return new WaitForSeconds(_dashCouldown);

        if (!_isAttack)
        {
            _input.OnDash += CloneAndDash;
        }
        _canDash = true;
    }

    IEnumerator WaitForDestroy()
    {
        yield return new WaitForSeconds(_meshDestroyDelay);

        if (!_isDashReturnTriggered)
        {
            Destroy(_clone);
            _input.OnDash -= BackToCloneDash;
            StartCoroutine(WaitCouldownDash());
        }

        _isDashReturnTriggered = false;
    }


}
