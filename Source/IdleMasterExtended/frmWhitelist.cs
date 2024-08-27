using IdleMasterExtended.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace IdleMasterExtended;

public partial class frmWhitelist : Form
{
    private readonly FrmMain _mainForm;

    public frmWhitelist(FrmMain parentForm)
    {
        this._mainForm = parentForm;
        InitializeComponent();
    }

    public void SaveWhitelist()
    {
        Settings.Default.whitelist.Clear();
        Settings.Default.whitelist.AddRange(lstWhitelist.Items.Cast<string>().ToArray());
        Settings.Default.Save();
    }

    private void FrmWhitelist_Load(object sender, EventArgs e)
    {
        // Localize form
        btnAdd.Text = localization.strings.add;
        btnSave.Text = localization.strings.save;
        Text = localization.strings.manage_whitelist;
        grpAdd.Text = localization.strings.add_game_whitelist;

        lstWhitelist.Items.AddRange(Settings.Default.whitelist.Cast<string>().ToArray());

        if (Settings.Default.customTheme)
        {
            ApplyTheme();
        }
    }

    private void ApplyTheme()
    {
        System.Drawing.Color colorBgd = Settings.Default.colorBgd;
        System.Drawing.Color colorTxt = Settings.Default.colorTxt;

        BackColor = colorBgd;
        ForeColor = colorTxt;

        btnAdd.FlatStyle = btnSave.FlatStyle = btnRemove.FlatStyle = FlatStyle.Flat;
        btnAdd.BackColor = btnSave.BackColor = btnRemove.BackColor = colorBgd;
        btnAdd.ForeColor = btnSave.ForeColor = btnRemove.ForeColor = colorTxt;

        lstWhitelist.BackColor = colorBgd;
        lstWhitelist.ForeColor = colorTxt;

        grpAdd.BackColor = colorBgd;
        grpAdd.ForeColor = colorTxt;

        txtAppid.BackColor = colorBgd;
        txtAppid.ForeColor = colorTxt;

        btnRemove.Image = Settings.Default.whiteIcons ? Resources.imgTrash_w : Resources.imgTrash;
    }

    private async void BtnSave_Click(object sender, EventArgs e)
    {
        SaveWhitelist();

        if (Settings.Default.IdlingModeWhitelist)
        {
            _mainForm.StopIdle();
            await _mainForm.LoadBadgesAsync();

            if (lstWhitelist.Items.Count == 1)
            {
                _mainForm.StartSoloIdle(
                    _mainForm.AllBadges.FirstOrDefault(b => b.AppId == int.Parse(lstWhitelist.Items[0].ToString()))
                );
            }
            else if (lstWhitelist.Items.Count > 1)
            {
                _mainForm.StartMultipleIdle();
            }

            _mainForm.DisableCardDropCheckTimer();
            _mainForm.UpdateStateInfo();
        }
        else
        {
            _mainForm.EnableCardDropCheckTimer();
        }

        Close();
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (int.TryParse(txtAppid.Text, out int result)
            && lstWhitelist.Items.Cast<string>().All(blApp => blApp != txtAppid.Text))
        {
            lstWhitelist.Items.Add(txtAppid.Text);
        }

        txtAppid.Text = string.Empty;
        txtAppid.Focus();
    }

    private void BtnRemove_Click(object sender, EventArgs e) => lstWhitelist.Items.Remove(lstWhitelist.SelectedItem);
}