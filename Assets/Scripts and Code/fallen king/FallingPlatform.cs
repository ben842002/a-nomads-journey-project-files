using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float dropDelay;
    [SerializeField] float delayBeforeReturn;
    [SerializeField] float moveSpeed;
    [SerializeField] bool moveBack;

    Vector3 originalPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPos = transform.position;
    }

    private void Update()
    {
        if (moveBack == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, originalPos, moveSpeed * Time.deltaTime);
            if (transform.position.y == originalPos.y)          
                moveBack = false;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && moveBack == false)
            Invoke(nameof(DropPlatform), dropDelay);
    }

    void DropPlatform()
    {
        rb.isKinematic = false;
        Invoke(nameof(ReturnToOriginalPos), delayBeforeReturn);
    }
    
    void ReturnToOriginalPos()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        moveBack = true;
    }
}
