using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        private List<Tower> _towers;
        
        public float Health = 100f;
        public int Money = 100;
        
        private static Machine Instance;
        
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
            if (walker.Happiness >= walker.SpawnThreshold) {
                AttemptToSpawnNewWalker(walker);
            }
            
            AddMoney(_moneyPerHit);
        }

        private void Attack()
        {
            Health--;
            if (Health <= 0f) {
                //End Game.
            }
        }
        
        private void AttemptToSpawnNewWalker(Walker walker)
        {
            if (_manager == null)
                return;

            _manager.SpawnWalker(walker);
        }

        public static void AddMoney(int moneyPerHit)
        {
            Instance.Money += moneyPerHit;
            Instance._moneyText.text = $"<sprite name=\"Money\"> {Instance.Money:N0}";
        }

        public static int GetMoney()
        {
            return Instance.Money;
        }

        public static List<Tower> GetTowers()
        {
            return Instance._towers;
        }
    }
}