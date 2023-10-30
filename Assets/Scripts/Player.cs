using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private CharacterController characterController;

    private Animator anim;

    private float jumpForce = .07f;
    private float gravity = .2f;
    private float verticalVelocity;
    
    private int desiredLane = 1; //0 = left, 2 = right

    private bool isRunning = false;

    private const float laneDistance = 2f;

    private float speed, originalSpeed = .1f;
    private float speedIncreaseLastTIck;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = .01f;

    void Start()
    {
        speed = originalSpeed;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isRunning)
        {
            return;
        }

        if((Time.time - speedIncreaseLastTIck) > speedIncreaseTime)
        {
            speedIncreaseLastTIck = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        if(MobileInput.Instance.SwipeLeft) { MoveLane(false); }
        if(MobileInput.Instance.SwipeRight) { MoveLane(true); }

        //calculate where we should be in the future
        var targetPosition = transform.position.z * Vector3.forward;
        if(desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        //calculate move delta
        var moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        var isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                //Jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if(MobileInput.Instance.SwipeDown)
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);
            //Fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                //Jump
                verticalVelocity = -jumpForce;
            }
        }

        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        characterController.Move(moveVector);

        //Rotate pengu
        var dir = characterController.velocity;
        //only happen when movement is detected
        if(dir != Vector3.zero)
        {
            dir.y = 0;
            var turnSpeed = 0.05f;
            transform.forward = Vector3.Lerp(transform.forward, dir, turnSpeed);
        }
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRuning");
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        characterController.height /= 2;
        characterController.center = new Vector3(
            characterController.center.x,
            characterController.center.y / 2,
            characterController.center.z);
    }

    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        characterController.height *= 2;
        characterController.center = new Vector3(
            characterController.center.x,
            characterController.center.y * 2,
            characterController.center.z);
    }
    private void MoveLane(bool goingRight)
    {
        desiredLane += goingRight ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        var groundRay = new Ray(new Vector3(
            characterController.bounds.center.x,
            (characterController.bounds.center.y - characterController.bounds.extents.y) + .2f,
            characterController.bounds.center.z), Vector3.down);

        return Physics.Raycast(groundRay, .2f + .1f);
    }

    private void Crash()
    {
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.IsDead = true;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}
