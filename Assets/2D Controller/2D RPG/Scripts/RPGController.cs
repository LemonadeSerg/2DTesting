using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class RPGController : MonoBehaviour
{
    public float moveSpeed;
    public float dashSpeed;
    public float carrySpeed;
    public float dashRechTime;
    public float dashTime;

    private Vector2 axis;
    private Vector2 dashDir;
    private Animator animator;
    private Rigidbody2D rb;

    public bool canDash;
    public bool dashing;
    private float dashInitTime;

    private bool fixedPos;
    private Vector2 lastDir;

    private bool pickingUp = false;
    private bool holding = false;

    // Start is called before the first frame update
    private void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        InputHandling();
        playerUpdater();
        if (!fixedPos) applyMovement();
        pushAnimationMotionToAnimtor();
    }

    private void playerUpdater()
    {
        if (dashInitTime + dashTime < Time.time)
            dashing = false;

        if (dashInitTime + dashRechTime < Time.time)
            canDash = true;
        else
            canDash = false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pickup") || animator.GetCurrentAnimatorStateInfo(0).IsName("Walking_Pickup"))
            fixedPos = false;
    }

    private void InputHandling()
    {
        axis.x = Input.GetAxis("Horizontal");
        axis.y = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && canDash && !holding && !pickingUp)
            Dash();
        if (!dashing && !fixedPos)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !holding)
                pickup();
            else if (Input.GetKeyDown(KeyCode.Z) && holding)
                throwO();
        }
    }

    private void applyMovement()
    {
        if (dashing)
            rb.velocity = (dashDir / 2 + axis / 2) * dashSpeed;
        else if (holding)
            rb.velocity = axis * carrySpeed;
        else
            rb.velocity = axis * moveSpeed;
    }

    private void pushAnimationMotionToAnimtor()
    {
        animator.SetBool("Moving", axis != Vector2.zero);
        if (fixedPos)
        {
            if (lastDir.x != 0 || lastDir.y != 0)
            {
                animator.SetFloat("XAxis", lastDir.x);
                animator.SetFloat("YAxis", lastDir.y);
            }
        }
        else
        {
            if (axis.x != 0 || axis.y != 0)
            {
                animator.SetFloat("XAxis", axis.x);
                animator.SetFloat("YAxis", axis.y);
            }
        }
        animator.SetBool("Pickup", pickingUp);
        animator.SetBool("Holding", holding);
    }

    private void pickup()
    {
        pickingUp = true;
        holding = true;

        fixPos();
    }

    private void throwO()
    {
        pickingUp = false;
        holding = false;
    }

    private void fixPos()
    {
        lastDir = rb.velocity.normalized;
        rb.velocity = Vector2.zero;
        fixedPos = !fixedPos;
    }

    private void Dash()
    {
        dashInitTime = Time.time;
        dashDir = rb.velocity.normalized;
        dashing = true;
    }
}