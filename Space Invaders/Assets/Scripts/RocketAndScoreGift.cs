using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class RocketAndScoreGift : MonoBehaviour
{
    public GameObject explosion;

    private Stopwatch sw;
    private GameController gameController;
    private AudioSource audioData;
    private bool shouldDestry;
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
        sw = Stopwatch.StartNew();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Utils.TagPlayer)
        {
            audioData.Play();
            //Destroy(gameObject);
            gameController.setExtraRocket(true);
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
        if (shouldDestry) { Destroy(gameObject); }
        if (sw.ElapsedMilliseconds > 8 * 1000)
        {
            Instantiate(explosion, transform.position,transform.rotation);
            Destroy(explosion);
            Destroy(gameObject);
        }
    }
}
