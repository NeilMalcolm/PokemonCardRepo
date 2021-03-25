using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace PokemonCardCatalogue.Controls
{
    public class SkeletonView : BoxView
    {
        private Animation animation;
        private int loopCount = 0;
        public SkeletonView()
        {
            animation = new Animation();

            animation.WithConcurrent((f) => this.Opacity = f, 0.2, 1, Easing.Linear);
            animation.WithConcurrent((f) => this.Opacity = f, 1, 0.2, Easing.Linear);

            this.Animate("FadeInOut",
                animation,
                16,
                1000,
                Easing.Linear,
                null,
                repeat: () =>
                {
                    loopCount++;
                    return loopCount < 5;
                }) ;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != "Opacity")
            {
            }
        }
    }
}
