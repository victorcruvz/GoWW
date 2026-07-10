using UnityEngine;

public class GroundStatus : MonoBehaviour
{
    public PlayerController playerController;

    void OnTriggerEnter2D(Collider2D other)
    {
        playerController.TipoDeSueloPisado(other.tag);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        playerController.TipoDeSueloPisado("Aire");
    }
}