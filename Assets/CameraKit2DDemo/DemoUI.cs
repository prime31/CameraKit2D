using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class DemoUI : MonoBehaviour
{
	public CameraWindow camWindow;
	public DualForwardFocus dualForwardFocus;
	public PositionLocking positionLocking;


	void OnGUI()
	{
		CameraKit2D.instance.enablePlatformSnap = GUILayout.Toggle( CameraKit2D.instance.enablePlatformSnap, "Enabled Platform Snap" );

		GUILayout.Label( "Horizontal Offset" );
		CameraKit2D.instance.horizontalOffset = GUILayout.HorizontalSlider( CameraKit2D.instance.horizontalOffset, -0.5f, 0.5f );

		GUILayout.Label( "Vertical Offset" );
		CameraKit2D.instance.verticalOffset = GUILayout.HorizontalSlider( CameraKit2D.instance.verticalOffset, -0.5f, 0.5f );


		if( GUILayout.Button( "Enable Camera Window" ) )
		{
			camWindow.axis = CameraAxis.Horizontal | CameraAxis.Vertical;
			camWindow.enabled = true;
			dualForwardFocus.enabled = positionLocking.enabled = false;
		}

		if( GUILayout.Button( "Enable Dual Forward Focus (Direction Based)" ) )
		{
			dualForwardFocus.enabled = true;
			dualForwardFocus.dualForwardFocusType = DualForwardFocus.DualForwardFocusType.DirectionBased;

			// we also enable the Camera Window in the vertical direction here
			camWindow.axis = CameraAxis.Vertical;
			camWindow.enabled = true;
			positionLocking.enabled = false;
		}

		if( GUILayout.Button( "Enable Dual Forward Focus (Threshold Based)" ) )
		{
			dualForwardFocus.enabled = true;
			dualForwardFocus.dualForwardFocusType = DualForwardFocus.DualForwardFocusType.ThresholdBased;

			// we also enable the Camera Window in the vertical direction here
			camWindow.axis = CameraAxis.Vertical;
			camWindow.enabled = true;
			positionLocking.enabled = false;
		}

		if( GUILayout.Button( "Enable Dual Forward Focus (Velocity Based)" ) )
		{
			dualForwardFocus.enabled = true;
			dualForwardFocus.dualForwardFocusType = DualForwardFocus.DualForwardFocusType.VelocityBased;

			// we also enable the Camera Window in the vertical direction here
			camWindow.axis = CameraAxis.Vertical;
			camWindow.enabled = true;
			positionLocking.enabled = false;
		}

		if( GUILayout.Button( "Enable Position Locking" ) )
		{
			CameraKit2D.instance.enablePlatformSnap = false;
			positionLocking.axis = CameraAxis.Horizontal | CameraAxis.Vertical;
			positionLocking.enabled = true;
			dualForwardFocus.enabled = camWindow.enabled = false;
		}


		if( GUILayout.Button( "Enable Camera Window + Position Locking" ) )
		{
			camWindow.enabled = positionLocking.enabled = true;
			camWindow.axis = CameraAxis.Vertical;
			positionLocking.axis = CameraAxis.Horizontal;
		}


		GUILayout.Space( 20 );

		GUILayout.Label( "Smoothing" );

		if( GUILayout.Button( "Enable SmoothDamp" ) )
			CameraKit2D.instance.cameraSmoothingType = CameraSmoothingType.SmoothDamp;

		GUILayout.Label( "Smooth Damp Time" );
		CameraKit2D.instance.smoothDampTime = GUILayout.HorizontalSlider( CameraKit2D.instance.smoothDampTime, 0.01f, 0.4f );


		if( GUILayout.Button( "Enable Spring" ) )
			CameraKit2D.instance.cameraSmoothingType = CameraSmoothingType.Spring;

		GUILayout.Label( "Damping Ratio" );
		CameraKit2D.instance.springDampingRatio = GUILayout.HorizontalSlider( CameraKit2D.instance.springDampingRatio, 0.01f, 1.0f );
		GUILayout.Label( "Angular Frequency" );
		CameraKit2D.instance.springAngularFrequency = GUILayout.HorizontalSlider( CameraKit2D.instance.springAngularFrequency, 0.0f, 35f );

		if( GUILayout.Button( "Enable Lerp" ) )
			CameraKit2D.instance.cameraSmoothingType = CameraSmoothingType.Lerp;
	}
}