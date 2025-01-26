using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Reference to the Throw Controller on the player")]
    [SerializeField] private ThrowController throwController;

    [Header("Offset for holding the item (optional)")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0.5f, 0f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        // If we already have an item in the ThrowController, don't pick up a new one
        if (throwController.objectToThrow != null) return;

        // Check if the collided object is a pickup item
        if (other.CompareTag("PickupItem"))
        {
            // Grab its Rigidbody
            Rigidbody itemRb = other.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                //Make item kinematic so it doesn't get tossed around by physics while held
                itemRb.isKinematic = true;

                // Parent the item to the player
                other.transform.SetParent(this.transform);
                other.transform.localPosition = holdOffset;
                other.transform.localRotation = Quaternion.identity;

                // ----> SET THE ITEM'S LAYER TO "HeldItem" <----
                int heldItemLayer = LayerMask.NameToLayer("HeldItem");
                other.gameObject.layer = heldItemLayer;

                //Assign this item to the ThrowController for throwing
                throwController.objectToThrow = itemRb;
            }
        }
    }
}
