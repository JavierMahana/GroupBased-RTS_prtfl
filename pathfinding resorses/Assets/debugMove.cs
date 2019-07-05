using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugMove : MonoBehaviour
{
    public bool inverse;
    public float speed = 10;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (inverse)
            input = -input;
        transform.Translate(input * Time.deltaTime * speed, Space.Self);
    }
}
