using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Elementos RB y Collider
    Rigidbody2D rb;
    CapsuleCollider2D hitBox;
    Animator anim;
    public BoxCollider2D groundDetection;
    // Direccion de movimiento
    float moveX;
    float moveY;
    bool jumpPressed;
    // Variables de ejecucion
    [Header("Fuerzas")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f;
    // Tipo de suelo pisado
    SueloPisado sueloPisado;
    // Direccion de personaje
    DireccionPersonaje direccionPersonaje;
    // Estados del jugador
    EstadoPlayer estadoPlayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitBox = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        direccionPersonaje = DireccionPersonaje.derecha;
    }

    void Update()
    {
        // Izquierda -1 | Derecha +1
        moveX = Input.GetAxisRaw("Horizontal");
        // Abajo -1 | Arriba +1
        moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        if (Input.GetKeyDown(KeyCode.J))
            AnimacionAtaque();

        if (Input.GetKeyDown(KeyCode.U))
            AnimacionAtaqueRun();
    }

    void FixedUpdate()
    {
        // Movimiento X
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // Movimiento Y
        if (jumpPressed && sueloPisado != SueloPisado.Aire)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }

        // Cambio direccion
        if (moveX > 0.1f && direccionPersonaje == DireccionPersonaje.izquierda)
        {
            GirarPersonaje(DireccionPersonaje.derecha);
        }
        else if (moveX < -0.1f && direccionPersonaje == DireccionPersonaje.derecha)
        {
            GirarPersonaje(DireccionPersonaje.izquierda);
        }

        CambiosDeAnimaciones();
    }

    public void TipoDeSueloPisado(string suelo)
    {
        // Convertimos y asignamos el suelo de String a enum
        sueloPisado = (SueloPisado)System.Enum.Parse(typeof(SueloPisado), suelo);
    }
    void GirarPersonaje(DireccionPersonaje nuevaDireccion)
    {
        direccionPersonaje = nuevaDireccion;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
    void CambiosDeAnimaciones()
    {
        AnimacionCorrer();
        AnimacionSaltoCaida();
    }
    void AnimacionCorrer()
    {
        if (moveX != 0)
        {
            anim.SetBool("Run", true);
            hitBox.size = new Vector2(hitBox.size.x, 0.24f);
            groundDetection.offset = new Vector2(groundDetection.offset.x, -0.14f);
            estadoPlayer = EstadoPlayer.Run;
        }
        else
        {
            anim.SetBool("Run", false);
            hitBox.size = new Vector2(hitBox.size.x, 0.29f);
            groundDetection.offset = new Vector2(groundDetection.offset.x, -0.17f);
        }
    }
    void AnimacionSaltoCaida()
    {
        anim.SetBool("Jump", sueloPisado == SueloPisado.Aire);
        estadoPlayer = EstadoPlayer.Jump;
    }
    void AnimacionAtaque()
    {
        anim.SetTrigger("Attack");
        estadoPlayer = EstadoPlayer.Attack;
    }
    public void AnimacionAtaqueOff()
    {
        Debug.Log("Ataque Finalizado");
        anim.SetBool("Attack", false);
    }
    void AnimacionAtaqueRun()
    {
        anim.SetTrigger("AttackRun");
        estadoPlayer = EstadoPlayer.Attack;
    }
    public void AnimacionAttaqueRunOff()
    {
        anim.SetBool("AttackRun", false);
        //AttackRun
    }
}