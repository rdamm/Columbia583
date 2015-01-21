
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace Columbia583.Android
{
	public class ViewPagerFragment : Fragment
	{
		protected Func<LayoutInflater, ViewGroup, Bundle, View> _view;

		public ViewPagerFragment(Func<LayoutInflater, ViewGroup, Bundle, View> view) {
			_view = view;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			base.OnCreateView(inflater, container, savedInstanceState);

			//Set the pager with an adapter
			//ViewPager pager = (ViewPager)findViewById(R.id.viewpager);
			//pager.setAdapter(new TestAdapter(getSupportFragmentManager()));

			//Bind the circle indicator to the adapter
			//CirclePageIndicator circleIndicator = (CirclePageIndicator)findViewById(R.id.titles);
			//circleIndicator.setViewPager(pager);

			return _view(inflater, container, savedInstanceState);
		}
	}
}

