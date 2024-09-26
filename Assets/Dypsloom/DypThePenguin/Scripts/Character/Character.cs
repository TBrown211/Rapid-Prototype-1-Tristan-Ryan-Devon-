/// ---------------------------------------------
/// Dyp The Penguin Character | Dypsloom
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.DypThePenguin.Scripts.Character
{
    using Dypsloom.DypThePenguin.Scripts.Damage;
    using Dypsloom.DypThePenguin.Scripts.Items;
    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using CharacterController = UnityEngine.CharacterController;

    /// <summary>
    /// The character controller.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour
    {
        public event Action OnDie;
        
        [Tooltip("The character speed in units/second.")]
        [SerializeField] protected float m_Speed = 1f;
        [Tooltip("The gravity.")]
        [SerializeField] protected float m_Gravity = 1f;
        [Tooltip("Grounded error correction time.")]
        [SerializeField] protected float m_AdditionalGroundedTime = 0.5f;
        [Tooltip("Grounded Raycast length.")]
        [SerializeField] protected float m_GroundRaycastLength = 0.3f;
        [Tooltip("The character jump force.")]
        [SerializeField] protected float m_JumpForce = 1f;
        [Tooltip("The gravity modifier while pressing the jump button.")]
        [SerializeField] protected float m_JumpFallOff = 1f;
        [Tooltip("The character speed in units/second.")]
        [SerializeField] protected Transform m_SpawnTransform;
        [Tooltip("The delay between death and respawn.")]
        [SerializeField] protected float m_RespawnDelay = 3f;
        [Tooltip("The character speed in units/second.")]
        [SerializeField] protected float m_PushPower = 2.0f;
        [Tooltip("The transform where the projectiles thrown will spawn From.")]
        [SerializeField] protected Transform m_ProjectilesSpawnPoint;
        [Tooltip("Death Effect.")]
        [SerializeField] protected GameObject m_DeathEffects;

        //Jetpack Components - Ryan
        [SerializeField] protected float jetpackForce = 10f; //The applied upward force on jetpack
        [SerializeField] protected float maxFuel = 10f; //Max amount of fuel 
        [SerializeField] protected float fuelConsumption = 1f; //The consumption rate of the fuel
        [SerializeField] protected float fuelRecharge = 5f; //The rate at which the fuel recharges

        protected bool isFlying; //Boolean checking when the player is flying
        protected float currentFuel; //This will keep track of player's fuel amount

        public ParticleSystem jetFlames;
        //-------------------------------------------------------------------------------

        protected Rigidbody m_Rigidbody;
        protected CharacterController m_CharacterController;
        protected Animator m_Animator;
        protected Inventory m_Inventory;
        protected IDamageable m_CharacterDamageable;

        protected ICharacterMover m_CharacterMover;
        protected CharacterRotator m_CharacterRotator;
        protected ICharacterInput m_CharacterInput;
        protected ICharacterAnimator m_CharacterAnimator;

        protected Camera m_Camera;
        protected bool m_IsDead;
        protected Task m_DeathTask;
        private float m_GroundedTimer;

        public float Speed => m_Speed;
        public float JumpForce => m_JumpForce;
        public float JumpFallOff => m_JumpFallOff;
        public float Gravity => m_Gravity;
        public Camera CharacterCamera => m_Camera;

        public Transform ProjectilesSpawnPoint => m_ProjectilesSpawnPoint;

        public Rigidbody Rigidbody => m_Rigidbody;
        public Animator Animator => m_Animator;
        public CharacterController CharacterController => m_CharacterController;
        
        public ICharacterInput CharacterInput => m_CharacterInput;
        public IDamageable  CharacterDamageable => m_CharacterDamageable;
        public ICharacterMover CharacterMover => m_CharacterMover;
        public ICharacterAnimator CharacterAnimator => m_CharacterAnimator;
        public Inventory Inventory => m_Inventory;
        public bool IsDead => m_IsDead;
        public bool IsGrounded
        {
            get => m_GroundedTimer >= Time.time;
            set => m_GroundedTimer = value ? Time.time + m_AdditionalGroundedTime : 0;
        }

        /// <summary>
        /// Initialize all the properties.
        /// </summary>
        protected virtual void Awake()
        {
            m_Camera = Camera.main;
            currentFuel = maxFuel;

            m_Rigidbody = GetComponent<Rigidbody>();
            m_CharacterController = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_Inventory = GetComponent<Inventory>();
            m_CharacterDamageable = GetComponent<IDamageable>();
            

            AssignCharacterControllers();
        }
        
        /// <summary>
        /// Assign the controllers for your character.
        /// </summary>
        protected virtual void AssignCharacterControllers()
        {
            m_CharacterMover = new CharacterMover(this);
            m_CharacterRotator = new CharacterRotator(this);
            m_CharacterAnimator = new CharacterAnimator(this);
            m_CharacterInput = new CharacterInput(this);
        }
        
        /// <summary>
        /// Tick all the properties which needs to update every frame.
        /// </summary>
        protected virtual void Update()
        {
            if (m_CharacterController.isGrounded ) {
                IsGrounded = true;
            } else if (
                Physics.Raycast(transform.position + transform.up*0.5f, -1f*transform.up, out var hit,
                    m_GroundRaycastLength+0.5f, int.MaxValue , QueryTriggerInteraction.Ignore)) {
                if (m_CharacterMover.IsJumping == false) {
                    IsGrounded = true;
                } else {
                    IsGrounded = false;
                }
               
            }

            if(Input.GetKeyDown(KeyCode.Space) && currentFuel > 0)
            {
                m_Gravity = 0f; 
            }
            else
            {
                m_Gravity = 1f;
            }

            m_CharacterMover.Tick();
            m_CharacterRotator.Tick();
            m_CharacterAnimator.Tick();
            JetpackMechanic();
        }

        /// <summary>
        /// Move objects when the character controller hits a collider.
        /// </summary>
        /// <param name="hit">The controller collider hit object.</param>
        protected virtual void  OnControllerColliderHit (ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;
 
            // no rigidbody
            if (body == null || body.isKinematic) { return; }
 
            Vector3 force;
            // We use gravity and weight to push things down, we use
            // our velocity and push power to push things other directions
            if (hit.moveDirection.y < -0.3) {
                force = (new Vector3 (0.1f, -0.5f, 0.1f) + hit.controller.velocity) * m_Rigidbody.mass;
            } else {
                force = m_CharacterMover.CharacterInputMovement * m_PushPower;
            }
 
            // Apply the push
            body.AddForceAtPosition(force, hit.point);
        }
        
        /// <summary>
        /// Make the character die.
        /// </summary>
        public virtual void Die()
        {
            if(m_IsDead || m_DeathTask != null){return;}
            m_DeathTask = ScheduleDeathRespawn();
        }

        /// <summary>
        /// Respawn 
        /// </summary>
        /// <returns>Return the asynchronous task.</returns>
        protected virtual async Task ScheduleDeathRespawn()
        {
            CharacterAnimator.Die(true);
            m_IsDead = true;
            
            await Task.Delay(1100);
            m_DeathEffects?.SetActive(true);

            await Task.Delay((int)(m_RespawnDelay*1000f)-1600);
            gameObject.SetActive(false);
            OnDie?.Invoke();

            await Task.Delay(500);
            m_DeathTask = null;
            Respawn();
        }

        /// <summary>
        /// Respawn the character.
        /// </summary>
        protected virtual void Respawn()
        {
            m_DeathEffects?.SetActive(false);
            CharacterAnimator.Die(false);
            m_CharacterDamageable.Heal(int.MaxValue);
            transform.position = m_SpawnTransform != null ? m_SpawnTransform.position : new Vector3(0,1,0);
            gameObject.SetActive(true);
            m_IsDead = false;
        }
       
        //Ryan - Jetpack Mechanic Class
        protected void JetpackMechanic()
        {
            if(Input.GetKey(KeyCode.Space) && currentFuel > 0)
            {
                ActivateJetpack();
            }
            else
            {
                DeactivateJetpack();
            }

            if (!isFlying && currentFuel < maxFuel)
            {
                currentFuel += fuelRecharge * Time.deltaTime; //Fuel recharges when on ground
                currentFuel = Mathf.Min(currentFuel, maxFuel); //Cap fuel at max value
            }
        }

        protected void ActivateJetpack()
        {
            isFlying = true;

            Vector3 flyForce = Vector3.up * jetpackForce; //Setting and applying force
            m_CharacterController.Move(flyForce * Time.deltaTime); 

            currentFuel -= fuelConsumption * Time.deltaTime; //Reducing the fuel amount
        }

        protected void DeactivateJetpack()
        {
            isFlying = false;
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 20, 200 * (currentFuel / maxFuel), 20), $"Fuel: {Mathf.Ceil(currentFuel)}");
        }
        //------------------------------------------------------------------------------
    }
}

