using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class TowerDeselectRegion : MonoBehaviour, IPointerClickHandler
    {
        private TowerPlacer _placer;
        public TowerPlacer Placer
        {
            get
            {
                if(_placer == null)
                {
                    _placer = GetComponentInParent<TowerPlacer>();
                }

                return _placer;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Placer.SetSelectedTower(null);
        }
    }
}