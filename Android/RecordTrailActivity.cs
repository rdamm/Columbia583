
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms.Platform.Android;
using Android.Locations;

namespace Columbia583.Android
{
	[Activity (Label = "RecordTrailActivity")]			
	public class RecordTrailActivity : AndroidActivity
	{
		protected Button btnStartRecordingTrail = null;
		protected Button btnStopRecordingTrail = null;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.RecordTrail);

			// Get the controls.
			btnStartRecordingTrail = FindViewById<Button> (Resource.Id.btnStartRecordingTrail);
			btnStopRecordingTrail = FindViewById<Button> (Resource.Id.btnStopRecordingTrail);

			// Set the event handlers.
			if (btnStartRecordingTrail != null) {
				btnStartRecordingTrail.Click += RecordTrailEvent;
			}
			if (btnStopRecordingTrail != null) {
				btnStopRecordingTrail.Click += StopRecordingTrailEvent;
			}
		}


		void RecordTrailEvent(object sender, EventArgs eventArgs)
		{
			// Start the record trail service.
			this.StartService (new Intent (this, typeof(RecordTrailService)));
		}


		void StopRecordingTrailEvent(object sender, EventArgs eventArgs)
		{
			// Get a reference to the service.
			RecordTrailService currentRecordTrailService = RecordTrailService.getService ();

			// Get the trail that has been recorded so far.
			if (currentRecordTrailService != null) {
				List<Location> recordedPoints = currentRecordTrailService.getRecordedPoints ();
			}

			// Stop the record trail service.
			StopService (new Intent (this, typeof(RecordTrailService)));

			// Open the upload trail form, passing in the recorded points.
		}
	}
}

