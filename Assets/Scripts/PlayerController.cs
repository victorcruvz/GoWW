using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Elementos RB y Collider
    Rigidbody2D rb;
    Collider2D hitBox;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitBox = GetComponent<Collider2D>();
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
    }

    public void TipoDeSueloPisado(string suelo)
    {
        // Convertimos y asignamos el suelo de String a enum
        sueloPisado = (SueloPisado)System.Enum.Parse(typeof(SueloPisado), suelo);
    }
}