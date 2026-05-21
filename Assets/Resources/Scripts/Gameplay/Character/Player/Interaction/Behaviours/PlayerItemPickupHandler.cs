using System.Collections.Generic;
using UnityEngine;

namespace FlameOfTheLost
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerItemPickupHandler : MonoBehaviour
    {
        ////  --- States ---
        ////  1. References
        //[Header("References")]
        //[SerializeField] private GameObject _playerRoot;
        //private GameObject PlayerRoot => _playerRoot;

        ////  2. Runtime
        //[Header("Runtime")]
        //[SerializeField] private List<PickupItemController> _nearbyPickupItemCtrls = new();
        //public IReadOnlyList<PickupItemController> NearbyPickupItemCtrls => _nearbyPickupItemCtrls;

        //// --- Unity Callbacks ---
        //private void Awake()
        //{
        //    this.CacheReferences();
        //}

        //private void OnTriggerEnter2D(Collider2D other)
        //{
        //    if (other.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;

        //    PickupItemController pickupItem = other.GetComponentInParent<PickupItemController>();
        //    if (pickupItem == null) return;
        //    if (_nearbyPickupItemCtrls.Contains(pickupItem)) return;

        //    _nearbyPickupItemCtrls.Add(pickupItem);
        //}

        //private void OnTriggerExit2D(Collider2D other)
        //{
        //    if (other.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;

        //    PickupItemController pickupItem = other.GetComponentInParent<PickupItemController>();
        //    if (pickupItem == null) return;

        //    _nearbyPickupItemCtrls.Remove(pickupItem);
        //}

        //public bool TryPickupNearestItem()
        //{
        //    this.RemoveInvalidPickupItems();

        //    PickupItemController nearestItem = this.FindNearestPickupItemCtrl();
        //    if (nearestItem == null) return false;

        //    GameObject interactor = this.PlayerRoot != null ? this.PlayerRoot : this.gameObject;

        //    bool pickedUp = nearestItem.TryInteract(interactor);
        //    if (pickedUp)
        //    {
        //        _nearbyPickupItemCtrls.Remove(nearestItem);
        //    }

        //    return pickedUp;
        //}

        //private PickupItemController FindNearestPickupItemCtrl()
        //{
        //    if (_nearbyPickupItemCtrls == null || this.NearbyPickupItemCtrls.Count == 0) return null;

        //    PickupItemController nearestItem = null;
        //    float nearestSqrDistance = float.MaxValue;

        //    Vector3 origin = this.PlayerRoot != null ? this.PlayerRoot.transform.position : this.transform.position;

        //    for (int i = 0; i < this.NearbyPickupItemCtrls.Count; i++)
        //    {
        //        PickupItemController pickupItem = this.NearbyPickupItemCtrls[i];
        //        if (pickupItem == null) continue;
        //        if (!pickupItem.gameObject.activeInHierarchy) continue;
        //        if (!pickupItem.CanInteract(this.PlayerRoot != null ? this.PlayerRoot : this.gameObject)) continue;

        //        float sqrDistance = (pickupItem.transform.position - origin).sqrMagnitude;
        //        if (sqrDistance >= nearestSqrDistance) continue;

        //        nearestSqrDistance = sqrDistance;
        //        nearestItem = pickupItem;
        //    }

        //    return nearestItem;
        //}

        //private void RemoveInvalidPickupItems()
        //{
        //    if (_nearbyPickupItemCtrls == null) return;

        //    for (int i = this.NearbyPickupItemCtrls.Count - 1; i >= 0; i--)
        //    {
        //        PickupItemController pickupItem = this.NearbyPickupItemCtrls[i];

        //        if (pickupItem == null || !pickupItem.gameObject.activeInHierarchy)
        //        {
        //            _nearbyPickupItemCtrls.RemoveAt(i);
        //        }
        //    }
        //}

        //private void CacheReferences()
        //{
        //    if (this.PlayerRoot != null) return;

        //    _playerRoot = this.transform.root.gameObject;
        //}
    }
}
