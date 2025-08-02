using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CatalogueItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TextMeshProUGUI _cost;

        [SerializeField]
        private RectTransform _dragTarget;
        
        private Vector2 _startingPositionMouse;
        private Vector2 _startingPosition;
        private Transform _originalParent;
        private Tower _tower;

        private Catalogue _catalogue;
        private Catalogue Catalogue
        {
            get
            {
                if (_catalogue == null) {
                    _catalogue = GetComponentInParent<Catalogue>();
                }

                return _catalogue;
            }
        }
        
        public void SetTower(Tower tower)
        {
            _tower = tower;
            _icon.sprite = tower.Stats.Sprite;
            _cost.text = $"<sprite name=\"Money\"> {tower.Stats.Cost:N0}";
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
            
            _startingPositionMouse = eventData.position;
            _originalParent = _dragTarget.transform.parent;
            _startingPosition = _dragTarget.anchoredPosition;
            _dragTarget.transform.parent = Catalogue.DragParent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnDrag(eventData);
            
            var delta = eventData.position - _startingPositionMouse;
            delta.x = 0;
            
            if (delta.y > Catalogue.ActivateThreshold) {
                Catalogue.ActivateTower(_tower);
                Destroy(_dragTarget.gameObject);
                return;
            }
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Catalogue.DragParent,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            _dragTarget.anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
            if (_dragTarget != null) {
                _dragTarget.parent =  _originalParent;
                _dragTarget.anchoredPosition = _startingPosition;
            }
        }
    }
}