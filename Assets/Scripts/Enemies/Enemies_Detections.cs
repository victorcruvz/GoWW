using UnityEngine;

public class Enemies_Detections : MonoBehaviour
{
    public bool isPlayerDetected { get; private set; }
    public Transform playerTransform { get; private set; }

    PlayerController player;

    void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    void Start()
    {
        isPlayerDetected = false;
    }

    void Update()
    {
        if (player != null)
        {
            if (player.estadoActualJugador != Estado.Normal)
            {
                //Debug.Log($"El estado es: {player.estadoActualJugador}");
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.estadoActualJugador == Estado.Normal)
            {
                isPlayerDetected = true;
                playerTransform = collision.transform;
            }
            else
            {
                isPlayerDetected = false;
                playerTransform = null;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (player.estadoActualJugador == Estado.Normal)
            {
                isPlayerDetected = true;
                playerTransform = collision.transform;
            }
            else
            {
                isPlayerDetected = false;
                playerTransform = null;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerDetected = false;
            playerTransform = null;
        }
    }
}