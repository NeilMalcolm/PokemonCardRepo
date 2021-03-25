using PokemonCardCatalogue.Enums;
using PokemonCardCatalogue.Services.Interfaces;
using UIKit;

namespace PokemonCardCatalogue.iOS.Services
{
    public class VibrationService_iOS : IVibrationService
    {
        private UISelectionFeedbackGenerator _selectionFeedbackGenerator = new UISelectionFeedbackGenerator();
        private UINotificationFeedbackGenerator _notificationFeedbackGenerator = new UINotificationFeedbackGenerator();
        
        public void PerformNotificationFeedbackVibration(VibrationNotificationType notificationType)
        {
            _notificationFeedbackGenerator.Prepare();

            switch (notificationType)
            {
                default:
                case VibrationNotificationType.Success:
                    _notificationFeedbackGenerator.NotificationOccurred(UINotificationFeedbackType.Success);
                    break;
                case VibrationNotificationType.Error:
                    _notificationFeedbackGenerator.NotificationOccurred(UINotificationFeedbackType.Error);
                    break;
                case VibrationNotificationType.Warning:
                    _notificationFeedbackGenerator.NotificationOccurred(UINotificationFeedbackType.Warning);
                    break;
            }
        }

        public void PerformSelectionFeedbackVibration()
        {
            _selectionFeedbackGenerator.Prepare();
            _selectionFeedbackGenerator.SelectionChanged();
        }
    }
}