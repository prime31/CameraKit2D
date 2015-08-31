using UnityEngine;
using System.Collections;


namespace Prime31
{
	public class DualForwardFocus : MonoBehaviour, ICameraBaseBehavior
	{
		public enum DualForwardFocusType
		{
			ThresholdBased,
			VelocityBased,
			DirectionBased
		}


		[Range( 0f, 20f )]
		public float width = 3f;


		public DualForwardFocusType dualForwardFocusType;

		[Header( "Threshold Based" )]
		[Range( 0.5f, 5f )]
		public float dualForwardFocusThresholdExtents = 0.5f;
		RectTransform.Edge _currentEdgeFocus;

		[Header( "Velocity Based" )]
		public float velocityInfluenceMultiplier = 3f;


		// this is only here so that we get the "Enabled" checkbox in the Inspector
		[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
		void Update()
		{
		}


		#region ICameraBaseBehavior

		public Vector3 getDesiredPositionDelta( Bounds targetBounds, Vector3 basePosition, Vector3 targetAvgVelocity )
		{
			var desiredOffset = Vector3.zero;

			if( dualForwardFocusType == DualForwardFocusType.ThresholdBased )
			{
				var deltaPositionFromBounds = Vector3.zero;
				var didLastEdgeContactChange = false;
				float leftEdge, rightEdge;

				if( _currentEdgeFocus == RectTransform.Edge.Left )
				{
					rightEdge = basePosition.x - width * 0.5f;
					leftEdge = rightEdge - dualForwardFocusThresholdExtents * 0.5f;
				}
				else
				{
					leftEdge = basePosition.x + width * 0.5f;
					rightEdge = leftEdge + dualForwardFocusThresholdExtents * 0.5f;
				}

				if( leftEdge > targetBounds.center.x )
				{
					deltaPositionFromBounds.x = targetBounds.center.x - leftEdge;

					if( _currentEdgeFocus == RectTransform.Edge.Left )
					{
						didLastEdgeContactChange = true;
						_currentEdgeFocus = RectTransform.Edge.Right;
					}
				}
				else if( rightEdge < targetBounds.center.x )
				{
					deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

					if( _currentEdgeFocus == RectTransform.Edge.Right )
					{
						didLastEdgeContactChange = true;
						_currentEdgeFocus = RectTransform.Edge.Left;
					}
				}


				var desiredX = _currentEdgeFocus == RectTransform.Edge.Left ? rightEdge : leftEdge;
				desiredOffset.x = targetBounds.center.x - desiredX;

				// if we didnt switch direction this works much like a normal camera window
				if( !didLastEdgeContactChange )
					desiredOffset.x = deltaPositionFromBounds.x;
			}
			else // velocity or direction based
			{
				var averagedHorizontalVelocity = targetAvgVelocity.x;

				// direction switches are determined by velocity
				if( averagedHorizontalVelocity > 0f )
					_currentEdgeFocus = RectTransform.Edge.Left;
				else if( averagedHorizontalVelocity < 0f )
					_currentEdgeFocus = RectTransform.Edge.Right;

				var desiredX = _currentEdgeFocus == RectTransform.Edge.Left ? basePosition.x - width * 0.5f : basePosition.x + width * 0.5f;
				desiredX = targetBounds.center.x - desiredX;

				if( dualForwardFocusType == DualForwardFocusType.DirectionBased )
				{
					desiredOffset.x = desiredX;
				}
				else
				{
					var velocityMultiplier = Mathf.Max( 1f, Mathf.Abs( averagedHorizontalVelocity ) );
					desiredOffset.x = Mathf.Lerp( 0f, desiredX, Time.deltaTime * velocityInfluenceMultiplier * velocityMultiplier );
				}
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
			Gizmos.color = new Color( 0f, 0.5f, 0.6f );

			var bounds = new Bounds( basePosition, new Vector3( width, 10f ) );
			var lineWidth = Camera.main.orthographicSize;

			bounds.center = new Vector3( bounds.center.x, basePosition.y, bounds.center.z );
			bounds.Expand( new Vector3( 0f, lineWidth - bounds.size.y ) );

			Gizmos.DrawLine( bounds.min, bounds.min + new Vector3( 0f, bounds.size.y ) );
			Gizmos.DrawLine( bounds.max, bounds.max - new Vector3( 0f, bounds.size.y ) );


			if( dualForwardFocusType == DualForwardFocusType.ThresholdBased )
			{
				bounds.Expand( new Vector3( dualForwardFocusThresholdExtents, 1f ) );
				Gizmos.color = Color.blue;
				Gizmos.DrawLine( bounds.min, bounds.min + new Vector3( 0f, bounds.size.y ) );
				Gizmos.DrawLine( bounds.max, bounds.max - new Vector3( 0f, bounds.size.y ) );
			}
		}
#endif

		#endregion

	}
}