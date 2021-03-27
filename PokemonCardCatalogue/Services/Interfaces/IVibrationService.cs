using PokemonCardCatalogue.Enums;

namespace PokemonCardCatalogue.Services.Interfaces
{
    public interface IVibrationService
    {
        void PerformSelectionFeedbackVibration();
        void PerformNotificationFeedbackVibration(VibrationNotificationType notificationType);
    }
}
