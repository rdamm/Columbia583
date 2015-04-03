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
	//This is our Binder subclass, the LocationServiceBinder
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

		public override IBinder OnBind (Intent intent)
		{
			binder = new RecordTrailServiceBinder (this);
			return binder;
		}


		public override void OnCreate ()
		{
			base.OnCreate ();
			service = this;
		}

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

			// Get the GPS provider.  If no provider is available, do not start the service.
			string Provider = LocationManager.GpsProvider;
			if (!locMgr.IsProviderEnabled (Provider))
			{
				Toast.MakeText (this, "GPS provider unavailable.", ToastLength.Long).Show ();
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


		public static RecordTrailService getService()
		{
			return service;
		}


		public List<Location> getRecordedPoints()
		{
			return recordedPoints;
		}


		public void OnProviderEnabled (string provider)
		{
			Toast.MakeText (this, "Provider enabled.", ToastLength.Short).Show ();
			Console.WriteLine ("Provider enabled.");
		}

		public void OnProviderDisabled (string provider)
		{
			Toast.MakeText (this, "Provider disabled.", ToastLength.Short).Show ();
			Console.WriteLine ("Provider disabled.");
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			Toast.MakeText (this, "Status has changed.", ToastLength.Short).Show ();
			Console.WriteLine ("Status has changed.");
		}

		public void OnLocationChanged (Location location)
		{
			// If currently recording a trail, add the current location.
			if (recordingInProgress == true) {
				recordedPoints.Add (location);
				Toast.MakeText (this, "Added (" + location.Longitude + ", " + location.Latitude + ") to trail points.", ToastLength.Short).Show ();
				Console.WriteLine ("Added (" + location.Longitude + ", " + location.Latitude + ") to trail points.");
			} else {
				Console.WriteLine ("OnLocationChanged called, but not currently recording.");
			}
		}
	}
}

