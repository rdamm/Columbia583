using System;

namespace Columbia583
{
	public class Webservice_Comment
	{
		public int id { get; set; }
		public int trail_id { get; set; }
		public int user_id { get; set; }
		public int rating { get; set; }
		public int approved { get; set; }
		public int spam { get; set; }
		public string comment { get; set; }
		public string created_at { get; set; }
		public string updated_at { get; set; }
		public Webservice_User user { get; set; }

		public Webservice_Comment (int id, int trail_id, int user_id, int rating, string comment, int approved, int spam, string created_at, string updated_at, Webservice_User user)
		{
			this.id = id;
			this.trail_id = trail_id;
			this.user_id = user_id;
			this.rating = rating;
			this.approved = approved;
			this.spam = spam;
			this.comment = comment;
			this.created_at = created_at;
			this.updated_at = updated_at;
			this.user = user;
		}
	}
}

