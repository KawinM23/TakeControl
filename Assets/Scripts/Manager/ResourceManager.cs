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

        public void LoadData(in GameData data)
        {
            _currency = data.currency;
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

        public int GetCurrency() { return _currency; }
        public void SetCurrency(int value) { _currency = value; OnCurrenyChange?.Invoke(value); }

        public void AddCurrency(int value) { SetCurrency(_currency + value); }

        public int GetBombCount()
        {
            return _currency / _bombCurrencyCost;
        }

        public bool UseBomb()
        {
            bool canUse = _currency > _bombCurrencyCost;
            if (canUse)
            {
                SetCurrency(_currency - _bombCurrencyCost);
            }
            return canUse;
        }

        public float GetCurrencyPercentage()
        {
            return ((((_currency % _bombCurrencyCost) + _bombCurrencyCost) % _bombCurrencyCost)) / (float)_bombCurrencyCost;
        }


    }
}