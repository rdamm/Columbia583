
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

namespace Columbia583.Android
{
	[Activity (Label = "ScanQrCodeActivity", NoHistory = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class ScanQrCodeActivity : AndroidActivity
	{
		protected Button btnScanQrCode = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.ScanQrCode);

			// Get the controls.
			btnScanQrCode = FindViewById<Button>(Resource.Id.btnScanQrCode);

			// Set the event handlers.
			if (btnScanQrCode != null) {
				btnScanQrCode.Click += ScanQrCodeEvent;
			}
		}


		async public void ScanQrCodeEvent(object sender, EventArgs eventArgs)
		{
			// Scan the QR code.
			var scanner = new ZXing.Mobile.MobileBarcodeScanner(this);
			var result = await scanner.Scan();

			// If a result was found, determine the trail ID from it and open the trail.
			if (result != null)
			{
				// DEBUG: Display the raw barcode.
				Console.WriteLine ("Scanned Barcode: " + result.Text);

				// Get the barcode's text from the result.
				string barcodeText = result.Text;

				// Check if the barcode text follows the expected format: http://trails.greenways.ca/trails/[TRAIL_ID]
				if (barcodeText.Length <= 34)
				{
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long);
					return;
				}
				string barcodeDomain = barcodeText.Substring(0, 34);
				if (barcodeDomain != "http://trails.greenways.ca/trails/") {
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long);
					return;
				}

				// Pull the ID from the barcode text.
				int trailId = 0;
				string trailIdString = barcodeText.Substring (34, barcodeText.Length - 34);
				try
				{
					trailId = Int32.Parse(trailIdString);
				}
				catch (Exception e) {
					Toast.MakeText (this, "Invalid QR code detected.", ToastLength.Long);
					return;
				}

				Console.WriteLine ("Read trail ID = " + trailIdString);

				// Load the trail details.
				Data_Access_Layer_View_Trail dataAccessLayerViewTrail = new Data_Access_Layer_View_Trail ();
				Trail trail = dataAccessLayerViewTrail.getTrail (trailId);
				if (trail == null)
				{
					Toast.MakeText (this, "Trail not found.", ToastLength.Long);
					return;
				}
				Activity[] activities = dataAccessLayerViewTrail.getActivities (trailId);
				Amenity[] amenities = dataAccessLayerViewTrail.getAmenities (trailId);
				Point[] points = dataAccessLayerViewTrail.getPoints (trailId);

				// Load the trail's page.
				var intent = new Intent (this, typeof(ViewTrailActivity));
				string trailJSONStr = Newtonsoft.Json.JsonConvert.SerializeObject (trail);
				string activitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(activities);
				string amenitiesJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(amenities);
				string pointsJSONstr = Newtonsoft.Json.JsonConvert.SerializeObject(points);
				intent.PutExtra ("viewedTrail", trailJSONStr);
				intent.PutExtra("activities", activitiesJSONstr);
				intent.PutExtra("amenities", amenitiesJSONstr);
				intent.PutExtra("points", pointsJSONstr);
				StartActivity (intent);
			}
		}
	}
}

