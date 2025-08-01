using UnityEngine;

namespace DefaultNamespace.Statuses
{
    [CreateAssetMenu]
    public class SpeedStatus : Status
    {
        [SerializeField]
        private float _speedAmount = 1.5f;
        
        public override void OnAdded(Walker walker)
        {
            walker.Speed *= _speedAmount;
        }

        public override void OnUpdate(Walker walker, float deltaTime)
        {
            
        }

        public override void OnRemoved(Walker walker)
        {
            walker.Speed /= _speedAmount;
        }
    }
}