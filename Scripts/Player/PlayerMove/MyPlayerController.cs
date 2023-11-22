using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polyperfect.People
{
    [RequireComponent(typeof(Rigidbody))]
    public class MyPlayerController : MonoBehaviour
    {
        public float walkSpeed, runSpeed, rotateSpeed, jumpForce, walkAnimationSpeed, runAnimatonSpeed;

        public Animator animator;
        public new Rigidbody rigidbody;

        Vector3 offset;

        public float distToGround;


        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            distToGround = GetComponent<Collider>().bounds.extents.y;
        }

        // Update is called once per frame
        void Update()
        {

            //Allow the player to move left and right
            float horizontalMove = Input.GetAxisRaw("Horizontal");
            //Allow the player to move forward and back
            float vertical = Input.GetAxisRaw("Vertical");

            float speed = walkSpeed;
            float animSpeed = walkAnimationSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                vertical *= 2f;
                speed = runSpeed;
                animSpeed = runAnimatonSpeed;
            }

            var translation = transform.forward * (vertical * Time.deltaTime);
            translation += transform.right * (horizontalMove * Time.deltaTime);
            translation *= speed;
            translation = rigidbody.position + translation;


            float horizontal = Input.GetAxis("Mouse X") * Time.deltaTime;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, horizontal * rotateSpeed, 0);

            animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontalMove, 0.1f, Time.deltaTime);

            animator.SetFloat("WalkSpeed", animSpeed);

            rigidbody.MovePosition(translation);
            rigidbody.MoveRotation(rotation);
        }
    }
}
