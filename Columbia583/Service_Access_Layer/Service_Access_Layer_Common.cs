using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Columbia583
{
	public class Service_Access_Layer_Common
	{
		public Service_Access_Layer_Common ()
		{

		}


		/// <summary>
		/// Gets all the database data, encapsulated in a list of trails.
		/// </summary>
		/// <returns>The list of trails.</returns>
		public List<Webservice_Trails> getAll()
		{
			// Define the webservice's getter method.
			string searchUrl = "http://trails.greenways.ca/api/v1/GetAll";

			// Call the webservice's getter method.
			System.Net.WebRequest req = System.Net.WebRequest.Create(searchUrl);
			System.Net.WebResponse resp = req.GetResponse();

			// Read the response.
			System.IO.StreamReader read = new System.IO.StreamReader(resp.GetResponseStream());
			string letter = read.ReadToEnd();
			read.Close();

			List<Webservice_Trails> allTrails = null;

			try
			{
				// Deserialize the data.
				allTrails = new List<Webservice_Trails>(Newtonsoft.Json.JsonConvert.DeserializeObject<Webservice_Trails[]>(letter));
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}

			return allTrails;
		}


		/// <summary>
		/// Gets all updates since the given date, encapsulated in a list of trails.  The given
		/// last updated date must be in the form of YYYY-MM-DD (eg. 2015-02-30).
		/// </summary>
		/// <returns>The updates.</returns>
		/// <param name="lastUpdated">Last updated date.</param>
		public List<Webservice_Trails> updateAll(String lastUpdated)
		{
			// Define the webservice's getter method.
			string updateUrl = "http://trails.greenways.ca/api/v1/GetChanges/" + lastUpdated;

			// Call the webservice's getter method.
			System.Net.WebRequest req = System.Net.WebRequest.Create(updateUrl);
			System.Net.WebResponse resp = req.GetResponse();

			// Read the response.
			System.IO.StreamReader read = new System.IO.StreamReader(resp.GetResponseStream());
			string letter = read.ReadToEnd();
			read.Close();

			List<Webservice_Trails> updateTrails = null;

			try
			{
				// Deserialize the data.
				updateTrails = new List<Webservice_Trails>(Newtonsoft.Json.JsonConvert.DeserializeObject<Webservice_Trails[]>(letter));
			}
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
			}

			return updateTrails;
		}
	}
}

