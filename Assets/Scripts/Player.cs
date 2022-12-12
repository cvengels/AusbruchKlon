using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    private Rigidbody2D myRigidbody;
    
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        float movement = Input.GetAxisRaw("Horizontal");

        myRigidbody.velocity = new Vector2(movement, 0) * speed;





    }
}
