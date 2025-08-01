using UnityEngine;

namespace DefaultNamespace.Towers
{
    public class BasicTower : Tower
    {
        [SerializeField]
        private int _moneyPerHit = 2;
        
        [SerializeField]
        private float _happinessPerHit = -0.3f;
        
        protected override void HitTarget(Walker target)
        {
            Machine.AddMoney(_moneyPerHit);
            target.AddHappiness(_happinessPerHit);
        }
    }
}