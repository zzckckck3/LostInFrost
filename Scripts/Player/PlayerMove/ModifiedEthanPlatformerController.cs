using ECM.Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECM.Examples
{
    /// <summary>
    /// 
    /// Example Character Controller
    /// 
    /// This example shows a tipical platformer controller with double jump eg: maxMidAirJumps = 1
    /// 
    /// </summary>

    public class ModifiedEthanPlatformerController : ModifiedCharacterController
    {
        // 무기 들었을 때 걷는 속도
        [SerializeField]
        private float weaponWalkSpeed = 3f;

        // 무기 들었을 떄 회전속도
        [SerializeField]
        private float weaponRotateSpeed = 200f;

        // 무기 들었을 때 애니메이션 속도
        [SerializeField]
        private float weaponAnimationSpeed = 2.2f;

        #region METHODS

        /// <summary>
        /// Performs Ethan animation.
        /// </summary>
        /// 

        protected override void Move()
        {
            // 기본 이동 모드일 때,
            if(changeAnimatorLayer.isBaseLayer)
            {
                // 기본 이동
                base.Move();
            }
            // 무기 이동 모드일 때,
            else
            {
                float horizontalMove = moveDirection.x;
                float vertical = moveDirection.z;
                var desiredVelocity = CalcDesiredVelocity();
                if (useRootMotion && applyRootMotion)
                {
                    movement.Move(desiredVelocity, weaponWalkSpeed, !allowVerticalMovement);
                }

                else
                {
                    // Move with acceleration and friction
                    var currentFriction = isGrounded ? groundFriction : airFriction;
                    var currentBrakingFriction = useBrakingFriction ? brakingFriction : currentFriction;

                    movement.Move(desiredVelocity, weaponWalkSpeed, acceleration, deceleration, currentFriction,
                        currentBrakingFriction, !allowVerticalMovement);
                }

            }
        }
        // 이동 모드 마다 캐릭터 회전 방법이 다르므로 재정의
        public override void RotateTowardsMoveDirection(bool onlyLateral = true)
        {
            if (changeAnimatorLayer.isBaseLayer)
            {
                base.RotateTowardsMoveDirection();
            }
            else
            {
                // 마우스 위치를 받아서
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // 레이캐스트를 캐릭터의 높이에서 지면까지만 계산하도록 수정
                Plane groundPlane = new Plane(Vector3.up, transform.position);
                float rayDistance;

                if (groundPlane.Raycast(ray, out rayDistance))
                {
                    // 캐릭터의 높이에서 마우스 포인트까지의 정확한 위치를 계산
                    Vector3 pointToLook = ray.GetPoint(rayDistance);
                    Vector3 directionToLookAt = pointToLook - transform.position;
                    // Y축 회전만 고려
                    directionToLookAt.y = 0;

                    // 해당 방향을 향하도록 캐릭터를 회전
                    RotateTowards(directionToLookAt, onlyLateral);
                }
            }
        }


        protected override void Animate()
        {
            var move = transform.InverseTransformDirection(moveDirection);
            
            if (!UserStatusManager.Instance.CanMove)
            {
                move = transform.InverseTransformDirection(Vector3.zero);
            }
            // Compute move vector in local space

            // Update the animator parameters
            var forwardAmount = move.z;
            var rightAmount = move.x;

            // If there is no animator, return
            if (animator == null)
                return;

            // 기본 이동 모드일 때 애니메이션 설정
            if (changeAnimatorLayer.isBaseLayer)
            {
                animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
                animator.SetFloat("Turn", Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

                animator.SetBool("OnGround", movement.isGrounded);

                animator.SetBool("Crouch", isCrouching);

                if (!movement.isGrounded)
                    animator.SetFloat("Jump", movement.velocity.y, 0.1f, Time.deltaTime);

                // Calculate which leg is behind, so as to leave that leg trailing in the jump animation
                // (This code is reliant on the specific run cycle offset in our animations,
                // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)

                var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
                var jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

                if (movement.isGrounded)
                    animator.SetFloat("JumpLeg", jumpLeg);
            }
            // 무기사용 모드일 때 애니메이션 설정
            else
            {
                animator.SetFloat("Vertical", forwardAmount, 0.1f, Time.deltaTime);
                animator.SetFloat("Horizontal", rightAmount, 0.1f, Time.deltaTime);
                animator.SetFloat("WalkSpeed", weaponAnimationSpeed);
            }
        }

        #endregion
    }
}