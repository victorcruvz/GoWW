using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteEntrenamientoClimb : Agent
{
    [SerializeField] private Transform meta;
    [SerializeField] private float velocidadMultiplier = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private LayerMask capaSuelo;

    private Rigidbody2D rb;
    private Collider2D col;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public override void OnEpisodeBegin()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // CANDADO NUEVO PARA EL COLLIDER: Lo buscamos si es null
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }

        // Regresamos al agente al inicio
        this.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        rb.linearVelocity = Vector2.zero; // Ya no dará error porque rb ya existe

        // Modificamos el rango de la meta
        meta.localPosition = new Vector3(Random.Range(8f, 13f), Random.Range(6f, 4.5f), 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 6 floats de posiciones (Agente y Meta)
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(meta.localPosition);

        // 1 bool convertido a float (¿Está tocando el suelo?)
        sensor.AddObservation(EsSuelo());
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movimiento en X
        float moverX = actions.ContinuousActions[0];
        rb.linearVelocity = new Vector2(moverX * velocidadMultiplier, rb.linearVelocity.y);

        // Control del salto discreto
        int registrarSalto = actions.DiscreteActions[0];
        if (registrarSalto == 1 && EsSuelo())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // NOTA: Quitamos el "if" de las coordenadas físicas porque ahora dependemos de las colisiones.
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