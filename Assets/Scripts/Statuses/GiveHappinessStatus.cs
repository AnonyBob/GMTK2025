using UnityEngine;

namespace DefaultNamespace.Statuses
{
    [CreateAssetMenu]
    public class GiveHappinessStatus : Status
    {
        [SerializeField]
        private float _happinessGiven;
        
        public override void OnAdded(Walker walker, StatusInstance instance)
        {
            walker.AddHappiness(_happinessGiven);
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