using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                PickupItem pickup = hit.collider.GetComponent<PickupItem>();
                if (pickup != null)
                {
                    pickup.PickUp();
                    return;
                }

                ElectricBox box = hit.collider.GetComponent<ElectricBox>();
                if (box != null)
                {
                    box.TryDisable();
                    return;
                }
            }
        }
    }
}
