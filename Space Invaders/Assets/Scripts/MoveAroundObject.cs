using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <<warning>>
// this class should not be called unless there are players!!
public class MoveAroundObject : MonoBehaviour
{
    public float speed;

    private Rigidbody enemyRigidBody;
    private GameObject[] players;
    private int playerIndexToAttack;

    private void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
        if (enemyRigidBody == null) throw new MissingComponentException("Enemy: Rigidbody is missing.");

        playerIndexToAttack = -1;
        ChoosePlayerToAttack();
        if (players.Length <= 0) throw new System.Exception("Not enough players. Should not load Enemy");
    }


    // physics
    private void FixedUpdate()
    {
        if (playerIndexToAttack == -1) return;
        GameObject player = players[playerIndexToAttack];
        FaceTarget(player.transform);
        MoveTowardsTarget(player.transform);
    }

    private void MoveTowardsTarget(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        enemyRigidBody.velocity = direction * speed * Time.deltaTime;
    }

    private void FaceTarget(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemyRigidBody.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    private void Update()
    {
        if (playerIndexToAttack != -1) return;
        ChoosePlayerToAttack();
        // decide how to attack ?
        // random 50%: Smash into
        // random 50%: rotate around
    }

    private void ChoosePlayerToAttack()
    {
        players = GameObject.FindGameObjectsWithTag(Utils.TagPlayer);
        if (players.Length <= 0) playerIndexToAttack = -1;
        playerIndexToAttack = Random.Range(0, players.Length);
    }


}
