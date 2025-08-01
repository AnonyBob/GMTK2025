using UnityEngine;

namespace DefaultNamespace.Towers
{
    public class ItemTower : Tower
    {
        [SerializeField]
        private ItemType _item;

        [SerializeField]
        private bool _addItem = true;
        
        protected override void HitTarget(Walker target)
        {
            if (_addItem) {
                target.AddItem(_item);    
            }
            else {
                target.RemoveItem(_item);
            }
        }

        protected override bool CanHit(Walker target)
        {
            if (_addItem) {
                return !target.HasItem(_item);
            }

            return target.HasItem(_item);
        }
    }
}