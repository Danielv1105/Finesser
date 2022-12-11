using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class HeroKnightCombat : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private CapsuleCollider2D enemyCollider;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health health;

    public static HeroKnightCombat instance;

    public bool canReceiveInput;
    public bool inputReceived;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        instance = this;
    }
    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        if ((cooldownTimer >= attackCooldown))
        {
            Attack();
            cooldownTimer = 0;
        }

        // Nur angreifen, wenn der Gegner in Reichweite ist und nicht schon tot ist
        // Das zweite "&" wird erst überpüft, wenn das Ergebnis aus der ersten Klammer = true ist
        //if ((cooldownTimer >= attackCooldown) & Input.GetMouseButtonDown(0))
        //{
        //print(combo);
        //anim.SetTrigger($"attack{combo}");
        //combo++;
        ////if(combo > 1)
        ////anim.ResetTrigger($"attack{combo-1}");
        //cooldownTimer = 0;
        //if (combo > 3)
        //ResetCombo();

        //if (combo == 1)
        //{
        //    cooldownTimer = 0;
        //    anim.SetTrigger("attack1");
        //    combo++;
        //}
        //else if (combo == 2)
        //{
        //    cooldownTimer = 0;
        //    anim.SetTrigger("attack2");
        //    combo++;

        //}
        //else if (combo == 3)
        //{
        //    cooldownTimer = 0;
        //    anim.SetTrigger("attack3");
        //    combo = 0;
        //}


        //}

    }

    // Überprüft, ob ein Gegner sich im Sichtfeld/Range von HeroKnight befindet

    private bool EnemyInSight()
    {
        RaycastHit2D enemyHit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);

        if (enemyHit.collider != null)
            health = enemyHit.transform.GetComponent<Health>();
        return enemyHit.collider != null;
    }

    // zeigt den Bereich an, der als Sichtfeld/Range des Gegners definiert wird
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    // Die Methode wird ausgeführt, wenn die Animation vom Enemy inmitten der Animation der attack-Animation getriggert wird
    // Wenn der Spieler in Reichweite ist, dann soll TakeDamage aufgerufen werden

    private void DamageEnemy()
    {

        if (EnemyInSight())
        {
            health.TakeDamage(damage);
        }

    }

    public void Attack ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inputReceived = true;
            canReceiveInput = false;
        }
        else
        {
            return;
        }
    }

    public void InputManager()
    {

        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }

    //private void ResetCombo()
    //{
    //    combo = 1;
    //    anim.ResetTrigger("attack1");
    //    anim.ResetTrigger("attack2");
    //    anim.ResetTrigger("attack3");
    //}
}
