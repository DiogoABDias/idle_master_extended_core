using System;
using System.Windows.Forms;

namespace IdleMasterExtended;

public partial class frmStatistics : Form
{
    private readonly Statistics _statistics;

    public frmStatistics(Statistics statistics)
    {
        InitializeComponent();
        _statistics = statistics;
    }

    private void FrmStatistics_Load(object sender, EventArgs e)
    {
        // Localize Form
        Text = localization.strings.statistics.Replace("&", "");
        btnOK.Text = localization.strings.accept;
        lblSessionHeader.Text = localization.strings.this_session + ":";
        lblTotalHeader.Text = localization.strings.total + ":";

        TimeSpan sessionMinutesIdled = TimeSpan.FromMinutes(_statistics.GetSessionMinutesIdled());
        TimeSpan totalMinutesIdled = TimeSpan.FromMinutes(Properties.Settings.Default.totalMinutesIdled);

        int sessionHoursIdled = sessionMinutesIdled.Days * 24 + sessionMinutesIdled.Hours;
        int totalHoursIdled = totalMinutesIdled.Days * 24 + totalMinutesIdled.Hours;

        //Session
        lblSessionTime.Text = sessionHoursIdled > 0
            ? string.Format("{0} hour{1}, {2} minute{3} idled",
                    sessionHoursIdled,
                    sessionHoursIdled == 1 ? "" : "s",
                    sessionMinutesIdled.Minutes,
                    sessionMinutesIdled.Minutes == 1 ? "" : "s")
            : string.Format("{0} minute{1} idled",
                    sessionMinutesIdled.Minutes,
                    sessionMinutesIdled.Minutes == 1 ? "" : "s");

        lblSessionCards.Text = _statistics.GetSessionCardIdled().ToString() + " cards idled";

        //Total
        lblTotalTime.Text = totalHoursIdled > 0
            ? string.Format("{0} hour{1}, {2} minute{3} idled",
                totalHoursIdled,
                totalHoursIdled == 1 ? "" : "s",
                totalMinutesIdled.Minutes,
                totalMinutesIdled.Minutes == 1 ? "" : "s")
            : string.Format("{0} minute{1} idled",
                totalMinutesIdled.Minutes,
                totalMinutesIdled.Minutes == 1 ? "" : "s");

        lblTotalCards.Text = Properties.Settings.Default.totalCardIdled.ToString() + " cards idled";

        if (Properties.Settings.Default.customTheme)
        {
            RuntimeCustomThemeStatistics(); // JN: Apply the dark theme
        }
    }

    // Make everything dark to match the dark theme
    private void RuntimeCustomThemeStatistics()
    {
        System.Drawing.Color colorBgd = Properties.Settings.Default.colorBgd; // Dark gray (from Steam)
        System.Drawing.Color colorTxt = Properties.Settings.Default.colorTxt; // Light gray (from Steam)

        // Form
        BackColor = colorBgd;
        ForeColor = colorTxt;

        // Button
        btnOK.FlatStyle = FlatStyle.Flat; // Flat style to customize buttons
        btnOK.BackColor = colorBgd;
        btnOK.ForeColor = colorTxt;
    }

    private void BtnOK_Click(object sender, EventArgs e) => Close();
}