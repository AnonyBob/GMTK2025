using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [System.Serializable]
    public struct TowerStats
    {
        public int Cost;
        public Sprite Sprite;
        
        public float TickRate;
        public string Blurb;
        public int RunCost;
    }
    
    public class Tower : MonoBehaviour
    {
        [field: SerializeField]
        public TowerStats Stats { get; private set; }
        
        [field: SerializeField]
        public Tower Upgrade { get; private set; }
        
        [SerializeField]
        private int _attempts = 10;
        
        private readonly List<Walker> _targets = new List<Walker>();
        private float _timeSinceLastTick;

        private void Update()
        {
            _timeSinceLastTick += Time.deltaTime;
            if (_timeSinceLastTick >= Stats.TickRate) {
                Fire();
                _timeSinceLastTick = 0f;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var walker = other.GetComponent<Walker>();
            if (walker != null && CanHit(walker)) {
                _targets.Add(walker);    
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            var walker = other.GetComponent<Walker>();
            if (walker != null) {
                _targets.Remove(walker);
            }
        }

        private void Fire()
        {
            if (Stats.RunCost > Machine.GetMoney()) return;
            
            if (_targets == null || _targets.Count == 0) return;
            for (var i = 0; i < _attempts; ++i) {
                var target = _targets[Random.Range(0, _targets.Count)];
                if (target != null && CanHit(target)) {
                    Machine.AddMoney(-Stats.RunCost);
                    HitTarget(target);
                    return;
                }
            }
        }

        protected virtual void HitTarget(Walker target)
        {
        }
        
        protected virtual bool CanHit(Walker target)
        {
            return true;
        }
    }
}