using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfGround : MonoBehaviour
{
    public bool isGrounded;

    void Start()
    {
        isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Floor" || collision.tag == "platform")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Floor" || collision.tag == "Platform")
        {
            isGrounded = false;
        }
    }

}
