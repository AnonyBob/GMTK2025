using UnityEngine;

namespace DefaultNamespace.Towers
{
    public class StatusTower : Tower
    {
        [SerializeField]
        private Status _status;

        [SerializeField]
        private bool _addStatus = true;
        
        protected override void HitTarget(Walker target)
        {
            if (_addStatus) {
                target.AddStatus(_status);    
            }
            else {
                target.RemoveStatus(_status);
            }
        }

        protected override bool CanHit(Walker target)
        {
            if (_addStatus) {
                return !target.HasStatus(_status);
            }

            return target.HasStatus(_status);
        }
    }
}