using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Elementos RB y Collider
    Rigidbody2D rb;
    CapsuleCollider2D hitBox;
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
    Animator anim;

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
    }
    void AnimacionCorrer()
    {
        if (moveX != 0)
        {
            anim.SetBool("Run", true);
            hitBox.size = new Vector2(hitBox.size.x, 0.24f);
            groundDetection.offset = new Vector2(groundDetection.offset.x, -0.14f);
        }
        else
        {
            anim.SetBool("Run", false);
            hitBox.size = new Vector2(hitBox.size.x, 0.29f);
            groundDetection.offset = new Vector2(groundDetection.offset.x, -0.17f);
        }
    }
}