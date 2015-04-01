using System;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

// TODO: Figure out how to display images that are loaded from the database (will probably need to use a stream).
// TODO: Implement image caching logic for the map tiles and the media.
// TODO: Create an ID getter for each table for Mariam's update methods. (TrailsToActivities and TrailsToAmenities).
// TODO: Remove the auto-increment attribute from the database primary keys.

// NOTE: Area of interest is 170 km x 50 km

namespace Columbia583
{
	/// <summary>
	/// Data layer common contains the SQLite queries that are globally common for the app.
	/// </summary>
	public class Data_Layer_Common
	{
		public Data_Layer_Common ()
		{

		}


		/// <summary>
		/// Gets the path to database.
		/// </summary>
		/// <returns>The path to database.</returns>
		public static string getPathToDatabase()
		{
			string docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			string pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			return pathToDatabase;
		}


		/// <summary>
		/// Determines whether the database has been initialized or not by checking for the existence of
		/// all the required tables.
		/// </summary>
		/// <returns><c>true</c>, if database was initialized, <c>false</c> otherwise.</returns>
		public bool databaseInitialized()
		{
			bool databaseInitialized = false;

			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get a list of the database tables.
				List<Type> tableTypes = new List<Type>();
				tableTypes.Add(typeof(AppGlobals));
				tableTypes.Add(typeof(Activity));
				tableTypes.Add(typeof(Amenity));
				tableTypes.Add(typeof(MapTile));
				tableTypes.Add(typeof(Organization));
				tableTypes.Add(typeof(Role));
				tableTypes.Add(typeof(User));
				tableTypes.Add(typeof(Trail));
				tableTypes.Add(typeof(Media));
				tableTypes.Add(typeof(Point));
				tableTypes.Add(typeof(TrailsToActivities));
				tableTypes.Add(typeof(TrailsToAmenities));
				tableTypes.Add(typeof(FavouriteTrails));
				tableTypes.Add(typeof(Comment));

				// Check if the tables exist.
				string tableExistsQuery = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = ?";
				bool allTablesExist = true;
				foreach(Type tableType in tableTypes)
				{
					// Get the table's name.
					string tableName = tableType.Name;

					// Check if the table exists.
					SQLiteCommand tableExistsCommand = connection.CreateCommand(tableExistsQuery, tableName);
					bool tableExists = (tableExistsCommand.ExecuteScalar<string>() != null);
					if (tableExists != true)
					{
						allTablesExist = false;
						break;
					}
				}

				// Check if the tables have all the correct columns.
				bool allColumnsExist = true;
				foreach(Type tableType in tableTypes)
				{
					// Get the table's name.
					string tableName = tableType.Name;
					
					// Get the column names from the table's properties.
					System.Reflection.PropertyInfo[] properties = tableType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

					// Check if the table contains all the fields.
					foreach(System.Reflection.PropertyInfo property in properties)
					{
						// Get the column name.
						string columnName = property.Name;

						try
						{
							// Check if the column exists.  An exception will be thrown if it does not exist.
							SQLiteCommand cmd = connection.CreateCommand("SELECT " + columnName + " FROM " + tableName, new object[0]);
							var response = cmd.ExecuteScalar<string>();
						}
						catch (Exception e)
						{
							allColumnsExist = false;
							break;
						}
					}
				}
				
				// Determine if the database has been initialized based on the table and column existences.
				databaseInitialized = allTablesExist && allColumnsExist;
				
				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return databaseInitialized;
		}


		/// <summary>
		/// Creates the tables in the database.
		/// </summary>
		public void createTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Create all the tables that have no foreign keys.
				connection.CreateTable<AppGlobals>();
				connection.CreateTable<Activity>();
				connection.CreateTable<Amenity>();
				connection.CreateTable<MapTile>();
				connection.CreateTable<Organization>();
				connection.CreateTable<Role>();
				connection.CreateTable<Comment>();

				// Create all the tables that have foreign keys.
				connection.CreateTable<User>();					// References Organization.
				connection.CreateTable<Trail>();				// References Organization and User.
				connection.CreateTable<Media>();				// References Trail.
				connection.CreateTable<Point>();				// References Trail and MapTile.
				connection.CreateTable<TrailsToActivities>();	// References Trail and Activity.
				connection.CreateTable<TrailsToAmenities>();	// References Trail and Amenity.
				connection.CreateTable<FavouriteTrails>();		// References Trail and User.

