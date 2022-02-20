using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Synthesis;

public class BulletTest : MonoBehaviour {
    public GameObject PuppetA;
    public GameObject PuppetB;
    public GameObject PuppetC;

    public GameObject PointA;
    public GameObject PointB;

    private (Shape shape, PhysicsObject obj) _bodyA;
    private (Shape shape, PhysicsObject obj) _bodyB;
    private (Shape shape, PhysicsObject obj) _bodyC;
    private HingeConstraint _hingeA;
    private HingeConstraint _hingeB;

    private (Shape shape, PhysicsObject obj) _ground;

    private void Awake() {
        PhysicsHandler.DestroySimulation();
    }

    private void Start() {

        // Create Dynamic A
        _bodyA.shape = BoxShape.Create(new Vec3 { x = 0.5f, y = 0.5f, z = 0.5f });
        _bodyA.obj = PhysicsObject.Create(_bodyA.shape, 1);
        _bodyA.obj.ActivationState = ActivationState.DISABLE_DEACTIVATION;
        _bodyA.obj.Position = new Vec3 { x = 0f, y = 5f, z = 0f };

        // Create Dynamic B
        _bodyB.shape = BoxShape.Create(new Vec3 { x = 0.5f, y = 0.5f, z = 0.5f });
        _bodyB.obj = PhysicsObject.Create(_bodyB.shape, 1);
        _bodyB.obj.ActivationState = ActivationState.DISABLE_DEACTIVATION;
        _bodyB.obj.Position = new Vec3 { x = 1f, y = 4f, z = 0f };

        // Create Dynamic C
        _bodyC.shape = BoxShape.Create(new Vec3 { x = 0.5f, y = 0.5f, z = 0.5f });
        _bodyC.obj = PhysicsObject.Create(_bodyC.shape, 0);
        _bodyC.obj.ActivationState = ActivationState.DISABLE_DEACTIVATION;
        _bodyC.obj.Position = new Vec3 { x = 0f, y = 6f, z = 1f };

        Vec3 pivotA, pivotB, axis;

        // Create Hinge A
        // Vec3 pivotA = ToBullet(0.5f, -0.5f, 0.0f);
        // Vec3 pivotB = ToBullet(-0.5f, 0.5f, 0.0f);
        // Vec3 axis = ToBullet(0, 0, 1);
        // _hingeA = HingeConstraint.Create(_bodyA.obj, _bodyB.obj, pivotA, pivotB, axis, axis, -Mathf.PI / 6.0f, Mathf.PI / 6.0f);
        // _hingeA.Softness = 0.05f;

        // Create Hinge B
        pivotA = ToBullet(0.0f, 0.5f, 0.5f);
        pivotB = ToBullet(0.0f, -0.5f, -0.5f);
        axis = ToBullet(1, 0, 0);
        _hingeB = HingeConstraint.Create(_bodyA.obj, _bodyC.obj, pivotA, pivotB, axis, axis, -Mathf.PI / 6.0f, Mathf.PI / 4.0f);
        _hingeB.Softness = 0.05f;

        // Create Static Ground
        var big = BoxShape.Create(new Vec3 { x = 5.0f, y = 0.1f, z = 5.0f });
        var compound = CompoundShape.Create(1);
        compound.AddShape(big, new Vec3(), ToBullet(Quaternion.identity));
        var groundObj = PhysicsObject.Create(compound);
        groundObj.Position = new Vec3 { x = 0, y = -0.1f,  z = 0 };
        _ground = (compound, groundObj);
    }

    private MeshRenderer _pointARenderer = null;
    private void Update() {
        PhysicsHandler.Step(Time.deltaTime);

        UpdatePuppet(PuppetA, _bodyA.obj);
        UpdatePuppet(PuppetB, _bodyB.obj);
        UpdatePuppet(PuppetC, _bodyC.obj);

        // Raycast testing

        Debug.Log(Input.mousePosition);
        float castDistance = 20;
        Vector3 from = Camera.main.transform.position;
        Vector3 to = from + ((Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)) - from).normalized * castDistance);
        var res = PhysicsHandler.RayCastClosest(ToBullet(from), ToBullet(to));
        if (_pointARenderer == null)
            _pointARenderer = PointA.GetComponent<MeshRenderer>();
        if (res.hit) {
            _pointARenderer.enabled = true;
            PointA.transform.position = ToUnity(res.hit_point);
        } else {
            _pointARenderer.enabled = false;
        }
        PointB.transform.position = to;
    }

    private void UpdatePuppet(GameObject puppet, PhysicsObject phys) {
        puppet.transform.position = ToUnity(phys.Position);
        puppet.transform.rotation = ToUnity(phys.Rotation);
    }

    public static Vec3 ToBullet(float x, float y, float z) => new Vec3 { x = x, y = y, z = z };
    public static Vec3 ToBullet(Vector3 v) => new Vec3 { x = v.x, y = v.y, z = v.z };
    public static Vector3 ToUnity(Vec3 v) => new Vector3(v.x, v.y, v.z);
    public static Quat ToBullet(float x, float y, float z, float w) => new Quat { x = x, y = y, z = z, w = w };
    public static Quat ToBullet(Quaternion q) => new Quat { x = q.x, y = q.y, z = q.z, w = q.w };
    public static Quaternion ToUnity(Quat q) => new Quaternion(q.x, q.y, q.z, q.w);
}

// namespace Synthesis {
//     public partial struct Vec3 {
//         public static implicit operator Vector3(Vec3 v) => new Vector3(v.x, v.y, v.z);
//     }
// }
