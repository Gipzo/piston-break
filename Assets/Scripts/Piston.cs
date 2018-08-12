using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
// [ExecuteInEditMode]
public class Piston : MonoBehaviour
{

    public Piston ParentPiston;
    public float Delay;
    public float ClosingTime = 1f;
    private float _timer = 0;
    public AnimationCurve ClosingCurve;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    [Range(0, 1f)]
    public float Value = 0f;
    public Vector2 OriginalSize;
    public bool Loaded = false;
    public Vector3 OriginalPosition;
    public bool ParentActivaton = false;
    public bool DisableWarning = false;
    public bool AllowedToActivate = false;

    public bool Activated = false;
    public bool Stopped = false;
    public bool Warned = false;
    public float Accelerate = 1f;
    public List<Piston> ChildPistons;

    public float TimeLeft
    {
        get
        {
            var tl = (1f - Value) * ClosingTime;
            if (_timer < 0)
                tl += -_timer / Accelerate;
            if (ParentPiston != null)
                tl += ParentPiston.TimeLeft;
            return tl;
        }

    }
    // Use this for initialization
    void Awake()
    {
        ChildPistons = new List<Piston>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        ParentPiston = null;
        if (transform.parent != null)
            ParentPiston = transform.parent.GetComponent<Piston>();
        transform.parent = Level.Instance.transform;
        OriginalSize = _spriteRenderer.size;
        OriginalPosition = transform.position;
        _boxCollider.enabled = false;
        _timer = -Delay;
        Loaded = true;
        // UpdateSize();
    }

    /// /// Start is called on the frame when a script is enabled just before
    /// <summary>
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (ParentPiston != null)
        {
            ParentPiston.ChildPistons.Add(this);
        }
        UpdateSize();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.Paused)
            return;
        if (!Level.Instance.Loaded)
            return;

        if (!Level.Instance.LevelStarted)
        {
            Value = 0;
            UpdateSize();
            return;
        }

        if (ParentActivaton && !AllowedToActivate)
            return;

        if (ParentPiston == null || ParentPiston.Value >= 1.0f && Value < 1f)
        {
            if (_timer < 0)
            {
                _timer += Time.deltaTime * Accelerate;

            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        var _timeLeft = 0f;
        if (ParentPiston != null)
        {
            _timeLeft += ParentPiston.TimeLeft;
        }
        _timeLeft += -_timer;

        if (!Warned && _timeLeft <= GameManager.Instance.WarningTime)
        {
            Warned = true;
            if (!DisableWarning)
                GameManager.Instance.PistonWarning(this);
        }

        Value = Mathf.Clamp01(_timer / ClosingTime);

        if (Value > 0 && !Activated)
        {
            Activated = true;
            GameManager.Instance.PistonActivated(this);
        }
        if (!Stopped && Value >= 1f)
        {
            Stopped = true;
            GameManager.Instance.PistonStopped(this);
        }
        UpdateSize();
    }





    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        var size = GetComponent<SpriteRenderer>().size;
        var rot = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        var mat = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (Application.isPlaying)
            size = OriginalSize;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero + new Vector3(0, -size.y / 2, 0f), new Vector3(size.x, 0.05f, 0));

        if (transform.parent != null)
            ParentPiston = transform.parent.GetComponent<Piston>();
        if (ParentPiston != null)
        {
            Gizmos.matrix = mat;
            Gizmos.DrawLine(transform.position, ParentPiston.transform.position);

        }
#endif

    }

    public void UpdateSize()
    {
        var v = ClosingCurve.Evaluate(Value);
        if (v > 0.99f)
            v = 1f;

        if (v <= 0.01f)
        {
            _boxCollider.enabled = false;
            _spriteRenderer.enabled = false;
        }
        else
        {

            _spriteRenderer.enabled = true;
            _boxCollider.enabled = true;
            _boxCollider.size = new Vector2(OriginalSize.x, OriginalSize.y * v);
        }
        _spriteRenderer.size = new Vector2(OriginalSize.x, OriginalSize.y * v);
        transform.position = OriginalPosition + Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * new Vector3(0, -OriginalSize.y / 2 * (1.0f - v), 0f);
    }
}
