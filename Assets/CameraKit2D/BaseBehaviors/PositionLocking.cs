using UnityEngine;
using System.Collections.Generic;
using System;


namespace Prime31
{
	public class PositionLocking : MonoBehaviour, ICameraBaseBehavior
	{
		[BitMaskAttribute]
		public CameraAxis axis;

		[Header( "Projected Focus" )]
		[Tooltip( "projected focus will have the camera push ahead in the direction of the current velocity which is averaged over 5 frames" )]
		public bool enableProjectedFocus;
		[Tooltip( "when projected focus is enabled the multiplier will increase the forward projection" )]
		public float projectedFocusMultiplier = 3f;


		// this is only here so that we get the "Enabled" checkbox in the Inspector
		[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
		void Update()
		{
		}


		/// <summary>
		/// gets a center point for our position locking calculation based on the CameraAxis. The targetPosition is needed so that if
		/// only one axis is present we don't calculate a desired position in that direction.
		/// </summary>
		/// <returns>The center based on contraints.</returns>
		/// <param name="targetPosition">Target position.</param>
		Vector3 getCenterBasedOnContraints( Vector3 basePosition, Vector3 targetPosition )
		{
			var centerPos = basePosition;
			centerPos.z = 0f;

			// if we arent contrained to an axis make it match the targetPosition so we dont have any offset in that direction
			if( ( axis & CameraAxis.Horizontal ) != CameraAxis.Horizontal )
				centerPos.x = targetPosition.x;

			if( ( axis & CameraAxis.Vertical ) != CameraAxis.Vertical )
				centerPos.y = targetPosition.y;

			return centerPos;
		}


		#region ICameraBaseBehavior

		public Vector3 getDesiredPositionDelta( Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity )
		{
			var centerPos = getCenterBasedOnContraints( basePosition, targetBounds.center );
			var desiredOffset = targetBounds.center - centerPos;

			// projected focus uses the velocity to project forward
			// TODO: this needs proper smoothing. it only uses the avg velocity right now which can jump around
			if( enableProjectedFocus )
			{
				var hasHorizontal = ( axis & CameraAxis.Horizontal ) == CameraAxis.Horizontal;
				var hasVertical = ( axis & CameraAxis.Vertical ) == CameraAxis.Vertical;
				var hasBothAxis = hasHorizontal && hasVertical;

				if( hasBothAxis )
					desiredOffset += targetAvgVelocity * Time.deltaTime * projectedFocusMultiplier;
				else if( hasHorizontal )
					desiredOffset.x += targetAvgVelocity.x * Time.deltaTime * projectedFocusMultiplier;
				else if( hasVertical )
					desiredOffset.y += targetAvgVelocity.y * Time.deltaTime * projectedFocusMultiplier;
			}
			
			return desiredOffset;
		}


		public bool isEnabled()
		{
			return enabled;
		}


#if UNITY_EDITOR
		public void onDrawGizmos( Vector3 basePosition )
		{
			Gizmos.color = new Color( 0f, 0.4f, 0.8f );

			var hasHorizontal = ( axis & CameraAxis.Horizontal ) == CameraAxis.Horizontal;
			var hasVertical = ( axis & CameraAxis.Vertical ) == CameraAxis.Vertical;
			var hasBothAxis = hasHorizontal && hasVertical;

			var lineWidth = hasBothAxis ? Camera.main.orthographicSize / 5f : Camera.main.orthographicSize / 2f;

			if( hasVertical || hasBothAxis )
			{
				Gizmos.DrawLine( basePosition + new Vector3( -lineWidth, 0f, 1f ), basePosition + new Vector3( lineWidth, 0f, 1f ) );
			}

			if( hasHorizontal || hasBothAxis )
			{
				Gizmos.DrawLine( basePosition + new Vector3( 0f, -lineWidth, 1f ), basePosition + new Vector3( 0f, lineWidth, 1f ) );
			}
		}
#endif
	
		#endregion

	}
}