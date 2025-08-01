using UnityEngine;

namespace DefaultNamespace.Towers
{
    public class BasicTower : Tower
    {
        [SerializeField]
        private int _moneyPerHit = 2;
        
        [SerializeField]
        private float _happinessPerHit = -0.3f;

        [SerializeField]
        private WalkerToTowerEffect _effect;
        
        protected override void HitTarget(Walker target)
        {
            var effect = (WalkerToTowerEffect)EffectPool.GetEffect(_effect);
            EffectPool.RunEffect(effect, new WalkerToTowerEffectData() {
                Walker = target,
                Tower = this,
                StartAtWalker = true,
                OnComplete = HandleComplete
            });
        }

        private void HandleComplete(Walker target, Tower tower)
        {
            var (money, happiness) = target.GetMotivation();
            Machine.AddMoney(Mathf.FloorToInt(money * _moneyPerHit));
            target.AddHappiness(_happinessPerHit * happiness);
        }
    }
}