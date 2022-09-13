using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuWaveAttack : MonoBehaviour
{
    public Transform spawn, finishSpawn;
    public float speed = 2.0f;
    private bool waveBack;

    void Update()
    {
        if (spawn != null)
        {
            if (!waveBack)
            {
                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, finishSpawn.position, step);

                if (Vector3.Distance(transform.position, finishSpawn.position) < 0.001f)
                {
                    waveBack = true;
                }
            }
            else
            {
                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, spawn.position, step);

                if (Vector3.Distance(transform.position, spawn.position) < 0.001f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
