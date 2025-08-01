using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
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
        public List<Status> Statuses = new List<Status>();
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
            Statuses.Add(status);
        }
        
        public bool HasStatus(Status status)
        {
            return Statuses.Contains(status);
        }
        
        public void RemoveStatus(Status status)
        {
            Statuses.Remove(status);
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