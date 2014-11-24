using System;
using Xamarin.Forms;

namespace Columbia583
{
	public class App
	{
		public static Page GetMainPage ()
		{
			return new ContentPage { 
				Content = new Label {
					Text = "Main Menu Page",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}

		public static Page GetSearchTrailsPage()
		{
			return new ContentPage {
				Content = new Label {
					Text = "Search Trails Page",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}

		public static Page GetViewTrailPage()
		{
			return new ContentPage { 
				Content = new Label {
					Text = "View Trail Page",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}
	}
}

