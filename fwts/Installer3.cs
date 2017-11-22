using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;


namespace fwts
{
    [RunInstaller(true)]
    public partial class Installer3 : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer3()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = "Service3";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
