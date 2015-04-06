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

namespace Nutiteq.AdvancedMap3D
{
	public class SearchSuggestionProvider : SearchRecentSuggestionsProvider
	{
		/**
	     * This is the provider authority identifier.  The same string must appear in your
	     * Manifest file, and any time you instantiate a 
	     * {@link android.provider.SearchRecentSuggestions} helper class. 
	     */
		public static String AUTHORITY = "com.nutiteq.osm";

		/*		*
	     * These flags determine the operating mode of the suggestions provider.  This value should 
	     * not change from run to run, because when it does change, your suggestions database may 
	     * be wiped.
	     */
		public static Android.Content.DatabaseMode MODE = DatabaseMode.Queries;

		/*		*
	     * The main job of the constructor is to call {@link #setupSuggestions(String, int)} with the
	     * appropriate configuration values.
	     */
		public SearchSuggestionProvider() :base()
		{
			SetupSuggestions(AUTHORITY, MODE);
		}
	}
}

