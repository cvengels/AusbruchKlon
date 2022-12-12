using UnityEngine;

public class Ball : MonoBehaviour
{
    public float startSpeed = 10;
    public Vector2 startPosition = new Vector2(0, -2);

    private Rigidbody2D ballRigidbody;

    private bool startEnabled;

    void Start()
    {
        ballRigidbody = GetComponent<Rigidbody2D>();
        ResetBall();
        startEnabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && startEnabled)
        {
            ballRigidbody.velocity = Vector2.down * startSpeed;
            startEnabled = false;
        }
    }

    void ResetBall()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        ballRigidbody.velocity = Vector2.zero;
        ballRigidbody.position = startPosition;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn"))
        {
            Debug.Log("Ball im Aus gelandet");
            ResetBall();
            startEnabled = true;
        }
    }
}
