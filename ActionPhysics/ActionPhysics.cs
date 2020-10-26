using System.Collections.Generic;
using UnityEngine;

namespace ActionEngine.Physics
{
    [AddComponentMenu("ActionEngine/Action Physics")]
    public class ActionPhysics : MonoBehaviour
    {
        public static List<ActionPhysics> PhysicsObjects = new List<ActionPhysics>();

        public Collider objectCollider;

        [HideInInspector]
        public List<ActionPhysics> collidingObjects = new List<ActionPhysics>();

        public delegate void CollidedWithOther(ActionPhysics other);
        public event CollidedWithOther OnCollision;

        public delegate void CollisionStopWithOther(ActionPhysics other);
        public event CollisionStopWithOther OnCollisionExit;

        /// <summary>
        /// 0 = forward
        /// 1 = right
        /// 2 = up
        /// </summary>
        [HideInInspector]
        public float[] vectorForce = new float[3];

        public bool calculatePhysics = true;
        public float mass;
        public float friction = 0.1f;
        public float gravity = 0.1f;

        private void OnEnable()
        {
            PhysicsObjects.Add(this);
            OnCollision += OnCollisionDetect;
        }

        private void OnDisable()
        {
            PhysicsObjects.Remove(this);
            OnCollision -= OnCollisionDetect;
        }

        private void FixedUpdate()
        {
            CollisionCheck();

            if (calculatePhysics)
            {
                ApplyFriction();
                ApplyGravity();
                ApplyMomentum();
            }
        }

        private void CollisionCheck()
        {
            foreach (var other in PhysicsObjects)
            {
                if (other.objectCollider == this.objectCollider) return;

                if (objectCollider.bounds.Intersects(other.objectCollider.bounds))
                {
                    if (!collidingObjects.Contains(other))
                    {
                        collidingObjects.Add(other);
                        other.collidingObjects.Add(this);
                        OnCollision?.Invoke(other);
                        OnPhysicsCollision(other);
                        other.OnCollision?.Invoke(this);
                    }
                }
                else if (collidingObjects.Contains(other))
                {
                    collidingObjects.Remove(other);
                    other.collidingObjects.Remove(this);
                    OnCollisionExit?.Invoke(other);
                    other.OnCollisionExit?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Checks if this object is colliding with any other ActionPhysics.
        /// </summary>
        /// <returns>Returns true if colliding with any ActionPhysics</returns>
        public bool IsColliding()
        {
            return collidingObjects.Count > 0;
        }

        private void ApplyMomentum()
        {
            //Forward
            if (friction < vectorForce[0])
                transform.Translate((Vector3.forward * vectorForce[0]) * Time.fixedDeltaTime);
            //Backward
            if (friction > vectorForce[0])
                transform.Translate((Vector3.forward * vectorForce[0]) * Time.fixedDeltaTime);
            //Right
            if (friction < vectorForce[1])
                transform.Translate((Vector3.right * vectorForce[1]) * Time.fixedDeltaTime);
            //Left
            if (friction > vectorForce[1])
                transform.Translate((Vector3.right * vectorForce[1]) * Time.fixedDeltaTime);
            //Up
            if (vectorForce[2] > 0)
                transform.Translate((Vector3.up * vectorForce[2]) * Time.fixedDeltaTime);
            //Down
            if (vectorForce[2] < 0)
                transform.Translate((Vector3.up * vectorForce[2]) * Time.fixedDeltaTime);
        }

        private void ApplyFriction()
        {
            //Forward
            if (vectorForce[0] > 0.0f)
            {
                if (vectorForce[0] - friction < 0)
                {
                    vectorForce[0] = 0;
                    return;
                }

                vectorForce[0] -= friction;
            }
            //Backward
            if (vectorForce[0] < 0.0f)
            {
                if (vectorForce[0] + friction > 0)
                {
                    vectorForce[0] = 0;
                    return;
                }

                vectorForce[0] += friction;
            }
            //Right
            if (vectorForce[1] > 0.0f)
            {
                if (vectorForce[1] - friction < 0)
                {
                    vectorForce[1] = 0;
                    return;
                }

                vectorForce[1] -= friction;
            }
            //Left
            if (vectorForce[1] < 0.0f)
            {
                if (vectorForce[1] + friction > 0)
                {
                    vectorForce[1] = 0;
                    return;
                }

                vectorForce[1] += friction;
            }
        }

        private void ApplyGravity()
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.cyan);
            if (!IsColliding())
            {
                vectorForce[2] -= gravity;
            }
        }

        private void OnPhysicsCollision(ActionPhysics other)
        {
            if (other.calculatePhysics)
            {
                other.vectorForce[0] = vectorForce[0] / 2;
                other.vectorForce[1] = vectorForce[1] / 2;
            }

            vectorForce[0] = 0.0f;
            vectorForce[1] = 0.0f;
        }

        private void OnCollisionDetect(ActionPhysics other)
        {
            //TODO: Implement raycast for gravity

            if (UnityEngine.Physics.Raycast(transform.position, Vector3.down, out var hit))
            {
                if (other.gameObject == hit.transform.gameObject)
                    vectorForce[2] = 0;
            }
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