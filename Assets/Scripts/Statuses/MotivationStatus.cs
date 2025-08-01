using UnityEngine;

namespace DefaultNamespace.Statuses
{
    [CreateAssetMenu]
    public class MotivationStatus : Status
    {
        public float WorkMultiplier = 1.2f;
        public float HappinessMultiplier = 1.1f;
        
        public override void OnAdded(Walker walker)
        {
            
        }

        public override void OnUpdate(Walker walker, float deltaTime)
        {
            
        }

        public override void OnRemoved(Walker walker)
        {
            
        }
    }
}