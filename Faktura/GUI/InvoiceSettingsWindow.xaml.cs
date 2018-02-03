using System;
using System.Windows;
using Faktura.Invoices;
using Faktura.Files;
using Faktura.Utils;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Faktura.GUI
{
    /// <summary>
    /// Interaction logic for InvoiceSettingsWindow.xaml
    /// </summary>
    public partial class InvoiceSettingsWindow : Window
    {
        //Properties
        public InvoiceSettings InvSettings { get; private set; }

        //These maps contain combo boxes values and inoice settings
        //values assosiacted with these indexes. Maps are organized
        //as follows <index,value>. Value for index of (-1) (not selected) is null
        private Dictionary<int, int?> VATRateMap;
        private Dictionary<int, int?> PaymentDaysMap;
        private Dictionary<int, int?> IssueDateMap;

        //Serialization
        private ISerializer SettingsSerializer;
        private const string InvoiceSettingsSerializationPath = @"data\invoiceSettings\settings.dat";
        private const string ControlsStateSerializationPath = @"data\invoiceSettings\controlsState.dat";

        public InvoiceSettingsWindow()
        {
            InitializeComponent();

            SettingsSerializer = new ObjectSerializer();
            VATRateMap = new Dictionary<int, int?>();
            PaymentDaysMap = new Dictionary<int, int?>();
            IssueDateMap = new Dictionary<int, int?>();

            InitializeControlsMaps();
            InitializeInvoiceSettings();
            InitializeInputFields();
            SetControlsValues();
        }

        private void InitializeControlsMaps()
        {
            if (null != VATRateMap && null != PaymentDaysMap && null != IssueDateMap)
            {
                VATRateMap.Add(-1, null);
                VATRateMap.Add(0, 8);
                VATRateMap.Add(1, 16);
                VATRateMap.Add(2, 23);

                PaymentDaysMap.Add(-1, null);
                PaymentDaysMap.Add(0, 1);
                PaymentDaysMap.Add(1, 2);
                PaymentDaysMap.Add(2, 3);
                PaymentDaysMap.Add(3, 4);
                PaymentDaysMap.Add(4, 5);
                PaymentDaysMap.Add(5, 7);

                IssueDateMap.Add(-1, null);
                IssueDateMap.Add(0, 0);
                IssueDateMap.Add(1, 1);
                IssueDateMap.Add(2, 2);
                IssueDateMap.Add(3, 3);
            }
        }

        /// <summary>
        /// Updates controls values with values from invoice settings
        /// </summary>
        private void SetControlsValues()
        {
            if (null != InvSettings)
            {
                TextBoxPaymentDays.Text = InvSettings.DaysForPayment.ToString();
                TextBoxVATRate.Text = InvSettings.VATRate.ToString();
                DatePickerIssueDate.SelectedDate = InvSettings.IssueDate;

                ControlState state = LoadControlsState();

                if (null != state)
                {
                    ComboBoxPaymentDays.SelectedIndex = state.ComboBoxPaymentDaysIndex;
                    ComboBoxIssueDate.SelectedIndex = state.ComboBoxIssueDateIndex;
                    ComboBoxVATRate.SelectedIndex = state.ComboBoxVatRateIndex;
                }
            }
        }

        private void InitializeInputFields()
        {
            TextBoxVATRate.DataContext = TextBoxVATRate;
            TextBoxVATRate.IsEnabled = false;

            TextBoxPaymentDays.DataContext = TextBoxPaymentDays;
            TextBoxPaymentDays.IsEnabled = false;

            DatePickerIssueDate.DataContext = DatePickerIssueDate;
            DatePickerIssueDate.IsEnabled = false;
        }

        /// <summary>
        /// Loads settings from .dat file. If loaded fails or file doesn't exsists creates new file
        /// </summary>
        private InvoiceSettings LoadInvoiceSettings()
        {
            InvoiceSettings loadedSettings = null;

            if (null != SettingsSerializer)
            {
                loadedSettings =
                    SettingsSerializer.DeserializeObject(InvoiceSettingsSerializationPath) as InvoiceSettings;
            }

            return loadedSettings;
        }

        private void InitializeInvoiceSettings()
        {
            InvoiceSettings loadedSettings = LoadInvoiceSettings();

            if (null != loadedSettings)
            {
                InvSettings = loadedSettings;
            }
            else //Loading settings failed, create new settings
            {
                InvSettings = new InvoiceSettings();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Hide window instead of closing so it can be opened without creating new instance of window's class
            e.Cancel = true;
            this.Hide();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ComboBoxVATRate_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ToggleControlsEnabled(InputParameter.VATRate);

            if (getCustomSettingsIndex(InputParameter.VATRate) != ComboBoxVATRate.SelectedIndex)
            {
                int? selectedValue = VATRateMap[ComboBoxVATRate.SelectedIndex];
                TextBoxVATRate.Text = selectedValue.ToString();
            }
        }

        private void ComboBoxPaymentDays_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ToggleControlsEnabled(InputParameter.PaymentDays);

            if (getCustomSettingsIndex(InputParameter.PaymentDays) != ComboBoxPaymentDays.SelectedIndex)
            {
                int? selectedValue = PaymentDaysMap[ComboBoxPaymentDays.SelectedIndex];
                TextBoxPaymentDays.Text = selectedValue.ToString();
            }
        }

        private void ComboBoxIssueDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleControlsEnabled(InputParameter.IssueDate);

            //If user selected custom date but did not selected any use current date
            if (getCustomSettingsIndex(InputParameter.IssueDate) != ComboBoxIssueDate.SelectedIndex)
            {
                int? selectedValue = IssueDateMap[ComboBoxIssueDate.SelectedIndex];
                DatePickerIssueDate.SelectedDate = DateTime.Now.AddDays(selectedValue ?? 0);
            }
        }

        /// <summary>
        /// Sets controls assosiacted with parameterType to on or off based
        /// on index choosed by user
        /// </summary>
        private void ToggleControlsEnabled(InputParameter parameterType)
        {
            //If user selects custom settings enable input field so that custom input can be entered

            int customSettingsIndex = getCustomSettingsIndex(parameterType);

            switch (parameterType)
            {
                case InputParameter.IssueDate:
                    DatePickerIssueDate.IsEnabled = (customSettingsIndex == ComboBoxIssueDate.SelectedIndex);
                    break;
                case InputParameter.PaymentDays:
                    TextBoxPaymentDays.IsEnabled = (customSettingsIndex == ComboBoxPaymentDays.SelectedIndex);
                    break;
                case InputParameter.VATRate:
                    TextBoxVATRate.IsEnabled = (customSettingsIndex == ComboBoxVATRate.SelectedIndex);
                    break;
                default:
                    break;
            }
        }

        private bool ApplySettings()
        {
            bool result = false;

            if (null != InvSettings)
            {
                UInt32 vatRate;
                UInt32 paymentDays;

                string vatRateStr = TextBoxVATRate.Text;
                string paymentDaysStr = TextBoxPaymentDays.Text;
                DateTime issueDate = DatePickerIssueDate.SelectedDate ?? DateTime.Now;

                ParseFailReason reason = InputParser.ParseInvoiceSettings
                    (vatRateStr, out vatRate, paymentDaysStr, out paymentDays);

                if (ParseFailReason.None != reason)
                {
                    GUIInfoHelper.DisplayInputParseError(reason);
                }
                else
                {
                    InvSettings.DaysForPayment = paymentDays;
                    InvSettings.VATRate = vatRate;
                    InvSettings.IssueDate = issueDate;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns index of custom settings selection of combo box assosiacted with "paratmerType" parameter
        /// </summary>
        private int getCustomSettingsIndex(InputParameter InputParamterType)
        {
            int customSettingsIndex = 0;

            switch (InputParamterType)
            {
                case InputParameter.IssueDate:
                    customSettingsIndex = 4;
                    break;
                case InputParameter.PaymentDays:
                    customSettingsIndex = 6;
                    break;
                case InputParameter.VATRate:
                    customSettingsIndex = 3;
                    break;
            }

            return customSettingsIndex;
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            SaveSettings();
            SaveControlsState();
        }

        private bool SaveSettings()
        {
            bool result = false;

            if (null != InvSettings && null != SettingsSerializer)
            {
                result = SettingsSerializer.SerializeObject(InvSettings, InvoiceSettingsSerializationPath);
                GUIInfoHelper.DisplaySerializationInfo(result);
            }

            return result;
        }

        /// <summary>
        /// Saves control states to .dat file. This is done so when user
        /// runs application again controls can be set to last saved state.
        /// </summary>
        private bool SaveControlsState()
        {
            bool result = false;

            if (null != SettingsSerializer)
            {
                ControlState state = new ControlState();
                state.ComboBoxIssueDateIndex = ComboBoxIssueDate.SelectedIndex;
                state.ComboBoxPaymentDaysIndex = ComboBoxPaymentDays.SelectedIndex;
                state.ComboBoxVatRateIndex = ComboBoxVATRate.SelectedIndex;

                result = SettingsSerializer.SerializeObject(state, ControlsStateSerializationPath);
            }

            return result;
        }

        /// <summary>
        /// Loads ControlState class instance from .dat file
        /// </summary>
        /// <returns>Saved instance of ControlState class. Null if loading failed or .dat file does not exsists</returns>
        private ControlState LoadControlsState()
        {
            ControlState loadedState = null;

            if (null != SettingsSerializer)
            {
                loadedState = SettingsSerializer.DeserializeObject(ControlsStateSerializationPath) as ControlState;
            }

            return loadedState;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (ApplySettings() && SaveSettings())
            {
                SaveControlsState();
                this.Hide();
            }
        }
    }

    /// <summary>
    /// This is class for storing invoice settings window's control's values. So
    /// that they can be save to .dat file and restored when application is runned again.
    /// </summary>
    [Serializable]
    class ControlState
    {
        public int ComboBoxVatRateIndex { get; set; }
        public int ComboBoxPaymentDaysIndex { get; set; }
        public int ComboBoxIssueDateIndex { get; set; }
    }

    public enum InputParameter
    {
        VATRate,
        PaymentDays,
        IssueDate
    }
}