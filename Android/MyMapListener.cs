using System;
using Android.Util;
using Android.Widget;
using Android.Content;
using Nutiteq.SDK;

namespace Nutiteq.AdvancedMap3D
{
	internal partial class MyMapListener: MapListener
	{

		override public void OnMapClicked ( double x, double y, bool longClick )
		{
			Android.Util.Log.Debug ( "NT", "OnMapClicked " + x + " " + y + " " + longClick );
		}

		override public void OnLabelClicked ( VectorElement element, bool longClick )
		{
			Android.Util.Log.Debug ( "NT", "OnLabelClicked " + element+ " " + longClick );
		}

		override public void OnVectorElementClicked ( VectorElement element, double x, double y,  bool longClick )
		{
			Android.Util.Log.Debug ( "NT", "OnVectorElementClicked "  + x + " " + y + " " + element+ " " + longClick );
		}

		override public void OnMapMoved ()
		{
			Android.Util.Log.Debug ( "NT", "OnMapMoved" );
		}
			
	}
}

