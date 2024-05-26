using UnityEngine;

public class EndScript : MonoBehaviour
{
    public Camera mainCamera;
    public Color targetColor = Color.red;

    private bool hasReachedPoint = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasReachedPoint)
        {
            // Изменяем цвет фона камеры на целевой цвет
            mainCamera.backgroundColor = targetColor;
            hasReachedPoint = true;
        }
    }
}
