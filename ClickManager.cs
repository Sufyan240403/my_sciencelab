using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ClickableObject obj = hit.transform.GetComponent<ClickableObject>();
                if (obj != null)
                    obj.OnObjectClicked();
            }
        }
    }
}