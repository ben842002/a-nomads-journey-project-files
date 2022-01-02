using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float upperCoordinate;
    [SerializeField] float lowerCoordinate;
    bool moveUp;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > upperCoordinate)
            moveUp = false;
        else if (transform.position.y < lowerCoordinate)
            moveUp = true;

        if (moveUp == true)
            transform.position = new Vector2(transform.position.x, transform.position.y + speed * Time.deltaTime);
        else
            transform.position = new Vector2(transform.position.x, transform.position.y - speed * Time.deltaTime);
    }
}
