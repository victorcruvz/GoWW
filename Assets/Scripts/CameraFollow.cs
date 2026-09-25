using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform posicionX;

    [Header("Movimiento Y")]
    [SerializeField] float suavizadoY = 0.3f;
    [SerializeField] float offsetY = 6f;
    [SerializeField] float limiteCamaraY = 0;

    [Header("Límites X")]
    [SerializeField] float limiteCamaraIzquierdo;
    [SerializeField] float limiteCamaraDerecho;

    float velocidadY = 0;

    public void LimiteInferiorActual(Transform alturaLmiteY)
    {
        limiteCamaraY = alturaLmiteY.position.y;
    }

    public void LimiteInferiorSalio(Transform alturaLmiteY)
    {
        // Por ahora no hacemos nada
    }

    void Update()
    {
        // Posición X del jugador
        float nuevaX = posicionX.position.x;

        // Limitar X
        nuevaX = Mathf.Clamp(
            nuevaX,
            limiteCamaraIzquierdo,
            limiteCamaraDerecho
        );

        // Altura objetivo de la cámara
        float objetivoY = limiteCamaraY + offsetY;

        // Movimiento suavizado en Y
        float nuevaY = Mathf.SmoothDamp(
            transform.position.y,
            objetivoY,
            ref velocidadY,
            suavizadoY
        );

        transform.position = new Vector3(
            nuevaX,
            nuevaY,
            transform.position.z
        );
    }
}