using UnityEngine;

namespace Q3Movement {

    [RequireComponent(typeof(CharacterController))]
    public class Q3PlayerController : MonoBehaviour {

        [Header("Aiming")]
        [SerializeField] private Camera m_Camera;
        [SerializeField] private MouseLook m_MouseLook = new MouseLook();

        [Header("Movement")]
        [SerializeField] private float m_Friction = 6;
        [SerializeField] private float m_Gravity = 20;
        [SerializeField] private float m_JumpForce = 8;
        [Tooltip("Automatically jump when holding jump button")]
        [SerializeField] private bool m_AutoBunnyHop = false;
        [Tooltip("Air control precision")]
        [SerializeField] private float m_AirControl = 0.3f;
        [SerializeField] private MovementSettings m_GroundSettings = new MovementSettings(7, 14, 10);
        [SerializeField] private MovementSettings m_AirSettings = new MovementSettings(7, 2, 2);
        [SerializeField] private MovementSettings m_StrafeSettings = new MovementSettings(1, 50, 50);

        public float Speed { get { return m_Character.velocity.magnitude; } }
        public float MaxAirSpeed { get { return m_AirSettings.MaxSpeed; } }
        public float MaxGroundSpeed { get { return m_GroundSettings.MaxSpeed; } }


        public IInputProvider InputProvider {
            get { return m_InputProvider; }
            set { m_InputProvider = value; }
        }

        public void HandleInput() {
            m_MoveInput = new Vector3(m_InputProvider.GetMovementInput().x, 0, m_InputProvider.GetMovementInput().y);
            Debug.Log("GetMovementInput" + m_InputProvider.GetMovementInput());
            m_MouseLook.UpdateCursorLock();
            QueueJump();
            if (m_Character.isGrounded) {
                Debug.Log("GroundMove");
                GroundMove();
            }
            else {
                Debug.Log("AirMove");
                AirMove();
            }

            m_MouseLook.LookRotation(m_Tran, m_CamTran);
            m_Character.Move(m_PlayerVelocity * InputProvider.GetDeltaTime());
            Debug.Log(m_PlayerVelocity);
        }

        public void SetUpController() {
            m_Tran = transform;
            m_Character = GetComponent<CharacterController>();

            if (!m_Camera)
                m_Camera = Camera.main;

            m_CamTran = m_Camera.transform;
            m_MouseLook.Init(m_Tran, m_CamTran);

            if (m_InputProvider == null) {
                m_InputProvider = new UnityInputProvider();
            }
        }

        private CharacterController m_Character;
        private Vector3 m_MoveDirectionNorm = Vector3.zero;
        public Vector3 m_PlayerVelocity = Vector3.zero;
        private bool m_JumpQueued = false;
        private float m_PlayerFriction = 0;
        private Vector3 m_MoveInput;
        private Transform m_Tran;
        private Transform m_CamTran;
        private IInputProvider m_InputProvider;

        private void Start() {
            SetUpController();
        }

        private void Update() {
            HandleInput();
        }

        private void QueueJump() {
            if (m_AutoBunnyHop) {
                m_JumpQueued = m_InputProvider.GetJumpButton();
                return;
            }

            if (m_InputProvider.GetJumpButtonDown() && !m_JumpQueued) {
                m_JumpQueued = true;
            }

            else {
                m_JumpQueued = false;
            }
        }

        // Handle air movement.
        private void AirMove() {
            float accel;

            var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
            wishdir = m_Tran.TransformDirection(wishdir);

            float wishspeed = wishdir.magnitude;
            wishspeed *= m_AirSettings.MaxSpeed;

            wishdir.Normalize();
            m_MoveDirectionNorm = wishdir;

            // CPM Air control.
            float wishspeed2 = wishspeed;
            if (Vector3.Dot(m_PlayerVelocity, wishdir) < 0) {
                accel = m_AirSettings.Deceleration;
            }
            else {
                accel = m_AirSettings.Acceleration;
            }

            // If the player is ONLY strafing left or right
            if (m_MoveInput.z == 0 && m_MoveInput.x != 0) {
                if (wishspeed > m_StrafeSettings.MaxSpeed) {
                    wishspeed = m_StrafeSettings.MaxSpeed;
                }

                accel = m_StrafeSettings.Acceleration;
            }

            Accelerate(wishdir, wishspeed, accel);
            if (m_AirControl > 0) {
                AirControl(wishdir, wishspeed2);
            }

            // Apply gravity
            m_PlayerVelocity.y -= m_Gravity * InputProvider.GetDeltaTime();
        }

