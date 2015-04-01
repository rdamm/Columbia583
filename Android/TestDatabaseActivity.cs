
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

using Xamarin.Forms.Platform.Android;

namespace Columbia583.Android
{
	[Activity (Label = "TestDatabaseActivity", ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]			
	public class TestDatabaseActivity : AndroidActivity
	{
		protected Button createTablesButton = null;
		protected Button insertTablesButton = null;
		protected Button commentsButton = null;
		protected Button getDatabaseLastUpdatedButton = null;
		protected Button setDatabaseLastUpdatedButton = null;
		protected Button getActivitiesButton = null;
		protected Button getAmenitiesButton = null;
		protected Button getUsersButton = null;
		protected Button getTrailsBySearchFilterButton = null;
		protected Button getTrailButton = null;
		protected Button deleteSomeFromTablesButton = null;
		protected Button deleteAllFromTablesButton = null;
		protected Button dropTablesButton = null;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Forms.Forms.Init (this, bundle);

			// Set the base page.
			SetPage (App.GetMainPage ());

			// Set the view for the page.
			SetContentView (Resource.Layout.TestDatabase);

			// Get the controls.
			createTablesButton = FindViewById<Button> (Resource.Id.btnCreateTables);
			insertTablesButton = FindViewById<Button> (Resource.Id.btnInsertTables);
			commentsButton = FindViewById<Button> (Resource.Id.btnComments);
			getDatabaseLastUpdatedButton = FindViewById<Button> (Resource.Id.btnGetDatabaseLastUpdated);
			setDatabaseLastUpdatedButton = FindViewById<Button> (Resource.Id.btnSetDatabaseLastUpdated);
			getActivitiesButton = FindViewById<Button> (Resource.Id.btnGetActivities);
			getAmenitiesButton = FindViewById<Button> (Resource.Id.btnGetAmenities);
			getUsersButton = FindViewById<Button> (Resource.Id.btnGetUsers);
			getTrailsBySearchFilterButton = FindViewById<Button> (Resource.Id.btnGetTrailsBySearchFilter);
			getTrailButton = FindViewById<Button> (Resource.Id.btnGetTrail);
			deleteSomeFromTablesButton = FindViewById<Button> (Resource.Id.btnDeleteSomeFromTables);
			deleteAllFromTablesButton = FindViewById<Button> (Resource.Id.btnDeleteAllFromTables);
			dropTablesButton = FindViewById<Button> (Resource.Id.btnDropTables);

			// Assign event handlers to the buttons.
			if (createTablesButton != null) {
				createTablesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.createTables();
					Console.WriteLine(response);
				};
			}
			if (insertTablesButton != null) {
				insertTablesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.insertIntoTables();
					Console.WriteLine(response);
				};
			}
			if (commentsButton != null) {
				commentsButton.Click += (sender, e) => {
					Data_Access_Layer_Common dataLayerTests = new Data_Access_Layer_Common();
					dataLayerTests.initializeComments();
				};
			}
			if (getDatabaseLastUpdatedButton != null) {
				getDatabaseLastUpdatedButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getDatabaseLastUpdated();
					Console.WriteLine(response);
				};
			}
			if (setDatabaseLastUpdatedButton != null) {
				setDatabaseLastUpdatedButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.setDatabaseLastUpdated();
					Console.WriteLine(response);
				};
			}
			if (getActivitiesButton != null) {
				getActivitiesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getActivities();
					Console.WriteLine(response);
				};
			}
			if (getAmenitiesButton != null) {
				getAmenitiesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getAmenities();
					Console.WriteLine(response);
				};
			}
			if (getUsersButton != null) {
				getUsersButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getUsers();
					Console.WriteLine(response);
				};
			}
			if (getTrailsBySearchFilterButton != null) {
				getTrailsBySearchFilterButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getTrailsBySearchFilter();
					Console.WriteLine(response);
				};
			}
			if (getTrailButton != null) {
				getTrailButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.getTrail();
					Console.WriteLine(response);
				};
			}
			if (deleteSomeFromTablesButton != null) {
				deleteSomeFromTablesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.deleteSomeFromTables();
					Console.WriteLine(response);
				};
			}
			if (deleteAllFromTablesButton != null) {
				deleteAllFromTablesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.deleteAllFromTables();
					Console.WriteLine(response);
				};
			}
			if (dropTablesButton != null) {
				dropTablesButton.Click += (sender, e) => {
					Data_Layer_Tests dataLayerTests = new Data_Layer_Tests();
					string response = dataLayerTests.dropTables();
					Console.WriteLine(response);
				};
			}
		}
	}
}

