using UnityEngine;
using UnityEngine.InputSystem;


// Note: animations are called via the controller for both the character and capsule using animator null checks

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class CharacterMovement : MonoBehaviour
{
    Animator anim;
    CharacterController _controller;
    Vector2 inputMove;
    Vector3 playerMove;
    [SerializeField] float XSpeed = 5f;
    [SerializeField] float YSpeed= 2.5f;
    float normalScale = 0.095f;
    float idleSwitch = 5f;   

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();
        normalScale = transform.localScale.y;
        anim.SetBool("isWalking", false);
    }

    private void Update()
    {
        Move();
    }

    public void SwitchDirection()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void Move()
    {
        if (playerMove != Vector3.zero)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        //switch direction
        if (playerMove.x < 0)
        {
            transform.localScale = new Vector3(-normalScale, normalScale, normalScale);
        }
        else if (playerMove.x > 0)
        {
            transform.localScale = new Vector3(normalScale, normalScale, normalScale);
        }
            
        _controller.Move(playerMove * Time.deltaTime);
        idleSwitch -= Time.deltaTime;

        if (idleSwitch <= 0)
        {
            anim.SetInteger("idleState", Random.Range(0, 2));
            idleSwitch = 10f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
        playerMove = new Vector3(inputMove.x * XSpeed, inputMove.y * YSpeed, 0);
    }

    public void EnableMovement(bool setter)
    {
        GetComponent<PlayerInput>().enabled = setter;
        if (!setter)
        {
            anim.SetBool("isWalking", false);
        }
        enabled = setter;
    }

    public void HidePlayer(bool setter)
    {
        GetComponentInChildren<Renderer>().enabled = !setter;
    }
}
