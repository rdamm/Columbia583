using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Columbia583
{
    public class Webservice_Trails
    {
		public int id { get; set; }
		public int userId { get; set; }
		public int orgId { get; set; }
		public string name { get; set; }
		public string location { get; set; }
		public string rating_cache { get; set; }
		public string rating_count { get; set; }
		public string kml_name { get; set; }
		public string kml_content { get; set; }
		public string path { get; set; }
		public string distance { get; set; }
		public string duration { get; set; }
		public string trail_head_elevation { get; set; }
		public string max_elevation { get; set; }
		public string cumulative_elevation { get; set; }
		public string elevation_gain { get; set; }
		public string start_elevation { get; set; }
		public string end_elevation { get; set; }
		public string start_lat { get; set; }
		public string start_lng { get; set; }
		public string end_lat { get; set; }
		public string end_lng { get; set; }
		public string description { get; set; }
		public string directions { get; set; }
		public string difficulty { get; set; }
		public int rating { get; set; }
		public string hazards { get; set; }
		public string surface { get; set; }
		public string landAccess { get; set; }
		public string maintenance { get; set; }
		public string season { get; set; }
		public string open { get; set; }
		public bool active { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		public Webservice_User user { get; set; }
		public Webservice_Organization organization { get; set; }
		public List<Webservice_Activity> activity { get; set; }
		public List<Webservice_Amenity> amenity { get; set; }
		// public List<Webservice_Point> point { get; set; }

		public Webservice_Trails()
		{

		}

        public Webservice_Trails (int id, int userId, int orgId, string name, string location, string kml_name, string kml_content, string distance, string duration,
			string description, string directions, string difficulty, int rating, string hazards, string surface, string landAccess, string maintenance, string season,
			string open, bool active, Webservice_User user, List<Webservice_Amenity> amenity, List<Webservice_Activity> activity, Webservice_Organization organization)
        {
            this.id = id;
            this.userId = userId;
            this.orgId = orgId;
            this.name = name;
            this.location = location;
			this.kml_name = kml_name;
			this.kml_content = kml_content;
            this.distance = distance;
            this.duration = duration;
            this.description = description;
            this.directions = directions;
            this.difficulty = difficulty;
            this.rating = rating;
            this.hazards = hazards;
            this.surface = surface;
            this.landAccess = landAccess;
            this.maintenance = maintenance;
            this.season = season;
            this.open = open;
            this.active = active;
            this.user = user;
            this.amenity = amenity;
            this.activity = activity;
            this.organization = organization;
        }
    }
}
