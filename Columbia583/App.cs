using System;
using System.Collections.Generic;
using Xamarin.Forms;

using Android.Content;


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
					Text = "View Trails Page",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
			/*
			const int NUM_PAGES = 2;
			List<ContentPage> pages = new List<ContentPage>(0);

			for (int i = 0; i < NUM_PAGES; i++) {
				pages.Add(new ContentPage { 
					Content = new Label {
						Text = "View Trail Page " + i,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					},
				});
			}

			return new CarouselPage {
				Children = {
					pages[0],
					pages[1]
				},
			};
			*/
		}
	}
}

