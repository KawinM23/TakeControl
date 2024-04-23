using UnityEngine;

namespace Assets.Scripts.Capabilities
{
    [RequireComponent(typeof(Controller))]
    public class Jump : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)] private float _jumpHeight = 3f;
        [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
        [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3.3f;
        [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.6f;

        [SerializeField, Range(0f, 0.3f)] private float _coyoteTime = 0.2f;
        [SerializeField, Range(0f, 0.3f)] private float _jumpBufferTime = 0.2f;

        private Controller _controller;
        private Rigidbody2D _body;
        private Ground _ground;
        private Vector2 _velocity;

        private int _jumpPhase;

        // _coyoteCounter is the time the player can jump after leaving the ground
        private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;

        private bool _desiredJump, _onGround, _isJumping;


        // Awake is called when the script instance is being loaded
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _ground = GetComponent<Ground>();
            _controller = GetComponent<Controller>();

            _defaultGravityScale = 1f;
        }

        // Update is called once per frame
        void Update()
        {
            _desiredJump |= _controller.Input.IsJumpPressed() && _controller.Input.GetVerticalMovement() >= 0f && Time.timeScale != 0f;
        }

        // FixedUpdate is called every fixed framerate frame
        private void FixedUpdate()
        {
            _onGround = _ground.IsOnGround;
            _velocity = _body.velocity;

            // If the player is on the ground, reset the jump phase and coyote counter
            if (_onGround && -0.01f <= _body.velocity.y && _body.velocity.y <= 0.01f)
            {
                _jumpPhase = 0;
                _coyoteCounter = _coyoteTime;
                _isJumping = false;
            }
            else
            {
                _coyoteCounter -= Time.deltaTime;
            }

            // If the player has pressed the jump button, set the jump buffer counter
            if (_desiredJump)
            {
                _desiredJump = false;
                _jumpBufferCounter = _jumpBufferTime;
            }
            else if (!_desiredJump && _jumpBufferCounter > 0)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }

            // If the jump buffer counter is greater than 0, jump
            if (_jumpBufferCounter > 0)
            {
                JumpAction();
            }

            // If the player is holding the jump button and is moving upwards, reduce gravity
            if (_controller.Input.IsJumpHeld() && _body.velocity.y > 0)
            {
                _body.gravityScale = _upwardMovementMultiplier;
            }
            else if (!_controller.Input.IsJumpHeld() || _body.velocity.y < 0)
            {
                _body.gravityScale = _downwardMovementMultiplier;
            }
            else if (_body.velocity.y == 0)
            {
                _body.gravityScale = _defaultGravityScale;
            }

            // Apply the velocity to the rigidbody
            _body.velocity = _velocity;
        }
        private void JumpAction()
        {
            // only play sfx at the start of the jump
            if (!_isJumping)
            {
                if (SoundManager.Instance) SoundManager.Instance.PlayJump();
            }

            if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
            {
                if (_isJumping)
                {
                    _jumpPhase += 1;
                    // sound for double jump
                    SoundManager.Instance.PlayJump();
                }

                _jumpBufferCounter = 0;
                _coyoteCounter = 0;
                _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
                _isJumping = true;

                if (_velocity.y > 0f)
                {
                    _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
                }
                else if (_velocity.y < 0f)
                {
                    _jumpSpeed += Mathf.Abs(_body.velocity.y);
                }
                _velocity.y += _jumpSpeed;
            }
        }

        public int GetMaxAirJump()
        {
            return _maxAirJumps;
        }

        public void SetMaxAirJump(int maxAirJump)
        {
            _maxAirJumps = maxAirJump;
        }
    }
}
