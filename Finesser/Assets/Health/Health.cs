using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour

{   [Header("Health")]
    [SerializeField] private float startingHealth;
    public float currentHealth { get; private set; }
    private Animator anim;
    [SerializeField] public bool dead;

    [Header("iFrames")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRend;
    private bool iFramesActive;
    private int iFramesFirstUse;
    // Awake-Methode wird ausgef�hrt, bevor das Spiel "richtig startet" -> noch bevor der Start-Methode
    private void Awake()
    {

        // currentHealth ist wichtig f�r die weitere Verarbeitung der Herzen und erh�lt den Wert, denn man zu Anfang, HeroKnight zuwei�t
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend= GetComponent<SpriteRenderer>();
    }

    // Schaden kann genommen werden
    //
    // _damage erh�lt den Wert bekommt den Wert des �bergabeparamaters, wenn die Methode aufgerufen wird -> siehe EnemyAttack.cs
    // Wenn Spieler-Leben �ber 0 ist, soll "nur" Schaden genommen werden und die hurt-Animation ausgef�hrt werden, anonsten wenn =>
    // Spieler-Leben unter 0 f�llt, wird die die-Animation ausgef�hrt und der Spieler kann HeroKnight nicht mehr bewegen
    // Der Wert f�r dead wird auf true gesetzt
    //
    // Die Mathf.Clamp-Funktion �berpr�ft den ersten Wert -> die derzeitige Anzahl Herzen und �berpr�ft,
    // ob currentHealth mit dem Schaden einbezogen,
    // zwischen Wert 2 (= 0 [Minimum]) und Wert 3 (= maximale Anzahl an Herzen (Bsp. 3) [Maximum]) liegt
    //
    // => Falls ja, dann wird currentHealth ausgegeben,
    // falls unter Wert 2 (= 0) dann wird currentHealth = 0
    // falls (theoretisch aber niemals praktisch) �ber Wert 3 (Bsp. 3 Herzen) dann wird currentHealth = 3
    public void TakeDamage(float _damage)
    {

        //if (iFramesActive || iFramesFirstUse == 1)


        //if (iFramesFirstUse <= 1)
        //{
        //    iFramesFirstUse++;
        //}
        if (!iFramesActive)
        {
            currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);
        }

        if (currentHealth > 0 & !iFramesActive)
        {
            
            anim.SetTrigger("hurt");
            StartCoroutine(Invunerability());
        }
        else if (currentHealth == 0)
        {
            
            if (!dead)
            {
                
                anim.SetTrigger("die");

                // HeroKnight
                if (GetComponent<HeroKnightMovement>() != null)
                GetComponent<HeroKnightMovement>().enabled = false;

                // Gegner
                if (GetComponentInParent<EnemyPatrol>() != null)
                {
                    GetComponentInParent<EnemyPatrol>().enabled = false;
                    GetComponent<MeleeEnemy>().enabled = false;
                    MonoBehaviour.Destroy(GetComponent<MeleeEnemy>().gameObject, 1f);
                }
                dead = true;

            }
            
        }
    }

    //Test Methode, um HeroKnight Herzen abzuziehen

    //public void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.E))
    //        TakeDamage(1);
    //}

    // Leben kann hinzugef�gt werden
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        Physics2D.IgnoreLayerCollision(8, 14, true);
        yield return new WaitForSeconds(0.3f);
        iFramesActive = true;
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.3f);
            spriteRend.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.2f);
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
        Physics2D.IgnoreLayerCollision(8, 14, false);
        iFramesActive = false;
    }

    private IEnumerator WaiterForiFramesAnim(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
