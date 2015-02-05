
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
		protected Button getImageAsBinaryButton = null;
		protected Button loadImageUsingBinary = null;

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
			getImageAsBinaryButton = FindViewById<Button> (Resource.Id.btnGetImageAsBinary);
			loadImageUsingBinary = FindViewById<Button> (Resource.Id.btnLoadImageUsingBinary);

			// Get the views.
			imageTestImage = FindViewById<ImageView> (Resource.Id.imageTestImage);

			// Assign event handlers to the buttons.
			if (getImageAsBinaryButton != null) {
				getImageAsBinaryButton.Click += (sender, e) => {

					// Load the image.
					WebClient client = new WebClient();
					byte[] imageBytes = client.DownloadData(testImageUrl);

					// Write the byte array to console.
					Console.WriteLine("Got image!");
					Console.WriteLine("Image size: " + imageBytes.Length + " bytes.");

				};
			}
			if (loadImageUsingBinary != null) {
				loadImageUsingBinary.Click += (sender, e) => {

					// Load the image.
					WebClient client = new WebClient();
					byte[] imageBytes = client.DownloadData(testImageUrl);

					// Convert the bytes into an image.
					MemoryStream stream = new MemoryStream(imageBytes);

					// Show the image.
					Bitmap bmp = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
					imageTestImage.SetImageBitmap(bmp);

				};
			}
		}
	}
}

