using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float punchRange;
    [SerializeField] private float initialVelocity_X;
    [SerializeField] private float initialVelocity_Y;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float gravity;
    [SerializeField] private float bufferTime;

    [SerializeField] private TMP_Text meterCounterText;
    [SerializeField] private TMP_Text coinsText;

    [SerializeField] GameObject gamePannel;
    [SerializeField] GameObject gameOverPannel;
    [SerializeField] GameObject fireball;
    [SerializeField] Transform fireballPosition;

    public bool isGrounded { get; private set; }
    public bool isFlipped { get; private set; }
    public bool isJumping { get; private set; }

    public float velocity_X { get; private set; }
    public float velocity_Y { get; private set; }

    private float jumpBufferCounter;
    private float flipBufferCounter;

    private Vector3 initialScale;
    private Rigidbody rb;
    private Collider triggerCollider;
    private Vector3 startPos;
    private PlayerHealth playerHealth;
    public bool isMoving { get; private set; }

    public float distance { get; private set; }
    public int nbCoins { private set; get; }
    float oneCoins = 0;
    float tenCoins = 0;
    float hundredCoins = 0;

    Animator animator;
    

    private void Start()
    {
        animator = GetComponent<Animator>();

        isGrounded = true;
        isFlipped = false;
        isJumping = false;

        nbCoins = 0;
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        velocity_X = initialVelocity_X;
        velocity_Y = initialVelocity_Y;

        initialScale = transform.localScale;

        coinsText.text = hundredCoins.ToString() + tenCoins.ToString() + oneCoins.ToString();
    }

    private void Update()
    {
        if (MinigameManager.GetInstance().isVictorius || playerHealth.isDead)
            isMoving = false;

        distance = transform.position.x;

        if (isMoving)
        {
            rb.velocity = new Vector3(velocity_X, velocity_Y, rb.velocity.z);

            if (isJumping)
            {
                velocity_Y -= isFlipped ? -gravity * Time.deltaTime : gravity * Time.deltaTime;
            }
            BufferCountdown();
            Jump();
            Flip();

            MeterCounter();
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        animator.SetFloat("velocityX", rb.velocity.x);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void BufferCountdown()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;
        if (flipBufferCounter > 0)
            flipBufferCounter -= Time.deltaTime;
    }

    public void OnFlip(InputAction.CallbackContext context)
    {
        if (context.performed)
            flipBufferCounter = bufferTime;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpBufferCounter = bufferTime;
    }

    public void OnPunch(InputAction.CallbackContext context)
    {
        if (context.performed)
            Punch();
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (isMoving)
        {
            if (context.performed) // keydown
            {
                Slide();
            }
            else if (context.canceled) //keyup
            {
                Stand();
            }
        }
    }

    private void Flip()
    {
        if (isGrounded && flipBufferCounter > 0)
        {
            int rand = Random.Range(17, 19);
            MinigameSoundManager.GetInstance().PlaySFX(rand); // Gravity SFX

            isFlipped = !isFlipped;

            velocity_Y = -velocity_Y;

            StartCoroutine(RotateOverTime(1f));

            flipBufferCounter = 0;
        }
    }

    private void Jump()
    {
        if (isGrounded && jumpBufferCounter > 0)
        {
            int rand = Random.Range(10, 12);
            MinigameSoundManager.GetInstance().PlaySFX(rand);

            isJumping = true;
            velocity_Y = isFlipped ? -jumpVelocity : jumpVelocity;
            jumpBufferCounter = 0;
        }
    }

    private void Punch()
    {
        if (isMoving)
        {
            animator.SetTrigger("Fire");

            //RaycastHit hit;
            //Debug.DrawRay(transform.position, new Vector3(punchRange, 0, 0), Color.green);

            //if (Physics.Raycast(transform.position, new Vector3(1f, 0, 0), out hit, punchRange))
            //{
            //    if (hit.collider.gameObject.tag == "Wall")
            //        Destroy(hit.collider.gameObject);
            //}
        }
    }


    private void Slide()
    {
        if (!playerHealth.isDead)
        {
            MinigameSoundManager.GetInstance().PlaySFX(SFX.ECRASE1);
            transform.localScale = new Vector3(transform.localScale.x, 0.66f, transform.localScale.z);

        }
    }

    private void Stand()
    {
        MinigameSoundManager.GetInstance().PlaySFX(SFX.ECRASE2);
        transform.localScale = initialScale;
    }

    public void Death()
    {
        if (!MinigameManager.GetInstance().isVictorius)
        {
            GameManager.GetInstance().canPause = false;

            animator.SetBool("isDead", true);

            int rand = Random.Range(5, 10);
            MinigameSoundManager.GetInstance().PlaySFX(rand);

            gamePannel.SetActive(false);
            gameOverPannel.SetActive(true);
            MinigameSoundManager.GetInstance().StopSongSmoothly();
        }
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Floor")
        {
            isGrounded = true;
            isJumping = false;
            velocity_Y = isFlipped ? -initialVelocity_Y : initialVelocity_Y;
        }
    }

    public void OnCollisionExit(Collision col)
    {
        var objTag = col.gameObject.tag;

        if (objTag == "Floor")
        {
            isGrounded = false;
        }
    }

    public void Fire()
    {
        GameObject a = Instantiate(fireball, fireballPosition.position, Quaternion.identity);
        a.transform.Rotate(0f, 0f, 90f);

        int rand = Random.Range(14, 17);
        MinigameSoundManager.GetInstance().PlaySFX(rand); // Fire SFXs
    }

    public void IncrementCoins()
    {
        int rand = Random.Range(12, 14);
        MinigameSoundManager.GetInstance().PlaySFX(rand); // Coin SFXs

        nbCoins++;
        oneCoins++;
        if (oneCoins >= 10)
        {
            oneCoins = 0;
            IncrementCoinsTen();
        }
        coinsText.text = hundredCoins.ToString() + tenCoins.ToString() + oneCoins.ToString();
    }

    private void IncrementCoinsTen()
    {
        tenCoins++;
        if (tenCoins >= 10)
        {
            tenCoins = 0;
            IncrementCoinsHundred();
        }
        coinsText.text = hundredCoins.ToString() + tenCoins.ToString() + oneCoins.ToString();
    }

    private void IncrementCoinsHundred()
    {
        hundredCoins++;
        coinsText.text = hundredCoins.ToString() + tenCoins.ToString() + oneCoins.ToString();
    }

    private void MeterCounter()
    {
        int tousandMeters = (int)(distance / 1000);
        int hundredMeters = (int)((distance - tousandMeters * 1000) / 100);
        int tenMeters = (int)((distance - (tousandMeters * 1000) - (hundredMeters * 100)) / 10);
        int oneMeters = (int)(distance - (tousandMeters * 1000) - (hundredMeters * 100) - (tenMeters * 10));

        meterCounterText.text = tousandMeters.ToString() + hundredMeters.ToString() + tenMeters.ToString() + oneMeters.ToString();
    }

    public void AllowMovements()
    {
        isMoving = true;
        animator.enabled = true;
    }

    public void DisableMovements()
    {
        isMoving = false;
        animator.enabled = false;
    }

    IEnumerator RotateOverTime(float time)
    {
        for (int i = 0; i < 30; i++)
        {
            transform.Rotate(new Vector3(0f, 0f, 6f));
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield break;
    }
}
