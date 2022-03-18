using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Synthesis;

public class BulletTest : MonoBehaviour {
    public BulletPuppet PuppetA;
    public BulletPuppet PuppetB;
    public BulletPuppet Ground;

    private HingeConstraint _hinge;

    public Mesh CubeMesh;
    public Mesh SphereMesh;
    public Material CubeMaterial;

    private void Awake() {
        PhysicsHandler.DestroySimulation();
    }

    private void Start() {

        BulletSetup();

        // Create Dynamic A
        // var shapeA = BoxShape.Create(new Vec3 { x = 0.5f, y = 0.5f, z = 0.5f });
        // var objA = PhysicsObject.Create(shapeA, 10);
        // objA.ActivationState = ActivationState.DISABLE_DEACTIVATION;
        // objA.Position = new Vec3 { x = 0f, y = 5f, z = 0f };
        // objA.Damping = (0.05f, 0.0f);
        // PuppetA.BulletRep = objA;

        // // Create Dynamic B
        // var shapeB = BoxShape.Create(new Vec3 { x = 0.5f, y = 0.5f, z = 0.5f });
        // var objB = PhysicsObject.Create(shapeB, 10);
        // objB.ActivationState = ActivationState.DISABLE_DEACTIVATION;
        // objB.Position = new Vec3 { x = 1f, y = 6f, z = 0f };
        // objB.Damping = (0.05f, 0.0f);
        // PuppetB.BulletRep = objB;

        // // Create Hinge A
        // Vec3 pivotA = ToBullet( 0.5f,  0.5f,  0.0f);
        // Vec3 pivotB = ToBullet(-0.5f, -0.5f,  0.0f);
        // Vec3 axis = ToBullet(0, 0, 1);
        // _hinge = HingeConstraint.Create(objA, objB, pivotA, pivotB, axis, axis, -Mathf.PI / 2.0f, Mathf.PI / 2.0f);
        // _hinge.LimitSoftness = 0.05f;
        // _hinge.MotorMaxImpulse = 1f;
        // _hinge.MotorTargetVelocity = -Mathf.PI;
        // _hinge.MotorEnable = true;

        // Create Static Ground
    }

    private StreamWriter sw;
    private void BulletSetup() {
        PhysicsHandler.SetWorldGravity(ToBullet(0.0f, -9.81f, 0.0f));

        PhysicsHandler.SetNumIteration(10);

        var groundShape = BoxShape.Create(ToBullet(10f, 0.1f, 10f));
        var groundObj = PhysicsObject.Create(groundShape);
        groundObj.Position = ToBullet(0, -2, 0);
        var q = ToBullet(Quaternion.Euler(20, 0, 0));
        Debug.Log($"{q.x}, {q.y}, {q.z}, {q.w}");
        groundObj.Rotation = q;
        
        Ground.BulletRep = groundObj;

        CreateConvexObject(SphereMesh);

        // int width = 5;
        // int length = 5;

        // int objectCount = 0;

        // for (int x = -width; x <= width; x++) {
        //     for (int z = -length; z <= length; z++) {
        //         for (int y = 4; y < 4 + 10; y++) {

        //             objectCount++;

        //             bool useCube = UnityEngine.Random.Range(0, 2) == 0;

        //             GameObject g = new GameObject($"{x},{y},{z}");
        //             var puppet = g.AddComponent<BulletPuppet>();
        //             Shape shape;
        //             if (useCube)
        //                 shape = BoxShape.Create(ToBullet(0.5f, 0.5f, 0.5f));
        //             else
        //                 shape = SphereShape.Create(0.5f);
        //             var obj = PhysicsObject.Create(shape, 1);
        //             obj.Position = ToBullet(x, y, z);
        //             puppet.BulletRep = obj;

        //             var filter = g.AddComponent<MeshFilter>();
        //             if (useCube)
        //                 filter.mesh = CubeMesh;
        //             else
        //                 filter.mesh = SphereMesh;
        //             var renderer = g.AddComponent<MeshRenderer>();
        //             renderer.material = CubeMaterial;
        //         }
        //     }
        // }

        // Debug.Log($"Num Objects: {objectCount}");

        // var big = BoxShape.Create(new Vec3 { x = 10.0f, y = 0.1f, z = 10.0f });
        // var compound = CompoundShape.Create(1);
        // compound.AddShape(big, new Vec3(), ToBullet(Quaternion.identity));
        // var groundObj = PhysicsObject.Create(compound);
        // groundObj.Position = new Vec3 { x = 0, y = -0.1f,  z = 0 };
        // Ground.BulletRep = groundObj;

        // sw = File.CreateText("Output.csv");
        // sw.WriteLine("entry_number,step_delta,puppet_delta");
        // sw.Flush();
    }

