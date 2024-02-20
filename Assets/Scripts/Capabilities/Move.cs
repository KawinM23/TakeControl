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
        private Rigidbody2D _body;
        private Ground _ground;
        private SpriteRenderer _spriteRenderer;

        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4.2f;

        [SerializeField, Range(0f, 100f)] private float dashingPower = 20f;
        [SerializeField, Range(0f, 10f)] private float dashingCooldown = 1f;
        [SerializeField, Range(0f, 1f)] private float dashingTime = 0.2f;
        private bool canDash = true;
        private bool isDashing;
        public bool isFacingRight;
        private bool followMovement; //facing follow the movement



        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();
            _spriteRenderer = transform.Find("PlayerSprite")?.GetComponent<SpriteRenderer>();
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
            if (_spriteRenderer) { _spriteRenderer.flipX = !isFacingRight; }

            if (_controller.input.RetrieveDashInput() && canDash)
            {
                StartCoroutine(Dash(_direction.x, isFacingRight));
            }

        }

        private void FixedUpdate()
        {
            if (isDashing)
            {
                _body.gravityScale = 0f;
            }
            else
            {
                _velocity = _body.velocity;
                _velocity.x = _desiredVelocity.x;
            }
            _body.velocity = _velocity;

        }

        private IEnumerator Dash(float x, bool isFacingRight)
        {
            canDash = false;
            isDashing = true;
            float originalGravity = _body.gravityScale;
            float direction;
            if (x > 0)
            {
                direction = 1f;
            }
            else if (x < 0)
            {
                direction = -1f;
            }
            else
            {
                direction = isFacingRight ? 1f : -1f;
            }
            _velocity = new Vector2(dashingPower * direction, 0f);

            yield return new WaitForSeconds(dashingTime);
            isDashing = false;
            _body.gravityScale = originalGravity;

            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        public void SetFollowMovement(bool followMovement)
        {
            this.followMovement = followMovement;
        }


    }
}