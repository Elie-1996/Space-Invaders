using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class DestroyEnemy : NetworkBehaviour
{
    public GameObject explosion;
    public GameObject rocke2Explosion;
    public GameObject woodBox;
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
        if (other.tag == Utils.TagBackground || other.tag == Utils.TagWoodBox || other.tag == Utils.TagEnemy|| other.tag == Utils.TagAsteroid)
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
                    Utils.CmdDestroyObjectByID(collider.gameObject.GetComponent<NetworkIdentity>());
                    if(collider.tag == Utils.TagEnemy) { gameController.enemyKilled(); HandlePropOfGift(); }
                }
                Instantiate(rocke2Explosion, other.transform.position, other.transform.rotation);
                gameController.addScore(score);
                return;
            }
        }
        score = Utils.getScoreByCollider(tag);
        gameController.addScore(score);
        Instantiate(explosion, other.transform.position, other.transform.rotation);
        HandlePropOfGift();
        Utils.CmdDestroyObjectByID(other.gameObject.GetComponent<NetworkIdentity>());
        Utils.CmdDestroyObjectByID(gameObject.GetComponent<NetworkIdentity>());
        gameController.enemyKilled();
    }

    public void HandlePropOfGift()
    {
        int rand = Random.Range(1, 101);
        if(rand < 40)
        {
            int randomGift = Random.Range(1, 4);
            GameObject gift = Instantiate(woodBox, transform.position, transform.rotation);
            HandleGiftColoring(randomGift);
            gift.SendMessage("onStart", randomGift);
        }
    }

    private void HandleGiftColoring(int giftType)
    {
        Light pointLight = GameObject.FindGameObjectWithTag(Utils.TagPontLight).GetComponent<Light>();
        Light spotLigh1 = GameObject.FindGameObjectWithTag(Utils.TagSpotLight1).GetComponent<Light>();
        Light spotLigh2 = GameObject.FindGameObjectWithTag(Utils.TagSpotLight2).GetComponent<Light>();
        if (giftType == 1)  // extra master rocket
        {
            pointLight.color = Color.red;
            spotLigh1.color = Color.red;
            spotLigh2.color = Color.red;
        }
        else if (giftType == 2) //extra score
        {
            pointLight.color = Color.yellow;
            spotLigh1.color = Color.yellow;
            spotLigh2.color = Color.yellow;
        }
        else
        {
            pointLight.color = Color.blue;
            spotLigh1.color = Color.blue;
            spotLigh2.color = Color.blue;
        }
    }
}
