// uncomment this if you have ZestKit in your project and you want to use the tweened methods
//#define ENABLE_ZEST_KIT

#if ENABLE_ZEST_KIT
using Prime31.ZestKit;
#endif
using UnityEngine;
using System.Collections.Generic;


namespace Prime31
{
	[RequireComponent( typeof( Camera ) )]
	public class CameraKit2D : MonoBehaviour
	{
		[System.NonSerialized][HideInInspector]
		public new Camera camera;
		public Collider2D targetCollider;
		[Tooltip( "percentage from -0.5 - 0.5 from the center of the screen" )]
		[Range( -0.5f, 0.5f )]
		public float horizontalOffset = 0f;
		[Tooltip( "percentage from -0.5 - 0.5 from the center of the screen" )]
		[Range( -0.5f, 0.5f )]
		public float verticalOffset = 0f;


		[Header( "Platform Snap" )]
		[Tooltip( "all platform snap settings only apply if enablePlatformSnap is true" )]
		public bool enablePlatformSnap;
		[Tooltip( "If true, no other base behaviors will be able to modify the y-position of the camera when grounded" )]
		public bool isPlatformSnapExclusiveWhenEnabled;
		[Range( -10f, 10f )]
		public float platformSnapVerticalOffset = 0f;
		[Tooltip( "This should be set as the player becomes grounded/ungrounded if using platform snap" )]
		public bool isPlayerGrounded;


		[Header( "Smoothing" )]
		public CameraSmoothingType cameraSmoothingType;
		[Tooltip( "approximately the time it will take to reach the target. A smaller value will reach the target faster." )]
		public float smoothDampTime = 0.08f;
		[Tooltip( "lower values are less damped and higher values are more damped resulting in less springiness. should be between 0.01f, 1f to avoid unstable systems." )]
		public float springDampingRatio = 0.7f;
		[Tooltip( "An angular frequency of 2pi (radians per second) means the oscillation completes one full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable" )]
		public float springAngularFrequency = 20f;
		public float lerpTowardsFactor = 0.002f;


		Transform _transform;
		List<ICameraBaseBehavior> _baseCameraBehaviors = new List<ICameraBaseBehavior>( 3 );
		List<ICameraEffector> _cameraEffectors = new List<ICameraEffector>( 3 );
		List<ICameraFinalizer> _cameraFinalizers = new List<ICameraFinalizer>( 1 );

		FixedSizedVector3Queue _averageVelocityQueue = new FixedSizedVector3Queue( 10 );
		Vector3 _targetPositionLastFrame;
		Vector3 _cameraVelocity;
		// used for SmoothDamp and spring smoothing

#pragma warning disable 0414
		float _originalOrthoSize;
#pragma warning restore 0414
#if ENABLE_ZEST_KIT
	ITween<float> _orthoSizeTween;
#endif

		private static CameraKit2D _instance;

		public static CameraKit2D instance
		{
			get
			{
				if( System.Object.Equals( _instance, null ) )
				{
					_instance = FindObjectOfType( typeof( CameraKit2D ) ) as CameraKit2D;

					if( System.Object.Equals( _instance, null ) )
						throw new UnityException( "CameraKit2D does not appear to exist" );
				}

				return _instance;
			}
		}


#region MonoBehaviour

		void Awake()
		{
			_instance = this;
			_transform = GetComponent<Transform>();
			camera = GetComponent<Camera>();
			_originalOrthoSize = camera.orthographicSize;

			var behaviors = GetComponents<ICameraBaseBehavior>();
			for( var i = 0; i < behaviors.Length; i++ )
				addCameraBaseBehavior( behaviors[i] );
		}


