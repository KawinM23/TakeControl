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
        [SerializeField, Range(0f, 1f)]  private float dashingTime = 0.2f;
        private bool canDash = true;
        private bool isDashing;
        private bool isFacingRight;
        private bool followMovement; //facing follow the movement
        
      

        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();
            _spriteRenderer = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
            followMovement = true;
        }

        private void Update()
        {
            _direction.x = _controller.input.RetrieveMoveInput();
            _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _ground.Friction, 0f);
            if (followMovement)
            {
                if(_desiredVelocity.x > 0)
                {
                    isFacingRight = true;
                    
                }
                else if( _desiredVelocity.x < 0)
                {
                    isFacingRight = false;
                    
                }
            }
            _spriteRenderer.flipX = !isFacingRight;

            if (_controller.input.RetrieveDashInput() && canDash)
            {
                StartCoroutine(Dash());
            }
            
        }



        private void FixedUpdate()
        {

            if (isDashing)
            {
                return;
            }

            _body.velocity = new Vector2( dashingPower, _body.velocity.y);
            _velocity = _body.velocity;

            _velocity.x = _desiredVelocity.x;

            _body.velocity = _velocity;
            
        }

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = _body.gravityScale;
            _body.gravityScale = 0f;
            _body.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
           
            yield return new WaitForSeconds(dashingTime);
            
            _body.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        public bool IsFacingRight()
        {
            return isFacingRight;
        }

        public void SetIsFacingRight(bool isFacingRight)
        {
            this.isFacingRight = isFacingRight;
        }

        public void SetFollowMovement(bool followMovement)
        {
            this.followMovement = followMovement;
        }


    }
}