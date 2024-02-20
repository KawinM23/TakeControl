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

        private void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            var hackPoint = _controller.input.RetrieveHackInput();
            if (hackPoint.HasValue)
            {
                HackAction(hackPoint.Value);
            }
        }

        private void HackAction(Vector2 hackPoint)
        {
            Debug.Log("Hack");
            // Find on hack point the enemy which is hackable 
            Collider2D? target = Physics2D.OverlapPoint(hackPoint, _hackableLayer);
            if (target == null)
            {
                return;
            }
            if (!(target.TryGetComponent<Health>(out var health) && health.Hackable()))
            {
                Debug.Log(target);
                return;
            }

            // Override the target's input with the hacker's input
            var targetCtrl = target.GetComponent<Controller>();
            targetCtrl.input = _controller.input;

            // Remoove hack effect
            var effect = target.GetComponentInChildren<Effect.Hack>();
            if (effect != null)
            {
                effect.gameObject.SetActive(false);
            }

            // Remove the hacker
            _controller.input = null;
            target.gameObject.layer = LayerMask.NameToLayer("Player");
            Destroy(gameObject);
        }
    }
}