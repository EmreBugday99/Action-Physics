using System.Collections.Generic;
using UnityEngine;

namespace ActionPhysics
{
    public class PhysicsBody : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 previousPosition;
        public Collider physicsCollider;

        [HideInInspector]
        public List<PhysicsBody> collidingBodies = new List<PhysicsBody>();

        /// <summary>
        /// 0 = forward
        /// 1 = right
        /// 2 = up
        /// </summary>
        [HideInInspector]
        public float[] vectorForce = new float[3];
        public bool calculatePhysics;
        public float mass;
        public float friction = 0.1f;
        public float gravity = 0.1f;

        private void OnEnable()
        {
            PhysicsManager.PhysicsObjects.Add(this);
        }

        private void OnDisable()
        {
            PhysicsManager.PhysicsObjects.Remove(this);
        }

        private void Start()
        {
            previousPosition = transform.position;
        }

        public void AddForceForward(float velocity)
        {
            if (!calculatePhysics) return;

            float force = velocity / mass;
            vectorForce[0] += force;
        }

        public void AddForceRight(float velocity)
        {
            if (!calculatePhysics) return;

            float force = velocity / mass;
            vectorForce[1] += force;
        }

        public void AddForceUp(float velocity)
        {
            if (!calculatePhysics) return;

            float force = velocity / mass;
            vectorForce[2] += force;
        }
    }
}