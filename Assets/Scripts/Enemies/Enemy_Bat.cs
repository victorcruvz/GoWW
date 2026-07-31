using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy_Bat : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    CapsuleCollider2D hitBox;

    int vidas = 2;

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
            Vector3 playerPosition = detector.playerTransform.position;
            playerPosition = new Vector3(playerPosition.x-0.6f, playerPosition.y+0.1f, playerPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, speed * Time.deltaTime);
            detectionEffect.SetActive(true);
        }
        else
        {
            detectionEffect.SetActive(false);
        }
    }
}
//X = 0.2 | Y = 0.1