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
    DondeEsta dondeEsta;
    // Direccion de personaje
    DireccionPersonaje direccionPersonaje;
    // Estados de Movimiento
    MovimientoHoriz movimientoHoriz;
    MovimientoVert movimientoVert;
    // Estados del jugador
    Estado estado;
    // Estados que afectan al jugador
    Efectos efectos;
    // Estado de Combate
    Combate combate;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitBox = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
    }
    void Start()
    {
        direccionPersonaje = DireccionPersonaje.Derecha;
        movimientoHoriz = MovimientoHoriz.Quieto;
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

        if (Input.GetKeyDown(KeyCode.L))
            movimientoHoriz = MovimientoHoriz.Dash;
    }

    void FixedUpdate()
    {
        // Movimiento X
        if (moveX != 0 && movimientoHoriz != MovimientoHoriz.Dash)
        {
            rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
            movimientoHoriz = MovimientoHoriz.Avanzando;
        }
        else if (moveX == 0 && movimientoHoriz != MovimientoHoriz.Dash)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            movimientoHoriz = MovimientoHoriz.Quieto;
        }
        else if (movimientoHoriz == MovimientoHoriz.Dash)
        {
            DashActivacion();
        }

        // Movimiento Y
        if (jumpPressed && dondeEsta != DondeEsta.Aire)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }

        // Cambio direccion
        if (moveX > 0.1f && direccionPersonaje == DireccionPersonaje.Izquierda)
        {
            GirarPersonaje(DireccionPersonaje.Derecha);
        }
        else if (moveX < -0.1f && direccionPersonaje == DireccionPersonaje.Derecha)
        {
            GirarPersonaje(DireccionPersonaje.Izquierda);
        }

        CambiosDeAnimaciones();
    }

    public void TipoDeSueloPisado(string suelo)
    {
        // Convertimos y asignamos el suelo de String a enum
        dondeEsta = (DondeEsta)System.Enum.Parse(typeof(DondeEsta), suelo);
    }
    void GirarPersonaje(DireccionPersonaje nuevaDireccion)
    {
        direccionPersonaje = nuevaDireccion;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
    void DashActivacion()
    {
        ReducirHitBox(0.16f, -0.1f);
        Debug.Log($"Se entro a DashActivacion");
        AnimacionDash();
    }
    void CambiosDeAnimaciones()
    {
        AnimacionCorrer();
        AnimacionSaltoCaida();
    }
    void RunActivacion()
    {
        AnimacionCorrer();
    }
    void AnimacionCorrer()
    {
        if (/*moveX != 0 && */movimientoHoriz == MovimientoHoriz.Avanzando)
        {
            anim.SetBool("Run", true);
            ReducirHitBox(0.24f, -0.14f);
        }
        else if (movimientoHoriz != MovimientoHoriz.Dash)
        {
            anim.SetBool("Run", false);
            ReestablecerHitBox();
        }
    }
    void ReestablecerHitBox()
    {
        hitBox.size = new Vector2(hitBox.size.x, 0.29f);
        groundDetection.offset = new Vector2(groundDetection.offset.x, -0.17f);
    }
    void ReducirHitBox(float altura, float suelo)
    {
        Debug.Log($"Se entro a ReducirHitBox {altura} y {suelo}");
        hitBox.size = new Vector2(hitBox.size.x, altura);
        groundDetection.offset = new Vector2(groundDetection.offset.x, suelo);
    }
    void AnimacionSaltoCaida()
    {
        anim.SetBool("Jump", dondeEsta == DondeEsta.Aire);
    }
    void AnimacionAtaque()
    {
            anim.SetTrigger("Attack");
    }
    public void AnimacionAtaqueOff()
    {
        anim.SetBool("Attack", false);
        anim.SetBool("AttackRun", false);
    }
    void AnimacionAtaqueRun()
    {
        anim.SetTrigger("AttackRun");
    }
    public void AnimacionAttaqueRunOff()
    {
        anim.SetBool("Attack", false);
        anim.SetBool("AttackRun", false);
    }
    void AnimacionDash()
    {
        anim.SetTrigger("Dash");
    }
    public void AnimacionDashOff()
    {
        movimientoHoriz = MovimientoHoriz.Quieto;
        anim.SetBool("Dash", false);
        ReestablecerHitBox();
    }
}




/*
DondeEsta
DireccionPersonaje
MovimientoHoriz
MovimientoVert
Combate
Estado
Efectos
*/