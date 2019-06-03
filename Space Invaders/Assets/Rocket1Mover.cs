using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket1Mover : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError(gameObject.name + " (Movement.cs): No Rigidbody component was found!");
            return;
        }
        rigidbody.velocity = transform.up  * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