		void LateUpdate()
		{
			var targetBounds = targetCollider.bounds;

			// we keep track of the target's velocity since some camera behaviors need to know about it
			var velocity = ( targetBounds.center - _targetPositionLastFrame ) / Time.deltaTime;
			velocity.z = 0f;
			_averageVelocityQueue.push( velocity );
			_targetPositionLastFrame = targetBounds.center;

			// fetch the average velocity for use in our camera behaviors
			var targetAvgVelocity = _averageVelocityQueue.average();


			// we use the transform.position plus the offset when passing the base position to our camera behaviors
			var basePosition = getNormalizedCameraPosition();
			var accumulatedDeltaOffset = Vector3.zero;

			for( var i = 0; i < _baseCameraBehaviors.Count; i++ )
			{
				var cameraBehavior = _baseCameraBehaviors[i];
				if( cameraBehavior.isEnabled() )
				{
					// once we get the desired position we have to subtract the offset that we previously added
					var desiredPos = cameraBehavior.getDesiredPositionDelta( targetBounds, basePosition, targetAvgVelocity );
					accumulatedDeltaOffset += desiredPos;
				}
			}

			if( enablePlatformSnap && isPlayerGrounded )
			{
				// when exclusive, no base behaviors can mess with y
				if( isPlatformSnapExclusiveWhenEnabled )
					accumulatedDeltaOffset.y = 0f;
			
				var desiredOffset = targetBounds.min.y - basePosition.y - platformSnapVerticalOffset;
				accumulatedDeltaOffset += new Vector3( 0f, desiredOffset );
			}


			// fetch our effectors
			var totalWeight = 0f;
			var accumulatedEffectorPosition = Vector3.zero;
			for( var i = 0; i < _cameraEffectors.Count; i++ )
			{
				var weight = _cameraEffectors[i].getEffectorWeight();
				var position = _cameraEffectors[i].getDesiredPositionDelta( targetBounds, basePosition, targetAvgVelocity );

				totalWeight += weight;
				accumulatedEffectorPosition += ( weight * position );
			}

			var desiredPosition = _transform.position + accumulatedDeltaOffset;

			// if we have a totalWeight we need to take into account our effectors
			if( totalWeight > 0 )
			{
				totalWeight += 1f;
				accumulatedEffectorPosition += desiredPosition;
				var finalAccumulatedPosition = accumulatedEffectorPosition / totalWeight;
				finalAccumulatedPosition.z = _transform.position.z;
				desiredPosition = finalAccumulatedPosition;
			}


			var smoothing = cameraSmoothingType;

			// and finally, our finalizers have a go if we have any
			for( var i = 0; i < _cameraFinalizers.Count; i++ )
			{
				desiredPosition = _cameraFinalizers[i].getFinalCameraPosition( targetBounds, transform.position, desiredPosition );

				// allow the finalizer with a 0 priority to skip smoothing if it wants to
				if( i == 0 && _cameraFinalizers[i].getFinalizerPriority() == 0 && _cameraFinalizers[i].shouldSkipSmoothingThisFrame() )
					smoothing = CameraSmoothingType.None;
			}



			// reset Z just in case one of the other scripts messed with it
			desiredPosition.z = _transform.position.z;

			// time to smooth our movement to the desired position
			switch( smoothing )
			{
				case CameraSmoothingType.None:
					_transform.position = desiredPosition;
					break;
				case CameraSmoothingType.SmoothDamp:
					_transform.position = Vector3.SmoothDamp( _transform.position, desiredPosition, ref _cameraVelocity, smoothDampTime );
					break;
				case CameraSmoothingType.Spring:
					_transform.position = fastSpring( _transform.position, desiredPosition );
					break;
				case CameraSmoothingType.Lerp:
					_transform.position = lerpTowards( _transform.position, desiredPosition, lerpTowardsFactor );
					break;
			}
		}


#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			var positionInFrontOfCamera = getNormalizedCameraPosition();
			positionInFrontOfCamera.z = 1f;

			var allCameraBehaviors = GetComponents<ICameraBaseBehavior>();
			foreach( var cameraBehavior in allCameraBehaviors )
			{
				if( cameraBehavior.isEnabled() )
					cameraBehavior.onDrawGizmos( positionInFrontOfCamera );
			}

			if( enablePlatformSnap )
			{
				Gizmos.color = new Color( 0.3f, 0.1f, 0.6f );

				var lineWidth = Camera.main.orthographicSize / 2f;
				Gizmos.DrawLine( positionInFrontOfCamera + new Vector3( -lineWidth, platformSnapVerticalOffset, 1f ), positionInFrontOfCamera + new Vector3( lineWidth, platformSnapVerticalOffset, 1f ) );
			}
		}
#endif


		void OnApplicationQuit()
		{
			_instance = null;
		}

#endregion


		Vector3 getNormalizedCameraPosition()
		{
			//Camera.main.ViewportToWorldPoint()
#if UNITY_EDITOR
			return GetComponent<Camera>().ViewportToWorldPoint( new Vector3( 0.5f + horizontalOffset, 0.5f + verticalOffset, 0f ) );
#else
		return camera.ViewportToWorldPoint( new Vector3( 0.5f + horizontalOffset, 0.5f + verticalOffset, 0f ) );
#endif
		}


#region smoothing

