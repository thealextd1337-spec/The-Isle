using System;
using UnityEngine;

namespace TheIsle
{
    /// <summary>Continuous swept collision prevents fast shells tunnelling through ships.</summary>
    public sealed class Cannonball : MonoBehaviour
    {
        private static Material shellMaterial;
        private Vector3 velocity;
        private ShipVitals owner;
        private ShipTeam ownerTeam;
        private int layer;
        private float travelled;
        private float lifetime;

        public static void Spawn(Vector3 position, Vector3 initialVelocity, ShipVitals firingShip, int flightLayer)
        {
            GameObject shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.name = "Kanonenkugel";
            shell.transform.position = position;
            shell.transform.localScale = Vector3.one * .5f;
            Collider collider = shell.GetComponent<Collider>();
            collider.enabled = false;
            Destroy(collider);
            if (shellMaterial == null)
            {
                Material includedPaint = Resources.Load<Material>("Art/Painted");
                Shader shader = Shader.Find("TheIsle/Painted Surface") ?? Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
                shellMaterial = includedPaint != null ? new Material(includedPaint) : new Material(shader);
                if (shellMaterial.HasProperty("_BaseColor")) shellMaterial.SetColor("_BaseColor", new Color(1f, .72f, .3f));
                else shellMaterial.color = new Color(1f, .72f, .3f);
            }
            shell.GetComponent<Renderer>().sharedMaterial = shellMaterial;
            TrailRenderer trail = shell.AddComponent<TrailRenderer>();
            trail.sharedMaterial = shellMaterial;
            trail.time = .25f;
            trail.startWidth = .18f;
            trail.endWidth = .02f;
            trail.startColor = new Color(1f, .85f, .55f, .8f);
            trail.endColor = new Color(1f, .65f, .2f, 0f);
            Cannonball projectile = shell.AddComponent<Cannonball>();
            projectile.velocity = initialVelocity;
            projectile.owner = firingShip;
            projectile.ownerTeam = firingShip.Team;
            projectile.layer = flightLayer;
            // Keep projectiles within the voyage tree so resetting a voyage removes them too.
            shell.transform.SetParent(firingShip.transform.parent, true);
        }

        private void FixedUpdate()
        {
            Vector3 step = velocity * Time.fixedDeltaTime;
            float distance = step.magnitude;
            if (distance < .00001f) { Destroy(gameObject); return; }
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, .25f, step / distance, distance, ~0, QueryTriggerInteraction.Ignore);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponentInParent<Cannonball>() != null) continue;
                ShipVitals target = hit.collider.GetComponentInParent<ShipVitals>();
                if (target != null)
                {
                    if (target == owner || target.Team == ownerTeam || target.IsDestroyed) continue;
                    ShipMotor targetMotor = target.GetComponent<ShipMotor>();
                    if (targetMotor == null || targetMotor.Layer != layer) continue;
                    target.Damage(8f, HitModule(target, hit.point));
                }
                transform.position = hit.point;
                Destroy(gameObject);
                return;
            }
            transform.position += step;
            travelled += distance;
            lifetime += Time.fixedDeltaTime;
            if (travelled >= BroadsideCombat.ShotRange || lifetime > 5f) Destroy(gameObject);
        }

        private static ShipModule HitModule(ShipVitals target, Vector3 point)
        {
            Vector3 local = target.transform.InverseTransformPoint(point);
            BoxCollider box = target.GetComponent<BoxCollider>();
            Vector3 size = box != null ? box.size : new Vector3(5f, 4f, 11f);
            if (box != null) local -= box.center;
            if (local.z < -size.z * .25f) return ShipModule.Engine;
            if (local.x < -size.x * .25f) return ShipModule.PortGuns;
            if (local.x > size.x * .25f) return ShipModule.StarboardGuns;
            return ShipModule.Hull;
        }
    }
}
