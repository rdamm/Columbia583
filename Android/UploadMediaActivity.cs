
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

using Android.Graphics;
using Android.Database;
using Android.Provider;
using System.IO;

namespace Columbia583.Android
{
	[Activity (Label = "TestUploadActivity", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class UploadMediaActivity : AndroidActivity
	{
		protected Button btnSelectImage = null;
		protected ImageView imgSelectedImage = null;
		protected EditText textboxTitle = null;
		protected EditText textboxComment = null;
		protected Button btnUploadMedia = null;

		public static readonly int SelectImageId = 1000;
		protected Bitmap selectedImageBitmap = null;

		protected int trailId = 0;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.UploadMedia);

			// Get the extras.
			trailId = Intent.GetIntExtra("trailId", 0);

			// Get the controls.
			btnSelectImage = FindViewById<Button> (Resource.Id.btnSelectImage);
			imgSelectedImage = FindViewById<ImageView> (Resource.Id.imgSelectedImage);
			textboxTitle = FindViewById<EditText> (Resource.Id.textboxTitle);
			textboxComment = FindViewById<EditText> (Resource.Id.textboxComment);
			btnUploadMedia = FindViewById<Button> (Resource.Id.btnUploadMedia);

			// Assign the event handlers.
			if (btnSelectImage != null) {
				btnSelectImage.Click += (sender, e) => {

					// Load the dialog to select an image.
					Intent = new Intent();
					Intent.SetType("image/*");
					Intent.SetAction(Intent.ActionGetContent);
					StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), SelectImageId);

				};
			}
			if (btnUploadMedia != null) {
				btnUploadMedia.Click += (sender, e) => {

					// Validate the form data.
					if (selectedImageBitmap == null)
					{
						// Display an error message and stop the upload.
						Toast.MakeText(this, "Please select an image.", ToastLength.Long).Show();
						return;
					}
					else if (textboxTitle.Text == null || textboxTitle.Text == "")
					{
						// Display an error message and stop the upload.
						Toast.MakeText(this, "Please set the title.", ToastLength.Long).Show();
						return;
					}
					
					// Convert the bitmap into a byte array.
					MemoryStream stream = new MemoryStream();
					selectedImageBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
					byte[] imageBytes = stream.ToArray();

					// Upload the media.
					// TODO: Merge media and comment features.
					Media media = new Media(0, trailId, textboxTitle.Text, "image", "", imageBytes, DateTime.Now, DateTime.Now, DateTime.Now, true);
					Data_Access_Layer_Upload dataAccessLayerUpload = new Data_Access_Layer_Upload();
					dataAccessLayerUpload.uploadMedia(media);

					// Close this activity.
					Finish();
				};
			}
		}


		/// <param name="requestCode">The integer request code originally supplied to
		///  startActivityForResult(), allowing you to identify who this
		///  result came from.</param>
		/// <param name="resultCode">The integer result code returned by the child activity
		///  through its setResult().</param>
		/// <param name="data">An Intent, which can return result data to the caller
		///  (various data can be attached to Intent "extras").</param>
		/// <summary>
		/// Called when an activity you launched exits, giving you the requestCode
		///  you started it with, the resultCode it returned, and any additional
		///  data from it.
		/// </summary>
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if ((requestCode == SelectImageId) && (resultCode == Result.Ok) && (data != null))
			{
				global::Android.Net.Uri uri = data.Data;
				imgSelectedImage.SetImageURI (uri);

				// Set the bitmap of the selected image.
				selectedImageBitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, uri);
			}
		}
	}
}

