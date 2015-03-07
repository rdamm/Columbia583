
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
	[Activity (Label = "TestImage", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
	public class TestImageActivity : AndroidActivity
	{
		protected Button loadImageFromWebButton = null;
		protected Button loadImageFromDatabaseButton = null;
		protected Button dynamicallyDisplayImagesButton = null;
		protected ImageView imageTestImage = null;
		protected LinearLayout dynamicallyDisplayImagesLinearLayout = null;

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
			dynamicallyDisplayImagesButton = FindViewById<Button> (Resource.Id.btnDynamicallyDisplayImages);
			imageTestImage = FindViewById<ImageView> (Resource.Id.imageTestImage);
			dynamicallyDisplayImagesLinearLayout = FindViewById<LinearLayout> (Resource.Id.linearLayout_dynamicallyDisplayImages);

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
						// Decode the byte array to get a bitmap.
						Bitmap bitmap = BitmapFactory.DecodeByteArray(activity.activityIcon, 0, activity.activityIcon.Length);

						// Define the view's display size.
						imageTestImage.SetMinimumHeight(256);
						imageTestImage.SetMinimumWidth(256);
						imageTestImage.SetMaxHeight(256);
						imageTestImage.SetMaxWidth(256);

						// Set the background color to be white to contrast with the image.
						imageTestImage.SetBackgroundColor(Color.White);

						// Set the image's bitmap in its view.
						imageTestImage.SetImageBitmap(bitmap);
						Console.WriteLine("First activity image set. (" + activity.activityIcon.Length + " bytes )");
					}
					else
					{
						Console.WriteLine("First activity's image not set in database.");
					}
				};
			}
			if (dynamicallyDisplayImagesButton != null) {
				dynamicallyDisplayImagesButton.Click += (sender, e) => {

					Console.WriteLine("Dynamically producing image views using database activities...");

					// Empty the layout view.
					dynamicallyDisplayImagesLinearLayout.RemoveAllViews();

					// Get the activities.
					Data_Layer_Common dataLayer = new Data_Layer_Common();
					List<Activity> activities = dataLayer.getActivities();

					foreach(Activity activity in activities)
					{
						byte[] activityImage = activity.activityIcon;
						if (activityImage != null)
						{
							// Create a new view for this activity's image.
							ImageView imageView = new ImageView(this);

							// Decode the byte array to get a bitmap.
							Bitmap bitmap = BitmapFactory.DecodeByteArray(activity.activityIcon, 0, activity.activityIcon.Length);

							// Define the view's display size.
							imageView.SetMinimumHeight(256);
							imageView.SetMinimumWidth(256);
							imageView.SetMaxHeight(256);
							imageView.SetMaxWidth(256);

							// Set the background color to be white to contrast with the image.
							imageView.SetBackgroundColor(Color.White);

							// Set the image's bitmap in its view.
							imageView.SetImageBitmap(bitmap);
							Console.WriteLine("First activity image set. (" + activity.activityIcon.Length + " bytes )");

							// Add the view to the layout.
							dynamicallyDisplayImagesLinearLayout.AddView(imageView);
						}
						else
						{
							Console.WriteLine("First activity's image not set in database.");
						}
					}

				};
			}

		}
	}
}

