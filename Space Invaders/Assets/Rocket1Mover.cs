using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket1Mover : MonoBehaviour
{
    public float speed;
    private Vector3 origin;
    private Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError(gameObject.name + " (Movement.cs): No Rigidbody component was found!");
            return;
        }
        rigidbody.velocity = transform.up * speed;
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distance = transform.position - origin;
        if (distance.x > 20 || distance.y > 20 || distance.z > 20)
        {
            Destroy(gameObject);
        }
    }
}
