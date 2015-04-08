
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
using Android.Content.PM;

namespace Columbia583.Android
{
	[Activity (Label = "RecordTrailActivity", NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class RecordTrailActivity : AndroidActivity
	{
		protected LinearLayout recordTrailWrapper = null;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.RecordTrail);

			// Get the controls.
			recordTrailWrapper = FindViewById<LinearLayout> (Resource.Id.layout_recordTrailWrapper);

			// Get a reference to the service.
			RecordTrailService currentRecordTrailService = RecordTrailService.getService ();

			// Check if recording is in progress.
			bool recordingInProgress = false;
			if (currentRecordTrailService != null)
			{
				recordingInProgress = currentRecordTrailService.getRecordingInProgress();
			}

			// If recording is in progress, show the stop button.  Otherwise, show the start button.
			if (recordingInProgress == true)
			{
				setStopRecordingButton ();
			}
			else
			{
				setStartRecordingButton ();
			}
		}


		/// <summary>
		/// Sets the record trail wrapper's button to be the start recording button.
		/// </summary>
		protected void setStartRecordingButton()
		{
			// Create a button for start record.
			Button startRecordButton = new Button(this);
			startRecordButton.Text = "Start Recording";

			// Set the event handler.
			startRecordButton.Click += RecordTrailEvent;

			// Add the button to the wrapper.
			recordTrailWrapper.RemoveAllViews();
			recordTrailWrapper.AddView (startRecordButton);
		}


		/// <summary>
		/// Sets the record trail wrapper's button to be the stop recording button.
		/// </summary>
		protected void setStopRecordingButton()
		{
			// Create a button for stop record.
			Button stopRecordButton = new Button(this);
			stopRecordButton.Text = "Stop Recording and Upload";

			// Set the event handler.
			stopRecordButton.Click += StopRecordingTrailEvent;

			// Add the button to the wrapper.
			recordTrailWrapper.RemoveAllViews();
			recordTrailWrapper.AddView (stopRecordButton);
		}


		/// <summary>
		/// This event starts recording the trail.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void RecordTrailEvent(object sender, EventArgs eventArgs)
		{
			// Start the record trail service.
			this.StartService (new Intent (this, typeof(RecordTrailService)));

			// Set the stop recording button.
			setStopRecordingButton ();

			// Inform the user that recording has started.
			Toast.MakeText(this, "Recording trail...", ToastLength.Short).Show();
		}


		/// <summary>
		/// This event stops recording the trail.  If any points have been recorded, the upload trail activity
		/// is loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="eventArgs">Event arguments.</param>
		protected void StopRecordingTrailEvent(object sender, EventArgs eventArgs)
		{
			// Get a reference to the service.
			RecordTrailService currentRecordTrailService = RecordTrailService.getService ();

			// Get the trail that has been recorded so far.
			List<Location> recordedPoints = null;
			if (currentRecordTrailService != null)
			{
				recordedPoints = currentRecordTrailService.getRecordedPoints ();
			}

			// Stop the record trail service.
			StopService (new Intent (this, typeof(RecordTrailService)));

			// If some points got recorded, load the upload trail form.  Otherwise, inform the user that
			// nothing has been recorded.
			if (recordedPoints != null && recordedPoints.Count > 0)
			{
				// Convert the recorded points into Point objects.
				// TODO: Order the IDs somehow.
				List<Point> points = new List<Point>();
				bool first = true;
				DateTime recordTime = DateTime.Now;
				foreach (Location location in recordedPoints)
				{
					points.Add (new Point (0, 0, 0, 0, "", "", location.Latitude, location.Longitude, first, recordTime));
				}

				// Open the upload trail form, passing in the recorded points.
				var intent = new Intent(this, typeof(UploadTrail));
				string pointsJsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(points);
				intent.PutExtra("points", pointsJsonStr);
				StartActivity(intent);
			}
			else
			{
				Toast.MakeText (this, "No points recorded.", ToastLength.Short).Show ();
			}

			// Set the start recording button.
			setStartRecordingButton();
		}
	}
}

