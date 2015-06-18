using UnityEngine;
using System.Collections;


namespace Prime31 {

public interface ICameraBaseBehavior : ICameraPositionAssertion
{
#if UNITY_EDITOR
	void onDrawGizmos( Vector3 basePosition );
#endif
}


public interface ICameraEffector : ICameraPositionAssertion
{
	/// <summary>
	/// each effector has a weight that changes how much it effects the final position. When the position is calculated the
	/// camera base behavior has a weight of 1. Your effectors can have weights larger than one if you want them weighted more than the
	/// base behavior
	/// </summary>
	float getEffectorWeight();
}


public interface ICameraPositionAssertion
{
	Vector3 getDesiredPositionDelta( Collider2D targetCollider, Vector3 basePosition, Vector3 targetAvgVelocity );
}}