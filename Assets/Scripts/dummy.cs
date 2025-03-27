using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dummy : MonoBehaviour
{

    [Header("Event")]
    [SerializeField] private UnityEvent m_hit;
    [SerializeField] private UnityEvent m_death;
    [SerializeField] private UnityEvent m_respawn;
    [SerializeField] private UnityEvent m_respawningPlay;
    [SerializeField] private UnityEvent m_respawningStop;


    [Header("Cooldown")]
    [SerializeField] private float m_hitAnimCd;
    [SerializeField] private float m_deathAnimCd;
    [SerializeField] private float m_respawnCd;
    [SerializeField] private GameObject m_model;
    private Animator m_animator;
    private int m_damageTaken;
    private bool m_isRespawning;
    private bool m_canRespawn;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_damageTaken = 0;
        m_isRespawning = false;
        m_canRespawn = false;
    }

    private void Update()
    {
        if (m_damageTaken > 5 && m_canRespawn)
            StartCoroutine(waitForRespawn());

        if (m_isRespawning)
            Respawning();
    }

    public void TakeDamage()
    {
        if (m_damageTaken > 5) return;

        m_animator.SetBool("hit", true);
        m_hit.Invoke();
        
        m_damageTaken++;
        m_animator.SetInteger("damage", m_damageTaken);

        StartCoroutine(hitAnim());

        if (m_damageTaken > 5)
            StartCoroutine(deathAnim());
    }

    private void Respawning()
    {
        m_isRespawning = false;
        m_damageTaken = 0;
        m_animator.SetInteger("damage", 0);

        m_respawn.Invoke();
    }

    private IEnumerator hitAnim()
    {
        yield return new WaitForSeconds(m_hitAnimCd); ;

        m_animator.SetBool("hit", false);
    }

    private IEnumerator deathAnim()
    {
        yield return new WaitForSeconds(m_deathAnimCd); ;

        m_death.Invoke();

        m_canRespawn = true;
    }

    private IEnumerator waitForRespawn()
    {
        m_canRespawn = false;

        yield return new WaitForSeconds(m_respawnCd / 2.0f);
        
        StartCoroutine(respawningAnim());
    }

    private IEnumerator respawningAnim()
    {

        m_respawningPlay.Invoke();

        yield return new WaitForSeconds(m_respawnCd / 2.0f);

        m_isRespawning = true;

        m_respawningStop.Invoke();
    }
}
