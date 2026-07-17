using UnityEngine;

public class GroundStatus : MonoBehaviour
{
    public PlayerController playerController;
    private Collider2D miColisionador;
    public ContactFilter2D filtroSuelo;
    private Collider2D[] resultadosColision = new Collider2D[5];

    public bool wallDetection = false;

    void Start()
    {
        miColisionador = GetComponent<Collider2D>();
    }
    void FixedUpdate()
    {
        int cantidadDeColisiones = miColisionador.Overlap(filtroSuelo, resultadosColision);

        if (!wallDetection)
        {
            if (cantidadDeColisiones > 0)
            {
                string tagSuelo = resultadosColision[0].tag;
                playerController.TipoDeSueloPisado(tagSuelo);
            }
            else
                playerController.TipoDeSueloPisado("Aire");
        }
        else if (wallDetection)
        {
            if (cantidadDeColisiones > 0)
            {
                string tagSuelo = resultadosColision[0].tag;
                playerController.TipoDeMuroContacto(tagSuelo);
            }
            else
                playerController.TipoDeMuroContacto("Aire");
        }
    }
}