				// Populate the app globals.  Initialize the database last updated time to the epoch.
				AppGlobals appGlobals = new AppGlobals(0, new DateTime(1970, 1, 1));
				connection.Insert(appGlobals);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		public void createCommentTable()
		{
			try {
				var connection = new SQLiteConnection (getPathToDatabase ());

				connection.CreateTable<Comment>();

				connection.Close ();
			} catch (SQLiteException ex) {
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		public void insertTrail(Trail trail){
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				connection.Insert(trail);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		public void insertComment(Comment comment){
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				connection.Insert(comment);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		/// <summary>
		/// Inserts the rows.
		/// NOTE: SQLite ignores primary keys during insertion.  It will insert the data regardless of what you set the primary key to.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="favouriteTrails">Favourite trails.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
		public void insertRows(Activity[] activities, Amenity[] amenities, FavouriteTrails[] favouriteTrails, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Insert the data that has no foreign keys.
				connection.InsertAll(activities);
				connection.InsertAll(amenities);
				connection.InsertAll(mapTiles);
				connection.InsertAll(organizations);
				connection.InsertAll(roles);

				// Insert the data that has foreign keys.
				connection.InsertAll(users);
				connection.InsertAll(trails);
				connection.InsertAll(media);
				connection.InsertAll(points);
				connection.InsertAll(trailsToActivities);
				connection.InsertAll(trailsToAmenities);
				connection.InsertAll(favouriteTrails);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		public void insertCommentRows(Comment[] comments)
		{
			try {
				var connection = new SQLiteConnection (getPathToDatabase ());

				connection.InsertAll(comments);

				connection.Close ();
			} catch (SQLiteException ex) {
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/// <summary>
		/// Gets the activities.
		/// </summary>
		/// <returns>The activities.</returns>
		public List<Activity> getActivities()
		{
			List<Activity> activities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all activities.
				var query = connection.Table<Activity>();
				activities = new List<Activity>();
				foreach(Activity activity in query)
				{
					activities.Add(activity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activities;
		}

		public string[] getDifficulty(){
			string[] names = Enum.GetNames (typeof(Difficulty));
			return names;
		}

		/// <summary>
		/// Gets the amenities.
		/// </summary>
		/// <returns>The amenities.</returns>
		public List<Amenity> getAmenities()
		{
			List<Amenity> amenities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all amenities.
				var query = connection.Table<Amenity>();
				amenities = new List<Amenity>();
				foreach(Amenity amenity in query)
				{
					amenities.Add(amenity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return amenities;
		}

		public List<User> getUsers()
		{
			List<User> users = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all amenities.
				var query = connection.Table<User>();
				users = new List<User>();
				foreach(User user in query)
				{
					users.Add(user);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
			return users;
		}


		/// <summary>
		/// Gets the IDs for all the activities.
		/// </summary>
		/// <returns>The activity identifiers.</returns>
		public List<int> getActivityIds()
		{
			List<int> activityIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Activity>();
				activityIds = new List<int>();
				foreach(Activity activity in query)
				{
					activityIds.Add(activity.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activityIds;
		}


		/// <summary>
		/// Gets the IDs for all the amenities.
		/// </summary>
		/// <returns>The amenity identifiers.</returns>
		public List<int> getAmenityIds()
		{
			List<int> amenityIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Amenity>();
				amenityIds = new List<int>();
				foreach(Amenity amenity in query)
				{
					amenityIds.Add(amenity.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return amenityIds;
		}

		/// <summary>
		/// Gets the IDs for all the comments.
		/// </summary>
		/// <returns>The comment identifiers.</returns>
		public List<int> getCommentIds()
		{
			List<int> commentIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Comment>();
				commentIds = new List<int>();
				foreach(Comment comment in query)
				{
					commentIds.Add(comment.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return commentIds;
		}


		/// <summary>
		/// Gets the IDs for all the map tiles.
		/// </summary>
		/// <returns>The map tile identifiers.</returns>
		public List<int> getMapTileIds()
		{
			List<int> mapTileIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<MapTile>();
				mapTileIds = new List<int>();
				foreach(MapTile mapTile in query)
				{
					mapTileIds.Add(mapTile.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mapTileIds;
		}


		/// <summary>
		/// Gets the IDs for all the media.
		/// </summary>
		/// <returns>The media identifiers.</returns>
		public List<int> getMediaIds()
		{
			List<int> mediaIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Media>();
				mediaIds = new List<int>();
				foreach(Media media in query)
				{
					mediaIds.Add(media.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mediaIds;
		}


		/// <summary>
		/// Gets the IDs for all the organizations.
		/// </summary>
		/// <returns>The organization identifiers.</returns>
		public List<int> getOrganizationIds()
		{
			List<int> organizationIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Organization>();
				organizationIds = new List<int>();
				foreach(Organization organization in query)
				{
					organizationIds.Add(organization.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return organizationIds;
		}


		/// <summary>
		/// Gets the IDs for all the points.
		/// </summary>
		/// <returns>The point identifiers.</returns>
		public List<int> getPointIds()
		{
			List<int> pointIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Point>();
				pointIds = new List<int>();
				foreach(Point point in query)
				{
					pointIds.Add(point.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return pointIds;
		}


		/// <summary>
		/// Gets the IDs for all the roles.
		/// </summary>
		/// <returns>The role identifiers.</returns>
		public List<int> getRoleIds()
		{
			List<int> roleIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Role>();
				roleIds = new List<int>();
				foreach(Role role in query)
				{
					roleIds.Add(role.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return roleIds;
		}


		/// <summary>
		/// Gets the IDs for all the trails.
		/// </summary>
		/// <returns>The trail identifiers.</returns>
		public List<int> getTrailIds()
		{
			List<int> trailIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<Trail>();
				trailIds = new List<int>();
				foreach(Trail trail in query)
				{
					trailIds.Add(trail.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailIds;
		}


		/// <summary>
		/// Gets the activity.
		/// </summary>
		/// <returns>The activity.</returns>
		/// <param name="id">Identifier.</param>
		public Activity getActivity(int id)
		{
			Activity activity = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the activity.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				activity = connection.Find<Activity>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return activity;
		}


		/// <summary>
		/// Gets the amenity.
		/// </summary>
		/// <returns>The amenity.</returns>
		/// <param name="id">Identifier.</param>
		public Amenity getAmenity(int id)
		{
			Amenity amenity = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the amenity.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				amenity = connection.Find<Amenity>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return amenity;
		}

		/// <summary>
		/// Gets the comment.
		/// </summary>
		/// <returns>The comment.</returns>
		/// <param name="id">Identifier.</param>
		public List<Comment> getCommentid()
		{
			List<Comment> comment = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the amenity.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				var query = connection.Table<Comment>();
				comment = new List<Comment>();
				foreach(Comment c in query)
				{
					comment.Add(c);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return comment;
		}
			
		/// <summary>
		/// Gets the comment.
		/// </summary>
		/// <returns>The comment.</returns>
		/// <param name="id">Identifier.</param>
		public Comment getComment(int id)
		{
			Comment comment = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the amenity.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				comment = connection.Find<Comment>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return comment;
		}


		/// <summary>
		/// Gets the map tile.
		/// </summary>
		/// <returns>The map tile.</returns>
		/// <param name="id">Identifier.</param>
		public MapTile getMapTile(int id)
		{
			MapTile mapTile = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the map tile.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				mapTile = connection.Find<MapTile>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return mapTile;
		}

		public Trail getTrailid(int id)
		{
			Trail trail = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the amenity.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				trail = connection.Find<Trail>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trail;
		}
			
		/// <summary>
		/// Gets the media.
		/// </summary>
		/// <returns>The media.</returns>
		/// <param name="id">Identifier.</param>
		public Media getMedia(int id)
		{
			Media media = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the media.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				media = connection.Find<Media>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return media;
		}


		/// <summary>
		/// Gets the organization.
		/// </summary>
		/// <returns>The organization.</returns>
		/// <param name="id">Identifier.</param>
		public Organization getOrganization(int id)
		{
			Organization organization = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the organization.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				organization = connection.Find<Organization>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return organization;
		}


		/// <summary>
		/// Gets the point.
		/// </summary>
		/// <returns>The point.</returns>
		/// <param name="id">Identifier.</param>
		public Point getPoint(int id)
		{
			Point point = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the point.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				point = connection.Find<Point>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return point;
		}


		/// <summary>
		/// Gets the role.
		/// </summary>
		/// <returns>The role.</returns>
		/// <param name="id">Identifier.</param>
		public Role getRole(int id)
		{
			Role role = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the role.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				role = connection.Find<Role>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return role;
		}


		/// <summary>
		/// Gets the trail.
		/// </summary>
		/// <returns>The trail.</returns>
		/// <param name="id">Identifier.</param>
		public Trail getTrail(int id)
		{
			Trail trail = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the trail.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				trail = connection.Find<Trail>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trail;
		}


		/// <summary>
		/// Gets the user.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="id">Identifier.</param>
		public User getUser(int id)
		{
			User user = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the user.
				// NOTE: Find will return null if row not found.  Don't use Get; it throws Object Not Supported exceptions.
				user = connection.Find<User>(id);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return user;
		}


		/// <summary>
		/// Gets the user by their email.
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="email">Email.</param>
		public User getUserByEmail(string email)
		{
			User user = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get the user.
				var userQueryResponse = connection.Query<User>("SELECT * FROM User WHERE email = ?", email);
				foreach(User u in userQueryResponse)
				{
					user = u;
					break;
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return user;
		}


		/// <summary>
		/// Gets the favourite trails.
		/// </summary>
		/// <returns>The favourite trails.</returns>
		public List<FavouriteTrails> getFavouriteTrails()
		{
			List<FavouriteTrails> favouriteTrails = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<FavouriteTrails>();
				favouriteTrails = new List<FavouriteTrails>();
				foreach(FavouriteTrails favouriteTrail in query)
				{
					favouriteTrails.Add(favouriteTrail);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return favouriteTrails;
		}


		/// <summary>
		/// Gets the trails to activities.
		/// </summary>
		/// <returns>The trails to activities.</returns>
		public List<TrailsToActivities> getTrailsToActivities()
		{
			List<TrailsToActivities> trailsToActivities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<TrailsToActivities>();
				trailsToActivities = new List<TrailsToActivities>();
				foreach(TrailsToActivities trailToActivity in query)
				{
					trailsToActivities.Add(trailToActivity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailsToActivities;
		}


		/// <summary>
		/// Gets the trails to amenities.
		/// </summary>
		/// <returns>The trails to amenities.</returns>
		public List<TrailsToAmenities> getTrailsToAmenities()
		{
			List<TrailsToAmenities> trailsToAmenities = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<TrailsToAmenities>();
				trailsToAmenities = new List<TrailsToAmenities>();
				foreach(TrailsToAmenities trailToAmenity in query)
				{
					trailsToAmenities.Add(trailToAmenity);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return trailsToAmenities;
		}


		/// <summary>
		/// Gets the IDs for all the users.
		/// </summary>
		/// <returns>The user identifiers.</returns>
		public List<int> getUserIds()
		{
			List<int> userIds = null;
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Get all IDs.
				// TODO: Find a more efficient way to get the IDs.
				var query = connection.Table<User>();
				userIds = new List<int>();
				foreach(User user in query)
				{
					userIds.Add(user.id);
				}

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}

			return userIds;
		}

		
		/// <summary>
		/// Updates the rows.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="comments">Comments.</param>
		/// <param name="favouriteTrails">Favourite trails.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
		public void updateRows(Activity[] activities, Amenity[] amenities, Comment[] comments, FavouriteTrails[] favouriteTrails, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());
				
				// Update the data that has no foreign keys.
				connection.UpdateAll(activities);
				connection.UpdateAll(amenities);
				connection.UpdateAll(mapTiles);
				connection.UpdateAll(organizations);
				connection.UpdateAll(roles);

				// Update the data that has foreign keys.
				// TODO: Figure out how to handle foreign keys for updates.
				// Currently ON UPDATE NO ACTION
				connection.UpdateAll(points);
				connection.UpdateAll(media);
				connection.UpdateAll(trails);
				connection.UpdateAll(users);
				connection.UpdateAll(comments);

				
				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		
		/// <summary>
		/// Deletes the rows.
		/// </summary>
		/// <param name="activities">Activities.</param>
		/// <param name="amenities">Amenities.</param>
		/// <param name="comments">Comments.</param>
		/// <param name="favouriteTrails">Favourite trails.</param>
		/// <param name="mapTiles">Map tiles.</param>
		/// <param name="media">Media.</param>
		/// <param name="organizations">Organizations.</param>
		/// <param name="points">Points.</param>
		/// <param name="roles">Roles.</param>
		/// <param name="trails">Trails.</param>
		/// <param name="trailsToActivities">Trails to activities.</param>
		/// <param name="trailsToAmenities">Trails to amenities.</param>
		/// <param name="users">Users.</param>
		public void deleteRows(Activity[] activities, Amenity[] amenities, Comment[] comments, FavouriteTrails[] favouriteTrails, MapTile[] mapTiles, Media[] media, Organization[] organizations,
			Point[] points, Role[] roles, Trail[] trails, TrailsToActivities[] trailsToActivities, TrailsToAmenities[] trailsToAmenities, User[] users)
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());
				
				// Delete the data that has foreign keys.
				// TODO: Fix deletion for TrailsToActivities and TrailsToAmenities.  Their class definition doesn't have a primary key.
				foreach(var row in favouriteTrails)		connection.Delete(row);
				foreach(var row in trailsToActivities)	connection.Delete(row);
				foreach(var row in trailsToAmenities)	connection.Delete(row);
				foreach(var row in points)				connection.Delete(row);
				foreach(var row in comments)			connection.Delete(row);
				foreach(var row in media)				connection.Delete(row);
				foreach(var row in trails)				connection.Delete(row);
				foreach(var row in users)				connection.Delete(row);
				
				// Delete the data that has no foreign keys.
				foreach(var row in activities)		connection.Delete(row);
				foreach(var row in amenities)		connection.Delete(row);
				foreach(var row in mapTiles)		connection.Delete(row);
				foreach(var row in organizations)	connection.Delete(row);
				foreach(var row in roles)			connection.Delete(row);

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}

		
		/// <summary>
		/// Clears the tables.
		/// NOTE: SQLite doesn't reset the primary key index when a table is cleared.  To reset, drop and recreate the tables.
		/// </summary>
		public void clearTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Delete the data that has foreign keys.
				connection.DeleteAll<Comment>();
				connection.DeleteAll<FavouriteTrails>();
				connection.DeleteAll<TrailsToActivities>();
				connection.DeleteAll<TrailsToAmenities>();
				connection.DeleteAll<Point>();
				connection.DeleteAll<Media>();
				connection.DeleteAll<Trail>();
				connection.DeleteAll<User>();

				// Delete the data that has no foreign keys.
				connection.DeleteAll<Activity>();
				connection.DeleteAll<Amenity>();
				connection.DeleteAll<MapTile>();
				connection.DeleteAll<Organization>();
				connection.DeleteAll<Role>();
				connection.DeleteAll<AppGlobals>();

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}


		/// <summary>
		/// Drops the tables.
		/// </summary>
		public void dropTables()
		{
			try
			{
				// Open connection to local database.
				var connection = new SQLiteConnection(getPathToDatabase());

				// Drop the tables that have foreign keys.
				connection.DropTable<Comment>();
				connection.DropTable<FavouriteTrails>();
				connection.DropTable<TrailsToActivities>();
				connection.DropTable<TrailsToAmenities>();
				connection.DropTable<Point>();
				connection.DropTable<Media>();
				connection.DropTable<Trail>();
				connection.DropTable<User>();

				// Drop the tables that have no foreign keys.
				connection.DropTable<Activity>();
				connection.DropTable<Amenity>();
				connection.DropTable<MapTile>();
				connection.DropTable<Organization>();
				connection.DropTable<Role>();
				connection.DropTable<AppGlobals>();

				// Close connection to local database.
				connection.Close();
			}
			catch (SQLiteException ex)
			{
				// TODO: Log the error message.
				Console.WriteLine (ex.Message);
			}
		}
	}
}

