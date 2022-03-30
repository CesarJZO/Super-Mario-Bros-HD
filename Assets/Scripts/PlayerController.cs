using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float _speed = 10f;
    bool _facingLeft;
    float _direction;

    [Header("Vertical Movement")]
    [SerializeField] float _jumpStrength = 15f;
    [SerializeField] float _jumpDelay = 0.25f;
    private float _jumpTimer;

    [Header("Physics")]
    [SerializeField] float _maxSpeed = 7f;
    [SerializeField] float _linearDrag = 4f;
    [SerializeField] float _gravity = 1f;
    [SerializeField] float _fallMultiplier = 5f;

    [Header("Collision")]
    [SerializeField] float _groundLength = 0.6f;
    [SerializeField] Vector3 _colliderOffset;
    bool _onGround;

    [Header("Components")]
    [SerializeField] LayerMask _groundLayer;

    #region Other Components references
    PlayerInput _playerInput;
    InputAction _move;
    InputAction _jump;
    InputAction _run;
    Animator _animator;
    SpriteRenderer _sprite;
    Rigidbody2D _rigidBody;
    #endregion

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();

        _move = _playerInput.actions["Move"];
        _jump = _playerInput.actions["Jump"];
        _run = _playerInput.actions["Run"];
    }

    void Update()
    {
        float moveValue = _move.ReadValue<float>();
        if (moveValue != 0f)
            _direction = moveValue > 0 ? 1f : -1f;
        else
            _direction = 0f;

        if (_jump.triggered)
            _jumpTimer = Time.time + _jumpDelay;
    }

    void FixedUpdate()
    {
        RaycastHit2D leftRaycast = Physics2D.Raycast(
            transform.position + _colliderOffset,
            Vector2.down, _groundLength, _groundLayer
        );
        RaycastHit2D rightRaycast = Physics2D.Raycast(
            transform.position - _colliderOffset,
            Vector2.down, _groundLength, _groundLayer
        );
        _onGround = leftRaycast || rightRaycast;

        MoveCharacter();

        if (_jumpTimer > Time.time && _onGround)
            Jump();

        ModifyPhysics();
    }

    void LateUpdate()
    {
        _sprite.flipX = _facingLeft;
        float xVelocity = _rigidBody.velocity.x;
        float yVelocity = _rigidBody.velocity.y;
        float xInput = _move.ReadValue<float>();
        _animator.SetFloat("Horizontal", Mathf.Abs(xVelocity));
        _animator.SetFloat("Vertical", yVelocity);
        _animator.SetBool("Grounded", _onGround);
        _animator.SetBool("Turn", xVelocity > 0 && xInput < 0 || xVelocity < 0 && xInput > 0);
    }

    void MoveCharacter()
    {
        _rigidBody.AddForce(Vector2.right * _direction * _speed);

        if (_direction > 0 && _facingLeft || _direction < 0 && !_facingLeft)
            _facingLeft = !_facingLeft;

        if (Mathf.Abs(_rigidBody.velocity.x) > _maxSpeed)
            _rigidBody.velocity = new Vector2(
                Mathf.Sign(_rigidBody.velocity.x) * _maxSpeed,
                _rigidBody.velocity.y
            );
    }

    void Jump()
    {
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _rigidBody.AddForce(Vector2.up * _jumpStrength, ForceMode2D.Impulse);
        _jumpTimer = 0f;
    }

    void ModifyPhysics()
    {
        bool changingDirections = _direction > 0 && _rigidBody.velocity.x < 0 || _direction < 0 && _rigidBody.velocity.x > 0;

        if (_onGround)
        {
            if (Mathf.Abs(_direction) < 0.4f || changingDirections)
                _rigidBody.drag = _linearDrag;
            else
                _rigidBody.drag = 0;
            _rigidBody.gravityScale = 0;
        }
        else
        {
            _rigidBody.gravityScale = _gravity;
            _rigidBody.drag = _linearDrag * 0.15f;
            if (_rigidBody.velocity.y < 0)
                _rigidBody.gravityScale = _gravity * _fallMultiplier;
            else if (_rigidBody.velocity.y > 0 && !_jump.IsPressed())
                _rigidBody.gravityScale = _gravity * (_fallMultiplier / 2);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + _colliderOffset, transform.position + _colliderOffset + Vector3.down * _groundLength);
        Gizmos.DrawLine(transform.position - _colliderOffset, transform.position - _colliderOffset + Vector3.down * _groundLength);
    }
}
