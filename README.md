### NYSUAsync.cs
A utility to assist with running multiple asynchronous tasks with callback delegates into a single block.

## Purpose
This utility is useful for situation where you have a task that requires multiple asynchornous operations to be called in order from a single caller. For example, if you wanted to have one method for loading a player's profile it may need to make a call to the local database to load the player data, then pull a profile image from a web URL, and then pull updated player data from your servers. Each of these processes takes an unknown amount of time and need to be run in order. This utility makes it possible to run all of these from a single caller and halt and callback with any errors.

## Usage
Here's how to use it.

NYSUAsync processes a list of methods with a signature ending with a callback delegate. The delegate format is
    
    (string error, object retval) 

The retval can be any of the basic types (string, float, int, etc.) as well as a Dictionary<string, object> type. If you require a custom type of delegate you can add your signature in NYSUAsync.cs

Here's an example of creating and adding methods to a list:

    List<System.Action> asyncTasks = new List<System.Action> ();
		asyncTasks.Add (() => StartCoroutine (_AsyncWaitTask (NYSUAsync.Next)));
		asyncTasks.Add (() => StartCoroutine (_AsyncWebCallWithStringTask (NYSUAsync.Next)));
		asyncTasks.Add (() => StartCoroutine (_AsyncWebCallWithDictionaryTask (NYSUAsync.Next)));

In this example, each of the methods are return type IEnumerator which is why we have to use StartCoroutine. Here are the signatures of the three methods listed:

    private IEnumerator _AsyncWaitTask (AsyncTaskCallbackType callback)
    private IEnumerator _AsyncWebCallWithStringTask (AsyncTaskWithStringDataCallbackType callback)
    private IEnumerator _AsyncWebCallWithDictionaryTask (AsyncTaskWithDictionaryCallbackType callback)

And these are the definitions of the CallbackType delegates

    private delegate void AsyncTaskCallbackType (string error);
	  private delegate void AsyncTaskWithStringDataCallbackType (string error, string data);
	  private delegate void AsyncTaskWithDictionaryCallbackType (string error, Dictionary<string, object> data);

Now that you have your list of methods you wish to run in series, pass it to NYSUAsync. It will return a list of the retvals the methods gather:

    StartCoroutine (NYSUAsync.RunTasksInSeries (asyncTasks, (string error, List<object> data) => {
			if (!string.IsNullOrEmpty (error)) {
				Debug.Log (string.Format ("Error running tasks: {0}", error));
				return;
			}

			Debug.Log (string.Format ("Returned this data count: {0}", data.Count));

			Dictionary <string, object> webDict = (Dictionary<string, object>)data [1];

			resultText.text = webDict ["message"].ToString ();
		}));

In the above example we run the three tasks in the asyncTasks list. It returns a list of generic objects. It is the callers responsibility to know what order and type the objects in this list are as they are populated in the order the methods are run. For more information see the included example project.
