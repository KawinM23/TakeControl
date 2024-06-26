﻿using Assets.Scripts.Combat;
using Assets.Scripts.Manager;
using System.Collections;
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
        [SerializeField] private bool _combo; // If enabled, hack event is continued until player fails
        [SerializeField] private float _comboHackDurationIncrement;
        [SerializeField] private int _comboButtonAmountIncrement;
        private Coroutine _coroutine;

        [Header("Pulse Bomb")]
        [SerializeField] private GameObject _pulsePrefab;

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

            if (Input.GetKeyDown(KeyCode.E) && gameObject == PlayerManager.Instance.Player && ResourceManager.Instance.UseBomb())
            {
                if (_pulsePrefab)
                {
                    GameObject go = Instantiate(_pulsePrefab, gameObject.transform.position, Quaternion.identity);
                }
            }
        }

        private void HackAction(Vector2 hackPoint)
        {
            if (HackEventManager.Instance.IsHacking)
            {
                return;
            }
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
            bool combo = target.GetComponent<Hack>()._combo;
            float comboHackDurationIncrement = enemyHack._comboHackDurationIncrement;
            int comboButtonAmountIncrement = enemyHack._comboButtonAmountIncrement;
            float hackDuration = enemyHack._hackDuration;
            int buttonAmount = enemyHack._buttonAmount;
            int successfulComboCount = 0;
            // if combo is enabled, the hack event is continued until player fails
            while (true)
            {
                HackEventManager.Instance.StartHack(hackDuration, buttonAmount);
                yield return new WaitUntil(HackEventManager.Instance.HackQuickTimeEvent);
                _hackResult = HackEventManager.Instance.EndHack();

                // stop if player pass normal hack or fails, stop event
                if ((_hackResult && !combo) || !_hackResult)
                {
                    break;
                }

                // Increase difficulty after each successful combo
                hackDuration += comboHackDurationIncrement;
                buttonAmount += comboButtonAmountIncrement;
                successfulComboCount++;
            }


            // If normal hack is successful or combo hack > 1 successful
            if ((_hackResult && !combo) || (combo && successfulComboCount > 0))
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
                target.gameObject.layer = gameObject.layer;
                int targetLayer = target.gameObject.layer;
                gameObject.tag = "Enemy";
                gameObject.layer = targetLayer;
                PlayerManager.Instance.Player = target.gameObject;

                //Reset target robot's health
                target.TryGetComponent(out Health health);
                health.ResetHealth();

                // Apply successful hack function if has one
                if (target.TryGetComponent(out HackCompletion hackCompletion))
                {
                    Debug.Log("Hack Success");
                    if (combo)
                    {
                        Debug.Log("Combo Hack Success");
                        hackCompletion.OnComboHackSuccess(successfulComboCount);
                    }
                    else
                    {
                        Debug.Log("Normal Hack Success");
                        hackCompletion.OnHackSuccess();
                    }
                }

                // Add count to enemy kill for BossManager
                BossManager.Instance.IncrementEnemyKillCount();

                // Destroy the previous body
                Destroy(gameObject);
            }
            // If hack failed
            else
            {
                // Apply fail hack function if has one
                if (target.TryGetComponent(out HackCompletion hackCompletion))
                {
                    if (combo)
                    {
                        hackCompletion.OnComboHackFail(successfulComboCount);
                    }
                    else
                    {
                        hackCompletion.OnHackFail();
                    }
                }
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