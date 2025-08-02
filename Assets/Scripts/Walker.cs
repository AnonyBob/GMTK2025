using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Statuses;
using UnityEngine;

namespace DefaultNamespace
{
    public class StatusInstance
    {
        public Status Status;
        public float RemainingTime;
        public Item FromItem;

        public StatusInstance(Status status, float duration, Item addedByItem = null)
        {
            Status = status;
            RemainingTime = duration;
            FromItem = addedByItem;
        }
    }
    
    public class Walker : MonoBehaviour
    {
        public Vector2 TargetPosition;
        public float SplinePosition;
        public float Happiness = 100;
        public float Speed = 1f;
        public float AttackThreshold = 50f;
        public float SpawnThreshold = 70f;
        
        public float TimeSinceHappinessUpdate;
        public float HappinessUpdateAmount = -1f;

        [field: SerializeField]
        public Transform DooberAnchor { get; private set;  }

        [SerializeField]
        private Transform[] _itemAnchors;
        
        [SerializeField]
        private SpriteRenderer _face;

        [SerializeField]
        private Color[] _skinColors;

        [SerializeField]
        private Color[] _hairGradient;

        [SerializeField]
        private Color[] _jacketGradient;

        [SerializeField]
        private Color[] _tieGradient;
        
        [SerializeField]
        private SpriteRenderer[] _skin;

        [SerializeField]
        private SpriteRenderer[] _ties;
        
        [SerializeField]
        private SpriteRenderer[] _suit;
        
        [SerializeField]
        private SpriteRenderer[] _hair;
        
        [SerializeField]
        private Color _angryColor = Color.red;
        
        public Dictionary<ItemType, List<Item>> Items = new ();
        public List<StatusInstance> Statuses = new List<StatusInstance>();
        private Color _startingFaceColor;

        private WalkerManager _manager;
        private int _itemAnchorIndex;

        private void Start()
        {
            RandomizeAppearance();
            AddToManager();
            SetAngryFaceAmount(100);
        }

        private void OnDestroy()
        {
            RemoveFromManager();
        }

        private void RandomizeAppearance()
        {
            var skinColor = _skinColors[Random.Range(0, _skinColors.Length)];
            foreach(var skin in _skin) {
                skin.color = skinColor;
            }

            var suitColor = _jacketGradient[Random.Range(0, _jacketGradient.Length)];
            foreach(var suit in _suit) {
                suit.color = suitColor;
            }
            
            var tieColor = _tieGradient[Random.Range(0, _tieGradient.Length)];
            foreach(var tie in _ties) {
                tie.color = tieColor;
            }
            
            var hairColor = _hairGradient[Random.Range(0, _hairGradient.Length)];
            foreach(var hair in _hair) {
                hair.color = hairColor;
            }
            
            _startingFaceColor = skinColor;
        }

        private void AddToManager()
        {
            _manager = GetComponentInParent<WalkerManager>();
            _manager.RegisterWalker(this);
        }

        private void RemoveFromManager()
        {
            if(_manager != null)
            {
                _manager.UnregisterWalker(this);
                _manager = null;
            }
        }

        public void AddHappiness(float happinessPerHit)
        {
            SetHappiness(Happiness + happinessPerHit);
        }

        public void AddItem(ItemType item)
        {
            if (!Items.TryGetValue(item, out var list)) {
                list = new List<Item>();
                Items[item] = list;
            }

            var itemPrefab = item.prefab;
            var anchor = GetNextItemAnchor();
            var newItem = Instantiate(itemPrefab, anchor.transform.position, Quaternion.identity, anchor);
            list.Add(newItem);
            
            if (newItem.Statuses is { Length: > 0 }) {
                foreach(var status in newItem.Statuses) {
                    AddStatus(status, newItem);
                }
            }
        }

        private Transform GetNextItemAnchor()
        {
            _itemAnchorIndex = (_itemAnchorIndex + 1) % _itemAnchors.Length;
            return _itemAnchors[_itemAnchorIndex];
        }

        public void RemoveItem(ItemType item)
        {
            if (!Items.TryGetValue(item, out var list) || list.Count == 0)
                return;
            
            // Remove the first item of the specified type
            var itemToRemove = list[0];
            list.RemoveAt(0);
            if (itemToRemove.Statuses is { Length: > 0 }) {
                foreach(var status in itemToRemove.Statuses) {
                    RemoveStatus(status, itemToRemove);
                }
            }
            
            Destroy(itemToRemove.gameObject);
        }

        public void RemoveItem(Item item)
        {
            if (!Items.TryGetValue(item.Type, out var list)) {
                return;
            }
            
            if (list.Remove(item)) {
                if (item.Statuses is { Length: > 0 }) {
                    foreach(var status in item.Statuses) {
                        RemoveStatus(status, item);
                    }
                }
                
                Destroy(item.gameObject);
            }
        }
        
        public bool HasItem(ItemType item)
        {
            return Items.TryGetValue(item, out var list) && list.Count > 0;
        }

        public void AddStatus(Status status, Item fromItem)
        {
            var instance = new StatusInstance(status, status.Duration, fromItem);
            status.OnAdded(this, instance);
            Statuses.Add(instance);
        }
        
        public bool HasStatus(Status status)
        {
            return Statuses.Any(s => s.Status == status);
        }
        
        public void RemoveStatus(Status status, Item fromItem = null)
        {
            for(var i = 0; i < Statuses.Count; i++)
            {
                if (Statuses[i].Status == status && (fromItem == null || Statuses[i].FromItem == fromItem)) {
                    var statusInstance = Statuses[i];
                    Statuses.RemoveAt(i);
                    statusInstance.Status.OnRemoved(this, statusInstance);
                    
                    return;
                }
            }
        }

        public (float MoneyMultiplier, float HappinessMultiplier) GetMotivation(float happinessPerHit)
        {
            var moneyMultiplier = 1f;
            var happinessMultiplier = 1f;
            foreach (var status in Statuses) {
                if (status.Status is MotivationStatus motivationStatus) {
                    moneyMultiplier *= motivationStatus.WorkMultiplier;
                    if (happinessPerHit > 0 || !motivationStatus.HappinessOnlyWhenPositive) {
                        happinessMultiplier *= motivationStatus.HappinessMultiplier;    
                    }
                }
            }
            
            return (moneyMultiplier, happinessMultiplier);
        }

        public void SetHappiness(float happiness)
        {
            var previousHappiness = Happiness;
            Happiness = happiness;
            SetAngryFaceAmount(previousHappiness);
        }

        private void SetAngryFaceAmount(float previousHappiness)
        {
            var angerAmount = 1f - Mathf.Abs(AttackThreshold - Happiness) / AttackThreshold;
            _face.color = Color.Lerp(_startingFaceColor, _angryColor, angerAmount);
            if (previousHappiness > AttackThreshold && Happiness <= AttackThreshold) {
            }
        }
    }
}