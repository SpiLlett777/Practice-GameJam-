using UnityEngine;

public class Flies : MonoBehaviour
{
    public AudioClip rustleSound;

    private AudioSource audioSource;
    private bool isPlayed = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (rustleSound != null && audioSource != null)
        {
            audioSource.clip = rustleSound;
            audioSource.loop = false; 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("������ ������ ������");
            if (audioSource != null && rustleSound != null && !isPlayed)
            {
                Debug.Log("������ ������ �����");
                audioSource.Play();
                isPlayed = true;
            }
        }
    }
}
