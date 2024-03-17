using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Capabilities
{
    [RequireComponent(typeof(Controller))]
    public class Move : MonoBehaviour
    {
        private Controller _controller;
        private Vector2 _direction, _desiredVelocity, _velocity;
        private Collider2D _collider;
        private Rigidbody2D _body;
        private Ground _ground;
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4.2f;

        [Header("Dash")]
        [SerializeField, Range(0f, 100f)] private float _dashPower = 20f;
        [SerializeField, Range(0f, 10f)] private float _dashCooldown = 1f;
        [SerializeField, Range(0f, 1f)] private float _dashTime = 0.2f;
        [SerializeField] private bool _canDash = true;
        private float _dashDirection;
        private bool _isDashing;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private float _previousGravity;
        private LayerMask _previousLayerMask;
        [SerializeField] private LayerMask _dodgeableLayer;

        [Space(10)]
        private bool _isFacingRight;
        private bool _isFollowingMovement; // facing follow the movement

        private GameObject _platform;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();

            // TODO: Someone fix this, smelly code
            _spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
            _isFollowingMovement = true;
        }

        private void Update()
        {
            _direction.x = _controller.input.GetHorizontalMovement();
            _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.Friction, 0f);
            if (_isFollowingMovement)
            {
                _isFacingRight = _desiredVelocity.x > 0 || (_desiredVelocity.x >= 0 && _isFacingRight);
            }

            if (_isDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0)
                {
                    _isDashing = false;
                    _collider.excludeLayers = _previousLayerMask;
                    _body.gravityScale = _previousGravity;
                    _dashCooldownTimer = _dashCooldown;
                }
            }
            else if (!_canDash && _dashCooldownTimer >= 0)
            {
                _dashCooldownTimer -= Time.deltaTime;
                if (_dashCooldownTimer <= 0)
                {
                    _canDash = true;
                }
            }

            if (_controller.input.IsDashPressed() && _canDash)
            {
                Dash(_direction.x, _isFacingRight);
            }
            if (_platform && _controller.input.GetVerticalMovement() < 0f && _controller.input.IsJumpPressed())
            {
                _platform.GetComponentInChildren<PlatformTrigger>().DropPlayer();
            }


        }

        private void FixedUpdate()
        {
            if (_isDashing)
            {
                _velocity = new Vector2(_dashPower * _dashDirection, 0f);
                _body.gravityScale = 0f;
            }
            else
            {
                _velocity = _body.velocity;
                _velocity.x = _desiredVelocity.x;
            }

            _body.velocity = _velocity;

            if (_spriteRenderer) { _spriteRenderer.flipX = !_isFacingRight; }
        }

        private void Dash(float x, bool isFacingRight)
        {
            Debug.Log("Dash");
            _canDash = false;
            _isDashing = true;
            _dashTimer = _dashTime;
            _previousGravity = _body.gravityScale;
            _previousLayerMask = _collider.excludeLayers;
            _collider.excludeLayers = _dodgeableLayer;
            if (x > 0)
            {
                _dashDirection = 1f;
            }
            else if (x < 0)
            {
                _dashDirection = -1f;
            }
            else
            {
                _dashDirection = isFacingRight ? 1f : -1f;
            }
        }

        public void SetFollowMovement(bool followMovement)
        {
            _isFollowingMovement = followMovement;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Platform")))
            {
                _platform = collision.gameObject;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Platform")))
            {
                _platform = null;
            }
        }


    }
}