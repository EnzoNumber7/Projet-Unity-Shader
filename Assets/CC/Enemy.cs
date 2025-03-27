using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField] MeshRenderer _skinnedMesh;
    [SerializeField] float _dissolveRate = 0.0125f;
    [SerializeField] float _refreshRate = 0.025f;
    [SerializeField] float _timeToRespawn = 3f;
    [SerializeField] int _maxHp = 5;
    [SerializeField] UnityEvent _onDissolve;
    Material[] _skinnedMaterials;
    BoxCollider _collider;
    int _Hp;

    void Start()
    {
        _Hp = _maxHp;
        _collider = GetComponent<BoxCollider>();
        if (_skinnedMesh != null)
            _skinnedMaterials = _skinnedMesh.materials;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent(out Sword sword))
        {
            TakeDamage(1);
        }

        if (other.TryGetComponent(out GroundSlash groundSlash))
        {
            TakeDamage(5);
            groundSlash.Destroy();
        }

        if (other.TryGetComponent(out RangeSLash rangeSlash))
        {
            TakeDamage(5);
            rangeSlash.Destroy();
        }
    }

    void TakeDamage(int damage)
    {
        _Hp -= damage;
        if(_Hp <= 0)
        {
            _Hp = 0;
            StartCoroutine(Dissolve());
        }
    }

    IEnumerator Dissolve()
    {
        if (_skinnedMaterials.Length > 0)
        {
            _onDissolve.Invoke();
            _collider.enabled = false;
            float counter = 0;
            while (_skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += _dissolveRate;
                for (int i = 0; i < _skinnedMaterials.Length; i++)
                {
                    _skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(_refreshRate);
            }
            StartCoroutine(Solve());
        }
        
    }

    IEnumerator Solve()
    {
        if (_skinnedMaterials.Length > 0)
        {
            yield return new WaitForSeconds(_timeToRespawn);
            float counter = 1;
            while (_skinnedMaterials[0].GetFloat("_DissolveAmount") > 0)
            {
                counter -= _dissolveRate;
                for (int i = 0; i < _skinnedMaterials.Length; i++)
                {
                    _skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(_refreshRate);
            }
            _Hp = _maxHp;
            _collider.enabled = true;
        }
    }
}
