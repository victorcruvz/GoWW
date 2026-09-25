using UnityEngine;

public class Enemy_Attacked : MonoBehaviour
{
    public bool isAttacked { get; set; }
    public Vector2 direccionDamage { get; private set; }

    void Start()
    {
        isAttacked = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ataque"))
        {
            isAttacked = true;
            direccionDamage = collision.transform.position;
        }
    }
}