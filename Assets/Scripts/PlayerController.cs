using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] float _speed = 10f;
    bool _facingLeft;
    // TODO: If y input is ignored, change this to float
    Vector2 _direction;

    [Header("Jump")]
    [SerializeField] float _jumpStrength = 10f;

    [Header("Physics")]
    [SerializeField] float _maxSpeed = 7f;
    [SerializeField] float _linearDrag = 4f;


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
        _direction = new Vector2();

        _move = _playerInput.actions["Move"];
        _jump = _playerInput.actions["Jump"];
        _run = _playerInput.actions["Run"];
    }

    void Update()
    {
        float moveValue = _move.ReadValue<float>();
        if (moveValue != 0f)
            _direction.x = moveValue > 0 ? 1f : -1f;
        else
            _direction.x = 0f;
    }

    void FixedUpdate()
    {
        MoveCharacter(_direction.x);
        ModifyPhysics();
    }

    void LateUpdate()
    {
        float rbx = Mathf.Abs(_rigidBody.velocity.x);
        float rby = _rigidBody.velocity.y;
        float inx = _move.ReadValue<float>();
        _animator.SetBool("Idle", rbx == 0 && rby == 0);
        _animator.SetBool("Jump", Mathf.Abs(rby) > 0.15f);
        _animator.SetBool("Turn", rbx > 0 && inx < 0 || rbx < 0 && inx > 0);
        _animator.SetFloat("Horizontal", rbx);
    }

    void MoveCharacter(float horizontal)
    {
        _rigidBody.AddForce(Vector2.right * horizontal * _speed);

        if (horizontal > 0 && _facingLeft || horizontal < 0 && !_facingLeft)
            _sprite.flipX = !_facingLeft;

        if (Mathf.Abs(_rigidBody.velocity.x) > _maxSpeed)
            _rigidBody.velocity = new Vector2(
                Mathf.Sign(_rigidBody.velocity.x) * _maxSpeed,
                _rigidBody.velocity.y
            );
    }

    void ModifyPhysics()
    {
        bool changingDirections = _direction.x > 0 && _rigidBody.velocity.x < 0 || _direction.x < 0 && _rigidBody.velocity.x > 0;

        if (Mathf.Abs(_direction.x) < 0.4f || changingDirections)
            _rigidBody.drag = _linearDrag;
        else
            _rigidBody.drag = 0;
    }
}
