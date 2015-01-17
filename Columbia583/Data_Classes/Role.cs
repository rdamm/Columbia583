using System;

namespace Columbia583
{
	public class Role
	{
		public int id { get; set; }
		public string name { get; set; }
		public DateTime timestamp { get; set; }

		public Role()
		{

		}

		public Role (int id, string name, DateTime timestamp)
		{
			this.id = id;
			this.name = name;
			this.timestamp = timestamp;
		}
	}
}

