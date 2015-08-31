using UnityEngine;
using System.Collections;


namespace Prime31
{
	public static class CameraKit2DUtils
	{
		public static void drawDesiredPositionGizmo( Vector3 position, Color color = default( Color ) )
		{
			position.z = 0f;
			if( color == default( Color ) )
				color = Color.green;
		
			Gizmos.color = color;
			var size = Camera.main.orthographicSize * 0.04f;
			var verticalOffset = new Vector3( 0f, size, 0f );
			var horizontalOffset = new Vector3( size, 0f, 0f );

			Gizmos.DrawLine( position - verticalOffset, position + verticalOffset );
			Gizmos.DrawLine( position - horizontalOffset, position + horizontalOffset );
		}


#if UNITY_EDITOR
		public static void drawCurrentPositionGizmo( Vector3 position, Color color = default( Color ) )
		{
			position.z = 0f;
			if( color == default( Color ) )
				color = Color.yellow;

			var size = Camera.main.orthographicSize * 0.04f;
			UnityEditor.Handles.color = color;
			UnityEditor.Handles.DrawWireDisc( position, Vector3.back, size );
		}
#endif


		public static void drawForGizmo( Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
		{
			Gizmos.DrawRay( pos, direction );
			drawArrowEnd( true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle );
		}


		public static void drawForGizmo( Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
		{
			Gizmos.DrawRay( pos, direction );
			drawArrowEnd( true, pos, direction, color, arrowHeadLength, arrowHeadAngle );
		}


		public static void ForDebug( Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
		{
			Debug.DrawRay( pos, direction );
			drawArrowEnd( false, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle );
		}


		public static void ForDebug( Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
		{
			Debug.DrawRay( pos, direction, color );
			drawArrowEnd( false, pos, direction, color, arrowHeadLength, arrowHeadAngle );
		}


		static void drawArrowEnd( bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f )
		{
			Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( arrowHeadAngle, 0, 0 ) * Vector3.back;
			Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( -arrowHeadAngle, 0, 0 ) * Vector3.back;
			Vector3 up = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, arrowHeadAngle, 0 ) * Vector3.back;
			Vector3 down = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, -arrowHeadAngle, 0 ) * Vector3.back;
			if( gizmos )
			{
				Gizmos.color = color;
				Gizmos.DrawRay( pos + direction, right * arrowHeadLength );
				Gizmos.DrawRay( pos + direction, left * arrowHeadLength );
				Gizmos.DrawRay( pos + direction, up * arrowHeadLength );
				Gizmos.DrawRay( pos + direction, down * arrowHeadLength );
			}
			else
			{
				Debug.DrawRay( pos + direction, right * arrowHeadLength, color );
				Debug.DrawRay( pos + direction, left * arrowHeadLength, color );
				Debug.DrawRay( pos + direction, up * arrowHeadLength, color );
				Debug.DrawRay( pos + direction, down * arrowHeadLength, color );
			}
		}

	}
}
