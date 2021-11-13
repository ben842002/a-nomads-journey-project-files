using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayerRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // rotate towards player
        float randomAngle = Random.Range(0, 180);
        transform.Rotate(0f, 0f, randomAngle);
    }
}
