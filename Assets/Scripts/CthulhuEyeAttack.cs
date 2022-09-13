using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthulhuEyeAttack : MonoBehaviour
{
    public Transform spawn;
    public float speed;
    public List<Transform> directionLeft, directionRight;
    public bool isRight;
    private float changeDirectionTimmer = 0;
    private Transform actualDirection;

    void Start()
    {
        if (isRight)
        {
            actualDirection = directionRight[0];
        }
        else
        {
            actualDirection = directionLeft[0];
        }
    }
    void Update()
    {
        changeDirectionTimmer -= Time.deltaTime;
        var step = speed * Time.deltaTime;
        //para que se mueva en la direccion seleccionada
        transform.position = Vector3.MoveTowards(transform.position, actualDirection.position, step);

        //para seleccionar la direccion actual
        if (changeDirectionTimmer <= 0)
        {
            changeDirectionTimmer = 1;
            int whatDirection = Random.Range(0, 2);
            if (whatDirection == 0)
            {
                if (isRight)
                {
                    actualDirection = directionRight[0];
                }
                else
                {
                    actualDirection = directionLeft[0];
                }
            }
            else if (whatDirection == 1)
            {
                if (isRight)
                {
                    actualDirection = directionRight[1];
                }
                else
                {
                    actualDirection = directionLeft[1];
                }
            }
        }
        if (!gameObject.transform.Find("ojoCthulhu").GetComponent<SpriteRenderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }
}
