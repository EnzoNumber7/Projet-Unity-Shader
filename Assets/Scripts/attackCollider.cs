using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<Dummy>(out Dummy dummy))
            dummy.TakeDamage();

        if (other.TryGetComponent<CubeEnemy>(out CubeEnemy cubeEnemy))
            cubeEnemy.TakeDamage();
    }
}
