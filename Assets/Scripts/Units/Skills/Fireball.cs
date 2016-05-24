﻿using Interfaces;
using UnityEngine;
using Library;
using UI;
using Event = Define.Event;

namespace Units.Skills
{
    public class Fireball : MonoBehaviour, IMovable, ICastable<IUsesSkills>
    {
        #region -- VARIABLES --
        [SerializeField, ReadOnly]
        private SkillData m_SkillData;

        [SerializeField]
        private float m_CurrentLifetime;
        [SerializeField]
        private float m_MaxLifetime;

        [SerializeField]
        private Vector3 m_TotalVelocity;
        [SerializeField]
        private Vector3 m_Velocity;
        [SerializeField]
        private float m_Speed;

        [SerializeField]
        private Moving m_IsMoving;

        [SerializeField]
        private IUsesSkills m_Parent;

        [SerializeField]
        private Vector3 m_CurrentRotation;
        [SerializeField]
        private Vector3 m_OriginalRotation;
        #endregion

        #region -- PROPERTIES --
        public SkillData skillData
        {
            get { return m_SkillData; }
            set { m_SkillData = value; }
        }

        public float currentLifetime
        {
            get { return m_CurrentLifetime; }
        }
        public float maxLifetime
        {
            get { return m_MaxLifetime; }
            set { m_MaxLifetime = value; }
        }

        public Vector3 totalVelocity
        {
            get { return m_TotalVelocity; }
            set { m_TotalVelocity = value; }
        }
        public Vector3 velocity
        {
            get { return m_Velocity; }
            set { m_Velocity = value; }
        }

        public Moving isMoving
        {
            get { return m_IsMoving; }
            set { m_IsMoving = value; }
        }
        public bool canMoveWithInput { get; set; }

        public IUsesSkills parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public float speed
        {
            get { return m_Speed; }
            set { m_Speed = value; }
        }
        #endregion

        #region -- UNITY FUNCTIONS --
        // Use this for initialization
        private void Start()
        {
            m_OriginalRotation = transform.eulerAngles;
            m_CurrentRotation = m_OriginalRotation;
        }

        private void FixedUpdate()
        {
            Move();
        }

        // Update is called once per frame
        private void Update()
        {
            m_CurrentLifetime += Time.deltaTime;

            if (m_CurrentLifetime >= m_MaxLifetime)
                Destroy(gameObject);
        }

        private void LateUpdate()
        {
            SetRotation();
        }
        #endregion

        #region -- OTHER VOID FUNCTIONS
        private void SetRotation()
        {
            if (m_Velocity == Vector3.zero)
                return;

            float rotationY = 90 + Mathf.Atan(m_Velocity.x / m_Velocity.z) * (180.0f / Mathf.PI);

            if ((m_Velocity.x < 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x > 0.0f && m_Velocity.z < 0.0f) ||
                (m_Velocity.x == 0.0f && m_Velocity.z < 0.0f))
                rotationY += 180;

            m_CurrentRotation = new Vector3(
                m_OriginalRotation.x,
                rotationY,
                m_OriginalRotation.z);

            transform.rotation = Quaternion.Euler(m_CurrentRotation);
        }

        private void OnTriggerEnter(Collider a_Collision)
        {
            if (a_Collision.transform.gameObject != m_Parent.gameObject)
            {
                IAttackable attackableObject = a_Collision.transform.gameObject.GetComponent<IAttackable>();

                if (attackableObject != null && attackableObject.faction != m_Parent.faction)
                {
                    attackableObject.damageFSM.Transition(DamageState.TakingDamge);
                    Debug.Log("Hit " + attackableObject.unitName);
                    attackableObject.health -= m_SkillData.damage;
                    UIAnnouncer.self.FloatingText(
                        m_SkillData.damage,
                        a_Collision.transform.position,
                        FloatingTextType.MagicDamage);

                    if (a_Collision.transform.GetComponent<IStats>() != null && attackableObject.health <= 0)
                        m_Parent.experience += a_Collision.transform.GetComponent<IStats>().experience;
                    Destroy(gameObject);
                }
            }
        }

        public void Move()
        {
            transform.position += (m_Velocity + m_TotalVelocity) * Time.deltaTime;
        }
        #endregion
    }
}
