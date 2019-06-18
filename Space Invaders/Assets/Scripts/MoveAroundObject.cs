using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundObject : MonoBehaviour
{
    private Transform target;
    Vector3 storeTarget;
    bool savePos;

    Vector3 acceleration;
    Vector3 velocity;
    public float maxSpeed = 5;
    float storeMaxSpeed;
    float targetSpeed;

    Rigidbody rigidbody;
    public List<Vector3> EscapeDirections = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        storeMaxSpeed = maxSpeed;
        targetSpeed = storeMaxSpeed;

        rigidbody = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag(Utils.TagPlayer).transform;
    }

    private void FixedUpdate()
    {
        target = GameObject.FindGameObjectWithTag(Utils.TagPlayer).transform;
        Debug.DrawLine(transform.localPosition, target.position);
        Vector3 forces = MoveForwardTarget(target.position);
        acceleration = forces;

        velocity += 2 * acceleration * Time.deltaTime;

        //if(velocity.magnitude  > maxSpeed)
        //{
        //    velocity = velocity.normalized * maxSpeed;
        //}

        rigidbody.velocity = velocity;
        Quaternion quaternion = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, Time.deltaTime * 3);
    }

    Vector3 MoveForwardTarget(Vector3 target)
    {
        Vector3 distance = target - transform.position;

            return distance.normalized * maxSpeed;
    }
}
