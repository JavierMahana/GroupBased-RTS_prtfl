using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbd2_test : MonoBehaviour
{
    Rigidbody2D rb2d;
    public float speed;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        
        Debug.Log(rb2d.velocity.ToString() + "  previo");
        
        rb2d.MovePosition(rb2d.position + Vector2.left * speed * Time.deltaTime);
        Debug.Log(rb2d.velocity.ToString() + "  posterior");
        rb2d.AddForce(Vector2.up * speed);
    }
}
