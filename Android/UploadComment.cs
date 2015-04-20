
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Newtonsoft.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Columbia583.Android
{
	[Activity (Label = "UploadComment")]			
	public class UploadComment : AndroidActivity
	{
		private const int activityDialog = 1;
		private const int amenityDialog = 2;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.UploadComment);

			//String[] autocomplete = new String[]{"trail","this","me","my"};

			var textbox = FindViewById<MultiAutoCompleteTextView> (Resource.Id.multiAutoCompleteTextView1);
			var rating = FindViewById<RatingBar> (Resource.Id.ratingBar1);
			var buttonUpload = FindViewById<Button> (Resource.Id.button1);
			//buttonUpload.Text = "Upload";

			//string trail = Intent.GetStringExtra ("Trail Data") ?? "Data not available!!";
			//Trail trail = this.Intent.Extras.Get("Trail Data") ?? "Data not available";
			string json = Intent.GetStringExtra("Trail Data");
			Trail trail = JsonConvert.DeserializeObject<Trail>(json);

			buttonUpload.Click += (object sender, EventArgs e) => {

				// Encapsulate the comment.
				Comment comment = new Comment(1, trail.id, textbox.Text, (int)rating.Rating, "", DateTime.Now, DateTime.Now, true);

				// Save and upload the comment.
				Data_Access_Layer_Upload dataAccessLayerUpload = new Data_Access_Layer_Upload();
				dataAccessLayerUpload.uploadComment(comment);

				// Alert user the comment has been uploaded.
				Toast.MakeText(this, "Comment has been uploaded.", ToastLength.Short).Show();

				// Close the page.
				Finish();

			};

			//			ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.UploadComment, autocomplete);
			//			textbox.Adapter = autoCompleteAdapter;

			//var adapter = new ArrayAdapter<String> (this, Resource.Layout.list_complete, autocomplete);

		}


	}
}

