using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @author Nadir Román Guerrero
 * @email nadir.ro.gue@gmail.com
 */  
public class IDFactory
{

	private static int _seedStart = 0;
	 
	private static readonly Object _lock = new Object ();

	public static int getNextID ()
	{
		int result = 0;
		lock (_lock) {
			result = ++_seedStart;
		}

		return result;
	}

	public static void reset ()
	{
		_seedStart = 0;
	}
}
