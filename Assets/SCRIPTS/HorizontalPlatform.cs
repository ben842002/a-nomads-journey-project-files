using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlatform : MonoBehaviour
{
    public float speed;
    public float leftCoordinate;
    public float rightCoordinate;
    bool moveRight;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > rightCoordinate)
            moveRight = false;
        else if (transform.position.x < leftCoordinate)
            moveRight = true;

        if (moveRight == true)
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
    }
}
