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
        [SerializeField, Range(0f, 100f)] private float dashingPower = 20f;
        [SerializeField, Range(0f, 10f)] private float dashingCooldown = 1f;
        [SerializeField, Range(0f, 1f)] private float dashingTime = 0.2f;
        [SerializeField] bool canDash = true;
        float dashDirection;
        private bool isDashing;
        private float dashTimer;
        private float dashCooldownTimer;
        private float previousGravity;
        private LayerMask previousLayerMask;
        [SerializeField] private LayerMask dodgeableLayer;

        [Space(10)]
        public bool isFacingRight;
        private bool followMovement; //facing follow the movement

        private GameObject platform;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();
            _spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
            followMovement = true;

        }

        private void Update()
        {
            _direction.x = _controller.input.RetrieveMoveInput();
            _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.Friction, 0f);
            if (followMovement)
            {
                isFacingRight = _desiredVelocity.x > 0 ? true : (_desiredVelocity.x < 0 ? false : isFacingRight);
            }
            
            if (isDashing)
            {
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false;
                    _collider.excludeLayers = previousLayerMask;
                    _body.gravityScale = previousGravity;
                    dashCooldownTimer = dashingCooldown;
                }
            }
            else if(!canDash && dashCooldownTimer >= 0)
            {
                dashCooldownTimer -= Time.deltaTime;
                if (dashCooldownTimer <= 0)
                {
                    canDash = true;
                }
            }

            if (_controller.input.RetrieveDashInput() && canDash)
            {
                Dash(_direction.x, isFacingRight);
            }
            if (platform && _controller.input.RetrieveVerticalInput() < 0f && _controller.input.RetrieveJumpInput())
            {
                platform.GetComponentInChildren<PlatformTrigger>().DropPlayer();
            }


        }

        private void FixedUpdate()
        {
            if (isDashing)
            {
                _velocity = new Vector2(dashingPower * dashDirection, 0f);
                _body.gravityScale = 0f;
            }
            else
            {
                _velocity = _body.velocity;
                _velocity.x = _desiredVelocity.x;
            }

            _body.velocity = _velocity;

            if (_spriteRenderer) { _spriteRenderer.flipX = !isFacingRight; }
        }

        private void Dash(float x, bool isFacingRight)
        {
            Debug.Log("Dash");
            canDash = false;
            isDashing = true;
            dashTimer = dashingTime;
            previousGravity = _body.gravityScale;
            previousLayerMask = _collider.excludeLayers;
            _collider.excludeLayers = dodgeableLayer;
            if (x > 0)
            {
                dashDirection = 1f;
            }
            else if (x < 0)
            {
                dashDirection = -1f;
            }
            else
            {
                dashDirection = isFacingRight ? 1f : -1f;
            }
        }

        public void SetFollowMovement(bool followMovement)
        {
            this.followMovement = followMovement;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Platform")))
            {
                platform = collision.gameObject;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Platform")))
            {
                platform = null;
            }
        }


    }
}