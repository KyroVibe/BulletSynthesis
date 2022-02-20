﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesis {
    public class PhysicsObject {

        #region Fields

        public Vec3 Position {
            get => PhysicsHandler.GetRigidBodyPosition(_rigidbody_ptr);
            set {
                PhysicsHandler.SetRigidBodyPosition(_rigidbody_ptr, value);
            }
        }
        public Quat Rotation {
            get => PhysicsHandler.GetRigidBodyRotation(_rigidbody_ptr);
            set {
                PhysicsHandler.SetRigidBodyRotation(_rigidbody_ptr, value);
            }
        }
        public Vec3 LinearVelocity {
            get => PhysicsHandler.GetRigidBodyLinearVelocity(_rigidbody_ptr);
            set {
                PhysicsHandler.SetRigidBodyLinearVelocity(_rigidbody_ptr, value);
            }
        }
        public Vec3 AngularVelocity {
            get => PhysicsHandler.GetRigidBodyLinearVelocity(_rigidbody_ptr);
            set {
                PhysicsHandler.SetRigidBodyLinearVelocity(_rigidbody_ptr, value);
            }
        }
        public ActivationState ActivationState {
            get => PhysicsHandler.GetRigidBodyActivationState(_rigidbody_ptr);
            set => PhysicsHandler.SetRigidBodyActivationState(_rigidbody_ptr, value);
        }

        #endregion

        internal VoidPointer _rigidbody_ptr;

        internal PhysicsObject(VoidPointer rb_ptr) {
            _rigidbody_ptr = rb_ptr;
        }

        ~PhysicsObject() {
            Destroy();
        }

        public void Destroy() {
            PhysicsHandler.DeleteRigidBodyShape(_rigidbody_ptr);
        }

        public static PhysicsObject Create(Shape shape, float mass = 0.0f) {
            PhysicsObject physObj;
            if (mass == 0.0f) {
                physObj = new PhysicsObject(PhysicsHandler.CreateRigidBodyStatic(shape._shape_ptr));
            } else {
                physObj = new PhysicsObject(PhysicsHandler.CreateRigidBodyDynamic(mass, shape._shape_ptr));
            }

            return physObj;
        }

        public void ApplyForce(Vec3 force, Vec3 relativePosition) {
            PhysicsHandler.ApplyForce(_rigidbody_ptr, force, relativePosition);
        }
    }
}