    public GameObject CreateConvexObject(Mesh m) {
        // var verts = SphereMesh.vertices.AsParallel().Select<Vector3, Vec3>(x => ToBullet(x));
        var verts = new Vec3[] {
            new Vec3 { x = -0.5f, y = -0.5f, z = -0.5f },
            new Vec3 { x = -0.5f, y =  0.5f, z = -0.5f },
            new Vec3 { x =  0.5f, y =  0.5f, z = -0.5f },
            new Vec3 { x =  0.5f, y = -0.5f, z = -0.5f },
            new Vec3 { x = -0.5f, y = -0.5f, z =  0.5f },
            new Vec3 { x = -0.5f, y =  0.5f, z =  0.5f },
            new Vec3 { x =  0.5f, y =  0.5f, z =  0.5f },
            new Vec3 { x =  0.5f, y = -0.5f, z =  0.5f }
        };
        var shape = ConvexShape.Create(verts.ToArray());
        // var shape = SphereShape.Create(0.5f);
        var obj = PhysicsObject.Create(shape, 1);
        var gameObject = new GameObject();
        var filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = m;
        var renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = CubeMaterial;
        var puppet = gameObject.AddComponent<BulletPuppet>();
        puppet.BulletRep = obj;
        obj.Position = ToBullet(0f, 10f, 0f);
        return gameObject;
    }

    private void Update() {
        BulletUpdate();
    }

    private int counter = 0;
    private void BulletUpdate() {
        DateTime before = DateTime.Now;
        PhysicsHandler.Step(Time.deltaTime);
        TimeSpan stepDelta = DateTime.Now - before;
        before = DateTime.Now;
        BulletManager.UpdatePuppets();
        TimeSpan puppetDelta = DateTime.Now - before;

        // sw.WriteLine($"{counter},{stepDelta.TotalMilliseconds},{puppetDelta.TotalMilliseconds}");
        // sw.Flush();
        counter++;

        // float sign = Mathf.Sign(_hinge.MotorTargetVelocity);
        // if (sign > 0 ? _hinge.Angle > 60 * Mathf.Deg2Rad : _hinge.Angle < -60 * Mathf.Deg2Rad) {
        //     _hinge.MotorTargetVelocity = _hinge.MotorTargetVelocity * -1;
        // }

        // Raycast testing

        // Debug.Log(Input.mousePosition);
        // float castDistance = 20;
        // Vector3 from = Camera.main.transform.position;
        // Vector3 to = from + ((Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1)) - from).normalized * castDistance);
        // var res = PhysicsHandler.RayCastClosest(ToBullet(from), ToBullet(to));
        // if (_pointARenderer == null)
        //     _pointARenderer = PointA.GetComponent<MeshRenderer>();
        // if (res.hit) {
        //     _pointARenderer.enabled = true;
        //     PointA.transform.position = ToUnity(res.hit_point);
        // } else {
        //     _pointARenderer.enabled = false;
        // }
        // PointB.transform.position = to;
    }

    private void OnDestroy() {
        // sw.Close();

        BulletManager.KillAll();
        PhysicsHandler.DestroySimulation();
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
