using UnityEngine;

public class ModelRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 45f;

    private void Update()
    {
        // Always spins automatically on Y axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}