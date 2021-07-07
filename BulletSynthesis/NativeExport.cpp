#include "Physics/PhysicsManager.h"
#include "BulletCollision/CollisionShapes/btConvexPointCloudShape.h"
#include <stdlib.h>

#define EXPORT __declspec(dllexport)

extern "C" {

	EXPORT void Physics_Step(double deltaT) {
		// synthesis::PhysicsManager::getInstance().Step(deltaT);
		synthesis::PhysicsManager::getInstance().Run(130, deltaT);
	}

	/// 
	/// Structs
	/// 

	struct Vec3 {
		float x;
		float y;
		float z;
	};

	struct Quat {
		float x;
		float y;
		float z;
		float w;
	};

	/// 
	/// Misc Functions
	/// 
	
	inline btQuaternion To_Bullet_Quaternion(const Quat& q) {
		return btQuaternion(btScalar(q.x), btScalar(q.y), btScalar(q.z), btScalar(q.w));
	}

	inline Quat To_Quaternion_Native(const btQuaternion& q) {
		return {
			q.getX(),
			q.getY(),
			q.getZ(),
			q.getW()
		};
	}

	inline btVector3 To_Bullet_Vector_3(const Vec3& v) {
		return btVector3(btScalar(v.x), btScalar(v.y), btScalar(v.z));
	}

	inline Vec3 To_Vector_3_Native(const btVector3& v) {
		return {
			v.getX(),
			v.getY(),
			v.getZ()
		};
	}

	btVector3* To_Vector_3_Array(float* verts, int len) {
		btVector3* bt_verts = new btVector3[len / 3];
		int j;
		for (int i = 0; i < len / 3; i++) {
			j = i * 3;
			bt_verts[i] = btVector3(btScalar(verts[j]), btScalar(verts[j + 1]), btScalar(verts[j + 2]));
		}
		return bt_verts;
	}

	/// 
	/// Deletion
	/// 

	EXPORT void Delete_Collision_Shape(void* collisionShape) {
		delete ((btCollisionShape*)collisionShape);
	}

	///
	/// Creation
	///

	EXPORT void* Create_Compound_Shape(int initChildCapacity) {
		return new btCompoundShape(true, initChildCapacity);
	}
	EXPORT void* Create_Box_Shape(Vec3 halfSize) {
		return new btBoxShape(To_Bullet_Vector_3(halfSize));
	}
	EXPORT void* Create_Convex_Shape(float* verts, int len) {
		btVector3* bt_verts = To_Vector_3_Array(verts, len);
		btConvexPointCloudShape* shape = new btConvexPointCloudShape(bt_verts, len / 3, btVector3(btScalar(1.), btScalar(1.), btScalar(1.)));
		delete[] bt_verts;
		return shape;
	}
	EXPORT void* Create_RigidBody_Dynamic(float mass, void* collisionShape) {
		return synthesis::PhysicsManager::getInstance().createDynamicRigidBody((btCollisionShape*)collisionShape, btScalar(mass));
	}
	EXPORT void* Create_Rigidbody_Static(void* collisionShape) {
		return synthesis::PhysicsManager::getInstance().createStaticRigidBody((btCollisionShape*)collisionShape);
	}

	///
	/// Shape Manipulation
	///

	EXPORT void Add_Shape_To_Compound_Shape(void* compound, void* shape, Vec3 pos, Quat rot) {
		btTransform trans = btTransform(To_Bullet_Quaternion(rot), To_Bullet_Vector_3(pos));
		((btCompoundShape*)compound)->addChildShape(trans, (btCollisionShape*)shape);
	}
	EXPORT Vec3 Get_Box_Shape_Extents(void* shape) {
		return To_Vector_3_Native(((btBoxShape*)shape)->getHalfExtentsWithMargin());
	}

	///
	/// Rigidbody Manipulation
	/// 
	
	EXPORT void Apply_Force(void* rigidbody, Vec3 force, Vec3 relativePosition) {
		((btRigidBody*)rigidbody)->applyForce(To_Bullet_Vector_3(force), To_Bullet_Vector_3(relativePosition));
	}

	EXPORT Vec3 Get_RigidBody_Linear_Velocity(void* rigidbody) {
		return To_Vector_3_Native(((btRigidBody*)rigidbody)->getLinearVelocity());
	}
	EXPORT void Set_RigidBody_Linear_Velocity(void* rigidbody, Vec3 vel) {
		((btRigidBody*)rigidbody)->setLinearVelocity(To_Bullet_Vector_3(vel));
	}

	EXPORT Vec3 Get_RigidBody_Angular_Velocity(void* rigidbody) {
		return To_Vector_3_Native(((btRigidBody*)rigidbody)->getAngularVelocity());
	}
	EXPORT void Set_RigidBody_Angular_Velocity(void* rigidbody, Vec3 vel) {
		((btRigidBody*)rigidbody)->setAngularVelocity(To_Bullet_Vector_3(vel));
	}

	EXPORT Quat Get_RigidBody_Rotation(void* rigidbody) {
		btTransform trans;
		((btRigidBody*)rigidbody)->getMotionState()->getWorldTransform(trans);
		return To_Quaternion_Native(trans.getRotation());
	}
	EXPORT void Set_RigidBody_Rotation(void* rigidbody, Quat rotation) {
		btTransform trans;
		((btRigidBody*)rigidbody)->getMotionState()->getWorldTransform(trans);
		trans.setRotation(To_Bullet_Quaternion(rotation));
	}

	EXPORT Vec3 Get_RigidBody_Position(void* rigidbody) {
		btTransform trans;
		((btRigidBody*)rigidbody)->getMotionState()->getWorldTransform(trans);
		return To_Vector_3_Native(trans.getOrigin());
	}
	EXPORT void Set_RigidBody_Position(void* rigidbody, Vec3 pos) {
		btTransform trans;
		((btRigidBody*)rigidbody)->getMotionState()->getWorldTransform(trans);
		trans.setOrigin(To_Bullet_Vector_3(pos));
	}

	/// 
	/// Research
	/// 

	EXPORT void Load_Verts(double* arr, int size) {
		printf("Size: %d \n", size);
		for (int i = 0; i < size; i++) {
			printf("%lf \n", arr[i]);
		}
		// free(arr);
	}

}
