using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Machine : MonoBehaviour
    {
        [SerializeField]
        private WalkerManager _manager;

        [SerializeField] 
        private int _moneyPerHit = 5;
        
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
                AttemptToSpawnNewWalker();
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
        
        private void AttemptToSpawnNewWalker()
        {
            if (_manager == null)
                return;

            _manager.SpawnWalker();
        }

        public static void AddMoney(int moneyPerHit)
        {
            Instance.Money += moneyPerHit;
        }

        public static int GetMoney()
        {
            return Instance.Money;
        }
    }
}