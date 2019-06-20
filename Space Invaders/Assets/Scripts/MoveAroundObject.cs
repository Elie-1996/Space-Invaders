using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// <<warning>>
// this class should not be called unless there are players!!
public class MoveAroundObject : MonoBehaviour
{
    public float speed;
    public float closeApproximity;

    public GameObject radiusCenter;
    public float radiusMovement;
    private float timeCounter;

    private int attackType;
    private Rigidbody enemyRigidBody;
    private GameObject[] players;
    private int playerIndexToAttack;

    private Vector3 randomTarget;
    private float gameRadius;
    private bool decidedRandomPosition;
    private float waitTimeRandomMovementPassed;

    private void Start()
    {
        waitTimeRandomMovementPassed = 0.0f;
        decidedRandomPosition = false;
        gameRadius = Utils.getGameBoundaryRadius(GameObject.FindGameObjectWithTag(Utils.TagBackground));
        timeCounter = 0.0f;
        enemyRigidBody = GetComponent<Rigidbody>();
        if (enemyRigidBody == null) throw new MissingComponentException("Enemy: Rigidbody is missing.");

        playerIndexToAttack = -1;
        ChoosePlayerToAttack();
        if (players.Length <= 0) throw new System.Exception("Not enough players. Should not load Enemy");


        attackType = -1;
        DecideAttackType();
    }

    private Transform FindTransformInChildWithTag(string _tag)
    {
        foreach (Transform child in gameObject.transform)
        {
            if( child.tag == _tag)
            {
                return child;
            }
        }
        return null;
    }

    // physics
    private void FixedUpdate()
    {
        if (playerIndexToAttack == -1) return;
        GameObject player = players[playerIndexToAttack];
        FaceTarget(player.transform);
        Attack(player.transform);
    }

    private void Attack(Transform targetTransform)
    {
        if (attackType == 1)
            MoveTowardsTarget(targetTransform);
        else if (attackType == 2)
            CirculateAroundTarget(targetTransform);
        else if (attackType == 3)
            RandomMovement();
        else
            throw new System.Exception("Incorrect attack type was = " + attackType);
    }

    private void RandomMovement()
    {
        waitTimeRandomMovementPassed += Time.deltaTime;
        if (decidedRandomPosition == false || randomTarget == transform.position)
        {
            randomTarget = Random.insideUnitSphere * gameRadius * 0.9f;
            decidedRandomPosition = true;
        }
        RandomMovementHelper();
        if (waitTimeRandomMovementPassed >= 10.0f)
        {
            DecideAttackType();
            decidedRandomPosition = false;
            waitTimeRandomMovementPassed = 0.0f;
        }
    }

    private void RandomMovementHelper()
    {
        FaceTarget(randomTarget);
        float realSpeed = decideSpeed(randomTarget);
        Vector3 direction = (randomTarget - transform.position).normalized;
        enemyRigidBody.velocity = direction * realSpeed * Time.deltaTime;
        Debug.DrawLine(transform.position, randomTarget, Color.green);
    }

    private void CirculateAroundTarget(Transform targetTransform)
    {
        timeCounter += Time.deltaTime;
        float realSpeed = decideSpeed(targetTransform);
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        enemyRigidBody.velocity = direction * realSpeed * Time.deltaTime + new Vector3(Mathf.Cos(timeCounter), Mathf.Sin(timeCounter), 0.0f) * radiusMovement;
        if (timeCounter >= 360.0f) timeCounter = 0.0f;
    }

    private void MoveTowardsTarget(Transform targetTransform)
    {
        float realSpeed = decideSpeed(targetTransform);
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        enemyRigidBody.velocity = direction * realSpeed * Time.deltaTime;
    }

    private float decideSpeed(Transform targetTransform)
    {
        if (Vector3.Distance(targetTransform.position, transform.position) <= closeApproximity)
            return speed / 5.0f;
        return speed;
    }

    private float decideSpeed(Vector3 targetPosition)
    {
        if (Vector3.Distance(targetPosition, transform.position) <= closeApproximity)
            return speed / 5.0f;
        return speed;
    }

    private void FaceTarget(Transform targetTransform)
    {
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemyRigidBody.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemyRigidBody.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
    }

    private void Update()
    {
        if (playerIndexToAttack != -1) return;
        ChoosePlayerToAttack();
        DecideAttackType();
    }

    private void DecideAttackType()
    {
        if (Random.value <= 0.2) attackType = 3;
        else  attackType = Random.value <= 0.5 ? 1 : 2;
    }

    private void ChoosePlayerToAttack()
    {
        players = GameObject.FindGameObjectsWithTag(Utils.TagPlayer);
        if (players.Length <= 0) playerIndexToAttack = -1;
        else playerIndexToAttack = Random.Range(0, players.Length);
    }


}
