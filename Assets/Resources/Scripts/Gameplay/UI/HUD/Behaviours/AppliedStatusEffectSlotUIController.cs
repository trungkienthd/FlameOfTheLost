using UnityEngine;
using UnityEngine.UI;

namespace FlameOfTheLost
{
    public class AppliedStatusEffectSlotUIController : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        public Image IconImage => _icon;

        public void SetIcon(Sprite iconSprite)
        {
            if (this.IconImage == null) return;

            this.IconImage.sprite = iconSprite;
            this.IconImage.enabled = iconSprite != null;
        }
    }
}