        // Air control occurs when the player is in the air, it allows players to move side 
        // to side much faster rather than being 'sluggish' when it comes to cornering.
        private void AirControl(Vector3 targetDir, float targetSpeed) {
            // Only control air movement when moving forward or backward.
            if (Mathf.Abs(m_MoveInput.z) < 0.001 || Mathf.Abs(targetSpeed) < 0.001) {
                return;
            }

            float zSpeed = m_PlayerVelocity.y;
            m_PlayerVelocity.y = 0;
            /* Next two lines are equivalent to idTech's VectorNormalize() */
            float speed = m_PlayerVelocity.magnitude;
            m_PlayerVelocity.Normalize();

            float dot = Vector3.Dot(m_PlayerVelocity, targetDir);
            float k = 32;
            k *= m_AirControl * dot * dot * InputProvider.GetDeltaTime();

            // Change direction while slowing down.
            if (dot > 0) {
                m_PlayerVelocity.x *= speed + targetDir.x * k;
                m_PlayerVelocity.y *= speed + targetDir.y * k;
                m_PlayerVelocity.z *= speed + targetDir.z * k;

                m_PlayerVelocity.Normalize();
                m_MoveDirectionNorm = m_PlayerVelocity;
            }

            m_PlayerVelocity.x *= speed;
            m_PlayerVelocity.y = zSpeed; // Note this line
            m_PlayerVelocity.z *= speed;
        }

        // Handle ground movement.
        internal void GroundMove() {
            // Do not apply friction if the player is queueing up the next jump
            if (!m_JumpQueued) {
                ApplyFriction(1.0f);
            }
            else {
                ApplyFriction(0);
            }

            var wishdir = new Vector3(m_MoveInput.x, 0, m_MoveInput.z);
            wishdir = m_Tran.TransformDirection(wishdir);
            wishdir.Normalize();
            m_MoveDirectionNorm = wishdir;

            var wishspeed = wishdir.magnitude;
            wishspeed *= m_GroundSettings.MaxSpeed;

            Accelerate(wishdir, wishspeed, m_GroundSettings.Acceleration);

            // Reset the gravity velocity
            m_PlayerVelocity.y = -m_Gravity * InputProvider.GetDeltaTime();

            if (m_JumpQueued) {
                m_PlayerVelocity.y = m_JumpForce;
                m_JumpQueued = false;
            }
        }

        private void ApplyFriction(float t) {
            // Equivalent to VectorCopy();
            Vector3 vec = m_PlayerVelocity;
            vec.y = 0;
            float speed = vec.magnitude;
            float drop = 0;

            // Only apply friction when grounded.
            if (m_Character.isGrounded) {
                float control = speed < m_GroundSettings.Deceleration ? m_GroundSettings.Deceleration : speed;
                drop = control * m_Friction * InputProvider.GetDeltaTime() * t;
            }

            float newSpeed = speed - drop;
            m_PlayerFriction = newSpeed;
            if (newSpeed < 0) {
                newSpeed = 0;
            }

            if (speed > 0) {
                newSpeed /= speed;
            }

            m_PlayerVelocity.x *= newSpeed;
            // playerVelocity.y *= newSpeed;
            m_PlayerVelocity.z *= newSpeed;
        }

        // Calculates acceleration based on desired speed and direction.
        private void Accelerate(Vector3 targetDir, float targetSpeed, float accel) {
            float currentspeed = Vector3.Dot(m_PlayerVelocity, targetDir);
            float addspeed = targetSpeed - currentspeed;
            if (addspeed <= 0) {
                return;
            }

            float accelspeed = accel * InputProvider.GetDeltaTime() * targetSpeed;
            if (accelspeed > addspeed) {
                accelspeed = addspeed;
            }

            m_PlayerVelocity.x += accelspeed * targetDir.x;
            m_PlayerVelocity.z += accelspeed * targetDir.z;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (m_Character.isGrounded) {
                return;
            }

            if (Vector3.Dot(hit.normal, Vector3.up) < 0.5f) {
                m_PlayerVelocity = Vector3.ProjectOnPlane(m_PlayerVelocity, hit.normal);
            }
        }
    }
}