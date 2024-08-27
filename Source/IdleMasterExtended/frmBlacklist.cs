using IdleMasterExtended.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace IdleMasterExtended;

public partial class frmBlacklist : Form
{
    public frmBlacklist() => InitializeComponent();

    public void SaveBlacklist()
    {
        Settings.Default.blacklist.Clear();
        Settings.Default.blacklist.AddRange(lstBlacklist.Items.Cast<string>().ToArray());
        Settings.Default.Save();
    }

    private void FrmBlacklist_Load(object sender, EventArgs e)
    {
        // Localize form
        btnAdd.Text = localization.strings.add;
        btnSave.Text = localization.strings.save;
        Text = localization.strings.manage_blacklist;
        grpAdd.Text = localization.strings.add_game_blacklist;

        lstBlacklist.Items.AddRange(Settings.Default.blacklist.Cast<string>().ToArray());

        if (Settings.Default.customTheme)
        {
            RuntimeCustomThemeBlacklist(); // JN: Apply the dark theme
        }
    }

    // Make everything dark to match the dark theme
    private void RuntimeCustomThemeBlacklist()
    {
        System.Drawing.Color colorBgd = Settings.Default.colorBgd; // Dark gray (from Steam)
        System.Drawing.Color colorTxt = Settings.Default.colorTxt; // Light gray (from Steam)

        // Form
        BackColor = colorBgd;
        ForeColor = colorTxt;

        // Button
        btnAdd.FlatStyle = btnSave.FlatStyle = btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnAdd.BackColor = btnSave.BackColor = btnRemove.BackColor = colorBgd;
        btnAdd.ForeColor = btnSave.ForeColor = btnRemove.ForeColor = colorTxt;

        // List
        lstBlacklist.BackColor = colorBgd;
        lstBlacklist.ForeColor = colorTxt;

        // Group
        grpAdd.BackColor = colorBgd;
        grpAdd.ForeColor = colorTxt;

        // Textbox Appid
        txtAppid.BackColor = colorBgd;
        txtAppid.ForeColor = colorTxt;

        btnRemove.Image = Settings.Default.whiteIcons ? Resources.imgTrash_w : Resources.imgTrash;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        SaveBlacklist();
        Close();
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (int.TryParse(txtAppid.Text, out int result))
        {
            if (lstBlacklist.Items.Cast<string>().All(blApp => blApp != txtAppid.Text))
            {
                lstBlacklist.Items.Add(txtAppid.Text);
            }
        }

        txtAppid.Text = string.Empty;
        txtAppid.Focus();
    }

    private void BtnRemove_Click(object sender, EventArgs e) => lstBlacklist.Items.Remove(lstBlacklist.SelectedItem);
}