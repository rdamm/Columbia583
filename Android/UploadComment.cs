
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
				Data_Access_Layer_Common data_access_layer = new Data_Access_Layer_Common();
				Random num = new Random();
				Comment comment = new Comment(num.Next(0,1000000),trail.id,textbox.Text,(int) rating.Rating,"defaultUser",DateTime.Now);
				data_access_layer.insertComment(comment);

				List<Comment> list_comments = data_access_layer.getCommentID();

				foreach(var i in list_comments){
					Console.Out.WriteLine(i.text);
				}

				//Alert user the commen has been uploaded
				Toast.MakeText(this, "Comment has been uploaded", ToastLength.Short).Show();

			};

			//			ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.UploadComment, autocomplete);
			//			textbox.Adapter = autoCompleteAdapter;

			//var adapter = new ArrayAdapter<String> (this, Resource.Layout.list_complete, autocomplete);

		}


	}
}

