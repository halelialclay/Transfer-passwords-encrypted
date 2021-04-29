using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Security.Cryptography;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Xamarin.Forms;


namespace MyApp.Droid
{
    [IntentFilter(new[] { Android.Hardware.Usb.UsbManager.ActionUsbDeviceAttached, Android.Hardware.Usb.UsbManager.ActionUsbDeviceDetached })]
    [Activity(Label = "HEProject", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            var appPreferences = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(this);
            var isAppInstalled = appPreferences.GetBoolean("isAppInstalled", false);
        }
    }
}