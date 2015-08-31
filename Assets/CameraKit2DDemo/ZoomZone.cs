using UnityEngine;
using System.Collections.Generic;
using Prime31;
using System.Collections;


/// <summary>
/// simple example of how to make a camera zoom zone. Note that if you are using ZestKit this is much easier. Just call
/// CameraKit2D.setOrthographicSize and ZestKit will handle the animation for you.
/// </summary>
public class ZoomZone : MonoBehaviour
{
	public float zoomDuration = 0.7f;
	public bool disableSmoothingDuringZoom;

	float _originalOrthoSize;


	void OnTriggerEnter2D( Collider2D other )
	{
		if( other.gameObject.CompareTag( "Player" ) )
		{
			_originalOrthoSize = CameraKit2D.instance.camera.orthographicSize;
			StartCoroutine( zoom( 3f ) );
		}
	}


	void OnTriggerExit2D( Collider2D other )
	{
		if( other.gameObject.CompareTag( "Player" ) )
		{
			StopAllCoroutines();
			StartCoroutine( zoom( _originalOrthoSize ) );
		}
	}


	float ease( float t, float d )
	{
		return -1 * ( ( t = t / d - 1 ) * t * t * t - 1 );
	}


	IEnumerator zoom( float to )
	{
		var originalSmoothingType = CameraKit2D.instance.cameraSmoothingType;
		if( disableSmoothingDuringZoom )
			CameraKit2D.instance.cameraSmoothingType = CameraSmoothingType.None;
		
		var from = CameraKit2D.instance.camera.orthographicSize;
		var elapsedTime = 0f;

		while( elapsedTime <= zoomDuration )
		{
			CameraKit2D.instance.camera.orthographicSize = Mathf.Lerp( from, to, ease( elapsedTime, zoomDuration ) );
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		CameraKit2D.instance.cameraSmoothingType = originalSmoothingType;
	}
}
