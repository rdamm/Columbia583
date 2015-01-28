package dk.ostebaronen.droid.viewpagerindicator;


public class IcsLinearLayout
	extends android.widget.LinearLayout
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_measureChildWithMargins:(Landroid/view/View;IIII)V:GetMeasureChildWithMargins_Landroid_view_View_IIIIHandler\n" +
			"n_onDraw:(Landroid/graphics/Canvas;)V:GetOnDraw_Landroid_graphics_Canvas_Handler\n" +
			"";
		mono.android.Runtime.register ("DK.Ostebaronen.Droid.ViewPagerIndicator.IcsLinearLayout, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", IcsLinearLayout.class, __md_methods);
	}


	public IcsLinearLayout (android.content.Context p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == IcsLinearLayout.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.IcsLinearLayout, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public IcsLinearLayout (android.content.Context p0, android.util.AttributeSet p1) throws java.lang.Throwable
	{
		super (p0, p1);
		if (getClass () == IcsLinearLayout.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.IcsLinearLayout, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}


	public IcsLinearLayout (android.content.Context p0, android.util.AttributeSet p1, int p2) throws java.lang.Throwable
	{
		super (p0, p1, p2);
		if (getClass () == IcsLinearLayout.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.IcsLinearLayout, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void measureChildWithMargins (android.view.View p0, int p1, int p2, int p3, int p4)
	{
		n_measureChildWithMargins (p0, p1, p2, p3, p4);
	}

	private native void n_measureChildWithMargins (android.view.View p0, int p1, int p2, int p3, int p4);


	public void onDraw (android.graphics.Canvas p0)
	{
		n_onDraw (p0);
	}

	private native void n_onDraw (android.graphics.Canvas p0);

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
