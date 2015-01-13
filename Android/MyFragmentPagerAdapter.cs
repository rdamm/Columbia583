using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace Columbia583.Android
{
	public class MyFragmentPagerAdapter : FragmentPagerAdapter
	{
		private List<Fragment> _fragmentList = null;

		public MyFragmentPagerAdapter (FragmentManager fm) : base(fm)
		{
			_fragmentList = new List<Fragment> ();
		}

		public void AddFragmentView(Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_fragmentList.Add(new ViewPagerFragment(view));
		}

		public override int Count
		{
			get { return _fragmentList.Count; }
		}


		public override Fragment GetItem(int position)
		{
			return _fragmentList[position];
		}
	}
}

