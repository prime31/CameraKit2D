using UnityEngine;
using System.Collections;


namespace Prime31
{
	[RequireComponent( typeof( CircleCollider2D ) )]
	public class CueFocusRing : MonoBehaviour, ICameraEffector
	{
		[Tooltip( "when true, an additional inner ring can be used to have it's own specific weight indpendent of the outer ring" )]
		public bool enableInnerRing = false;
		public float outerRingRadius = 8f;
		public float insideRingRadius = 2f;
		public float outsideEffectorWeight = 0.5f;
		public float insideEffectorWeight = 5f;
		[Tooltip( "The curve should go from 0 to 1 being the normalized distance from center to radius. It's value will be multiplied by the effectorWeight to get the final weight used." )]
		public AnimationCurve effectorFalloffCurve = AnimationCurve.Linear( 0f, 0f, 1f, 1f );

		Transform _trackedTarget;


		void Awake()
		{
			GetComponent<CircleCollider2D>().radius = outerRingRadius;
		}


		void OnTriggerEnter2D( Collider2D other )
		{
			if( other.gameObject.CompareTag( "Player" ) )
			{
				_trackedTarget = other.transform;
				CameraKit2D.instance.addCameraEffector( this );
			}
		}


		void OnTriggerExit2D( Collider2D other )
		{
			if( other.transform == _trackedTarget )
			{
				_trackedTarget = null;
				CameraKit2D.instance.removeCameraEffector( this );
			}
		}


#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			UnityEditor.Handles.color = Color.green;

			if( enableInnerRing )
				UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.back, insideRingRadius );
			else
				UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.back, outerRingRadius );
		}
#endif
	

		#region ICameraEffector

		public float getEffectorWeight()
		{
			var distanceToTarget = Vector3.Distance( transform.position, _trackedTarget.position );
			if( enableInnerRing && distanceToTarget <= insideRingRadius )
				return insideEffectorWeight;

			return effectorFalloffCurve.Evaluate( 1f - ( distanceToTarget / outerRingRadius ) ) * outsideEffectorWeight;
		}


		public Vector3 getDesiredPositionDelta( Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity )
		{
			return transform.position;
		}

		#endregion

	}
}