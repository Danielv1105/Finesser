using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class MeleeEnemy : MonoBehaviour
{

    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private CapsuleCollider2D playerCollider;
    private bool enemyTypeWithoutAttack;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health health;
    private EnemyPatrol enemyPatrol;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void OnDisable()
    {
        anim.SetBool("running", false);
    }
    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        // Nur angreifen, wenn der Gegner in Reichweite ist und nicht schon tot ist
        if (PlayerInSight() && !health.dead)
        {
            if(cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("attack");
            }

        }

        if (enemyPatrol != null & !enemyTypeWithoutAttack)//& !enemyTypeWithoutAttack)
            enemyPatrol.enabled = !PlayerInSight();

    }

    private bool EnemyTypeWithoutAttack()
    {
            return true;
    }
    // Überprüft, ob HeroKnight sich im Sichtfeld/Range des Gegners befindet

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, 
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y - 0.5f, boxCollider.bounds.size.z), 
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            health = hit.transform.GetComponent<Health>();
        return hit.collider != null;
    }

    // zeigt den Bereich an, der als Sichtfeld/Range des Gegners definiert wird
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
        new Vector3(boxCollider.bounds.size.x * range, (boxCollider.bounds.size.y + -0.5f), boxCollider.bounds.size.z));
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            enemyTypeWithoutAttack = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            enemyTypeWithoutAttack = false;
        }

    }

    // Die Methode wird ausgeführt, wenn die Animation vom Enemy inmitten der Animation der attack-Animation getriggert wird
    // Wenn der Spieler in Reichweite ist, dann soll TakeDamage aufgerufen werden
    private void DamageHeroKnight()
    {

        if(PlayerInSight())
        {
            health.TakeDamage(damage);
        }

    }
}
