using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;

    private SimpleMovementInput movement;
    private Rigidbody2D body;
    private Camera mainCam;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        movement = new SimpleMovementInput();
        mainCam = Camera.main;
    }

    private void Update()
    {   
        if (Input.GetMouseButton(0))
        {
            var mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            var direction = mouseWorldPos - transform.position;
            if (Vector2.Distance(mouseWorldPos,transform.position) > 0.2f)
            {
                Move(direction, body, speed);
            }
        }
        
    }

    public void Move(Vector2 direction, Rigidbody2D body, float speed)
    {
        body.AddForce(direction.normalized * speed, ForceMode2D.Impulse);
    }
}

public class SimpleMovementInput
{
    
}
