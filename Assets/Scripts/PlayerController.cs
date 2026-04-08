using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private float xInput;
    private float yInput;
    public bool isCleaning;
    public bool isMoving;
    public bool isHiding;
    public bool isChilling;

    public GameManager manager;


    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float speedRun;
    [SerializeField] private bool isRunning;
    [SerializeField] public bool isKnockedBack;

    [SerializeField] private float direction;

    [Header("Stamina")]
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float runStaminaCost;
    [SerializeField] private float staminaRecoveryRate;
    [SerializeField] private Image StaminaBar;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

    }

   
    void Update()
    {
        if(!manager.gameIsOver && !manager.isPaused){

            if (!isKnockedBack && !isHiding)
            { Move(); }

            UpdateStaminaBar();

        }

      if(manager.hasDied|| isHiding ||manager.isPaused)
        {
            rb.linearVelocity = new Vector2 (0f, 0f);
            isMoving = false;
        }

        HandAnimation();

        if (!isRunning && stamina < maxStamina)
        {
            StartCoroutine(ReplenishStamina());
        }

    }

    private void Move()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(xInput * speed, yInput * speed);
        isRunning = false;

        if (xInput !=0 || yInput !=0)
        {
            isCleaning = false;
            isMoving = true;
            FacingDirection();
        }
        else
        {
            isMoving = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && (xInput !=0 || yInput != 0))
        {
            rb.linearVelocity = new Vector2(xInput * speedRun, yInput * speedRun);
            isRunning = true;
            stamina -= Time.deltaTime * runStaminaCost;
            if (stamina < 0)
            { stamina = 0; }
            

        }

        isChilling = manager.isPaused;
        //Debug.Log(stamina);
    }

    void HandAnimation()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("IsCleaning", isCleaning);
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("FacingDirection", direction);
        anim.SetBool("IsKnockedBack", isKnockedBack);
        anim.SetBool("IsDead",manager.hasDied);
    }

    private IEnumerator ReplenishStamina()
    {
        if (stamina == 0)
        {
            yield return new WaitForSeconds(3f);
            stamina += Time.deltaTime * staminaRecoveryRate;
        }
        else
        {
            stamina += Time.deltaTime * staminaRecoveryRate;
        }

        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }
    }

    private void FacingDirection()
    {
        if ( rb.linearVelocity.x < 0) 
        {
            direction = 1;
        }
        else if (rb.linearVelocity.x > 0)
        {
            direction = 2;
        }
        else if (rb.linearVelocity.y < 0)
        {
            direction = 3;
        }
        else if (rb.linearVelocity.y > 0)
        {
            direction = 4;
        }

        //Debug.Log(direction);

    }

    private void UpdateStaminaBar()
    {
        StaminaBar.fillAmount = stamina / maxStamina;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Colpito Player");

           Vector2 direction = (transform.position - collision.transform.position).normalized;
            rb.AddForce(direction * 3, ForceMode2D.Impulse);
            StartCoroutine(KnockBack());
            isCleaning = false;
            isMoving = false;

        }

    }

   private IEnumerator KnockBack()
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(0.3f);
        isKnockedBack = false;

    }


}
