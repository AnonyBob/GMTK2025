using UnityEngine;

namespace DefaultNamespace.Statuses
{
    public enum HappinessType
    {
        Positive,
        Negative,
        Neutral
    }
    
    [CreateAssetMenu]
    public class MotivationStatus : Status
    {
        public float WorkMultiplier = 1.2f;
        public float HappinessMultiplier = 1.1f;
        public HappinessType HappinessType;
        
        public override void OnAdded(Walker walker, StatusInstance instance)
        {
        }

        public override void OnUpdate(Walker walker, StatusInstance instance, float deltaTime)
        {
            
        }

        public override void OnRemoved(Walker walker, StatusInstance instance)
        {
            if (instance.FromItem != null) {
                walker.RemoveItem(instance.FromItem);
            }
        }
    }
}