using HtmlAgilityPack;
using IdleMasterExtended.Properties;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IdleMasterExtended;

public class Badge
{
    public double AveragePrice { get; set; }

    public int AppId { get; set; }

    public string Name { get; set; }

    public string StringId
    {
        get => AppId.ToString();
        set => AppId = string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
    }

    public int RemainingCard { get; set; }

    public double HoursPlayed { get; set; }

    private Process _idleProcess;

    public bool InIdle => _idleProcess != null && !_idleProcess.HasExited;

    public Process Idle()
    {
        if (InIdle)
        {
            return _idleProcess;
        }

        _idleProcess = Process.Start(new ProcessStartInfo("steam-idle.exe", AppId.ToString()) { WindowStyle = ProcessWindowStyle.Hidden });
        return _idleProcess;
    }

    public void StopIdle()
    {
        if (InIdle)
        {
            _idleProcess.Kill();
        }
    }

    public async Task<bool> CanCardDrops()
    {
        try
        {
            HtmlDocument document = new();
            string response = await CookieClient.GetHttpAsync(Settings.Default.myProfileURL + "/gamecards/" + StringId);
            // Response should be empty. User should be unauthorised.
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            document.LoadHtml(response);

            HtmlNode hoursNode = document.DocumentNode.SelectSingleNode("//div[@class=\"badge_title_stats_playtime\"]");
            string hours = Regex.Match(hoursNode.InnerText, @"[0-9\.,]+").Value;

            HtmlNode cardNode = hoursNode.ParentNode.SelectSingleNode(".//span[@class=\"progress_info_bold\"]");
            string cards = cardNode == null ? string.Empty : Regex.Match(cardNode.InnerText, @"[0-9]+").Value;

            UpdateStats(cards, hours);
            return RemainingCard != 0;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "Badge -> CanCardDrops, for id = " + AppId);
        }

        return false;
    }

    public void UpdateStats(string remaining, string hours)
    {
        RemainingCard = string.IsNullOrWhiteSpace(remaining) ? 0 : int.Parse(remaining);
        HoursPlayed = string.IsNullOrWhiteSpace(hours) ? 0 : double.Parse(hours, new NumberFormatInfo());
    }

    public override bool Equals(object obj) => obj is Badge badge && Equals(AppId, badge.AppId);

    public override int GetHashCode() => AppId.GetHashCode();

    public override string ToString() => string.IsNullOrWhiteSpace(Name) ? StringId : Name;

    public Badge(string id, string name, string remaining, string hours)
      : this()
    {
        StringId = id;
        Name = name;
        UpdateStats(remaining, hours);
    }

    // CONSTRUCTOR
    public Badge()
    { }
}
