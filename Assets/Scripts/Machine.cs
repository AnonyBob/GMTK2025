using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Machine : MonoBehaviour
    {
        [SerializeField]
        private WalkerManager _manager;
        
        public float Health = 100f;
        
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
    }
}