using UnityEngine;

public class Controller : MonoBehaviour
{
    public float movingSpeedStanding;
    public float movingSpeedCrouched;
    public float movingSpeedAttacking;
    public float movingSpeedAir;

    public float jumpForceCrouched;
    public float jumpForceStanding;
    public float dashForce;

    public bool grounded;
    public bool crouched;
    public bool climbing;
    public bool attacking;

    public float dashMax;
    public float dashLeft;

    public float climbTime = 1f;

    public GameObject GroundSensor, HeadSensor;
    public Collider2D StandingCollider, MainCollider;
    public LayerMask groundLayer;

    private float velocityHomingPower = 0.1f;
    private float xAxis;
    private float yAxis;
    private Rigidbody2D rb;
    private SpriteRenderer renderer;
    private Animator anim;

    private bool canClimb;
    private float climbStart = 0;
    private Climbable climbable;

    // Start is called before the first frame update
    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        renderer = this.GetComponent<SpriteRenderer>();
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");

        flipSpriteFixCollider(xAxis);

        if (Input.GetMouseButton(0))
        {
            attack();
            attacking = true;
        }

        if (!climbing)
        {
            groundCheck();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                crouch();
            if (Input.GetButtonDown("Jump") && grounded)
                Jump();
            if (Input.GetButtonDown("Jump") && !grounded && dashLeft > 0)
                Dash();
            if (!grounded && canClimb && Input.GetButtonDown("Jump"))
                climb();

            if (grounded)
            {
                if (xAxis != 0)
                    groundHorizontalMovement();
            }
            else
            {
                airHorizontalMovement();
            }
        }
        else
        {
            climbingA();
        }

        passDataToAnimator();
        attacking = false;
    }

    private void airHorizontalMovement()
    {
        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, xAxis * movingSpeedAir, velocityHomingPower), rb.velocity.y);
    }

    private void groundHorizontalMovement()
    {
        if (attacking)
            rb.velocity = velocityLerp(movingSpeedAttacking);
        else
               if (!crouched)
            rb.velocity = velocityLerp(movingSpeedStanding);
        else
               if (crouched)
            rb.velocity = velocityLerp(movingSpeedCrouched);
    }

    private void groundCheck()
    {
        if (collisionCheck(GroundSensor.transform.position, Vector2.down) < 0.15f)
        {
            grounded = true;
            dashLeft = dashMax;
        }
        else
        {
            grounded = false;
        }
    }

    private void climbingA()
    {
        this.transform.position = Vector2.Lerp(climbable.gameObject.transform.position, climbable.Landing.transform.position, (Time.time - climbStart) / climbTime);
        if (Time.time - climbTime > climbStart)
        {
            climbing = false;
            this.transform.position = climbable.Landing.transform.position;
            rb.simulated = true;
        }
    }

    private void attack()
    {
        if (crouched && canUncrouch())
            crouch();
        if (!crouched)
            anim.Play("Attack");
    }

    private void climb()
    {
        rb.simulated = false;
        climbing = true;
        climbStart = Time.time;
    }

    private void crouch()
    {
        if (crouched && canUncrouch())
        {
            crouched = false;
            StandingCollider.enabled = true;
        }
        else if (!crouched)
        {
            crouched = true;
            StandingCollider.enabled = false;
        }
    }

    private void Dash()
    {
        dashLeft--;
        rb.velocity = Vector2.zero;
        rb.AddForce((Vector2.up * yAxis + Vector2.right * xAxis).normalized * dashForce, ForceMode2D.Impulse);
    }

    private void Jump()
    {
        if (crouched)
            rb.AddForce(Vector2.up * jumpForceCrouched, ForceMode2D.Impulse);
        if (!crouched)
            rb.AddForce(Vector2.up * jumpForceStanding, ForceMode2D.Impulse);
        if (canUncrouch() && crouched)
            crouch();
    }

    private void passDataToAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(xAxis));
        anim.SetFloat("YVelocity", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
    }

    private bool canUncrouch()
    {
        if (collisionCheck(HeadSensor.transform.position, Vector2.up) > 1f)
            return true;
        else
            return false;
    }

    private void flipSpriteFixCollider(float xAxis)
    {
        if (xAxis > 0)
        {
            renderer.flipX = false;
            MainCollider.offset = new Vector2(-0.12f, MainCollider.offset.y);
            StandingCollider.offset = new Vector2(-0.12f, StandingCollider.offset.y);
        }
        if (xAxis < 0)
        {
            renderer.flipX = true;
            MainCollider.offset = new Vector2(0.12f, MainCollider.offset.y);
            StandingCollider.offset = new Vector2(0.12f, StandingCollider.offset.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climb")
        {
            climbable = collision.GetComponent<Climbable>();
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Climb")
        {
            canClimb = false;
        }
    }

    private float collisionCheck(Vector2 Origin, Vector2 Direction)
    {
        RaycastHit2D Rc = Raycast(Origin, Direction, Color.red);
        if (Rc.collider != null)
            return Rc.distance;
        return 999;
    }

    private Vector2 velocityLerp(float moveSpeed)
    {
        return new Vector2(Mathf.Lerp(rb.velocity.x, xAxis * moveSpeed, velocityHomingPower), rb.velocity.y);
    }

    //Raycast but draws debug line with it
    private RaycastHit2D Raycast(Vector2 Origin, Vector2 Dir, Color color)
    {
        RaycastHit2D Rc = Physics2D.Raycast(Origin, Dir, 10, groundLayer);
        Debug.DrawRay(Origin, Dir, color);
        return Rc;
    }
}