using UnityEngine;
using System.Collections.Generic;
using Prime31;


public class SimplePlayerController : MonoBehaviour
{
	public float maxSpeed = 6f;
	public Transform feetTransform;
	public float feetRadius = 0.1f;
	public float jumpForce = 500f;

	Rigidbody2D _rigidBody;
	bool _grounded;
	bool grounded
	{
		get { return _grounded; }
		set
		{
			if( _grounded == value )
				return;

			CameraKit2D.instance.isPlayerGrounded = value;
			_grounded = value;
		}
	}


	void Awake()
	{
		Physics2D.gravity = new Vector2( 0f, -25f );
		_rigidBody = GetComponent<Rigidbody2D>();
	}


	void FixedUpdate()
	{
		grounded = Physics2D.OverlapCircle( feetTransform.position, feetRadius, 1 << LayerMask.NameToLayer( "Default" ) );
		
		var move = Input.GetAxis( "Horizontal" );
		_rigidBody.velocity = new Vector2( move * maxSpeed , _rigidBody.velocity.y );
	}


	void Update()
	{
		if( _grounded && ( Input.GetKeyDown( KeyCode.Z ) || Input.GetKeyDown( KeyCode.Space ) || Input.GetKeyDown( KeyCode.UpArrow ) ) )
			_rigidBody.AddForce( new Vector2( 0f, jumpForce ) );
	}
}
