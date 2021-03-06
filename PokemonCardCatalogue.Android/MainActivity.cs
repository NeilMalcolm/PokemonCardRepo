using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using PokemonCardCatalogue.Droid.Services;
using PokemonCardCatalogue.Services;

namespace PokemonCardCatalogue.Droid
{
    [Activity(Label = "PokemonCardCatalogue", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = 
            ConfigChanges.ScreenSize 
            | ConfigChanges.Orientation 
            | ConfigChanges.UiMode 
            | ConfigChanges.ScreenLayout 
            | ConfigChanges.SmallestScreenSize 
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public static MainActivity Instance { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);

            global::Xamarin.Forms.Forms.SetFlags(new string[] {
                "Brush_Experimental",
                "SwipeView_Experimental",
                "CollectionView_Experimental"
            });

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(new DependencyHandler_Android(new DependencyContainer())));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}