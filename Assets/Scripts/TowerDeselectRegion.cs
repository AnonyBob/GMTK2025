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
        
        private InfoScreen _infoScreen;
        private InfoScreen InfoScreen
        {
            get
            {
                if (_infoScreen == null) {
                    _infoScreen = FindFirstObjectByType<InfoScreen>();
                }

                return _infoScreen;
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Placer.SetSelectedTower(null);
            InfoScreen.Hide();
        }
    }
}