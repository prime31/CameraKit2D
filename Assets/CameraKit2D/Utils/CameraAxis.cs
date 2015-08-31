using UnityEngine;
using System.Collections;
using System;


namespace Prime31
{
	[Flags]
	public enum CameraAxis
	{
		Horizontal = 1 << 0,
		Vertical = 1 << 1
	}
}