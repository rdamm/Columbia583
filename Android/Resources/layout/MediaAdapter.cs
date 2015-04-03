using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Columbia583.Android
{
	public class MediaAdapter : BaseAdapter
	{
		Context context;
		Media[] mediaList;

		public MediaAdapter (Context c, Media[] mediaList)
		{
			context = c;
			this.mediaList = mediaList;
		}

		public override int Count {
			get { return mediaList.Length; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}

		public override long GetItemId (int position)
		{
			return 0;
		}

		// create a new ImageView for each item referenced by the Adapter
		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			ImageView imageView;

			if (convertView == null) {  // if it's not recycled, initialize some attributes
				imageView = new ImageView (context);
				imageView.LayoutParameters = new GridView.LayoutParams (200, 200);
				imageView.SetScaleType (ImageView.ScaleType.CenterCrop);
				imageView.SetPadding (8, 8, 8, 8);
			} else {
				imageView = (ImageView)convertView;
			}

			Bitmap bitmap = BitmapFactory.DecodeByteArray(mediaList[position].mediaImage, 0, mediaList[position].mediaImage.Length);
			imageView.SetBackgroundColor (global::Android.Graphics.Color.White);
			imageView.SetImageBitmap (bitmap);
			return imageView;
		}
	}
}

