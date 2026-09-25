using UnityEngine;

public class DamageRecived : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Damage"))
        {
            playerController.AtaqueRecivido();
        }
    }
}
