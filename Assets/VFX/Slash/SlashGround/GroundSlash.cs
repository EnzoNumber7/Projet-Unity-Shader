using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GroundSlash : MonoBehaviour
{
    [SerializeField] float slashSpeed = 10f;
    [SerializeField] UnityEvent GroundSlashAttackShader;
    private Transform Player;
    bool isDestroy = false;
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
        transform.position = new Vector3(Player.position.x, Player.position.y, Player.position.z);
        transform.forward = Player.forward;
        GroundSlashAttackShader.Invoke();
        StartCoroutine(DestroyCo());
    }

    private void Update()
    {
        transform.position += transform.forward * slashSpeed * Time.deltaTime;
    }
    IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(1f);
        if (!isDestroy)
        {
            Destroy(gameObject);
            isDestroy = true;
        }
    }

    public void Destroy()
    {
        if (isDestroy) return;
        Destroy(gameObject);
        isDestroy = true;
    }
}
