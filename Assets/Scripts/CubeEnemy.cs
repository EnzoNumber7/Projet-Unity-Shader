using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEnemy : MonoBehaviour
{

    MeshRenderer m_Renderer;
    [SerializeField] private float m_effectDuration;
  
    void Start()
    {
        m_Renderer = GetComponent<MeshRenderer>();
    }

    public void TakeDamage()
    {
        StartCoroutine(HitEffectDuration());
    }

    private IEnumerator HitEffectDuration()
    {
        m_Renderer.material.SetFloat("_Effect", 0.5f);

        yield return new WaitForSeconds(m_effectDuration);

        m_Renderer.material.SetFloat("_Effect", 0);
    }
}
