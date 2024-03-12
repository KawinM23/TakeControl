﻿using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Capabilities
{
    [RequireComponent(typeof(Controller))]
    public class Move : MonoBehaviour
    {
        private Controller _controller;
        private Transform _transform;

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
        private bool canDash = true;
        private bool isDashing;
        [SerializeField] private LayerMask dodgeableLayer;

        [Space(10)]
        public bool isFacingRight;
        private bool followMovement; //facing follow the movement

        [Space(10)]
        public bool isCrouching;
        [SerializeField] private float crouchSpeedMultiplier = 0.5f;
        [SerializeField] private float crouchHeightMultiplier = 0.5f;
        private float defaultHeight;
        private float crouchHeight;

        private GameObject platform;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();
            _transform = GetComponent<Transform>();
            _spriteRenderer = transform.Find("PlayerSprite")?.GetComponent<SpriteRenderer>();
            followMovement = true;
            defaultHeight = _transform.localScale.y;
            crouchHeight = _transform.localScale.y * crouchHeightMultiplier;

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
            if (platform && _controller.input.RetrieveVerticalInput() < 0f && _controller.input.RetrieveJumpInput())
            {
                platform.GetComponentInChildren<PlatformTrigger>().DropPlayer();
            }

        }

        private void FixedUpdate()
        {
            // Handle crouching and movement speed
            if (_controller.input.RetrieveCrouchInput())
            {
                Debug.Log("Crouching!");
                isCrouching = true;
                // lower body height
                _transform.localScale = new Vector2(_transform.localScale.x, crouchHeight);
            }
            else
            {
                isCrouching = false;
                // reset body height
                _transform.localScale = new Vector2(_transform.localScale.x, defaultHeight);
            }

            // Handle dashing
            if (isDashing)
            {
                _body.gravityScale = 0f;
            }
            else
            {
                _velocity = _body.velocity;
                // slow down if crouching
                _velocity.x = _desiredVelocity.x * (isCrouching ? crouchSpeedMultiplier : 1);
            }
            _body.velocity = _velocity;
        }

        private IEnumerator Dash(float x, bool isFacingRight)
        {
            canDash = false;
            isDashing = true;
            LayerMask previousLayerMask = _collider.excludeLayers;
            _collider.excludeLayers = dodgeableLayer;
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
            _collider.excludeLayers = previousLayerMask;
            isDashing = false;
            _body.gravityScale = originalGravity;

            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
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