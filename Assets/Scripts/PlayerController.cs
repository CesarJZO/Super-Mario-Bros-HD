using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpStrength = 10;
    public string horizontalAxis = "Horizontal";
    public string jumpButton = "Jump";
    public string runButton = "Run";
    float deadZone = 0.15f;
    Vector2 direction;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = new Vector2();
    }

    void Update()
    {
        Debug.Log(Input.GetAxis(horizontalAxis));
        Debug.Log(Input.GetAxis("Vertical"));
        // direction.y = rb.velocity.y;
        direction.x = Input.GetAxis(horizontalAxis) * speed;
        // rb.velocity = direction;
        if (Mathf.Abs(rb.velocity.x) < speed)
            rb.AddForce(direction);
        if (Input.GetButtonDown(jumpButton))
            rb.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
        if (Mathf.Abs(rb.velocity.x) > deadZone)
            sr.flipX = rb.velocity.x < 0;
    }

    void LateUpdate()
    {
        float rbx = rb.velocity.x;
        float rby = rb.velocity.y;
        float inx = Input.GetAxis(horizontalAxis);
        animator.SetBool("Idle", rbx == 0 && rby == 0);
        animator.SetBool("Jump", Mathf.Abs(rby) > 0.15f);
        animator.SetBool("Turn", rbx > 0 && inx < 0 || rbx < 0 && inx > 0);
        animator.SetFloat("Horizontal", rbx);
    }
}
