using System;
using System.Collections.Generic;
using DefaultNamespace.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    [System.Serializable]
    public struct TowerStats
    {
        public string Name;
        public int Cost;
        public float TickRate;
        public int UnlockCost;
        public int RunCost;
        public int Life;
        
        public Sprite Sprite;
        [TextArea] public string Blurb;
        public bool SingleHit;
    }
    
    public enum PlacementState
    {
        Placing,
        Placed,
        Selected
    }
    
    public class Tower : MonoBehaviour, IPointerClickHandler
    {
        private static readonly int Placing = Animator.StringToHash("Placing");
        private static readonly int Selected = Animator.StringToHash("Selected");
        private static readonly int Invalid = Animator.StringToHash("Invalid");
        private static readonly int LifeFloat = Animator.StringToHash("Life");
        private static readonly int DeadTrigger = Animator.StringToHash("Dead");
        private static readonly int ActivateTrigger = Animator.StringToHash("Activate");

        [field: SerializeField]
        public TowerStats Stats { get; private set; }
        
        [field: SerializeField]
        public Tower Upgrade { get; private set; }

        [field: SerializeField]
        public Transform DooberAnchor { get; private set; }
        
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private BoxCollider2D _boxCollider;
        
        [SerializeField]
        private int _attempts = 10;

        [SerializeField]
        private LayerMask _collisionMask;
        
        [SerializeField]
        private Vector2 _finalOffsetOfCollider;
        
        [SerializeField]
        private Vector2 _finalSizeOfCollider;
        
        private TowerPlacer _placer;
        public TowerPlacer Placer
        {
            get
            {
                if(_placer == null)
                {
                    _placer = GetComponentInParent<TowerPlacer>();
                }

                return _placer;
            }
        }
        
        public int LifeRemaining => _lifeRemaining;
        
        private readonly List<Walker> _targets = new List<Walker>();
        private float _timeSinceLastTick;
        private EffectPool _effectPool;
        private PlacementState _placementState;
        private int _lifeRemaining;

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

        public virtual float HappinessAmount => 0;
        public virtual float MoneyAmount => 0;

        public void PrepareForPlacing()
        {
            gameObject.layer = LayerMask.NameToLayer("TowerPlacing");
            _placementState = PlacementState.Placing;
            _animator.SetBool(Placing, true);
            _animator.SetBool(Selected, false);

            _lifeRemaining = Stats.Life;
        }

        public bool CheckCanPlace(Vector3 position)
        {
            position += (Vector3)_boxCollider.offset;
            Collider2D hit = Physics2D.defaultPhysicsScene.OverlapBox(position, _boxCollider.size, 0, _collisionMask);
            var canPlace = hit == null;
            _animator.SetBool(Invalid, !canPlace);
            return canPlace;
        }

        public void Place(Vector3 position)
        {
            gameObject.layer = LayerMask.NameToLayer("Tower");
            transform.position = position;
            _placementState = PlacementState.Placed;
            
            _animator.SetBool(Placing, false);
            _animator.SetBool(Selected, false);
            _animator.SetBool(Invalid, false);

            _boxCollider.offset = _finalOffsetOfCollider;
            _boxCollider.size = _finalSizeOfCollider;
        }

        public void Select()
        {
            _placementState = PlacementState.Selected;
            _animator.SetBool(Placing, false);
            _animator.SetBool(Selected, true);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_placementState == PlacementState.Placed) {
                Placer.SetSelectedTower(this);
                Select();
                
                FindFirstObjectByType<InfoScreen>().Show(this, DooberAnchor);
            }
            else if (_placementState == PlacementState.Selected) {
                Place(transform.position);
                FindFirstObjectByType<InfoScreen>().Hide();
            }
        }

        public void Activate()
        {
            if (_animator != null) {
                _animator.SetTrigger(ActivateTrigger);    
            }
        }

        public void Die()
        {
            _animator.SetTrigger(DeadTrigger);
        }

        public void Remove()
        {
            Destroy(gameObject);
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
            if (Stats.RunCost > Machine.GetMoney() || (Stats.Life > 0 && _lifeRemaining <= 0)) return;
            
            if (_targets == null || _targets.Count == 0) return;
            for (var i = 0; i < _attempts; ++i) {
                var targetIndex = Random.Range(0, _targets.Count);
                var target = _targets[targetIndex];
                if (target != null && CanHit(target)) {
                    Machine.AddMoney(-Stats.RunCost);
                    HitTarget(target);

                    if (Stats.SingleHit) {
                        _targets.RemoveAt(targetIndex);
                    }

                    if (Stats.Life > 0) {
                        _lifeRemaining--;
                        UpdateLifeVisuals();
                    }
                    
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
        
        protected virtual void UpdateLifeVisuals()
        {
            if (_animator != null) {
                _animator.SetFloat(LifeFloat, (1f * _lifeRemaining) / Stats.Life);
            }

            if (_lifeRemaining <= 0) {
                Die();
            }
        }
    }
}