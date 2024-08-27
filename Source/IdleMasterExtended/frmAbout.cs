using System;
using System.Diagnostics;

// TODO Although ClickOnce is supported on .NET 5+, apps do not have access to the System.Deployment.Application namespace. For more details see https://github.com/dotnet/deployment-tools/issues/27 and https://github.com/dotnet/deployment-tools/issues/53.
using System.Reflection;
using System.Windows.Forms;

namespace IdleMasterExtended;

public partial class FrmAbout : Form
{
    public FrmAbout() => InitializeComponent();

    private void BtnOK_Click(object sender, EventArgs e) => Close();

    private void FrmAbout_Load(object sender, EventArgs e)
    {
        SetLocalization();
        SetTheme();
        SetVersion();
    }

    private void SetLocalization() => btnOK.Text = localization.strings.ok;

    private void SetTheme()
    {
        Properties.Settings settings = Properties.Settings.Default;
        bool customTheme = settings.customTheme;

        if (customTheme)
        {
            BackColor = settings.colorBgd;
            ForeColor = settings.colorTxt;

            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.BackColor = BackColor;
            btnOK.ForeColor = ForeColor;

            linkLabelVersion.LinkColor = ForeColor;
        }
    }

    private void SetVersion()
    {
        Version version = Assembly.GetExecutingAssembly().GetName().Version;
        linkLabelVersion.Text = string.Format("Idle Master Extended v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
    }

    private void LinkLabelVersion_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://github.com/JonasNilson/idle_master_extended/releases");
}