using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

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
        private float _happinessAddedOnSpawn = 5f;
        
        [SerializeField]
        private Transform _startPos;

        [SerializeField]
        private Walker _walkerPrefab;

        [SerializeField]
        private Image _happyFace;

        [SerializeField]
        private Image _sadFace;

        [SerializeField]
        private TextMeshProUGUI _satisfactionText;
        
        private readonly List<Walker> _walkers = new List<Walker>();
        private readonly List<StatusInstance> _statusesToRemove = new List<StatusInstance>();
        
        public int NumberOfWalkers => _walkers.Count;
        public float AverageHappiness { get; private set; }

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
            var happinessSum = 0f;
            foreach (var walker in _walkers) {
                UpdatePosition(walker);
                UpdateStatuses(walker);
                UpdateHappiness(walker);
                
                happinessSum += walker.Happiness;
            }

            AverageHappiness = happinessSum / _walkers.Count;
            _satisfactionText.text = (AverageHappiness / 100f).ToString("P0");
            var readyToAttack = Machine.BelowAttackThreshold(AverageHappiness);
            _happyFace.gameObject.SetActive(!readyToAttack);
            _sadFace.gameObject.SetActive(readyToAttack);
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
                status.Status.OnUpdate(walker, status, Time.deltaTime);
                status.RemainingTime -= Time.deltaTime;

                if (status.RemainingTime <= 0f && status.Status.Duration > 0) {
                    _statusesToRemove.Add(status);
                }
            }
            
            foreach (var status in _statusesToRemove) {
                walker.Statuses.Remove(status);
                status.Status.OnRemoved(walker, status);
            }
            _statusesToRemove.Clear();
        }

        public void SpawnWalker()
        {
            var instance = Instantiate(_walkerPrefab, _startPos.position, Quaternion.identity, transform);
            instance.SetHappiness(AverageHappiness + _happinessAddedOnSpawn);
        }
    }
}