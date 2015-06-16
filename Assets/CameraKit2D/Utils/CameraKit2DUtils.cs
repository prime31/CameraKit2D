using UnityEngine;
using System.Collections;


namespace Prime31 {

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


	public static void drawArrowLineGizmo( Vector3 start, Vector3 end, Color color = default( Color ) )
	{
		start.z = end.z = 0f;
		if( color == default( Color ) )
			color = Color.yellow;

		color.a = 0.5f;
		Gizmos.color = color;
		Gizmos.DrawLine( start, end );
	}
}
}