
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
using System.Net;
using System.IO;
using Android.Graphics;

namespace Columbia583.Android
{
	[Activity (Label = "TestImage")]			
	public class TestImageActivity : AndroidActivity
	{
		protected Button loadImageFromWebButton = null;
		protected Button loadImageFromDatabaseButton = null;

		protected ImageView imageTestImage = null;

		protected string testImageUrl = "https://www.google.ca/images/srpr/logo11w.png";

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.TestImage);

			// Get the controls.
			loadImageFromWebButton = FindViewById<Button> (Resource.Id.btnLoadImageFromWeb);
			loadImageFromDatabaseButton = FindViewById<Button> (Resource.Id.btnLoadImageFromDatabase);

			// Get the views.
			imageTestImage = FindViewById<ImageView> (Resource.Id.imageTestImage);

			// Assign event handlers to the buttons.
			if (loadImageFromWebButton != null) {
				loadImageFromWebButton.Click += (sender, e) => {

					Console.WriteLine("Loading image from web...");

					// Load the image from the web.
					WebClient client = new WebClient();
					byte[] imageBytes = client.DownloadData(testImageUrl);
					
					// Show the image.
					Bitmap bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
					imageTestImage.SetImageBitmap(bitmap);

					Console.WriteLine("Set web image. (" + imageBytes.Length + " bytes)");

				};
			}
			if (loadImageFromDatabaseButton != null) {
				loadImageFromDatabaseButton.Click += (sender, e) => {

					Console.WriteLine("Loading image from database...");

					// Get the activities.
					Data_Layer_Common dataLayer = new Data_Layer_Common();
					List<Activity> activities = dataLayer.getActivities();

					// Show the first activity's image.
					Activity activity = activities.First<Activity>();
					byte[] activityImage = activity.activityIcon;
					if (activityImage != null)
					{
						Bitmap bitmap = BitmapFactory.DecodeByteArray(activity.activityIcon, 0, activity.activityIcon.Length);
						imageTestImage.SetImageBitmap(bitmap);
						Console.WriteLine("First activity image set. (" + activity.activityIcon.Length + " bytes )");
					}
					else
					{
						Console.WriteLine("First activity's image not set in database.");
					}
				};
			}

		}
	}
}

