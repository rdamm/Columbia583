
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

namespace Columbia583.Android
{
	[Activity (Label = "ViewMediaActivity")]			
	public class ViewMediaActivity : AndroidActivity
	{
		protected TextView mediaGallerySelectedMediaTitle = null;
		protected ImageView mediaGallerySelectedMediaImage = null;
		protected TextView mediaGallerySelectedMediaComment = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.ViewMedia);

			// Get the extras.
			int mediaId = Intent.GetIntExtra ("mediaId", 0);

			// Load the media.
			Data_Access_Layer_Common dataAccessLayerCommon = new Data_Access_Layer_Common();
			Media media = dataAccessLayerCommon.getMedia (mediaId);

			// Get the controls.
			mediaGallerySelectedMediaTitle = FindViewById<TextView> (Resource.Id.mediaGallerySelectedMediaTitle);
			mediaGallerySelectedMediaImage = FindViewById<ImageView> (Resource.Id.mediaGallerySelectedMediaImage);
			mediaGallerySelectedMediaComment = FindViewById<TextView> (Resource.Id.mediaGallerySelectedMediaComment);
			
			// Show the media's text.
			mediaGallerySelectedMediaTitle.Text = media.title;
			mediaGallerySelectedMediaComment.Text = media.title;

			// Show the media's image.
			Bitmap bitmap = BitmapFactory.DecodeByteArray(media.mediaImage, 0, media.mediaImage.Length);
			mediaGallerySelectedMediaImage.SetMinimumHeight (256);
			mediaGallerySelectedMediaImage.SetMinimumWidth (256);
			mediaGallerySelectedMediaImage.SetMaxHeight (256);
			mediaGallerySelectedMediaImage.SetMaxWidth (256);
			mediaGallerySelectedMediaImage.SetImageBitmap(bitmap);
		}
	}
}

