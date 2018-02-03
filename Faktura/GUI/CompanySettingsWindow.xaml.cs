using System.Windows;
using System;
using Faktura.Companies;
using Faktura.Utils;
using Faktura.Files;
using System.Windows.Controls;
using System.ComponentModel;
using System.Linq;

namespace Faktura.GUI
{
    /// <summary>
    /// Interaction logic for CompanySettingsWindow.xaml
    /// </summary>
    public partial class CompanySettingsWindow : Window
    {
        //Consts

        //This number indicates maximum number of possible
        //company settings presets that user can
        //save
        private const UInt32 CompanyPresetsNumber = 5;

        //This is path neede by ObjectSerializer class
        //for saving .dat file containing saved user presets
        private const string CompanySettingsPresetsSerializationPath = @"data\companySettings\company_settings.dat";

        //This is path neede by ObjectSerializer class
        //for saving .dat file containing controls state
        private const string ControlsStateSerializationPath = @"data\companySettings\controls_state.dat";

        //----------------------------------------------------------

        //Class fields

        //This field hold class with values of various controls.
        //Instance of this class is loaded from. dat file
        private ControlState ControlsState;

        //This array hold values of all presets of
        //company settings. Index is null if specific
        //preset has not been set yet or failed loading
        //from file
        private CompanySettings[] CompanySettingsPresets;

        //This is Company class instance coresponding to selected
        //company settings from settings presets. Is null if currently
        //seleceted preset is new and has not been saved yet or
        //loading preset from file failed
        public CompanySettings CurrentCompany { get; private set; }

        //This is serializer for saving company settings to .dat file
        private ISerializer Serializer;

        public CompanySettingsWindow()
        {
            InitializeComponent();
            Serializer = new ObjectSerializer();
            InitializePresets();
            InitializeCurrentCompany();
            InitializeControlsValues();
        }

        /// <summary>
        /// Loads saved company settings from file. Sets preset array to null
        /// if loading .dat file failed
        /// </summary>
        private CompanySettings[] LoadPresets() //TODO: Add loading from file
        {
            CompanySettings[] loadedPresets = null;

            if (null != Serializer)
            {
                loadedPresets =
                    Serializer.DeserializeObject(CompanySettingsPresetsSerializationPath) as CompanySettings[];
            }

            return loadedPresets;
        }

        private void InitializePresets()
        {
            CompanySettings[] loadedPresets = LoadPresets();

            if (null != loadedPresets && CompanyPresetsNumber == loadedPresets.Length)
            {
                CompanySettingsPresets = loadedPresets;
            }
            else //Loading failed, create new company settings presets
            {
                CompanySettingsPresets = new CompanySettings[CompanyPresetsNumber];
            }
        }

        private void InitializePresetsComboBox()
        {
            if (null != CompanySettingsPresets)
            {
                //Company settings presets combo box initialization

                UInt32 index = 0;

                for (index = 0; index < CompanyPresetsNumber; ++index)
                {
                    //Add all possible presets indexes to combo box. Add string
                    //indicating whether given preset contains saved and valid company
                    //settings
                    string ComboBoxItemName =
                        (null == CompanySettingsPresets[index]) ? ((index + 1).ToString()) :
                        ((index + 1).ToString() + " " + CompanySettingsPresets[index].CompanyName);

                    ComboBoxCompanySettingsPresets.Items.Add(ComboBoxItemName);
                }

                //Initial index the combox selection is set to in case loading 
                //.dat file with saved control values failed
                const int initialIndex = 0;

                ComboBoxCompanySettingsPresets.SelectedIndex = (null == ControlsState) ?
               initialIndex : ControlsState.ComboBoxCompanySettingsPresetsIndex;
            }
        }

        /// <summary>
        /// Sets values of controls to values of CurrentCompany.
        /// </summary>
        /// <param name="empty">If true all controls are set to initial empty values.
        /// If false controls values are taken from company settings class</param>
        private void UpdateCompanySettingsControlsValues(bool empty)
        {
            if (null != CurrentCompany && !empty)
            {
                TextBoxCompanyName.Text = CurrentCompany.CompanyName;
                TextBoxNIP.Text = CurrentCompany.NIP.ToString();
                TextBoxREGON.Text = CurrentCompany.REGON.ToString();

                //Company address
                TextBoxAddressCity.Text = CurrentCompany.City;
                TextBoxAddressHouseNumber.Text = CurrentCompany.HouseNumber.ToString();
                TextBoxAddressStreet.Text = CurrentCompany.Street;

                //1st postal code text box should contain 2 first digits of postal code and 2nd postal code
                //text box should contain 3 next digits of postal code
                TextBoxAddressPostCode1.Text = CurrentCompany.PostalCode.ToString().Substring(0, 2);
                TextBoxAddressPostCode2.Text = CurrentCompany.PostalCode.ToString().Substring(2, 3);
            }
            else if (empty)
            {
                string emptyValue = String.Empty;

                TextBoxCompanyName.Text = emptyValue;
                TextBoxNIP.Text = emptyValue;
                TextBoxREGON.Text = emptyValue;

                //Company address
                TextBoxAddressCity.Text = emptyValue;
                TextBoxAddressHouseNumber.Text = emptyValue;
                TextBoxAddressStreet.Text = emptyValue;

                //1st postal code text box should contain 2 first digits of postal code and 2nd postal code
                //text box should contain 3 next digits of postal code
                TextBoxAddressPostCode1.Text = emptyValue;
                TextBoxAddressPostCode2.Text = emptyValue;
            }
        }

