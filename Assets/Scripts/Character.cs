using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{


    public bool CanJump = true;
    public bool IsJumping = true;
    public bool OnFloor;
    public bool InAir;

    public float MaxHorizontalVelocity = 1f;
    public float MaxUpVelocity = 1f;
    public float MaxDownVelocity = 1f;

    public float DefaultGravity = 1f;
    public float MidGravity = 2f;
    public float DownGravity = 3f;

    public float VelocityCutoff = 0.4f;
    public float BreakCoef = 1.4f;
    public float ForceMultiplier = 50f;
    public float JumpForceMultiplier = 50f;
    public float AirControl = 0.5f;
    public float FloatTimout = 0.1f;
    private float _floatTimer = 0;
    private Rigidbody2D _body;
    public Vector2 InputVelocity;
    public Collider2D SquashCollider;

    private Level _level;
    private Collider2D _lastCollider;


    void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
    }



    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!Level.Instance.Loaded || !GameManager.Instance.LevelStarted)
            return;
        if (other.otherCollider == SquashCollider)
        {
            if (_lastCollider != null && _lastCollider != other.collider)
            {
                GameManager.Instance.Die(this);
            }
            else
            {
                _lastCollider = other.collider;
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        _lastCollider = null;
    }

    void CheckForSurroundings()
    {
        RaycastHit2D floor_cast = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, 1 << LayerMask.NameToLayer("Level"));
        if (floor_cast.collider != null)
        {
            if (floor_cast.collider.gameObject.tag == "Piston")
            {
                GameManager.Instance.StandingOn(floor_cast.collider.GetComponent<Piston>());
            }
            OnFloor = true;
            InAir = false;
            _floatTimer = 0;
        }
        else
        {

            if (_floatTimer < FloatTimout)
            {
                _floatTimer += Time.deltaTime;
            }
            else
            {

                OnFloor = false;
                InAir = true;
            }
        }
        CanJump = OnFloor;
    }

    void FixedUpdate()
    {
        if (InAir)
        {
            InputVelocity = new Vector2(InputVelocity.x * AirControl, InputVelocity.y);
        }
        if (_body.velocity.x < MaxHorizontalVelocity && InputVelocity.x > 0 ||
        _body.velocity.x > -MaxHorizontalVelocity && InputVelocity.x < 0)
            _body.AddForce(new Vector2(InputVelocity.x, 0) * ForceMultiplier);

        if (Mathf.Abs(InputVelocity.x) < 0.1f && Mathf.Abs(_body.velocity.x) > VelocityCutoff)
        {
            _body.velocity = new Vector2(_body.velocity.x / BreakCoef, _body.velocity.y);
        }
        else if (Mathf.Abs(_body.velocity.x) < VelocityCutoff)
        {
            _body.velocity = new Vector2(0, _body.velocity.y);
        }

        if (!IsJumping && CanJump && InputVelocity.y > 0)
        {
            if (_body.velocity.y < MaxUpVelocity)
            {
                _body.AddForce(new Vector2(0, InputVelocity.y) * JumpForceMultiplier, ForceMode2D.Impulse);
                GameManager.Instance.Jump(this);
            }
        }

        if (_body.velocity.y < 0 && InAir)
        {
            IsJumping = false;
            _body.gravityScale = DownGravity;
        }
        else if (_body.velocity.y > 0 && InputVelocity.y <= 0)
        {
            _body.gravityScale = MidGravity;
        }
        else
        {
            _body.gravityScale = DefaultGravity;
        }

    }


    void CheckInput()
    {
        var jump = 0.0f;
        if (Input.GetButton("Jump"))
            jump = 1f;
        if (Input.GetButtonUp("Jump"))
            IsJumping = false;
        InputVelocity = new Vector2(Input.GetAxis("Horizontal"), jump);
        if (!GameManager.Instance.LevelFinished && !GameManager.Instance.LevelStarted && InputVelocity.magnitude > 0)
        {
            GameManager.Instance.StartGame();
        }
    }

    public void Freeze()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void Unfreeze()
    {
        _body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckForSurroundings();
    }
}
