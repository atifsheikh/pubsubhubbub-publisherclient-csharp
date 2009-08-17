using System;
using System.Web;
using System.Net;
using System.Text;
using System.IO;


public class PubSubHubbubPublisher
{

	/// <summary>
	/// Publish a topic on a PubSubHubbub-hub, notifies the hub that there's an update.
	/// </summary>
	/// <param name="hubURL">URL to the PubSubHubbub-hub</param>
	/// <param name="topicURL">URL to the topic</param>
	public static void Publish(string hubURL, string topicURL)
	{
		if (String.IsNullOrEmpty(hubURL))
			throw new Exception("Error publishing to PubSubHubbub-hub, hubURL is not defined!");
		if (String.IsNullOrEmpty(topicURL))
			throw new Exception("Error publishing to PubSubHubbub-hub, topicURL is not defined!");

		try
		{
			string postDataStr = "hub.mode=publish&hub.url=" + HttpUtility.UrlEncode(topicURL);
			byte[] postData = Encoding.UTF8.GetBytes(postDataStr);

			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(hubURL);
			httpRequest.Method = "POST";
			httpRequest.ContentType = "application/x-www-form-urlencoded";
			httpRequest.ContentLength = postData.Length;

			Stream requestStream = httpRequest.GetRequestStream();
			requestStream.Write(postData, 0, postData.Length);
			requestStream.Flush();
			requestStream.Close();

			HttpWebResponse webResponse = (HttpWebResponse)httpRequest.GetResponse();
			if (httpRequest.HaveResponse)
			{
				Stream responseStream = webResponse.GetResponseStream();
				StreamReader responseReader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
				responseReader.ReadToEnd();

				if (webResponse.StatusCode != HttpStatusCode.NoContent)
					throw new Exception("Received unexpected statusCode from hub: '" + webResponse.StatusCode.ToString() + "' (should be 204 'No Content')");
			}
			else
				throw new Exception("Didn't receive any response from the hub");
		}
		catch (Exception ex)
		{
			throw new Exception("Error publishing topicURL '" + topicURL + "' to pubSubHubbub-hub '" + hubURL + "'", ex);
		}
	}

}
