using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy_Bat : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    CapsuleCollider2D hitBox;

    int vidas = 2;
    bool miraHaciaDerecha = true;

    [Header("Otros Scrips")]
    [SerializeField] Enemies_Detections detector;
    [Header("Fuerzas")]
    [SerializeField] float speed = 5f;
    [Header("Componentes")]
    [SerializeField] GameObject detectionEffect;

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
        SeguirJugador();
    }

    void SeguirJugador()
    {
        if (detector != null && detector.isPlayerDetected && detector.playerTransform != null)
        {
            Vector3 targetPos = detector.playerTransform.position;
            // Orientar en X
            GirarHaciaObjetivo(targetPos.x);
            float offsetX = (targetPos.x > transform.position.x) ? -0.6f : 0.6f;
            Vector3 playerPosition = new Vector3(targetPos.x + offsetX, targetPos.y + 0.1f, targetPos.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
            detectionEffect.SetActive(true);
        }
        else
        {
            if (detectionEffect != null)
                detectionEffect.SetActive(false);
        }
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
}