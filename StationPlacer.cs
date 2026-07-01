using TMPro;
using UnityEngine;

public class StationPlacer : MonoBehaviour
{
    [Header("Station Parent")]
    [SerializeField] private GameObject biologyStation;
    [SerializeField] private GameObject chemistryStation;
    [SerializeField] private GameObject physicsStation;
    [SerializeField] private GameObject spaceStation;

    [Header("Placement Settings")]
    [SerializeField] private float spread = 2.0f;
    [SerializeField] private float distanceFromCamera = 3.0f;

    [Header("UI")]
    [SerializeField] private GameObject tapToPlaceUi;
    [SerializeField] private TextMeshProUGUI instructionText;

    private bool isPlaced = false;
    private Camera mainCamera;

    private void Start()
    {
        // Find any active camera
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();

        // Show all stations by default in Editor
        SetStationsActive(true);

        if (tapToPlaceUi != null)
            tapToPlaceUi.SetActive(false);
    }

    private void Update()
    {
        if (isPlaced) return;
        if (mainCamera == null) return;

        bool tapped = Input.touchCount > 0 &&
                      Input.GetTouch(0).phase == TouchPhase.Began;
        bool clicked = Input.GetMouseButtonDown(0);

        if (tapped || clicked)
        {
            isPlaced = true;

            if (tapToPlaceUi != null)
                tapToPlaceUi.SetActive(false);

            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0;
            forward = forward.normalized;

            Vector3 center = mainCamera.transform.position +
                             forward * distanceFromCamera;
            center.y = 0;

            if (biologyStation != null)
            {
                biologyStation.transform.position =
                    center + new Vector3(-spread, 0, -spread);
                biologyStation.SetActive(true);
            }
            if (chemistryStation != null)
            {
                chemistryStation.transform.position =
                    center + new Vector3(spread, 0, -spread);
                chemistryStation.SetActive(true);
            }
            if (physicsStation != null)
            {
                physicsStation.transform.position =
                    center + new Vector3(-spread, 0, spread);
                physicsStation.SetActive(true);
            }
            if (spaceStation != null)
            {
                spaceStation.transform.position =
                    center + new Vector3(spread, 0, spread);
                spaceStation.SetActive(true);
            }
        }
    }

    private void SetStationsActive(bool active)
    {
        if (biologyStation != null) biologyStation.SetActive(active);
        if (chemistryStation != null) chemistryStation.SetActive(active);
        if (physicsStation != null) physicsStation.SetActive(active);
        if (spaceStation != null) spaceStation.SetActive(active);
    }
}