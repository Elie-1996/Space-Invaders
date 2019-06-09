using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Utils.TagBackground)
        {
            return;
        }
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