        /// <summary>
        /// This function sets intial controls values from loades settings.
        /// This function should be used after company settings window loading.
        /// </summary>
        private void InitializeControlsValues()
        {
            ControlsState = LoadControlsState();
            UpdateCompanySettingsControlsValues(false);
            InitializePresetsComboBox();
        }

        /// <summary>
        /// Updates name of given index of company settings presets combo box.
        /// This function should be used after successful settings save.
        /// </summary>
        private void UpdatePresetsComboBox(int index)
        {
            if (index >= 0 && index < CompanyPresetsNumber
                && null != CompanySettingsPresets && null != CompanySettingsPresets[index])
            {
                ComboBoxCompanySettingsPresets.Items[index] =
                    (index + 1).ToString() + " " + CompanySettingsPresets[index].CompanyName;
            }
        }


        /// <summary>
        /// Generates company settings class instance from input provided by user.
        /// Displays error if input provided by user is incorrect
        /// </summary>
        /// <returns>True if user passed correct values and CompanySettings class instance was created.
        /// False otherwise</returns>
        private CompanySettings GenerateSettings()
        {
            CompanySettings newSettings = null;

            UInt64 REGON;
            UInt64 NIP;
            UInt16 houseNumber;
            UInt32 postalCode;

            //Postal code if combination of two text boxes values. Spaces should be removed in case user inserted some
            string postalCodeStr = (TextBoxAddressPostCode1.Text + TextBoxAddressPostCode2.Text).Replace(" ", string.Empty);

            ParseFailReason reason = InputParser.ParseCompanySettigns(TextBoxCompanyName.Text, TextBoxNIP.Text, out NIP,
                TextBoxREGON.Text, out REGON, TextBoxAddressStreet.Text, TextBoxAddressHouseNumber.Text, out houseNumber,
                TextBoxAddressCity.Text, postalCodeStr, out postalCode);

            if (ParseFailReason.None == reason)
            {
                newSettings = new CompanySettings(TextBoxCompanyName.Text, NIP, REGON, TextBoxAddressStreet.Text,
                   houseNumber, TextBoxAddressCity.Text, postalCode);
            }
            else
            {
                GUIInfoHelper.DisplayInputParseError(reason);
            }

            return newSettings;
        }

        private bool UpdateSettings()
        {
            bool result = false;
            CompanySettings generatedCompanySettings = GenerateSettings();

            if (null != generatedCompanySettings && (-1) != ComboBoxCompanySettingsPresets.SelectedIndex)
            {
                CompanySettingsPresets[ComboBoxCompanySettingsPresets.SelectedIndex] = generatedCompanySettings;
                CurrentCompany = generatedCompanySettings;
                UpdateCompanySettingsControlsValues(false);
                UpdatePresetsComboBox(ComboBoxCompanySettingsPresets.SelectedIndex);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Saves company settings to .dat file
        /// </summary>
        /// <returns>True if saving was successful, false otherwise.</returns>
        private bool SaveSettings()
        {
            bool result = false;

            if (null != CompanySettingsPresets && null != Serializer)
            {
                result = Serializer.SerializeObject(CompanySettingsPresets, CompanySettingsPresetsSerializationPath);
                GUIInfoHelper.DisplaySettingsSerializationInfo(result);
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

            if (null != Serializer)
            {
                ControlState state = new ControlState();
                state.ComboBoxCompanySettingsPresetsIndex = ComboBoxCompanySettingsPresets.SelectedIndex;

                result = Serializer.SerializeObject(state, ControlsStateSerializationPath);
            }

            return result;
        }

        private ControlState LoadControlsState()
        {
            ControlState loadedState = null;

            if (null != Serializer)
            {
                loadedState = Serializer.DeserializeObject(ControlsStateSerializationPath) as ControlState;
            }

            return loadedState;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //Hide this window instead of closing it so it can be reopened
            //without creating new instance of CompanySettingsWindow class
            e.Cancel = true;
            this.Hide();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateSettings() && SaveSettings())
            {
                SaveControlsState();
                this.Hide();
            }
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateSettings())
            {
                SaveSettings();
                SaveControlsState();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void ComboBoxCompaniesPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ComboBoxCompanySettingsPresets.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < CompanyPresetsNumber)
            {
                CurrentCompany = CompanySettingsPresets[selectedIndex];
                UpdateCompanySettingsControlsValues((null == CurrentCompany) ? true : false);
            }
        }

        /// <summary>
        /// Sets current company settings from company settings presets based on
        /// given index. Does nothing is index is invalid.
        /// </summary>
        private bool SetCurrentCompany(int presetsIndex)
        {
            bool result = false;

            if (presetsIndex >= 0 && presetsIndex < CompanyPresetsNumber && null != CompanySettingsPresets)
            {
                CurrentCompany = CompanySettingsPresets[presetsIndex];
                result = true;
            }

            return result;
        }

        private void InitializeCurrentCompany()
        {
            //If current company cannot be set (ie. company settings presets combo box's value
            //could not be loaded) try to set first preset that is not null
            if (!SetCurrentCompany(ComboBoxCompanySettingsPresets.SelectedIndex))
            {
                CurrentCompany = CompanySettingsPresets.FirstOrDefault(x => x != null);
            }
        }

        /// <summary>
        /// This is class for storing invoice settings window's control's values. So
        /// that they can be save to .dat file and restored when application is runned again.
        /// </summary>
        [Serializable]
        class ControlState
        {
            public int ComboBoxCompanySettingsPresetsIndex { get; set; }
        }
    }
}
