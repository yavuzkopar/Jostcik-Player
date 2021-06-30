using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private Animator animator;
    [SerializeField] Transform lookpos;
    [SerializeField] GameObject[] bombPrefab;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    public static int bombCount = 3;
    public FixedJoystick joystick;


    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityModifier;
    [SerializeField] private bool isOnGround = true;
    Vector3 dir;
    [HideInInspector]
    public Vector3 inputVector;
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        #region GravityChange
        if (playerRb.velocity.y < 0)
        {
            playerRb.velocity += Vector3.up * Physics.gravity.y * 2f * Time.deltaTime;
            
        }
        else
        {
            playerRb.velocity += Vector3.up * Physics.gravity.y * (2f - 1f) * Time.deltaTime;
           
        }
        #endregion
        MoveOn();
        if (transform.position.y < -5)
        {
            GameManager.Instance.GameOver();
        }
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DropBomb(0);
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            DropBomb(1);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {                    
            Jumping();        
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Attack();
        }
    }
    #region Jumping
    public void Jumping( )
    {
        
        if ( isOnGround)
        {

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            animator.SetTrigger("jump");
        }
        
    }
    #endregion
   public void Attack()
    {
        animator.SetTrigger("attack");
    }
    void MoveOn()
    {
        // For pc
         //    inputVector = new Vector3(Input.GetAxisRaw("Horizontal") , 0, Input.GetAxisRaw("Vertical") );
        // For Mobile
           inputVector = Vector3.right * joystick.Horizontal + Vector3.forward * joystick.Vertical;
        Quaternion lookRotation = Quaternion.LookRotation(inputVector.normalized);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        transform.position += inputVector * speed * Time.deltaTime;
        if (inputVector.magnitude > 0)
        {
            animator.SetBool("walk", true);
        }
        else
            animator.SetBool("walk", false);
    }
    #region DropItem
   public void DropBomb(int bombindex)
    {
        if (bombCount > 0)
        {
            Instantiate(bombPrefab[bombindex], transform.position + new Vector3(0, .5f, 0), bombPrefab[bombindex].transform.rotation);
            bombCount--;
        }
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gold"))
        {
            GameManager.Instance.score++;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Prebomb"))
        {
            bombCount++;
            Destroy(other.gameObject);
        }
    }


}
