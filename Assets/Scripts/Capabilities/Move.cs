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
        [SerializeField, Range(0f, 100f)] private float _dashingPower = 20f;
        [SerializeField, Range(0f, 10f)] private float _dashingCooldown = 1f;
        [SerializeField, Range(0f, 1f)] private float _dashingTime = 0.2f;
        private bool _canDash = true;
        private bool _isDashing;
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
            if (_spriteRenderer) { _spriteRenderer.flipX = !_isFacingRight; }

            if (_controller.input.IsDashPressed() && _canDash)
            {
                StartCoroutine(DashAction(_direction.x, _isFacingRight));
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

        /// <summary>
        /// Coroutine that performs a dash action.
        /// </summary>
        /// <param name="x">The horizontal input value.</param>
        /// <param name="isFacingRight">Flag indicating if the character is facing right.</param>
        /// <returns>An IEnumerator representing the coroutine.</returns>
        private IEnumerator DashAction(float x, bool isFacingRight)
        {
            _canDash = false;
            _isDashing = true;
            LayerMask previousLayerMask = _collider.excludeLayers;
            _collider.excludeLayers = _dodgeableLayer;
            float originalGravity = _body.gravityScale;
            float direction = x != 0 ? Mathf.Sign(x) : (isFacingRight ? 1f : -1f);
            _velocity = new Vector2(_dashingPower * direction, 0f);

            yield return new WaitForSeconds(_dashingTime);
            _collider.excludeLayers = previousLayerMask;
            _isDashing = false;
            _body.gravityScale = originalGravity;

            yield return new WaitForSeconds(_dashingCooldown);
            _canDash = true;
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