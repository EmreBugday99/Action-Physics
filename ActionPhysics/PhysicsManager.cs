using System.Collections.Generic;
using UnityEngine;

namespace ActionPhysics
{
    public class PhysicsManager : MonoBehaviour
    {
        public static List<PhysicsBody> PhysicsObjects = new List<PhysicsBody>();
        public static float ElasticRate = 2.0f;

        private void FixedUpdate()
        {
            SimulateFriction();
            SimulateGravity();
            SimulateVelocity();
            CollisionDetection();
        }

        private static void SimulateFriction()
        {
            foreach (var body in PhysicsObjects)
            {
                if (!body.calculatePhysics) continue;

                //Forward
                if (body.vectorForce[0] > 0.0f)
                {
                    if ((body.vectorForce[0] - body.friction) < 0)
                    {
                        body.vectorForce[0] = 0;
                        return;
                    }

                    body.vectorForce[0] -= body.friction;
                }

                //Backward
                if (body.vectorForce[0] < 0.0f)
                {
                    if (body.vectorForce[0] + body.friction > 0)
                    {
                        body.vectorForce[0] = 0;
                        return;
                    }

                    body.vectorForce[0] += body.friction;
                }

                //Right
                if (body.vectorForce[1] > 0.0f)
                {
                    if ((body.vectorForce[1] - body.friction) < 0)
                    {
                        body.vectorForce[1] = 0;
                        return;
                    }

                    body.vectorForce[1] -= body.friction;
                }

                //Left
                if (body.vectorForce[1] < 0.0f)
                {
                    if ((body.vectorForce[1] + body.friction) > 0)
                    {
                        body.vectorForce[1] = 0;
                        return;
                    }

                    body.vectorForce[1] += body.friction;
                }
            }
        }

        private static void SimulateGravity()
        {
            foreach (var body in PhysicsObjects)
            {
                if (!body.calculatePhysics) continue;

                body.vectorForce[2] -= body.gravity;
            }
        }

        private static void SimulateVelocity()
        {
            foreach (var body in PhysicsObjects)
            {
                if (!body.calculatePhysics) continue;

                body.previousPosition = body.transform.position;

                //Forward
                if (body.friction < body.vectorForce[0])
                    body.transform.Translate((Vector3.forward * body.vectorForce[0]) * Time.fixedDeltaTime);
                //Backward
                if (body.friction > body.vectorForce[0])
                    body.transform.Translate((Vector3.forward * body.vectorForce[0]) * Time.fixedDeltaTime);
                //Right
                if (body.friction < body.vectorForce[1])
                    body.transform.Translate((Vector3.right * body.vectorForce[1]) * Time.fixedDeltaTime);
                //Left
                if (body.friction > body.vectorForce[1])
                    body.transform.Translate((Vector3.right * body.vectorForce[1]) * Time.fixedDeltaTime);
                //Up
                if (body.vectorForce[2] > 0.0f)
                    body.transform.Translate((Vector3.up * body.vectorForce[2]) * Time.fixedDeltaTime);
                //Down
                if (body.vectorForce[2] < 0.0f)
                    body.transform.Translate((Vector3.up * body.vectorForce[2]) * Time.fixedDeltaTime);
            }
        }

        private static void CollisionDetection()
        {
            foreach (var body in PhysicsObjects)
            {
                foreach (var otherBody in PhysicsObjects)
                {
                    if (body == otherBody) continue;
                    Bounds bounds = body.physicsCollider.bounds;
                    Bounds otherBounds = otherBody.physicsCollider.bounds;

                    if (!bounds.Intersects(otherBounds)) continue;

                    // Y Min -> Y Max Check : My bottom colliding with other top
                    if (Mathf.Abs(bounds.min.y - otherBounds.max.y) < body.physicsCollider.bounds.size.y)
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(oldPos.x, body.previousPosition.y, oldPos.z);
                        if (body.vectorForce[2] < 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[2] = body.vectorForce[2] / ElasticRate;

                            body.vectorForce[2] = -body.vectorForce[2] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }

                    //Y Max -> Y Min Check : My top colliding with other bottom
                    if (Mathf.Abs(bounds.max.y - otherBounds.min.y) < (body.physicsCollider.bounds.size.y))
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(oldPos.x, body.previousPosition.y, oldPos.z);
                        if (body.vectorForce[2] > 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[2] = body.vectorForce[2] / ElasticRate;

                            body.vectorForce[2] = -body.vectorForce[2] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }

                    // X Min -> X Max Check : My Left colliding with other right
                    if (Mathf.Abs(bounds.min.x - otherBounds.max.x) < body.physicsCollider.bounds.size.y)
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(body.previousPosition.x, oldPos.y, oldPos.z);
                        if (body.vectorForce[1] < 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[1] = body.vectorForce[1] / ElasticRate;

                            body.vectorForce[1] = -body.vectorForce[1] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }

                    // X Max -> X Min Check : My right colliding with other left
                    if (Mathf.Abs(bounds.max.x - otherBounds.min.x) < body.physicsCollider.bounds.size.y)
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(body.previousPosition.x, oldPos.y, oldPos.z);
                        if (body.vectorForce[1] > 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[1] = body.vectorForce[1] / ElasticRate;

                            body.vectorForce[1] = -body.vectorForce[1] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }

                    // Z Min -> Z Max Check : My back colliding with other forward
                    if (Mathf.Abs(bounds.min.z - otherBounds.max.z) < body.physicsCollider.bounds.size.y)
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(oldPos.x, oldPos.y, body.previousPosition.z);
                        if (body.vectorForce[0] < 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[0] = body.vectorForce[0] / ElasticRate;

                            body.vectorForce[0] = -body.vectorForce[0] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }

                    // Z Max -> Z Min Check : My forward colliding with other back
                    if (Mathf.Abs(bounds.max.z - otherBounds.min.z) < body.physicsCollider.bounds.size.y)
                    {
                        Vector3 oldPos = body.transform.position;
                        Vector3 newPosition = new Vector3(oldPos.x, oldPos.y, body.previousPosition.z);
                        if (body.vectorForce[0] > 0.0f)
                        {
                            if (otherBody.calculatePhysics)
                                otherBody.vectorForce[0] = body.vectorForce[0] / ElasticRate;

                            body.vectorForce[0] = -body.vectorForce[0] / ElasticRate;
                            body.transform.position = newPosition;
                        }
                    }
                }
            }
        }
    }
}