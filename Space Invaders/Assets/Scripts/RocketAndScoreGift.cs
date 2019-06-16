using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class RocketAndScoreGift : MonoBehaviour
{
    public GameObject explosion;

    private float sw;
    private GameController gameController;
    private AudioSource audioData;
    private bool shouldDestry;
    private int giftType;// 1 for extra master rocket, 2 for additional 25 score, 3 for 10 seconds 
    // Start is called before the first frame update
    void Start()
    {
        GameObject gameConrollerObject = GameObject.FindWithTag(Utils.TagGameConroller);
        if (gameConrollerObject != null)
        {
            gameController = gameConrollerObject.GetComponent<GameController>();
        }
        audioData = GetComponent<AudioSource>();
        shouldDestry = false;
        sw = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Utils.TagPlayer)
        {
            audioData.Play();
            if (giftType == 1)  // extra master rocket
            {
                gameController.setExtraRocket(true);
            }
            else if (giftType == 2) //extra score
            {
                gameController.addScore(25);
            }
            else
            {
                gameController.setSpeedGift(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == Utils.TagPlayer)
        {
            shouldDestry = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        sw += Time.time;
        if (shouldDestry) { Destroy(gameObject); }
        if (sw > 8*1000)
        {
            Instantiate(explosion, transform.position,transform.rotation);
            Destroy(explosion);
            Destroy(gameObject);
        }
    }
    void onStart(int gift)
    {
        giftType = gift;
    }
}
