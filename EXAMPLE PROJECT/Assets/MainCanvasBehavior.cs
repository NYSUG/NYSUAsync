using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MainCanvasBehavior : MonoBehaviour {

	public Text resultText;

#region Callback Delegate Definitions

	private delegate void AsyncTaskCallbackType (string error);
	private delegate void AsyncTaskWithStringDataCallbackType (string error, string data);
	private delegate void AsyncTaskWithDictionaryCallbackType (string error, Dictionary<string, object> data);

#endregion

#region Button Actions

	public void RunAsyncTasksButtonPressed ()
	{
		List<System.Action> asyncTasks = new List<System.Action> ();
		asyncTasks.Add (() => StartCoroutine (_AsyncWaitTask (NYSUAsync.Next)));
		asyncTasks.Add (() => StartCoroutine (_AsyncWebCallWithStringTask (NYSUAsync.Next)));
		asyncTasks.Add (() => StartCoroutine (_AsyncWebCallWithDictionaryTask (NYSUAsync.Next)));

		StartCoroutine (NYSUAsync.RunTasksInSeries (asyncTasks, (string error, List<object> data) => {
			if (!string.IsNullOrEmpty (error)) {
				Debug.Log (string.Format ("Error running tasks: {0}", error));
				return;
			}

			Debug.Log (string.Format ("Returned this data count: {0}", data.Count));

			Dictionary <string, object> webDict = (Dictionary<string, object>)data [1];

			resultText.text = webDict ["message"].ToString ();
		}));
	}

#endregion

#region Async Tasks

	private IEnumerator _AsyncWaitTask (AsyncTaskCallbackType callback)
	{
		yield return new WaitForSeconds (1.0f);

		Debug.Log ("_AsyncWaitTask is complete");

		callback (string.Empty);
	}

	private IEnumerator _AsyncWebCallWithStringTask (AsyncTaskWithStringDataCallbackType callback)
	{
		WWW www = new WWW ("http://www.noyoushutupgames.com/lib/NYSUAsync/TestData.json");
		
		// Wait for it to complete
		while (!www.isDone) {            
			yield return new WaitForSeconds (0.1f);
		}
		
		if (!string.IsNullOrEmpty (www.error)) {
			callback (www.error, null);
			yield break;
		}

		try {
			string payload = ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF8, Encoding.ASCII, www.bytes));

			Debug.Log ("_AsyncWebCallWithStringTask is complete");

			callback (string.Empty, payload);
		} catch (System.Exception e) {
			callback (e.ToString (), null);
		}

		yield break;
	}

	private IEnumerator _AsyncWebCallWithDictionaryTask (AsyncTaskWithDictionaryCallbackType callback)
	{
		WWW www = new WWW ("http://www.noyoushutupgames.com/lib/NYSUAsync/TestData.json");
		
		// Wait for it to complete
		while (!www.isDone) {            
			yield return new WaitForSeconds (0.1f);
		}
		
		if (!string.IsNullOrEmpty (www.error)) {
			callback (www.error, null);
			yield break;
		}
		
		try {
			string payload = ASCIIEncoding.ASCII.GetString (Encoding.Convert (Encoding.UTF8, Encoding.ASCII, www.bytes));
			Dictionary<string, object> data = (Dictionary<string, object>)MiniJSON.Json.Deserialize (payload);

			Debug.Log ("_AsyncWebCallWithDictionaryTask is complete");

			callback (string.Empty, data);
		} catch (System.Exception e) {
			callback (e.ToString (), null);
		}

		yield break;
	}

#endregion

}
