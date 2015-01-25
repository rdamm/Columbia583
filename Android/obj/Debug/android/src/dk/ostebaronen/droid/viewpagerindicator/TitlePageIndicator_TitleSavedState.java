package dk.ostebaronen.droid.viewpagerindicator;


public class TitlePageIndicator_TitleSavedState
	extends android.view.View.BaseSavedState
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_writeToParcel:(Landroid/os/Parcel;I)V:GetWriteToParcel_Landroid_os_Parcel_IHandler\n" +
			"n_InitializeCreator:()Ldk/ostebaronen/droid/viewpagerindicator/TitlePageIndicator_TitleSavedState_TitleSavedStateCreator;:__export__\n" +
			"";
		mono.android.Runtime.register ("DK.Ostebaronen.Droid.ViewPagerIndicator.TitlePageIndicator/TitleSavedState, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", TitlePageIndicator_TitleSavedState.class, __md_methods);
	}


	public TitlePageIndicator_TitleSavedState (android.os.Parcel p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == TitlePageIndicator_TitleSavedState.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.TitlePageIndicator/TitleSavedState, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.OS.Parcel, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public TitlePageIndicator_TitleSavedState (android.os.Parcelable p0) throws java.lang.Throwable
	{
		super (p0);
		if (getClass () == TitlePageIndicator_TitleSavedState.class)
			mono.android.TypeManager.Activate ("DK.Ostebaronen.Droid.ViewPagerIndicator.TitlePageIndicator/TitleSavedState, DK.Ostebaronen.Droid.ViewPagerIndicator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.OS.IParcelable, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0 });
	}


	public static dk.ostebaronen.droid.viewpagerindicator.TitlePageIndicator_TitleSavedState_TitleSavedStateCreator CREATOR = InitializeCreator ();


	public void writeToParcel (android.os.Parcel p0, int p1)
	{
		n_writeToParcel (p0, p1);
	}

	private native void n_writeToParcel (android.os.Parcel p0, int p1);

	public static dk.ostebaronen.droid.viewpagerindicator.TitlePageIndicator_TitleSavedState_TitleSavedStateCreator InitializeCreator ()
	{
		return n_InitializeCreator ();
	}

	private static native dk.ostebaronen.droid.viewpagerindicator.TitlePageIndicator_TitleSavedState_TitleSavedStateCreator n_InitializeCreator ();

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
