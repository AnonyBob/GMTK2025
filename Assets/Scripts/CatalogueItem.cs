using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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
        private TextMeshProUGUI _lockText;

        [SerializeField]
        private RectTransform _lockedTransform;
        
        [SerializeField]
        private RectTransform _notEnoughMoneyTransform;
        
        [SerializeField]
        private RectTransform _dragTarget;
        
        [SerializeField]
        private RectTransform _infoAnchor;
        
        private Vector2 _startingPositionMouse;
        private Vector2 _startingPosition;
        private Transform _originalParent;
        private Tower _tower;
        private bool _detached;
        private bool _dragging;

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
        
        public void SetTower(Tower tower)
        {
            _tower = tower;
            _icon.sprite = tower.Stats.Sprite;
            _cost.text = $"<sprite name=\"Money\"> {tower.Stats.Cost:N0}";
            _lockText.text = $"Unlocks at \n<sprite name=\"Money\"> {tower.Stats.UnlockCost:N0}";
            
            UpdateLockedAndPurchasable();
        }

        private void Update()
        {
            if (_tower != null) {
                UpdateLockedAndPurchasable();
            }
        }

        private void UpdateLockedAndPurchasable()
        {
            var unlocked = Machine.CheckUnlocked(_tower);
            _lockedTransform.gameObject.SetActive(!unlocked);
            _cost.gameObject.SetActive(unlocked);
            _notEnoughMoneyTransform.gameObject.SetActive(!Machine.CanAfford(_tower) && unlocked);
        }

        public void SetSelected()
        {
            if(Machine.CheckUnlocked(_tower) && Machine.CanAfford(_tower)) {
                Catalogue.ActivateTower(_tower);
                InfoScreen.Hide();
            }
        }

        public void ShowInfo()
        {
            if (InfoScreen.CurrentTower == _tower) {
                InfoScreen.Hide();
                return;
            }
            InfoScreen.Show(_tower, _infoAnchor, fromUI: true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnBeginDrag(eventData);
            // if (!Machine.CheckUnlocked(_tower) || !Machine.CanAfford(_tower)) {
            //     return;
            // }
            //
            // _dragging = true;
            // _startingPositionMouse = eventData.position;
            // _originalParent = _dragTarget.transform.parent;
            // _startingPosition = _dragTarget.anchoredPosition;
            // _dragTarget.SetParent(Catalogue.DragParent);
        }

        public void OnDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnDrag(eventData);
            // if (!Machine.CheckUnlocked(_tower) || _detached || !_dragging) {
            //     return;
            // }
            //
            // if(!Machine.CanAfford(_tower)) {
            //     ResetDragging();
            //     return;
            // }
            //
            // var delta = eventData.position - _startingPositionMouse;
            // if (delta.y > Catalogue.ActivateThreshold) {
            //     Catalogue.ActivateTower(_tower);
            //     _detached = true;
            //     ResetDragging();
            //     return;
            // }
            //
            // var eventPosition = eventData.position;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //     Catalogue.DragParent,
            //     eventPosition,
            //     eventData.pressEventCamera,
            //     out Vector2 localPoint
            // );
            //
            // _dragTarget.anchoredPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            GetComponentInParent<ScrollRect>().OnEndDrag(eventData);
            // if (!Machine.CheckUnlocked(_tower) || !_dragging) {
            //     return;
            // }
            //
            // ResetDragging();
        }
        
        private void ResetDragging()
        {
            _dragging = false;
            if (_dragTarget != null) {
                _dragTarget.SetParent(_originalParent);
                _dragTarget.anchoredPosition = _startingPosition;
            }
        }
    }
}