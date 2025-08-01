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

        public StatusInstance(Status status, float duration)
        {
            Status = status;
            RemainingTime = duration;
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
            
        }
        
        public void RemoveItem(ItemType item)
        {
            // var existingItem = Items.FirstOrDefault(i => i.Type == item);
            // if (existingItem != null)
            // {
            //     Items.Remove(existingItem);
            // }
        }
        
        public bool HasItem(ItemType item)
        {
            return true;
            //return Items.Any(i => i.Type == item);
        }

        public void AddStatus(Status status)
        {
            Statuses.Add(new StatusInstance(status, status.Duration));
        }
        
        public bool HasStatus(Status status)
        {
            return Statuses.Any(s => s.Status == status);
        }
        
        public void RemoveStatus(Status status)
        {
            for(var i = 0; i < Statuses.Count; i++)
            {
                if (Statuses[i].Status == status) {
                    Statuses.RemoveAt(i);
                    return;
                }
            }
        }

        public (float MoneyMultiplier, float HappinessMultiplier) GetMotivation()
        {
            var moneyMultiplier = 1f;
            var happinessMultiplier = 1f;
            foreach (var status in Statuses)
            {
                if (status.Status is MotivationStatus motivationStatus)
                {
                    moneyMultiplier += motivationStatus.WorkMultiplier;
                    happinessMultiplier += motivationStatus.HappinessMultiplier;
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