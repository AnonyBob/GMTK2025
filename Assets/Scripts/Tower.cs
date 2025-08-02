using System;
using System.Collections.Generic;
using DefaultNamespace.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
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
    
    public enum PlacementState
    {
        Placing,
        Placed,
        Selected
    }
    
    public class Tower : MonoBehaviour, IPointerClickHandler
    {
        [field: SerializeField]
        public TowerStats Stats { get; private set; }
        
        [field: SerializeField]
        public Tower Upgrade { get; private set; }

        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private int _attempts = 10;
        
        private readonly List<Walker> _targets = new List<Walker>();
        private float _timeSinceLastTick;
        private EffectPool _effectPool;
        private PlacementState _placementState;

        protected EffectPool EffectPool
        {
            get
            {
                if(_effectPool == null) {
                    _effectPool = FindFirstObjectByType<EffectPool>();
                }

                return _effectPool;
            }
        }

        public void PrepareForPlacing()
        {
            _placementState = PlacementState.Placing;
            _animator.SetBool("Placing", true);
            _animator.SetBool("Selected", false);
        }

        public void Place()
        {
            _placementState = PlacementState.Placed;
            _animator.SetBool("Placing", false);
            _animator.SetBool("Selected", false);
        }

        public void Select()
        {
            _placementState = PlacementState.Selected;
            _animator.SetBool("Placing", false);
            _animator.SetBool("Selected", true);
            
            //TODO: Show Options....
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_placementState == PlacementState.Placed) {
                Select();
            }
            else if (_placementState == PlacementState.Selected) {
                Place();
            }
        }
        
        private void Update()
        {
            if (_placementState == PlacementState.Placing) {
                return;
            }
            
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