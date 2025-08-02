using UnityEngine;

namespace DefaultNamespace.Statuses
{
    [CreateAssetMenu]
    public class SpeedStatus : Status
    {
        [SerializeField]
        private float _speedAmount = 1.5f;
        
        public override void OnAdded(Walker walker, StatusInstance instance)
        {
            walker.Speed *= _speedAmount;
        }

        public override void OnUpdate(Walker walker, StatusInstance instance, float deltaTime)
        {
            
        }

        public override void OnRemoved(Walker walker, StatusInstance instance)
        {
            walker.Speed /= _speedAmount;
            if (instance.FromItem != null) {
                walker.RemoveItem(instance.FromItem);
            }
        }
    }
}