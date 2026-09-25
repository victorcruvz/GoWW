using UnityEngine;

public class DetectionDownLimit : MonoBehaviour
{
    [Header("Otros Scrips")]
    [SerializeField] CameraFollow cameraFollow;

    void OnTriggerEnter2D(Collider2D collision)
    {
        cameraFollow.LimiteInferiorActual(collision.transform);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        cameraFollow.LimiteInferiorSalio(collision.transform);
    }
}
