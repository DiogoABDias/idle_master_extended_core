using IdleMasterExtended.Properties;
using System;
using System.Net;
using System.Text;
using System.Xml;

namespace IdleMasterExtended;

internal class SteamProfile
{
    internal static string GetSteamId()
    {
        string steamid = WebUtility.UrlDecode(Settings.Default.steamLoginSecure);
        int index = steamid.IndexOfAny(['|'], 0);
        return index != -1 ? steamid.Remove(index) : steamid;
    }

    internal static string GetSteamUrl() => "https://steamcommunity.com/profiles/" + GetSteamId();

    internal static string GetSignedAs()
    {
        string steamUrl = GetSteamUrl();
        string userName = "User " + GetSteamId();
        try
        {
            string xmlRaw = new WebClient() { Encoding = Encoding.UTF8 }.DownloadString(string.Format("{0}/?xml=1", steamUrl));
            XmlDocument xml = new();
            xml.LoadXml(xmlRaw);
            XmlNode nameNode = xml.SelectSingleNode("//steamID");
            if (nameNode != null)
            {
                userName = WebUtility.HtmlDecode(nameNode.InnerText);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Logger.Exception(ex, "frmMain -> GetSignedAs, for steamUrl = " + steamUrl);
        }

        return localization.strings.signed_in_as + " " + userName;
    }
}