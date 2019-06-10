using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    public int speed;
    public int tumble;

    private SphereCollider sCollider;

    void Start()
    {
        sCollider = GetComponent<SphereCollider>();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 direction = Utils.getAsteroidDirection();
        rigidbody.velocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = sCollider.transform.TransformPoint(sCollider.bounds.center);
        transform.Rotate(center, Time.deltaTime * tumble);
    }
}
