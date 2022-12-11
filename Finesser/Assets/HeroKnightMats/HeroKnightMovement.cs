using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEditor;
using UnityEngine;

public class HeroKnightMovement : MonoBehaviour
{

    [SerializeField] public float speed = 5;
    [SerializeField] float jumph = 5;
    private Rigidbody2D body;
    private Animator anim;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isRolling = false;
    [SerializeField] float dirX;
    [SerializeField] float yVelocity;
    private float fallingPosition;
    private const float fallingThreshold = 2;
    [SerializeField] const float groundCheckRadius = 0.25f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask obstaclesLayer;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform heroKnight;
    private Vector3 initScale;
    private Health health;
    private float currentFallTime;
    private IEnumerator rollingRoutine;

    //public static HeroKnightMovement moveinstance;

    MovementState state = MovementState.idle;
    //public float FallingThreshold = -10f;



    //Aktuelle Informationen über den SpriteRenderer
    private SpriteRenderer sprite;

    // Hier werden die Zustände für die verschiedenen Animationen instanziert
    private enum MovementState { idle, running, jumping, falling, rolling, attack1, attack2, attack3, death }

    // Start is called before the first frame update
    private void Awake()
    {
        initScale = heroKnight.localScale;

        health = GetComponent<Health>();

        body = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        sprite = GetComponent<SpriteRenderer>();

        yVelocity = body.velocity.y;

        rollingRoutine = RollingBehaviour();

        //moveinstance = this;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        // In der Variable "dirX" wird der horizontale Wert des Inputs gespeichert
        // => Links: "A" -> dirX = -1 | Rechts: "D" -> dirX = 1 | Stehen: -> dirX = 0

        dirX = Input.GetAxis("Horizontal");

        //body.velocity = new Vector2(dirX * speed, body.velocity.y);

        if (dirX >= 0.01f)
        {
            transform.localScale = Vector3.one * 0.7f;
        }
        else if (dirX < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1) * 0.7f;
        }

