using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed *= 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        //WASD movement
        if (Input.GetKey(KeyCode.W)) transform.position += new Vector3(0, 0, speed);
        if (Input.GetKey(KeyCode.A)) transform.position += new Vector3(-speed, 0, 0);
        if (Input.GetKey(KeyCode.S)) transform.position += new Vector3(0, 0, -speed);
        if (Input.GetKey(KeyCode.D)) transform.position += new Vector3(speed, 0, 0);
    }
}
