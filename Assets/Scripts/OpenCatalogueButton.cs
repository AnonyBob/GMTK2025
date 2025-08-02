using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class OpenCatalogueButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private Catalogue _catalogue;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_catalogue) {
                _catalogue.Open(Machine.GetTowers());    
            }
        }
    }
}