        // Altes InputSystem
        //if (state == MovementState.running)
        //{
            transform.Translate(Vector2.right * speed * dirX * Time.deltaTime);
        //}
        //else
        //{
        //    transform.Translate(Vector2.right * (speed/2) * dirX * Time.deltaTime);
        //}
        UpdateAnimationState();
        GroundCheck();

        
        if(isRolling)
        {
            
        }
    }

    private void FixedUpdate()
    {
        
    }


    // Eine Methode, die auf die, in den Übergabeparametern angegeben Wert (int) wartet
    // siehe -> UpdateAnimation() > if-Block (isrolling) {...}
    //IEnumerator WaitForNotRolling(int animationFrames)
    //{
    //    isCurrentlyRolling = true;
    //    for (int i = 0; i < animationFrames; i++)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }
    //    isCurrentlyRolling= false;
    //    isRolling = false;
    //}

    private IEnumerator RollingBehaviour()
    {
        //yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => isGrounded);
        anim.SetTrigger("rolling");
        print("rolling");
        currentFallTime = 0;
    }

    public void GroundCheck()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        Collider2D[] collidersObsctacles = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, obstaclesLayer);
        if (colliders.Length > 0 )
        {
            isGrounded = true;
        }
        else if (collidersObsctacles.Length > 0)
        {
            isGrounded = true;
        }

    }
    private void UpdateAnimationState()
    {

        // Dieser Abschnitt ist für die Animation beim Laufen
        // Da beim Drücken der Tasten "A" oder "D", richtung = -1 || richtung = +1 gesetzt wird
        // => Schlussvolgerung, wenn richtung != 0 ist -> Player bewegt sich



        // Wenn "A" gedrückt wird -> richtung = -1 -> Player wird in der X-Achse "geflipped" und bewegt sich nach Links
        // sogar ohne negative Variable, da Player geflipped wird und anders als bei der Methode die Y-Achse bei Transform nicht rotiert wird
        // Wenn "D" gedrückt wird -> richtung = +1 -> Player wird in der X-Achse nicht "geflipped" &&|| flip = false und bewegt sich nach Rechts

        //if (isRolling)
        //{
        //    state = MovementState.rolling;
        //    //StartCoroutine(WaitForNotRolling());
            
        //}


        if (isGrounded && state != MovementState.rolling)
        {
            state = MovementState.idle;
        }

        if (dirX != 0 && isGrounded)
        {
            state = MovementState.running;

            //sprite.flipX = richtung < 0 ;
            //heroKnight.localScale = new Vector3(Mathf.Abs(initScale.x) * dirX, initScale.y, initScale.z);
            
        }

        // Player springt beim Drücken der Space-Taste + nur, wenn Player zuvor auf dem Boden war/bzw. diesen berührt hat
        // => Doppelsprünge sind nicht möglich

        // Es wird überprüft, ob HeroKnight auf etwas steht, was den Tag: "ground" trägt, falls nicht soll die Sprung-Animation ausgeführt werden
        // Die Tilemap hat den Tag: "ground" erhalten

        //Die GroundcheckMethode übergibt den Wert true für isgrounded

        if ((Input.GetKeyDown(KeyCode.Space) && isGrounded) || (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded))
        {
            if (body.velocity.y <= 1)
            {
                body.AddForce(Vector2.up * jumph, ForceMode2D.Impulse);
            }
        }

        if (body.velocity.y > 0.35f && !isGrounded)
        {

            state = MovementState.jumping;
        }

        if (body.velocity.y < -0.35f && !isGrounded)
        {
            
            currentFallTime += Time.deltaTime;
            if (state != MovementState.falling)
            {
                StartCoroutine(rollingRoutine);
            }
            state = MovementState.falling;
            print(currentFallTime);
        }

        //&& (fallingPosition - rb.position.y > fallingThreshold))

        

        anim.SetInteger("state", (int)state);
    }


    // Die GroundCheck-Methode überprüft, ob ein Objekt, welches zwischen den Beinen von HeroKnight ist,
    // mit der Tilemap kollidiert

    //public void Move(float dir)
    //{

    //    float xVal = dir * speed * 100 * Time.deltaTime;
    //    Vector2 targetVelocity = new Vector2(xVal, rb.velocity.y);
    //    rb.velocity = targetVelocity;


    //}


    // Beim "Betreten" der Tilemap bzw. bei einer Kollision mit der Tilemap wird isrolling = true gesetzt
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground")) //collision.gameObject.tag == "ground"
        {

            //state = MovementState.rolling;

            //if(currentFallTime > maxFallTime)
            //{
            //if (rollingCounter >= 1)
            //{
            if(currentFallTime <= 0.6)
            {
                StopCoroutine(rollingRoutine);
            }
            isGrounded = true;
            //}
            //    rollingCounter++;
            //}


            //fallingPosition - rb.position.y > fallingThreshold;
        }
        
    }

    // isgrounded bekommt den Wert "true", wenn Player sich auf der/dem Tilemap/ground befindet/ nach einem Sprung landet
    // UND STEHEN bleibt bezogen auf die Rollanimation, weil diese vorher noch abgespielt wird

    //public void OnCollisionStay2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "ground")
    //    {
    //        GroundCheck();
    //        isJumping = false;
    //        isGrounded = true;
    //    }
    //}

    // Wenn HeroKnight den Kollisionszustand mit der Tilemap verlässt wird isjumping auf true gesetzt
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            
            isGrounded = false;
            isRolling= false;
        }
    }


    // Wenn der Spieler mit einer Spike-Falle in Berührung kommt, dann soll die TakeDamage()-Methode aufgerufen werden
    // Resultat -> Spieler verliert ein Herz
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "trapsSpikes")
        {
            health.TakeDamage(1);
        }
        //else if (collision.gameObject.tag == "FireHeadEnemy")
        //{
        //    health.TakeDamage(1);
        //}
    }

    public void SetRollingToFalse()
    {
        isRolling = false;
    }
}
