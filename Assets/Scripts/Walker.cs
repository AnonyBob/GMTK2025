using System;
using System.Collections.Generic;
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

        public List<Item> Items = new List<Item>();
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
    }
}