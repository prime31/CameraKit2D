using UnityEngine;
using System.Collections.Generic;


namespace Prime31
{
	public class MapExtentsFinalizer : ICameraFinalizer
	{
		public bool snapToBottom;
		public bool snapToTop;
		public bool snapToRight;
		public bool snapToLeft;

		public Bounds bounds;

		bool _shouldSkipSmoothingThisFrame;


		#region ICameraFinalizer

		public Vector3 getFinalCameraPosition( Bounds targetBounds, Vector3 currentCameraPosition, Vector3 desiredCameraPosition )
		{
			_shouldSkipSmoothingThisFrame = false;

			// orthographicSize is 0.5 * height. aspect is width / height. that makes this calculation equal 0.5 * width
			var orthoSize = CameraKit2D.instance.camera.orthographicSize;
			var orthographicHalfWidth = orthoSize * CameraKit2D.instance.camera.aspect;

			// clamp the camera position to the maps bounds
			// left
			if( snapToLeft && desiredCameraPosition.x - orthographicHalfWidth < bounds.min.x )
			{
				_shouldSkipSmoothingThisFrame = true;
				desiredCameraPosition.x = bounds.min.x + orthographicHalfWidth;
			}

			// right
			if( snapToRight && desiredCameraPosition.x + orthographicHalfWidth > bounds.max.x )
			{
				_shouldSkipSmoothingThisFrame = true;
				desiredCameraPosition.x = bounds.max.x - orthographicHalfWidth;
			}

			// top
			if( snapToTop && desiredCameraPosition.y + orthoSize > bounds.max.y )
			{
				_shouldSkipSmoothingThisFrame = true;
				desiredCameraPosition.y = bounds.max.y - orthoSize;
			}

			// bottom
			if( snapToBottom && desiredCameraPosition.y - orthoSize < bounds.min.y )
			{
				_shouldSkipSmoothingThisFrame = true;
				desiredCameraPosition.y = bounds.min.y + orthoSize;
			}

			return desiredCameraPosition;
		}


		public int getFinalizerPriority()
		{
			return 0;
		}


		public bool shouldSkipSmoothingThisFrame()
		{
			return _shouldSkipSmoothingThisFrame;
		}

		#endregion

	}
}