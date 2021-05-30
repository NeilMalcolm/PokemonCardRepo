using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FFImageLoading;
using FFImageLoading.Config;
using Foundation;
using PokemonCardCatalogue.iOS.Services;
using PokemonCardCatalogue.Services;
using UIKit;

namespace PokemonCardCatalogue.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            //SetUpNativeHttpClientForImageLoading();
            global::Xamarin.Forms.Forms.SetFlags(new string[] {
                "Brush_Experimental",
                "SwipeView_Experimental",
                "CollectionView_Experimental"
            });
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new DependencyHandler_iOS(new DependencyContainer())));

            return base.FinishedLaunching(app, options);
        }

        private void SetUpNativeHttpClientForImageLoading()
        {
            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = new HttpClient(new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                })
            });
        }
    }
}
