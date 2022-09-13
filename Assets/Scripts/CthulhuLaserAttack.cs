using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuLaserAttack : MonoBehaviour
{
    public Transform spawn, finishSpawn;
    public float speed;
    void Update()
    {
        if (spawn != null)
        {
            Quaternion rotation = Quaternion.LookRotation
             (finishSpawn.position - transform.position, transform.TransformDirection(Vector3.up));
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);

            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, finishSpawn.position, step);

            if (Vector3.Distance(transform.position, finishSpawn.position) < 0.001f)
            {
                Destroy(gameObject);
            }
        }
    }
}
