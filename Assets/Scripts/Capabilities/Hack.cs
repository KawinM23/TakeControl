using System.Collections;
using Assets.Scripts.Combat;
using Assets.Scripts.Effect;
using UnityEngine;

#nullable enable

namespace Assets.Scripts.Capabilities
{
    [RequireComponent(typeof(Controller))]
    public class Hack : MonoBehaviour
    {
        [Tooltip("The layer that the hackable objects are on")]
        [SerializeField] private LayerMask _hackableLayer;

        private Controller _controller;

        [Header("Quick Time Event")]
        private bool _hackResult;
        [SerializeField] private float _hackDuration;
        [SerializeField] private int _buttonAmount;
        private Coroutine _coroutine;

        [Header("Animation")]
        [SerializeField] private GameObject _hackLinePrefab;
        [SerializeField] private float _hackLineDuration = 0.5f;

        private void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            var hackPoint = _controller.Input.GetHackInput();
            if (hackPoint.HasValue)
            {
                HackAction(hackPoint.Value);
            }
        }

        private void HackAction(Vector2 hackPoint)
        {
            
            // Find on hack point the enemy which is hackable 
            Collider2D? target = Physics2D.OverlapPoint(hackPoint, _hackableLayer);
            if (target == null)
            {
                return;
            }
            target.TryGetComponent(out Health health);
            if (!(target.CompareTag("Enemy") && health && health.IsHackable()))
            {
                Debug.Log("Can't hack", target);
                return;
            }
            Debug.Log("Hack " + target.name);
            SoundManager.Instance.PlayHack();
            _coroutine = StartCoroutine(HackQuickTimeEvent(target.gameObject));
        }

        IEnumerator HackQuickTimeEvent(GameObject target)
        {
            Hack enemyHack = target.GetComponent<Hack>();
            HackEventManager.Instance.StartHack(enemyHack._hackDuration, enemyHack._buttonAmount);
            yield return new WaitUntil(HackEventManager.Instance.HackQuickTimeEvent);
            _hackResult = HackEventManager.Instance.EndHack();
            if (_hackResult)
            {
                // Override the target's input with the hacker's input
                var targetController = target.GetComponent<Controller>();
                targetController.Input = _controller.Input;

                // Remove hack effect
                var effect = target.GetComponentInChildren<Effect.Hack>();
                if (effect != null)
                {
                    effect.gameObject.SetActive(false);
                }

                // Remove the hacker (player)
                _controller.Input = null;

                // Take Control; changing the target's tag to "Player" and set it as the player
                target.gameObject.tag = "Player";
                PlayerManager.Instance.Player = target.gameObject;

                //Reset target robot's health
                target.TryGetComponent(out Health health);
                health.ResetHealth();

                // Destroy the previous body
                Destroy(gameObject);
            }
        }

        IEnumerator HackLineAnimation(Vector2 from, Vector2 to)
        {
            var line = Instantiate(_hackLinePrefab, from, Quaternion.identity);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
            yield return new WaitForSeconds(_hackLineDuration);
            Destroy(line);
        }
    }
}