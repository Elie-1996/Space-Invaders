using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConnectionHandling : MonoBehaviour
{
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefab == null) throw new MissingReferenceException();

        Instantiate(PlayerPrefab);
    }

}
