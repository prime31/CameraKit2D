using UnityEngine;
using System.Collections;


namespace Prime31 {

public class CameraWindow : MonoBehaviour, ICameraBaseBehavior
{
	[Range( 0f, 20f )]
	public float width = 3f;
	[Range( 0f, 20f )]
	public float height = 3f;

	[BitMaskAttribute]
	public CameraAxis axis;


	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	void Update(){}


	public Vector3 getDesiredPositionDelta( Collider2D targetCollider, Vector3 basePosition, Vector3 targetAvgVelocity )
	{
		var desiredOffset = Vector3.zero;
		var hasHorizontal = ( axis & CameraAxis.Horizontal ) == CameraAxis.Horizontal;
		var hasVertical = ( axis & CameraAxis.Vertical ) == CameraAxis.Vertical;
		var bounds = new Bounds( basePosition, new Vector3( width, height, 5f ) );

		if( !bounds.Contains( targetCollider.bounds.max ) || !bounds.Contains( targetCollider.bounds.min ) )
		{
			// figure out the minimum distance we need to move to get the player back in our bounds
			// x-axis
			if( hasHorizontal && bounds.min.x > targetCollider.bounds.min.x )
			{
				desiredOffset.x = targetCollider.bounds.min.x - bounds.min.x;
			}
			else if( hasHorizontal && bounds.max.x < targetCollider.bounds.max.x )
			{
				desiredOffset.x = targetCollider.bounds.max.x - bounds.max.x;
			}

			// y-axis. disregard movement above the trap when in platform snap mode
			if( hasVertical && bounds.min.y > targetCollider.bounds.min.y )
			{
				desiredOffset.y = targetCollider.bounds.min.y - bounds.min.y;
			}
			else if( /*!inPlatformSnapMode &&*/ hasVertical && bounds.max.y < targetCollider.bounds.max.y )
			{
				desiredOffset.y = targetCollider.bounds.max.y - bounds.max.y;
			}
		}

		return desiredOffset;
	}


	public void onDrawGizmos( Vector3 basePosition )
	{
		Gizmos.color = new Color( 1f, 0f, 0.6f );

		var hasHorizontal = ( axis & CameraAxis.Horizontal ) == CameraAxis.Horizontal;
		var hasVertical = ( axis & CameraAxis.Vertical ) == CameraAxis.Vertical;
		var hasBothAxis = hasHorizontal && hasVertical;
		var bounds = new Bounds( basePosition, new Vector3( width, height ) );
		var lineWidth = Camera.main.orthographicSize;

		// expand our bounds to have larger lines if we only have a single axis
		if( hasVertical && !hasBothAxis )
		{
			bounds.Expand( new Vector3( lineWidth - bounds.size.x, 0f ) );
		}

		if( hasHorizontal && !hasBothAxis )
		{
			bounds.Expand( new Vector3( 0f, lineWidth - bounds.size.y ) );
		}

		if( hasVertical || hasBothAxis )
		{
			Gizmos.DrawLine( bounds.min, bounds.min + new Vector3( bounds.size.x, 0f ) );
			Gizmos.DrawLine( bounds.max, bounds.max - new Vector3( bounds.size.x, 0f ) );
		}

		if( hasHorizontal || hasBothAxis )
		{
			Gizmos.DrawLine( bounds.min, bounds.min + new Vector3( 0f, bounds.size.y ) );
			Gizmos.DrawLine( bounds.max, bounds.max - new Vector3( 0f, bounds.size.y ) );
		}
	}

}
}