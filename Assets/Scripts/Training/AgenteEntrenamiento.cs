using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgenteEntrenamiento : Agent
{
    [SerializeField] private Transform meta;
    [SerializeField] private float velocidadMultiplier = 5f;
    private Rigidbody2D rb;

    //void Update()
    //{
    //    // Movimiento forzado de emergencia para probar físicas e inputs
    //    float h = Input.GetAxisRaw("Horizontal");
    //    float v = Input.GetAxisRaw("Vertical");
    //    if (h != 0 || v != 0)
    //    {
    //        rb.linearVelocity = new Vector2(h, v) * velocidadMultiplier;
    //    }
    //}

    // Se ejecuta una sola vez al arrancar (como el Start)
    //public void Awake()
        public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Qué pasa cada vez que una prueba se reinicia (ya sea porque ganó o se perdió)
    public override void OnEpisodeBegin()
    {
        // Regresamos al agente al centro del escenario
        this.transform.localPosition = Vector3.zero;
        rb.linearVelocity = Vector2.zero; // En Unity 6 es linearVelocity en vez de velocity

        // Movemos la meta a una posición aleatoria para que la IA realmente aprenda a buscarla
        // y no se memorice un solo camino físico
        meta.localPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0f);
    }

    // 1. LOS OJOS: Recolectar datos del entorno
    public override void CollectObservations(VectorSensor sensor)
    {
        // Posición del agente (3 floats: X, Y, Z)
        sensor.AddObservation(this.transform.localPosition);
        // Posición de la meta (3 floats: X, Y, Z)
        sensor.AddObservation(meta.localPosition);
    }

    // 2. LAS PIERNAS: Recibir las decisiones de la IA y mover al objeto
    public override void OnActionReceived(ActionBuffers actions)
    {
        // La IA nos va a dar dos números continuos entre -1 y 1
        float moverX = actions.ContinuousActions[0]; // Controla izquierda/derecha
        float moverY = actions.ContinuousActions[1]; // Controla arriba/abajo

        // Aplicamos la fuerza física para mover el Rigidbody2D
        rb.linearVelocity = new Vector2(moverX, moverY) * velocidadMultiplier;

        // --- EL CONTENEDOR VIRTUAL (Tu duda) ---
        // Si el agente se aleja mucho del centro por andar de vago, lo castigamos y reiniciamos
        if (Mathf.Abs(this.transform.localPosition.x) > 8f || Mathf.Abs(this.transform.localPosition.y) > 5f)
        {
            SetReward(-1.0f); // Castigo por salirse del mapa
            EndEpisode();     // Reiniciar el episodio
        }
    }

    // 3. LOS PREMIOS: Detectar si tocamos la Meta
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target") || collision.gameObject.name == "Meta")
        {
            SetReward(1.0f); // ¡Premio gordo por alcanzar el objetivo!
            EndEpisode();    // Reiniciar episodio (ya ganó)
        }
    }

    // EXTRA: Para que tú puedas probar el juego con las flechas del teclado/WASD antes de entrenar
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Inicializamos en 0
        continuousActions[0] = 0f;
        continuousActions[1] = 0f;

        // Leemos las teclas físicas directamente (compatible con cualquier Input System activo)
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) continuousActions[0] = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) continuousActions[0] = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) continuousActions[1] = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) continuousActions[1] = -1f;
    }
}