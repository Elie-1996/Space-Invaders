﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemy : MonoBehaviour
{
    public GameObject explosion;
    public GameObject rocke2Explosion;
    private GameController gameController;
    private void Start()
    {
        GameObject gameConrollerObject = GameObject.FindWithTag(Utils.TagGameConroller);
        if(gameConrollerObject != null)
        {
            gameController = gameConrollerObject.GetComponent<GameController>();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        int score =0;
        if (other.tag == Utils.TagBackground)
        {
            return;
        }
        if(other.tag == Utils.TagPlayer)
        {
            gameController.GameOverFunction();
        }
        if (other.tag == Utils.TagRocket2)
        {
           Collider[] radious =  Physics.OverlapSphere(other.transform.position, 10f);
            if(radious!= null)
            {
                foreach (Collider collider in radious) {
                    if (collider.tag == Utils.TagBackground || collider.tag == Utils.TagGameConroller || collider.tag == Utils.TagPlayer) {continue;}
                    score += Utils.getScoreByCollider(collider.tag);
                    Instantiate(explosion, collider.transform.position, collider.transform.rotation);
                    Destroy(collider.gameObject);
                }
                Instantiate(rocke2Explosion, other.transform.position, other.transform.rotation);
                gameController.addScore(score);
                return;
            }
        }
        score = Utils.getScoreByCollider(tag);
        gameController.addScore(score);
        Instantiate(explosion, other.transform.position, other.transform.rotation);
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
