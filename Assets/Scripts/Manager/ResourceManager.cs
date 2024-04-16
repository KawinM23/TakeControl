using Assets.Scripts.SaveLoad;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Manager
{
    public class ResourceManager : MonoBehaviour, IDataPersist
    {
        public static ResourceManager Instance { get; private set; }

        [SerializeField] private int _currency;
        public static event UnityAction<int> OnCurrenyChange;

        [SerializeField] private int _bombCurrencyCost;
        [SerializeField] private float _bombCooldown;
        private float _bombCooldownTimer;

        public void LoadData(in GameData data)
        {
            SetCurrency(data.currency);
        }

        public void SaveData(ref GameData data)
        {
            data.currency = _currency;
        }

        void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            _bombCooldownTimer = 0;
        }

        private void Update()
        {
            if (_bombCooldownTimer > 0)
            {
                _bombCooldownTimer -= Time.deltaTime;
            }
        }

        public int GetCurrency() { return _currency; }
        public void SetCurrency(int value) { _currency = value; OnCurrenyChange?.Invoke(value); }

        public void AddCurrency(int value) { SetCurrency(_currency + value); }

        public int GetBombCount()
        {
            return _currency / _bombCurrencyCost;
        }

        public bool UseBomb()
        {
            bool canUse = _currency > _bombCurrencyCost && _bombCooldownTimer <= 0;
            if (canUse)
            {
                SetCurrency(_currency - _bombCurrencyCost);
                _bombCooldownTimer = _bombCooldown;
            }
            return canUse;
        }

        public float GetCurrencyPercentage()
        {
            return ((((_currency % _bombCurrencyCost) + _bombCurrencyCost) % _bombCurrencyCost)) / (float)_bombCurrencyCost;
        }


    }
}