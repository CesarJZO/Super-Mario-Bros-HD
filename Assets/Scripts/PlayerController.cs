using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] float jumpStrength = 10;

    PlayerInput _playerInput;
    InputAction _move;
    InputAction _jump;
    InputAction _run;
    Vector2 _direction;
    float _absAxis;
    Rigidbody2D _rigidBody;
    SpriteRenderer _sprite;
    Animator _animator;

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
        _direction.x = _move.ReadValue<float>() * speed;        
    }

    void FixedUpdate()
    {
        _direction.y = _rigidBody.velocity.y;
        _absAxis = Mathf.Abs(_rigidBody.velocity.x);
        if (_absAxis < speed)
            _rigidBody.AddForce(_direction);
        if (_jump.triggered)
            _rigidBody.AddForce(Vector2.up * jumpStrength, ForceMode2D.Impulse);
    }

    void LateUpdate()
    {
        if (_absAxis > 0.1f)
            _sprite.flipX = _rigidBody.velocity.x < 0;
        float rbx = _rigidBody.velocity.x;
        float rby = _rigidBody.velocity.y;
        float inx = _move.ReadValue<float>();
        _animator.SetBool("Idle", rbx == 0 && rby == 0);
        _animator.SetBool("Jump", Mathf.Abs(rby) > 0.15f);
        _animator.SetBool("Turn", rbx > 0 && inx < 0 || rbx < 0 && inx > 0);
        _animator.SetFloat("Horizontal", rbx);
    }
}
