using System;
using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class InventoryNavigationButtonUIController : MonoBehaviour
    {
        [SerializeField] private ItemType _category;
        public ItemType Category => _category;

        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonBackground;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _selectedSprite;
        private Button Button => _button;
        private Image ButtonBackground => _buttonBackground;
        private Sprite NormalSprite => _normalSprite;
        private Sprite SelectedSprite => _selectedSprite;

        public event Action<InventoryNavigationButtonUIController> OnClicked;

        private void Awake()
        {
            if (this.Button == null) _button = this.GetComponent<Button>();
            if (this.Button != null) this.Button.onClick.AddListener(this.HandleClick);

            if (this.ButtonBackground == null) _buttonBackground = this.GetComponent<Image>();
            if (this.NormalSprite == null) _normalSprite = SharedSpritesUIUtility.GetSprite("NRBGUI_Button_Rectangle_3");
            if (this.SelectedSprite == null) _selectedSprite = SharedSpritesUIUtility.GetSprite("NRBGUI_Button_Rectangle_3_Selected");
        }
        private void OnDestroy()
        {
            if (this.Button != null) this.Button.onClick.RemoveListener(this.HandleClick);
        }

        public void SetSelected(bool selected)
        {
            if (this.ButtonBackground == null) return;
            this.ButtonBackground.sprite = selected ? this.SelectedSprite : this.NormalSprite;
        }

        public void SetButtonInteractable(bool interactable)
        {
            if (this.Button == null) return;
            this.Button.interactable = interactable;
        }

        private void HandleClick()
        {
            this.OnClicked?.Invoke(this);
        }
    }
}
