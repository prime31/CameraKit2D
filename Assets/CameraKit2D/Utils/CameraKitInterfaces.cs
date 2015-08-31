using UnityEngine;
using System.Collections;


namespace Prime31
{
	/// <summary>
	/// BaseBehaviors get to decide a desired position for the camera. They are first in line for the calculation.
	/// </summary>
	public interface ICameraBaseBehavior : ICameraPositionAssertion
	{
		bool isEnabled();
#if UNITY_EDITOR
		// useful for while we are in the editor to provide a UI
		void onDrawGizmos( Vector3 basePosition );
#endif
	}


	/// <summary>
	/// Effectors get evaluated after BaesBehaviors. They each have a weight that is used for a weighted average to get a final
	/// camera position. The BaseBehavior has a weight of 1f which should be taken into account when deciding your Effector's weight.
	/// </summary>
	public interface ICameraEffector : ICameraPositionAssertion
	{
		/// <summary>
		/// each effector has a weight that changes how much it effects the final position. When the position is calculated the
		/// camera base behavior has a weight of 1. Your effectors can have weights larger than one if you want them weighted more than the
		/// base behavior
		/// </summary>
		float getEffectorWeight();
	}


	/// <summary>
	/// common interface for BaseBehaviors and Effectors. Importantly, basePosition is the camera's position plus the offsets
	/// of CameraKit. The method should return the desired offset from that position that it wants the camera to be moved by.
	/// </summary>
	public interface ICameraPositionAssertion
	{
		Vector3 getDesiredPositionDelta( Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity );
	}


	/// <summary>
	/// camera finalizers get the final say for the camera position. They are sorted by priority and passed the current and desired
	/// camera positions. shouldSkipSmoothingThisFrame will ONLY be called on a priority 0 finalizer. It allows the finalizer to
	/// force smoothing to None. This is important when the finalizer has a position change that is absolute (extents are a good
	/// example since you never want to display outside of your extents).
	/// </summary>
	public interface ICameraFinalizer
	{
		Vector3 getFinalCameraPosition( Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition );

		int getFinalizerPriority();

		bool shouldSkipSmoothingThisFrame();
	}
}