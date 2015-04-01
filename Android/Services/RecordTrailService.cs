using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Widget;

namespace Columbia583.Android
{
	// Binds the record trail service.
	public class RecordTrailServiceBinder : Binder
	{
		public RecordTrailService Service
		{
			get { return this.service; }
		} protected RecordTrailService service;

		public bool IsBound { get; set; }

		// constructor
		public RecordTrailServiceBinder (RecordTrailService service)
		{
			this.service = service;
		}
	}


	[Service]
	public class RecordTrailService : Service, ILocationListener
	{
		public static RecordTrailService service = null;

		protected IBinder binder = null;
		protected bool recordingInProgress = false;
		protected List<Location> recordedPoints = null;
		protected LocationManager locMgr = null;

		public RecordTrailService ()
		{

		}


		/// <summary>
		/// Raises the bind event.
		/// </summary>
		/// <param name="intent">Intent.</param>
		public override IBinder OnBind (Intent intent)
		{
			binder = new RecordTrailServiceBinder (this);
			return binder;
		}


		/// <summary>
		/// Called by the system when the service is first created.
		/// </summary>
		public override void OnCreate ()
		{
			base.OnCreate ();
			service = this;
		}


		/// <summary>
		/// Raises the start command event.
		/// </summary>
		/// <param name="intent">Intent.</param>
		/// <param name="flags">Flags.</param>
		/// <param name="startId">Start identifier.</param>
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			// If recording is already in progress, do not start the service.
			if (recordingInProgress == true)
			{
				Toast.MakeText (this, "Recording already in progress.", ToastLength.Short).Show ();
				return StartCommandResult.StickyCompatibility;
			}

			// Get the location manager
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			// Get the GPS provider.  If the GPS provider is not available, do not start the service.
			string Provider = LocationManager.GpsProvider;
			if (!locMgr.IsProviderEnabled (Provider))
			{
				Toast.MakeText (this, "GPS provider unavailable.  Cannot record trail.", ToastLength.Short).Show ();
				return StartCommandResult.StickyCompatibility;
			}

			// Set the recording flag.
			recordingInProgress = true;

			// Create a new list of recorded points.
			recordedPoints = new List<Location>();

			// Request a location update every second.  Ignore distance increments.
			locMgr.RequestLocationUpdates (Provider, 1000, 0.0f, this);

			return StartCommandResult.Sticky;
		}


		/// <summary>
		/// Called by the system to notify a Service that it is no longer used and is being removed.
		/// </summary>
		public override void OnDestroy ()
		{
			base.OnDestroy ();

			// Stop the recording.
			recordingInProgress = false;

			// Remove the location update event.
			locMgr.RemoveUpdates (this);

			// Clear the recorded points.
			recordedPoints = null;

			// Clear the reference to the service.
			service = null;
		}


		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <returns>The service.</returns>
		public static RecordTrailService getService()
		{
			return service;
		}


		/// <summary>
		/// Gets the recording state flag.
		/// </summary>
		/// <returns><c>true</c>, if recording in progress, <c>false</c> otherwise.</returns>
		public bool getRecordingInProgress()
		{
			return recordingInProgress;
		}


		/// <summary>
		/// Gets the recorded points.
		/// </summary>
		/// <returns>The recorded points.</returns>
		public List<Location> getRecordedPoints()
		{
			return recordedPoints;
		}


		/// <param name="provider">the name of the location provider associated with this
		///  update.</param>
		/// <summary>
		/// Called when the provider is enabled by the user.
		/// </summary>
		public void OnProviderEnabled (string provider)
		{
			Toast.MakeText (this, "GPS provider enabled.  Trail recording resumed.", ToastLength.Short).Show ();
			Console.WriteLine ("GPS provider enabled.  Trail recording resumed.");
		}


		/// <param name="provider">the name of the location provider associated with this
		///  update.</param>
		/// <summary>
		/// Called when the provider is disabled by the user.
		/// </summary>
		public void OnProviderDisabled (string provider)
		{
			Toast.MakeText (this, "GPS provider disabled.  Trail recording paused.", ToastLength.Short).Show ();
			Console.WriteLine ("GPS provider disabled.  Trail recording paused.");
		}


		/// <param name="provider">the name of the location provider associated with this
		///  update.</param>
		/// <summary>
		/// Raises the status changed event.
		/// </summary>
		/// <param name="status">Status.</param>
		/// <param name="extras">Extras.</param>
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			if (status == Availability.Available)
			{
				Toast.MakeText (this, "Acquired GPS signal.  Trail recording resumed.", ToastLength.Short).Show ();
				Console.WriteLine ("Acquired GPS signal.  Trail recording resumed.");
			}
			else if (status == Availability.TemporarilyUnavailable)
			{
				Toast.MakeText (this, "GPS signal lost.  Trail recording paused.", ToastLength.Short).Show ();
				Console.WriteLine ("GPS signal lost.  Trail recording paused.");
			}
			else
			{
				Toast.MakeText (this, "GPS signal lost.  Trail recording paused.", ToastLength.Short).Show ();
				Console.WriteLine ("GPS signal lost.  Trail recording paused.");
			}
		}


		/// <param name="location">The new location, as a Location object.</param>
		/// <summary>
		/// Called when the location has changed.
		/// </summary>
		public void OnLocationChanged (Location location)
		{
			// If currently recording a trail, log the current location.
			if (recordingInProgress == true)
			{
				recordedPoints.Add (location);
				Console.WriteLine ("Added (" + location.Longitude + ", " + location.Latitude + ") to trail points.");
			}
		}
	}
}

