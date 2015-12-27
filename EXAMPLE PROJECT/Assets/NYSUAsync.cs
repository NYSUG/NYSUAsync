/******************
The MIT License (MIT)
Copyright (c) 2015 No, You Shut Up Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
********************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NYSUAsync {
	
	///<summary>
	/// This class handles processing multiple async methods and fires
	/// a callback to a single caller. 
	/// </summary>
	
	// Callback definition
	public delegate void NYSUAsyncCallbackType (string error, List<object> data);
	
	// Process Queue
	private readonly static Queue<System.Action> _queue = new Queue<System.Action> ();
	
	// Callback
	private static NYSUAsyncCallbackType _callback;
	
	// Helpers
	private static int _runningTaskCount;
	private static List<object> _retval = new List<object> ();
	
	public static IEnumerator RunTasksInSeries (List<System.Action> tasks, NYSUAsyncCallbackType callback)
	{
		// Are we already processing a queue?
		while (_queue.Count > 0) {
			yield return new WaitForSeconds (0.5f);
		}
		
		// Sanity checkNYSUAsyncCallbackType
		if (tasks.Count == 0) {
			callback ("NYSUAsync Error: no tasks provided", null);
			yield break;
		}
		
		// Set the final callback
		_callback = callback;
		
		foreach (System.Action task in tasks) {
			NYSUAsync._queue.Enqueue (task);
		}
		
		// Run the first task
		NYSUAsync._queue.Dequeue ().Invoke ();
	}
	
#region Next Blocks

	public static void Next (string error, object retval)
	{
		_Next (error, retval);
	}

	public void Next (string error, bool retval)
	{
		_Next (error, (object)retval);
	}

	public static void Next (string error, string retval)
	{
		_Next (error, (object)retval);
	}

	public static void Next (string error, int retval)
	{
		_Next (error, (object)retval);
	}

	public static void Next (string error, float retval)
	{
		_Next (error, (object)retval);
	}

	public static void Next (string error, double retval)
	{
		_Next (error, (object)retval);
	}

	public static void Next (string error, Dictionary<string, object> retval)
	{
		_Next (error, (object)retval);
	}
	
	public static void Next (string error)
	{
		_Next (error, null);
	}

	public static void _Next (string error, object retval)
	{
		if (!string.IsNullOrEmpty (error)) {
			// Callback the error
			NYSUAsync._callback (error, _retval);
			
			// Clear the queue
			NYSUAsync._queue.Clear ();
			return;
		}
		
		// Add our object to the retval list
		if (retval != null)
			_retval.Add (retval);
		
		// Check to see if there's anything else
		if (NYSUAsync._queue.Count > 0) {
			// Process the next task in the queue
			NYSUAsync._queue.Dequeue ().Invoke ();
		} else {
			// We're all done
			_callback (string.Empty, _retval);
			_retval.Clear ();
		}
	}
	
#endregion
	
}
