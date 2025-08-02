using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Machine : MonoBehaviour
    {
        [SerializeField]
        private WalkerManager _manager;

        [SerializeField] 
        private int _moneyPerHit = 5;

        [SerializeField]
        private TextMeshPro _moneyText;

        [SerializeField]
        private Button _hireButton;
        
        [SerializeField]
        private TextMeshProUGUI _walkerCostText;

        [SerializeField]
        private List<Tower> _towers;
        
        public float Health = 100f;
        public int Money = 100;
        
        [SerializeField]
        private AnimationCurve _costToSpawnWalkerMultiplier;
        
        private static Machine Instance;
        private int _highestMoney;
        private int _costToSpawnWalker;

        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            var money = Money;
            AddMoney(-money);
            AddMoney(money);
        }

        private void Update()
        {
            var count = _manager.NumberOfWalkers;
            _costToSpawnWalker = (int)_costToSpawnWalkerMultiplier.Evaluate(count);
            _walkerCostText.text = $"<sprite name=\"Money\"> {_costToSpawnWalker:N0}";
            _hireButton.interactable = Money >= _costToSpawnWalker;
        }

        private void OnDestroy()
        {
            if (Instance == this) {
                Instance = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var walker = other.GetComponent<Walker>();
            if (walker == null || walker.SplinePosition < 0.5f)
                return;

            if (walker.Happiness < walker.AttackThreshold) {
                Attack();
            }
            
            AddMoney(_moneyPerHit);
        }

        private void Attack()
        {
            Health--;
            if (Health <= 0f) {
               Debug.LogError("Game Over!");
            }
        }
        
        public void AttemptToSpawnNewWalker()
        {
            if (_manager == null)
                return;

            if (Money >= _costToSpawnWalker) {
                _manager.SpawnWalker();  
                AddMoney(-_costToSpawnWalker);
            }
            else {
                // Show not enough money...
            }
        }

        public static void AddMoney(int moneyPerHit)
        {
            Instance.Money += moneyPerHit;
            Instance._moneyText.text = $"<sprite name=\"Money\"> {Instance.Money:N0}";
            
            if(Instance.Money > Instance._highestMoney)
            {
                Instance._highestMoney = Instance.Money;
            }
        }

        public static int GetMoney()
        {
            return Instance.Money;
        }

        public static List<Tower> GetTowers()
        {
            return Instance._towers;
        }

        public static bool CheckUnlocked(Tower tower)
        {
            return Instance._highestMoney >= tower.Stats.UnlockCost;
        }

        public static bool CanAfford(Tower tower)
        {
            return Instance.Money >= tower.Stats.Cost;
        }
    }
}