using Android.OS;
using Android.Views;
using PokemonCardCatalogue.Enums;
using PokemonCardCatalogue.Services.Interfaces;

namespace PokemonCardCatalogue.Droid.Services
{
    public class VibrationService_Android : IVibrationService
    {
        private Vibrator _vibrator;
        protected Vibrator Vibrator
        {
            get => _vibrator ??= Vibrator.FromContext(MainActivity.Instance);
        }
        

        public void PerformNotificationFeedbackVibration(VibrationNotificationType notificationType)
        {
            switch (notificationType)
            {
                case VibrationNotificationType.Success:
                    Vibrator.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));
                    break;
                case VibrationNotificationType.Error:
                    Vibrator.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.EffectHeavyClick));
                    break;
                case VibrationNotificationType.Warning:
                    Vibrator.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.EffectDoubleClick));
                    break;
            }
        }

        public void PerformSelectionFeedbackVibration()
        {
            Vibrator.Vibrate(VibrationEffect.CreateOneShot(200, VibrationEffect.DefaultAmplitude));
        }
    }
}