		Vector3 lerpTowards( Vector3 from, Vector3 to, float remainingFactorPerSecond )
		{
			return Vector3.Lerp( from, to, 1f - Mathf.Pow( remainingFactorPerSecond, Time.deltaTime ) );
		}


		/// <summary>
		/// uses the semi-implicit euler method. faster, but not always stable.
		/// see http://allenchou.net/2015/04/game-math-more-on-numeric-springing/
		/// </summary>
		/// <returns>The spring.</returns>
		/// <param name="currentValue">Current value.</param>
		/// <param name="targetValue">Target value.</param>
		/// <param name="velocity">Velocity by reference. Be sure to reset it to 0 if changing the targetValue between calls</param>
		/// <param name="dampingRatio">lower values are less damped and higher values are more damped resulting in less springiness.
		/// should be between 0.01f, 1f to avoid unstable systems.</param>
		/// <param name="angularFrequency">An angular frequency of 2pi (radians per second) means the oscillation completes one
		/// full period over one second, i.e. 1Hz. should be less than 35 or so to remain stable</param>
		Vector3 fastSpring( Vector3 currentValue, Vector3 targetValue )
		{
			_cameraVelocity += -2.0f * Time.deltaTime * springDampingRatio * springAngularFrequency * _cameraVelocity + Time.deltaTime * springAngularFrequency * springAngularFrequency * ( targetValue - currentValue );
			currentValue += Time.deltaTime * _cameraVelocity;

			return currentValue;
		}

#endregion


#if ENABLE_ZEST_KIT
	public void setOrthographicSize( float orthographicSize, float duration = 0.3f, EaseType easeType = EaseType.QuartInOut )
	{
		// we create a non-recyclable tween so we can reuse it whenever the orthosize changes
		if( _orthoSizeTween == null )
			_orthoSizeTween = camera.ZKorthographicSizeTo( 0f, 0f ).setRecycleTween( false );
		else
			_orthoSizeTween.stop();
		
		_orthoSizeTween.prepareForReuse( camera.orthographicSize, orthographicSize, duration )
			.setEaseType( easeType )
			.start();
	}


	public void resetOrthoSize( float duration = 0.3f, EaseType easeType = EaseType.QuartInOut )
	{
		setOrthographicSize( _originalOrthoSize, duration, easeType );
	}
#endif


#region Camera Behavior and Effector management

		public void addCameraBaseBehavior( ICameraBaseBehavior cameraBehavior )
		{
			_baseCameraBehaviors.Add( cameraBehavior );
		}


		public void removeCameraBaseBehavior<T>() where T : ICameraBaseBehavior
		{
			var requestedType = typeof( T );
			for( var i = _baseCameraBehaviors.Count - 1; i >= 0; i-- )
			{
				if( _baseCameraBehaviors[i].GetType() == requestedType )
				{
					_baseCameraBehaviors.RemoveAt( i );
					return;
				}
			}
		}


		public T getCameraBaseBehavior<T>() where T : ICameraBaseBehavior
		{
			var requestedType = typeof( T );
			for( var i = 0; i < _baseCameraBehaviors.Count; i++ )
			{
				if( _baseCameraBehaviors[i].GetType() == requestedType )
					return (T)_baseCameraBehaviors[i];
			}

			return default( T );
		}


		public void addCameraEffector( ICameraEffector cameraEffector )
		{
			_cameraEffectors.Add( cameraEffector );
		}


		public void removeCameraEffector( ICameraEffector cameraEffector )
		{
			for( var i = _cameraEffectors.Count - 1; i >= 0; i-- )
			{
				if( _cameraEffectors[i] == cameraEffector )
				{
					_cameraEffectors.RemoveAt( i );
					return;
				}
			}
		}


		public void addCameraFinalizer( ICameraFinalizer cameraFinalizer )
		{
			_cameraFinalizers.Add( cameraFinalizer );

			// sort the list if we need to
			if( _cameraFinalizers.Count > 1 )
				_cameraFinalizers.Sort( ( first, second ) => first.getFinalizerPriority().CompareTo( second.getFinalizerPriority() ) );
		}


		public void removeCameraFinalizer( ICameraFinalizer cameraFinalizer )
		{
			for( var i = _cameraFinalizers.Count - 1; i >= 0; i-- )
			{
				if( _cameraFinalizers[i] == cameraFinalizer )
				{
					_cameraFinalizers.RemoveAt( i );
					return;
				}
			}
		}

#endregion

	}
}