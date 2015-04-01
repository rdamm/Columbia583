package columbia583.android;


public class SearchTrailsActivity
	extends xamarin.forms.platform.android.AndroidActivity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"n_onCreateDialog:(ILandroid/os/Bundle;)Landroid/app/Dialog;:GetOnCreateDialog_ILandroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("Columbia583.Android.SearchTrailsActivity, Columbia583.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SearchTrailsActivity.class, __md_methods);
	}


	public SearchTrailsActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SearchTrailsActivity.class)
			mono.android.TypeManager.Activate ("Columbia583.Android.SearchTrailsActivity, Columbia583.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);


	public android.app.Dialog onCreateDialog (int p0, android.os.Bundle p1)
	{
		return n_onCreateDialog (p0, p1);
	}

	private native android.app.Dialog n_onCreateDialog (int p0, android.os.Bundle p1);

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
