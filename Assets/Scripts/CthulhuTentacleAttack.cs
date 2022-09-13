using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuTentacleAttack : MonoBehaviour
{
    public Transform spawn, finishSpawn;
    public float speed = 2.0f;
    private bool tentacleUp;
 
    void Update()
    {
        if(spawn != null)
        {
            if (!tentacleUp)
            {
                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, finishSpawn.position, step);

                if (Vector3.Distance(transform.position, finishSpawn.position) < 0.001f)
                {
                    tentacleUp = true;
                }
            }
            else
            {
                var step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, spawn.position, step);

                if (Vector3.Distance(transform.position, spawn.position) < 0.001f)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }  
    }
}
