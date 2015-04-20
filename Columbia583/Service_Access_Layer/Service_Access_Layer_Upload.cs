using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Columbia583
{
	public class Service_Access_Layer_Upload
	{
		private const string apiBase = "http://localhost:50561/api/";

		public Service_Access_Layer_Upload ()
		{
			
		}


		/// <summary>
		/// Uploads the trail to the server.  Will return true if the upload was successful, and false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if trail was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="trail">Trail.</param>
		public bool uploadTrail(Trail trail)
		{
			bool uploadSuccessful = false;

			try
			{
				// Define the base API url.
				string apiFunction = "trail";

				// Encapsulate the trail in a JSON object.
				string jsonTrail = Newtonsoft.Json.JsonConvert.SerializeObject(trail);

				// Create a string containing the API arguments.
				string apiArguments = "?";
				apiArguments += "trail=" + jsonTrail;

				// Join the API strings.
				string apiRequest = apiBase + apiFunction + apiArguments;

				// Create an HTTP web request using the URL:
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (apiRequest));
				request.ContentType = "application/json";
				request.Method = "POST";

				// Upload the trail to the server and check the response.
				using (WebResponse response = request.GetResponse())
				{
					// TODO: Check if the upload was successful.
				}
			}
			catch (Exception e)
			{
				// TODO: Log the exception.
				Console.WriteLine(e.Message);
			}

			return uploadSuccessful;
		}


		/// <summary>
		/// Uploads the points to the server.  Will return true if the upload was successful, and false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if points was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="points">Points.</param>
		public bool uploadPoints(Point[] points)
		{
			bool uploadSuccessful = false;

			try
			{
				// Define the base API url.
				string apiFunction = "point";

				foreach (Point point in points)
				{
					// Encapsulate the point in a JSON object.
					string jsonPoint = Newtonsoft.Json.JsonConvert.SerializeObject(point);

					// Create a string containing the API arguments.
					string apiArguments = "?";
					apiArguments += "point=" + jsonPoint;

					// Join the API strings.
					string apiRequest = apiBase + apiFunction + apiArguments;

					// Create an HTTP web request using the URL:
					HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (apiRequest));
					request.ContentType = "application/json";
					request.Method = "POST";

					// Upload the point to the server and check the response.
					using (WebResponse response = request.GetResponse())
					{
						// TODO: Check if the upload was successful.
					}
				}
			}
			catch (Exception e)
			{
				// TODO: Log the exception.
				Console.WriteLine(e.Message);
			}

			return uploadSuccessful;
		}


		/// <summary>
		/// Uploads the user to the server.  Will return true if the upload was successful, and false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if user was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="user">User.</param>
		public bool uploadUser(User user)
		{
			bool uploadSuccessful = false;

			try
			{
				// Define the base API url.
				string apiFunction = "user";

				// Encapsulate the user in a JSON object.
				string jsonUser = Newtonsoft.Json.JsonConvert.SerializeObject(user);

				// Create a string containing the API arguments.
				string apiArguments = "?";
				apiArguments += "user=" + jsonUser;

				// Join the API strings.
				string apiRequest = apiBase + apiFunction + apiArguments;

				// Create an HTTP web request using the URL:
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (apiRequest));
				request.ContentType = "application/json";
				request.Method = "POST";

				// Upload the user to the server and check the response.
				using (WebResponse response = request.GetResponse())
				{
					// TODO: Check if the upload was successful.
				}
			}
			catch (Exception e)
			{
				// TODO: Log the exception.
				Console.WriteLine(e.Message);
			}

			return uploadSuccessful;
		}


		/// <summary>
		/// Uploads the comment to the server.  Will return true if the upload was successful, and false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if comment was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="comment">Comment.</param>
		public bool uploadComment(Comment comment)
		{
			bool uploadSuccessful = false;

			try
			{
				// Define the base API url.
				string apiFunction = "comment";

				// Encapsulate the comment in a JSON object.
				string jsonComment = Newtonsoft.Json.JsonConvert.SerializeObject(comment);

				// Create a string containing the API arguments.
				string apiArguments = "?";
				apiArguments += "comment=" + jsonComment;

				// Join the API strings.
				string apiRequest = apiBase + apiFunction + apiArguments;

				// Create an HTTP web request using the URL:
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (apiRequest));
				request.ContentType = "application/json";
				request.Method = "POST";

				// Upload the comment to the server and check the response.
				using (WebResponse response = request.GetResponse())
				{
					// TODO: Check if the upload was successful.
				}
			}
			catch (Exception e)
			{
				// TODO: Log the exception.
				Console.WriteLine(e.Message);
			}

			return uploadSuccessful;
		}


		/// <summary>
		/// Uploads the media to the server.  Will return true if the upload was successful, and false otherwise.
		/// </summary>
		/// <returns><c>true</c>, if media was uploaded, <c>false</c> otherwise.</returns>
		/// <param name="media">Media.</param>
		public bool uploadMedia(Media media)
		{
			bool uploadSuccessful = false;

			try
			{
				// Define the base API url.
				string apiFunction = "media";

				// Encapsulate the media in a JSON object.
				string jsonMedia = Newtonsoft.Json.JsonConvert.SerializeObject(media);

				// Create a string containing the API arguments.
				string apiArguments = "?";
				apiArguments += "media=" + jsonMedia;

				// Join the API strings.
				string apiRequest = apiBase + apiFunction + apiArguments;

				// Create an HTTP web request using the URL:
				HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (new Uri (apiRequest));
				request.ContentType = "application/json";
				request.Method = "POST";

				// Upload the media to the server and check the response.
				using (WebResponse response = request.GetResponse())
				{
					// TODO: Check if the upload was successful.
				}
			}
			catch (Exception e)
			{
				// TODO: Log the exception.
				Console.WriteLine(e.Message);
			}

			return uploadSuccessful;
		}
	}
}

