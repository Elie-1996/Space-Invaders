using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket2Movment : MonoBehaviour
{
    public float speed;
    private Vector3 origin;
    private Vector3 distance;
    public GameObject explosion;
    public GameObject rocke2Explosion;
    private GameController gameController;
    private void Start()
    {
        GameObject gameConrollerObject = GameObject.FindWithTag(Utils.TagGameConroller);
        if (gameConrollerObject != null)
        {
            gameController = gameConrollerObject.GetComponent<GameController>();
        }
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Debug.LogError(gameObject.name + " (Movement.cs): No Rigidbody component was found!");
            return;
        }
        rigidbody.velocity = transform.forward * speed;
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        distance = transform.position - origin;
        if (distance.x > 20 || distance.y > 20 || distance.z > 20)
        {
            int score = 0;
            Collider[] radious = Physics.OverlapSphere(transform.position, 5f);
            if (radious != null)
            {
                foreach (Collider collider in radious)
                {
                    if (collider.tag == Utils.TagBackground || collider.tag == Utils.TagGameConroller || collider.tag == Utils.TagPlayer) { continue; }
                    score += Utils.getScoreByCollider(collider.tag);
                    Instantiate(explosion, collider.transform.position, collider.transform.rotation);
                    Destroy(collider.gameObject);
                }
                Instantiate(rocke2Explosion, transform.position, transform.rotation);
                gameController.addScore(score);
                return;
            }
        }
    }
}
