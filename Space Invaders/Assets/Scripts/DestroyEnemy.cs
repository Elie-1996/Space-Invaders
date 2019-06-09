using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    public GameObject explosion;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Utils.TagBackground)
        {
            return;
        }
        Instantiate(explosion, other.transform.position, other.transform.rotation);
        Destroy(other.gameObject);
        Destroy(explosion);
        Destroy(gameObject);
    }
}
