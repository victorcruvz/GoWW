using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Elementos RB y Collider
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sprite;
    CapsuleCollider2D hitBox;
    public BoxCollider2D groundDetection;
    public GameObject contAttkDobColl;
    public GameObject contAttkRunColl;
    // Vidas
    int vidas = 2;
    // Direccion de movimiento
    float moveX;
    float moveY;
    bool jumpPressed;
    bool jumpWall;
    // Variables de ejecucion
    [Header("Fuerzas")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 5f;
    // Valores para Dash
    [SerializeField] float dashSpeed = 1f;
    [SerializeField] float dashDuration = 0.2f;
    // Valor para invulnerable
    [SerializeField] float invulTime = 2f;
    // Valor relentizacion
    [SerializeField] float slowFact = 0.4f;
    // Tipo de suelo pisado
    DondeEsta dondeEsta;
    // Estado tocando pared
    ParedContacto paredContacto;
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
    // Cuando se recibe daño
    bool isKnockedBack;
    [SerializeField] Vector2 knockbackForce = new Vector2(2f, 1f);
    [SerializeField] float knockbackDuration = 0.18f;
    Vector2 directDamag = new Vector2(0, 0);

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
        direccionPersonaje = DireccionPersonaje.Derecha;
        movimientoHoriz = MovimientoHoriz.Quieto;
        combate = Combate.Quieto;
        estado = Estado.Normal;
        efectos = Efectos.Normal;
        isKnockedBack = false;
    }

    void Update()
    {
        if (!isKnockedBack)
        {
            if (estado != Estado.Muerto)
            {
                // Izquierda -1 | Derecha +1
                moveX = Input.GetAxisRaw("Horizontal");
                // Abajo -1 | Arriba +1
                moveY = Input.GetAxisRaw("Vertical");

                if (Input.GetButtonDown("Jump") && combate != Combate.Ataque)
                {
                    if (dondeEsta != DondeEsta.Aire)
                        jumpPressed = true;
                    else if (paredContacto == ParedContacto.Pared)
                    {
                        jumpPressed = true;
                        jumpWall = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.J) && paredContacto != ParedContacto.Pared)
                    StartCoroutine(Atacar());

                if (Input.GetKeyDown(KeyCode.L) && dondeEsta != DondeEsta.Aire && combate != Combate.Ataque)
                    movimientoHoriz = MovimientoHoriz.Dash;
            }
            else if (estado == Estado.Muerto)
                moveX = 0;
        }
    }

    void FixedUpdate()
    {
        // Movimiento X
        if (!isKnockedBack)
        {
            if (moveX != 0 && movimientoHoriz != MovimientoHoriz.Dash && combate != Combate.Ataque)
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
        }

        // Movimiento Y
        if (jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;

            if (jumpWall)
                SaltoEnMuro();
        }

        // Cambio direccion
        if (combate == Combate.Quieto)
        {
            if (moveX > 0.1f && direccionPersonaje == DireccionPersonaje.Izquierda)
                GirarPersonaje(DireccionPersonaje.Derecha);
            else if (moveX < -0.1f && direccionPersonaje == DireccionPersonaje.Derecha)
                GirarPersonaje(DireccionPersonaje.Izquierda);
        }

        CambiosDeAnimaciones();
    }

    public void TipoDeSueloPisado(string suelo)
    {
        // Convertimos y asignamos el suelo de String a enum
        dondeEsta = (DondeEsta)System.Enum.Parse(typeof(DondeEsta), suelo);
    }
    public void TipoDeMuroContacto(string muro)
    {
        // Convertimos y asignamos la pared de String a enum
        paredContacto = (ParedContacto)System.Enum.Parse(typeof(ParedContacto), muro);
    }
    void GirarPersonaje(DireccionPersonaje nuevaDireccion)
    {
        direccionPersonaje = nuevaDireccion;

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
    void RecibirAtaque()
    {
        vidas--;

        if (vidas == 1)
        {
            anim.SetTrigger("Damage");
            estado = Estado.Invulnerable;
            StartCoroutine(KnockbackRoutine());
            StartCoroutine(AnimacionGolpe());
        }
        else if (vidas == 0)
        {
            estado = Estado.Muerto;
            AnimacionMuerte();
        }
    }
    void SaltoEnMuro()
    {
        jumpWall = false;
        int ladoImpulso = 0;

        if (direccionPersonaje == DireccionPersonaje.Izquierda)
            ladoImpulso = 1;
        else
            ladoImpulso = -1;

        rb.linearVelocity = new Vector2(10 * ladoImpulso, jumpForce);
    }
    void DashActivacion()
    {
        ReducirHitBox(0.16f, -0.1f);
        StartCoroutine(IniciarDash());
        AnimacionDash();
    }
    public void RecibirDireccionDelDamage(Vector2 recibirDireccionDelDamage)
    {
        directDamag = recibirDireccionDelDamage;
    }
    IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        float hitDirectionX = transform.position.x < directDamag.x ? -1f : 1f;
        Vector2 force = new Vector2(hitDirectionX * knockbackForce.x, knockbackForce.y);
        
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }
    IEnumerator IniciarDash()
    {
        int direccion;

        if (direccionPersonaje == DireccionPersonaje.Izquierda)
            direccion = -1;
        else
            direccion = 1;

        rb.linearVelocity = new Vector2(dashSpeed * direccion, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
    }
    void RunActivacion()
    {
        AnimacionCorrer();
    }
    IEnumerator Atacar()
    {
        if (movimientoHoriz == MovimientoHoriz.Dash)
            yield break;

        if (dondeEsta == DondeEsta.Aire)
            combate = Combate.AtaqueAereo;
        else if (dondeEsta == DondeEsta.Suelo)
            combate = Combate.Ataque;

        AnimacionAtaque();

        // Esperamos tiempo para q se sincronice el collider con la animacion
        yield return new WaitForSeconds(0.1f);

        if (movimientoHoriz == MovimientoHoriz.Avanzando)
            contAttkRunColl.SetActive(true);
        else if (movimientoHoriz == MovimientoHoriz.Quieto)
        {
            yield return new WaitForSeconds(0.2f);
            contAttkDobColl.SetActive(true);
        }
    }
    void ReestablecerHitBox()
    {
        hitBox.size = new Vector2(hitBox.size.x, 0.29f);
        groundDetection.offset = new Vector2(groundDetection.offset.x, -0.17f);
    }
    void ReducirHitBox(float altura, float suelo)
    {
        hitBox.size = new Vector2(hitBox.size.x, altura);
        groundDetection.offset = new Vector2(groundDetection.offset.x, suelo);
    }
    void CambiosDeAnimaciones()
    {
        AnimacionCorrer();
        AnimacionSaltoCaida();
        AnimacionAgarradoMuro();
    }
    void AnimacionCorrer()
    {
        if (movimientoHoriz == MovimientoHoriz.Avanzando)
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
        // Desactivar collider de ataque
        contAttkDobColl.SetActive(false);
        contAttkRunColl.SetActive(false);
        // Desactivar animacion ataque
        anim.SetBool("Attack", false);
        combate = Combate.Quieto;
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
    void AnimacionAgarradoMuro()
    {
        if (paredContacto == ParedContacto.Pared && dondeEsta != DondeEsta.Suelo)
        {
            anim.SetBool("Wall", true);
        }
        else
        {
            anim.SetBool("Wall", false);
        }
    }
    void AnimacionMuerte()
    {
        anim.SetBool("Dead", true);
    }
    IEnumerator AnimacionGolpe()
    {
        Color c = sprite.color;

        float tiempo = 0f;

        // Parpade por daño
        while (tiempo < invulTime)
        {
            c.a = 0.3f;
            sprite.color = c;
            yield return new WaitForSeconds(0.1f);

            c.a = 1f;
            sprite.color = c;
            yield return new WaitForSeconds(0.1f);

            tiempo += 0.2f;
        }

        c.a = 1f;
        sprite.color = c;
        estado = Estado.Normal;
    }
    public void FinAnimacionGolpe()
    {
        anim.SetBool("Damage", false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Damage" && estado != Estado.Invulnerable)
        {
        //Debug.Log("Entro el Daño");
            RecibirAtaque();
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}