using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] private string stationName;
    [SerializeField] private int clipIndex;
    [SerializeField] private ScienceAgent agent;

    private void Start()
    {
        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnXRSelect);
    }

    private void OnXRSelect(SelectEnterEventArgs args)
    {
        OnObjectClicked();
    }

    public void OnObjectClicked()
    {
        Debug.Log("[ClickableObject] Clicked: " + gameObject.name);
        if (agent != null)
            agent.WalkToStationWithClip(stationName, clipIndex);
        else
            Debug.LogWarning("[ClickableObject] Agent not assigned on " + gameObject.name);
    }
}