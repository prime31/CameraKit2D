using UnityEngine;
using System.Collections.Generic;


namespace Prime31
{
	public class FixedSizedVector3Queue
	{
		List<Vector3> _list;
		int _limit;


		public FixedSizedVector3Queue( int limit )
		{
			_limit = limit;
			_list = new List<Vector3>( limit );
		}


		public void push( Vector3 item )
		{
			if( _list.Count == _limit )
				_list.RemoveAt( 0 );
		
			_list.Add( item );
		}


		public Vector3 average()
		{
			var avg = Vector3.zero;

			// early out for no items
			if( _list.Count == 0 )
				return avg;

			for( var i = 0; i < _list.Count; i++ )
				avg += _list[i];
		
			return avg / _list.Count;
		}
	}
}