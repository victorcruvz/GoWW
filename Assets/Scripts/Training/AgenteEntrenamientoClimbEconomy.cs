using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteEntrenamientoClimbEconomy : Agent
{
    [SerializeField] private Transform meta;
    [SerializeField] private float velocidadMultiplier = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private LayerMask capaSuelo;

    Rigidbody2D rb;
    Collider2D col;

    float menorDistanciaRegistrada;

    Vector3[] posicionesPlayer = new Vector3[]
{
    new Vector3(0f, 1.1f, 0f),
    new Vector3(5f, 2.1f, 0f),
};
    // Tus 3 posiciones fijas determinadas
    Vector3[] posicionesMeta = new Vector3[]
    {
    new Vector3(13f, 6f, 0f),
    new Vector3(7f, 8f, 0f),
    new Vector3(-1.3f, 3.7f, 0f)
    };

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public override void OnEpisodeBegin()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (col == null) col = GetComponent<Collider2D>();

        // 1. Elegimos y asignamos posición del jugador
        int indiceAleatorioPlayer = Random.Range(0, posicionesPlayer.Length);
        Vector3 posicionInicialPlayer = posicionesPlayer[indiceAleatorioPlayer];
        this.transform.localPosition = posicionInicialPlayer;

        rb.linearVelocity = Vector2.zero;

        // 2. Elegimos y asignamos posición de la meta
        int indiceAleatorioMeta = Random.Range(0, posicionesMeta.Length);
        Vector3 posicionInicialMeta = posicionesMeta[indiceAleatorioMeta];
        meta.localPosition = posicionInicialMeta;

        menorDistanciaRegistrada = Vector2.Distance(posicionInicialPlayer, posicionInicialMeta);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //// Mandamos solo X e Y (2 floats cada uno)
        //sensor.AddObservation((Vector2)this.transform.localPosition);
        //sensor.AddObservation((Vector2)meta.localPosition);
        // Solo le enviamos la posición relativa de la meta con respecto al agente (Vector2)
        Vector2 dirHaciaMeta = (meta.localPosition - this.transform.localPosition);
        sensor.AddObservation(dirHaciaMeta);


        // 1 bool (1 float)
        sensor.AddObservation(EsSuelo());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movimiento en X y Salto
        float moverX = actions.ContinuousActions[0];
        rb.linearVelocity = new Vector2(moverX * velocidadMultiplier, rb.linearVelocity.y);

        int registrarSalto = actions.DiscreteActions[0];
        if (registrarSalto == 1 && EsSuelo())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            AddReward(-0.05f); // Bajamos un poco el castigo para que no le tenga miedo a saltar
        }

        // --- SISTEMA DE RÉCORD DE PROXIMIDAD ---
        float distanciaActual = Vector2.Distance(this.transform.localPosition, meta.localPosition);

        if (distanciaActual < menorDistanciaRegistrada)
        {
            float progreso = menorDistanciaRegistrada - distanciaActual;
            AddReward(progreso * 0.2f); // Subimos el premio por romper récord para motivarlo más
            menorDistanciaRegistrada = distanciaActual;
        }

        // Premio extra muy sutil por moverse a la derecha (hacia el reto)
        if (moverX > 0.1f)
        {
            AddReward(0.0005f);
        }

        // Castigo por tiempo fijo (mantiene la urgencia)
        AddReward(-0.001f);
    }

    private bool EsSuelo()
    {
        RaycastHit2D hit = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y + 0.1f, capaSuelo);
        return hit.collider != null;
    }

    // 3. DETECCIÓN DE COLISIONES SÓLIDAS (Para el daño)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el agente choca contra el suelo naranja o la pared de peligro
        if (collision.gameObject.CompareTag("Damage") || collision.gameObject.layer == LayerMask.NameToLayer("Damage"))
        {
            SetReward(-1.0f); // Castigo por morir
            EndEpisode();     // Reiniciar intento
        }
    }

    // 4. DETECCIÓN DE TRIGGERS (Para la Meta)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target") || collision.gameObject.name == "Meta")
        {
            SetReward(1.0f); // ¡Premio por llegar!
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        continuousActions[0] = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) continuousActions[0] = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) continuousActions[0] = -1f;

        discreteActions[0] = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))
        {
            discreteActions[0] = 1;
        }
    }
}