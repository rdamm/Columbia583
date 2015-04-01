package dk.ostebaronen.droid.viewpagerindicator;


public class TabPageIndicator_TabView
	extends android.widget.TextView
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onMeasure:(II)V:GetOnMeasure_IIHandler\n" +
			"";
		mono.android.Runtime.register ("DK.Ostebaronen.Droid.ViewPagerIndicator.TabPageIndicator/TabView, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TabPageIndicator_TabView.class, __md_methods);
	}


	public TabPageIndicator_TabView (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == TabPageIndicator_TabView.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.TabPageIndicator/TabView, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public TabPageIndicator_TabView (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == TabPageIndicator_TabView.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.TabPageIndicator/TabView, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public TabPageIndicator_TabView (android.content.Context p0, android.util.AttributeSet p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == TabPageIndicator_TabView.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.TabPageIndicator/TabView, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onMeasure (int p0, int p1)
	{
		n_onMeasure (p0, p1);
	}

	private native void n_onMeasure (int p0, int p1);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
