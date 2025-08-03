using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace.Towers
{
    public enum TowerType
    {
        Basic,
        Item,
        Status
    }
    
    public class BasicTower : Tower
    {
        public override float HappinessAmount => _happinessPerHit;
        public override float MoneyAmount => _moneyPerHit;
        
        [SerializeField]
        private TowerType _towerType;
        
        [SerializeField, ShowIf("@_towerType == TowerType.Basic")]
        private int _moneyPerHit = 2;
        
        [SerializeField, ShowIf("@_towerType == TowerType.Basic")]
        private float _happinessPerHit = -0.3f;

        [SerializeField, ShowIf("@_towerType == TowerType.Item")]
        private ItemType _itemGiven;
        
        [SerializeField, ShowIf("@_towerType == TowerType.Item")]
        private bool _ignoreIfTheyHaveItem;

        [SerializeField, ShowIf("@_towerType == TowerType.Status")]
        private Status _statusGiven;
        
        [SerializeField, ShowIf("@_towerType == TowerType.Status")]
        private bool _ignoreIfTheyHaveStatus;
        
        [SerializeField]
        private WalkerToTowerEffect _effect;
        
        [SerializeField]
        private bool _startAtWalker = true;
        
        protected override bool CanHit(Walker target)
        {
            switch (_towerType) {
                case TowerType.Basic:
                    return true;
                case TowerType.Item:
                    if (_ignoreIfTheyHaveItem) {
                        return !target.HasItem(_itemGiven);
                    }
                    return true;
                case TowerType.Status:
                    if (_ignoreIfTheyHaveStatus) {
                        return !target.HasStatus(_statusGiven);
                    }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected override void HitTarget(Walker target)
        {
            var effect = (WalkerToTowerEffect)EffectPool.GetEffect(_effect);
            EffectPool.RunEffect(effect, new WalkerToTowerEffectData() {
                Walker = target,
                Tower = this,
                StartAtWalker = _startAtWalker,
                OnComplete = HandleComplete
            });
        }

        private void HandleComplete(Walker target, Tower tower)
        {
            switch (_towerType) {
                case TowerType.Basic:
                    var (money, happiness) = target.GetMotivation(_happinessPerHit);
                    Machine.AddMoney(Mathf.FloorToInt(money * _moneyPerHit));
                    target.AddHappiness(_happinessPerHit * happiness);
                    break;
                case TowerType.Item:
                    if (_itemGiven != null) {
                        target.AddItem(_itemGiven);
                    }
                    break;
                case TowerType.Status:
                    if (_statusGiven != null) {
                        target.AddStatus(_statusGiven, null);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            tower.Activate();
        }
    }
}