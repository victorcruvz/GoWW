using System.Collections;
using UnityEngine;

public class Enemy_Bat : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    CapsuleCollider2D hitBox;

    bool miraHaciaDerecha = true;
    bool persiguiendo = false;
    bool persecucionBloqueada = false;
    int puntoActual = 0;
    bool recibiendoAtaque = false;

    [Header("Cantidad Vidas")]
    [SerializeField] int vidas = 2;

    [Header("Otros Scrips")]
    [SerializeField] Enemies_Detections detector;
    [SerializeField] Enemy_Attacked enemyAttacked;

    [Header("Fuerzas")]
    [SerializeField] float speed = 5f;

    [Header("Componentes")]
    [SerializeField] GameObject detectionEffect;

    [Header("Patrullaje")]
    [SerializeField] Transform[] puntosPatrullaje;
    [SerializeField] float distanciaPunto = 0.1f;

    [Header("Límite de persecución")]
    [SerializeField] float limitePersecucionIzquierdo;
    [SerializeField] float limitePersecucionDerecho;

    [Header("Persecución")]
    [SerializeField] float distanciaConJugador = 0.32f;

    [Header("Daño recibido")]
    [SerializeField] float knockbackDuration = 0.18f;
    [SerializeField] Vector2 knockbackForce = new Vector2(2f, 1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitBox = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        vidas = 2;
    }

    void Update()
    {
        if (enemyAttacked.isAttacked)
        {
            //recibiendoAtaque = true;
            enemyAttacked.isAttacked = false;
            RecibeAtaque();
        }

        if (recibiendoAtaque)
            return;

        if (detector != null && detector.isPlayerDetected && detector.playerTransform != null)
        {
            if (!persecucionBloqueada)
            {
                persiguiendo = true;
            }
        }
        else
        {
            persecucionBloqueada = false;
            persiguiendo = false;
        }

        if (persiguiendo)
        {
            SeguirJugador();
        }
        else if (!persiguiendo)
        {
            RutaPatrullaje();
        }
    }


    void SeguirJugador()
    {
        Vector3 targetPos = detector.playerTransform.position;

        if (targetPos.x < limitePersecucionIzquierdo ||
            targetPos.x > limitePersecucionDerecho)
        {
            persiguiendo = false;
            persecucionBloqueada = true;

            if (detectionEffect != null)
                detectionEffect.SetActive(false);

            return;
        }

        // Orientar hacia el jugador
        GirarHaciaObjetivo(targetPos.x);

        float offsetX = (targetPos.x > transform.position.x) ? -distanciaConJugador : distanciaConJugador;

        Vector3 playerPosition = new Vector3(
            targetPos.x + offsetX,
            targetPos.y + 0.1f,
            targetPos.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            playerPosition,
            speed * Time.deltaTime
        );

        if (detectionEffect != null)
            detectionEffect.SetActive(true);
    }


    void RutaPatrullaje()
    {
        if (puntosPatrullaje == null || puntosPatrullaje.Length == 0)
            return;

        Transform objetivo = puntosPatrullaje[puntoActual];

        // Moverse hacia el punto
        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo.position,
            speed * Time.deltaTime
        );

        // Girar hacia el punto
        GirarHaciaObjetivo(objetivo.position.x);

        // ¿Llego al punto?
        if (Vector3.Distance(transform.position, objetivo.position) <= distanciaPunto)
        {
            puntoActual++;

            // Volver al primer punto al terminar la ruta
            if (puntoActual >= puntosPatrullaje.Length)
            {
                puntoActual = 0;
            }
        }

        if (detectionEffect != null)
            detectionEffect.SetActive(false);
    }


    void GirarHaciaObjetivo(float objetivoX)
    {
        Vector3 escala = transform.localScale;

        if (objetivoX > transform.position.x)
        {
            escala.x = Mathf.Abs(escala.x);
        }
        else if (objetivoX < transform.position.x)
        {
            escala.x = -Mathf.Abs(escala.x);
        }

        transform.localScale = escala;
    }

    void Muerte()
    {

    }

    public void Desaparicion()
    {

    }

    void RecibeAtaque()
    {
        if (vidas >= 1)
        {
            recibiendoAtaque = true;
            vidas--;
            StartCoroutine(KnockbackRoutine());
        }
    }
    IEnumerator KnockbackRoutine()
    {
        Debug.Log($"Recibio Daño al enemigo: {vidas}");

        float hitDirectionX =
            transform.position.x < enemyAttacked.direccionDamage.x
            ? -1f
            : 1f;

        Vector2 force = new Vector2(
            hitDirectionX * knockbackForce.x,
            knockbackForce.y
        );

        Vector3 posicionInicial = transform.position;

        Vector3 desplazamiento = new Vector3(
            force.x,
            force.y,
            0f
        );

        float tiempo = 0f;

        while (tiempo < knockbackDuration)
        {
            tiempo += Time.deltaTime;

            float progreso = tiempo / knockbackDuration;

            transform.position = posicionInicial + desplazamiento * progreso;

            yield return null;
        }

        recibiendoAtaque = false;
    }
}