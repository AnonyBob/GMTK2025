using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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

        //public Dictionary<ItemType, List<Item>> Items = new List<Item>();
        public List<Status> Statuses = new List<Status>();

        private WalkerManager _manager;
        

        private void Start()
        {
            RandomizeAppearance();
            AddToManager();
        }

        private void OnDestroy()
        {
            RemoveFromManager();
        }

        private void RandomizeAppearance()
        {
            
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
            Happiness += happinessPerHit;
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
    }
}