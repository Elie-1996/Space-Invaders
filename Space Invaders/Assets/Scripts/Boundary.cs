﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError(typeof(Boundary).Name + ", Fatal error: No player GameObject provided.");
            // we need this in order to prevent the player from going out of bounds
            // and safe-gaurding against unwanted advantageous glitches.
            throw new MissingComponentException();
        }
    }

    private void FixedUpdate()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        // same reason for throwing as in function Start().
        if (collider == null) { Debug.LogError(typeof(Boundary).Name + ", Fatal error: No boundary collider found!"); throw new MissingComponentException(); }
        Vector3 closestPoint = collider.ClosestPoint(player.transform.position);
        float distance = Vector3.Distance(closestPoint, player.transform.position);
        if (distance > 0)
        {
            Debug.DrawLine(closestPoint, player.transform.position);
            Debug.Log("You are outside the legal boundaries! (distance =" + distance + ", radius = " + collider.radius + ")");
        }
        if (distance >= 100)
        {
            Debug.Log("Went to far away into illegal territory. You should be dead!");
            // kill self
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Utils.TagPlayer)
        {
            Debug.Log("You have just exited the boundaries! Please go back inside the game");
        }
        else
        {
            Destroy(other);
        }
    }
}
