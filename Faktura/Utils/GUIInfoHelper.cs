using System.Windows;

namespace Faktura.Utils
{
    /// <summary>
    /// This is a class to make displaying info to user easy.
    /// </summary>
    class GUIInfoHelper
    {
        public static void DisplayInputParseError(ParseFailReason reason)
        {
            const string defaultErrorMsg = "Nieznany błąd";
            const string emptyInputMsg = "Wypełnij wszystkie wymagane pola";
            const string wrongInputMsg = "Nieprawidłowe dane wejściowe";
            const string overflowInputMsg = "Została wprowadzona zbyt duża warotść";
            const string invalidInputLenghtMsg = "Wprowadzona wartość ma nieprawidłową długość";

            string errorMessage = defaultErrorMsg;

            switch (reason)
            {
                case ParseFailReason.EmptyInput:
                    errorMessage = emptyInputMsg;
                    break;
                case ParseFailReason.WrongInput:
                    errorMessage = wrongInputMsg;
                    break;
                case ParseFailReason.InputOverflow:
                    errorMessage = overflowInputMsg;
                    break;
                case ParseFailReason.InvalidInputLenght:
                    errorMessage = invalidInputLenghtMsg;
                    break;
            }

            MessageBox.Show(errorMessage, "Błąd", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Display serialization result info
        /// </summary>
        /// <param name="result">If true displays serialization successful message.
        /// If false displays serialization failed message</param>
        public static void DisplaySettingsSerializationInfo(bool result)
        {
            string errorMsg = result ? "Zapisano ustawienia" : "Zapisywanie nie powiodło się";

            MessageBox.Show(errorMsg, result ? "Zapisano" : "Błąd",
                MessageBoxButton.OK, result ? MessageBoxImage.Information : MessageBoxImage.Error);
        }
    }
}
