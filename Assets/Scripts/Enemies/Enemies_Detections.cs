using UnityEngine;

public class Enemies_Detections : MonoBehaviour
{
    public bool isPlayerDetected { get; private set; }
    public Transform playerTransform { get; private set; }

    private void Start()
    {
        isPlayerDetected = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerDetected = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerDetected = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerDetected = false;
            playerTransform = null;
        }
    }
}