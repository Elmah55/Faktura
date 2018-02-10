namespace Faktura.GUI
{
    /// <summary>
    /// This is class for managing all the windows in the application.
    /// </summary>
    class WindowManager
    {
        // Window's fields are static because the general in concern is
        // that there can be only one instance of each window class.
        // Upon accessing window's class properties if any given
        // window has not been created yet it is created first.
        //Upon closing coresponding window's field is set to null
        //to avoid exceptions thrown by using window's class with
        //deallocated resources.

        private static InvoiceSettingsWindow _InvoiceSettingsWin;
        public InvoiceSettingsWindow InvoiceSettingsWin
        {
            get
            {
                if (null == _InvoiceSettingsWin)
                {
                    _InvoiceSettingsWin = new InvoiceSettingsWindow();
                    _InvoiceSettingsWin.Closed += (sender, e) => { _InvoiceSettingsWin = null; };
                }

                return _InvoiceSettingsWin;
            }
        }

        private static CompanySettingsWindow _CompanySettingsWin;
        public CompanySettingsWindow CompanySettingsWin
        {
            get
            {
                if (null == _CompanySettingsWin)
                {
                    _CompanySettingsWin = new CompanySettingsWindow();
                    _CompanySettingsWin.Closed += (sender, e) => { _CompanySettingsWin = null; };
                }

                return _CompanySettingsWin;
            }
        }

        private static MainWindow _MainWindow;
        public MainWindow MainWin
        {
            get
            {
                if (null == _MainWindow)
                {
                    _MainWindow = new MainWindow();
                    _MainWindow.Closed += (sender, e) => { _MainWindow = null; };
                }

                return _MainWindow;
            }
        }
    }
}
