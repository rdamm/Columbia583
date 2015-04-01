using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;

namespace Columbia583
{
	public class User : IEquatable<User>
	{
		// Keys and references.
		[PrimaryKey]
		public int id { get; set; }
		[ForeignKey(typeof(Organization))]
		public int orgId { get; set; }

		// Data.
		// NOTE: The login token will be kept separate from the database so that the login information
		// of other users cannot be compromised.  The client should not have access to the passwords
		// or tokens of other users.
		public string email { get; set; }
		public string username { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool pushToServer { get; set; }
		
		public User()
		{
			this.id = 0;
			this.orgId = 0;
			this.email = "";
			this.username = "";
			this.createdAt = new DateTime(1970, 1, 1);
			this.updatedAt = new DateTime(1970, 1, 1);
			this.pushToServer = false;
		}

		public User (int id, int orgId, string email, string username, DateTime createdAt, DateTime updatedAt, bool pushToServer)
		{
			this.id = id;
			this.orgId = orgId;
			this.email = email;
			this.username = username;
			this.createdAt = createdAt;
			this.updatedAt = updatedAt;
			this.pushToServer = pushToServer;
		}


		/// <param name="other">The object to be compared to this.  Needed for equatable.</param>
		/// <summary>
		/// Checks whether this and the given object are equal by comparing IDs.
		/// </summary>
		/// <returns>True if the IDs are the same, false otherwise.</returns>
		public bool Equals(User other)
		{
			// Check whether the compared object is null. 
			if (Object.ReferenceEquals(other, null)) return false;

			// Check whether the compared object references the same data. 
			if (Object.ReferenceEquals(this, other)) return true;

			// Check whether the IDs are equal.
			return id.Equals(other.id);
		}


		/// <summary>
		/// Gets the hash code for this object.  Needed for equatable.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return id;
		}
	}
}

