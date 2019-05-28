using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundObject : MonoBehaviour
{
    public enum MotionShape { CIRCULAR, RECTANGULAR };

    public Transform Center;
    public MotionShape motion;
    public float speed;
    public float startDistance;

    private float time = 0;
    private Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (startDistance <= 0) Debug.LogError(typeof(MoveAroundObject).Name + ": startDistance should be a positive float number! (" + startDistance + ")");
        if (speed < 0) Debug.LogError(typeof(MoveAroundObject).Name + ": speed cannot be negative. (" + speed + ")");
        if (Center == null) Debug.LogError(typeof(MoveAroundObject).Name + ": Should be instantiated!");
        time = 0.0f;

        // get random x,y,z
        Vector3 randomSphere = Random.onUnitSphere * startDistance;
        startingPosition = randomSphere + Center.position;
        transform.position = startingPosition;
    }

    private void FixedUpdate()
    {
        switch (motion)
        {
            case MotionShape.CIRCULAR:
                CircularMotion();
                break;
            case MotionShape.RECTANGULAR:
                RectangularMotion();
                break;
        }
    }

    void CircularMotion()
    {
        // Buggy, should circulate around the player (meaning "Center")
        time += Time.deltaTime*speed;
        float x = startingPosition.x + Mathf.Cos(startingPosition.x + time);
        float y = startingPosition.y + Mathf.Sin(startingPosition.y + time);
        float z = startingPosition.z;
        transform.position = new Vector3(x, y, z);
    }

    void RectangularMotion()
    {

    }
}
