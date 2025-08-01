using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace DefaultNamespace
{
    public class WalkerManager : MonoBehaviour
    {
        [SerializeField]
        private SplineContainer _spline;

        [SerializeField]
        private int _startingWalkers = 10;

        [SerializeField]
        private float _timeBetweenSpawns = 1f;
        
        [SerializeField]
        private float _stepSize = 0.01f;
        
        [SerializeField]
        private float _positionRandomScale = 0.5f;
        
        [SerializeField]
        private float _happinessUpdateInterval = 2f;
        
        [SerializeField]
        private float _walkerSpawnChance = 0.5f;
        
        [SerializeField]
        private Transform _startPos;

        [SerializeField]
        private Walker _walkerPrefab;
        
        private readonly List<Walker> _walkers = new List<Walker>();

        private IEnumerator Start()
        {
            for(var i = 0; i < _startingWalkers; i++)
            {
                var walker = Instantiate(_walkerPrefab, _startPos.position, Quaternion.identity, transform);
                RegisterWalker(walker);
                yield return new WaitForSeconds(_timeBetweenSpawns);
            }
        }
        
        public void RegisterWalker(Walker walker)
        {
            if (walker == null || _walkers.Contains(walker)) return;
            _walkers.Add(walker);
            walker.TimeSinceHappinessUpdate = UnityEngine.Random.Range(0f, _happinessUpdateInterval);
            var startingPos = _spline.EvaluatePosition(0f);
            walker.transform.position = startingPos;
            walker.TargetPosition = (Vector3)startingPos;
        }
        
        public void UnregisterWalker(Walker walker)
        {
            if (walker == null || !_walkers.Contains(walker)) return;
            _walkers.Remove(walker);
        }
        
        private void Update()
        {
            foreach (var walker in _walkers) {
                UpdatePosition(walker);
                UpdateStatuses(walker);
                UpdateHappiness(walker);
            }
        }

        private void UpdatePosition(Walker walker)
        {
            if (Vector2.Distance(walker.transform.position, walker.TargetPosition) < 0.01f) {
                walker.SplinePosition += _stepSize;
                if (walker.SplinePosition > 1f) {
                    walker.SplinePosition -= 1f;
                }
                var position = (Vector3)_spline.EvaluatePosition(walker.SplinePosition) + (Vector3)(UnityEngine.Random.insideUnitCircle * _positionRandomScale);
                walker.TargetPosition = position;
            }
            
            walker.transform.position = Vector3.MoveTowards(walker.transform.position, walker.TargetPosition, walker.Speed * Time.deltaTime);
        }
        
        private void UpdateHappiness(Walker walker)
        {
            walker.TimeSinceHappinessUpdate += Time.deltaTime;
            if (walker.TimeSinceHappinessUpdate >= _happinessUpdateInterval) {
                walker.AddHappiness(walker.HappinessUpdateAmount);
                walker.TimeSinceHappinessUpdate = 0f;
            }
        }
        
        private void UpdateStatuses(Walker walker)
        {
            foreach (var status in walker.Statuses) {
                status.OnUpdate(walker, Time.deltaTime);
            }
        }

        public void SpawnWalker()
        {
            if (UnityEngine.Random.value <= _walkerSpawnChance) {
                Instantiate(_walkerPrefab, _startPos.position, Quaternion.identity, transform);
            }
        }
    }
}