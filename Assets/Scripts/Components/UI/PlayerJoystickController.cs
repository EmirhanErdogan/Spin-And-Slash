using Emir;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Highborn
{
    public class PlayerJoystickController : MonoBehaviour
    {
        public bool rotationLock;
        public bool movementLock;
        public bool animationLock;
        public bool soundLock;
        public bool muteFootSound;
        public float maxAnimationSpeed = 1;
        public bool reverseAnimation;
        [SerializeField] float playerRotationSpeed = 10;

        public float movementSpeed
        {
            get { return agent.speed; }
            set { agent.speed = value; }
        }

        [SerializeField] Animator animator;

        public Animator Animator
        {
            get { return animator; }
        }

        public NavMeshAgent agent { get; private set; }
        Vector3 direction;
        bool running;
        float runAnimState;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            HandleDirection();

            HandleRotation();

            HandlePosition();

            HandleAnimation();

            if (Input.GetMouseButtonUp(0))
            {
                if (agent.enabled)
                    agent.SetDestination(transform.position);
            }
        }

        public void ResetAgent()
        {
            agent.SetDestination(transform.position);
        }

        private void HandleDirection()
        {
            direction = (Vector3.forward * InterfaceManager.Instance.GetJoystick().Vertical) +
                        (Vector3.right * InterfaceManager.Instance.GetJoystick().Horizontal);
        }

        private void HandleRotation()
        {
            if (rotationLock)
                return;

            if (Input.GetMouseButton(0))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    playerRotationSpeed * Time.deltaTime);
            }
        }

        private void HandlePosition()
        {
            if (movementLock)
                return;

            if (InterfaceManager.Instance.GetJoystick().Horizontal != 0 ||
                InterfaceManager.Instance.GetJoystick().Vertical != 0)
            {
                agent.SetDestination(transform.position + transform.forward);
            }
        }

        private void HandleAnimation()
        {
            if (animationLock)
                return;

            if (InterfaceManager.Instance.GetJoystick().Horizontal != 0 ||
                InterfaceManager.Instance.GetJoystick().Vertical != 0)
            {
                //animator.SetFloat("Move", reverseAnimation ? -maxAnimationSpeed : maxAnimationSpeed);

                runAnimState = Mathf.Lerp(runAnimState, maxAnimationSpeed, Time.deltaTime * 20);

                animator.SetFloat("Move", reverseAnimation ? -runAnimState : runAnimState);
            }
            else
            {
                runAnimState = Mathf.Lerp(runAnimState, 0, Time.deltaTime * 10);

                animator.SetFloat("Move", runAnimState);

                //runAnimState = 0;
                //running = false;
                //animator.SetFloat("Move", 0);
            }
        }

        public void LockAll()
        {
            animationLock = true;
            movementLock = true;
            rotationLock = true;
            animationLock = true;
            agent.enabled = false;
            soundLock = true;

            InterfaceManager.Instance.DeactivateJoystick();

            animator.SetFloat("Move", 0);
        }

        public void UnlockAll()
        {
            animationLock = false;
            movementLock = false;
            rotationLock = false;
            animationLock = false;
            agent.enabled = true;
            soundLock = false;

            InterfaceManager.Instance.ActivateJoystick();
        }
    }
}