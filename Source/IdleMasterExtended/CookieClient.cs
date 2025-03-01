﻿using HtmlAgilityPack;
using IdleMasterExtended.Properties;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IdleMasterExtended;

public class CookieClient : WebClient
{
    internal CookieContainer _cookie = new();

    internal Uri _responseUri;

    /// <summary>
    /// Instantiates a new <c>CookieClient</c> object.
    /// </summary>
    public CookieClient()
    {
        _cookie = GenerateCookies();
        Encoding = Encoding.UTF8;
    }

    /// <summary>
    /// Method <c>GetHttpAsync</c> will asynchronosly use a HTTP GET request (via <c>WebClient</c>) to the provided parameter <c>url</c>
    /// </summary>
    /// <param name="url">URL to send a GET request to</param>
    /// <param name="count">Number of retries to send a GET request</param>
    /// <returns>The HTML content of the HTTP GET response</returns>
    public static async Task<string> GetHttpAsync(string url, int count = 3)
    {
        while (true)
        {
            CookieClient client = new();
            string content = string.Empty;

            try
            {
                content = await client.DownloadStringTaskAsync(url);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex, "CookieClient -> GetHttpAsync, for url = " + url);
            }

            if (!string.IsNullOrWhiteSpace(content) || count == 0)
            {
                return content;
            }

            count--;
        }
    }

    /// <summary>
    /// Method <c>IsLogined</c> will attempt to find out if the user has successfully logged in or not.
    /// </summary>
    /// <returns><c>true</c> if there was information available that confirms the login as successful, otherwise <c>false</c></returns>
    public static async Task<bool> IsLogined()
    {
        HtmlDocument document = new();

        try
        {
            string response = await GetHttpAsync(Settings.Default.myProfileURL);
            document.LoadHtml(response);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "CookieClient -> IsLogined, for url = " + Settings.Default.myProfileURL);
        }

        // The 'notification_count' contains the Steam profile notifications count. Every logged in user should have it available.
        HtmlNode notificationCountNode = document.DocumentNode.SelectSingleNode("//span[@class='notification_count']");

        if (notificationCountNode is null || string.IsNullOrEmpty(notificationCountNode.InnerHtml))
        {
            // The user has no notification count available --> not logged in properly.
            ClearCookies();
            return false;
        }
        else
        {
            return true;
        }
    }

    public static async Task<bool> RefreshLoginToken()
    {
        HtmlDocument document = new();

        CookieContainer cookies = new();
        Uri target = new("https://steamcommunity.com");

        cookies.Add(new Cookie("Cookie", "steamRefresh_steam=" + Settings.Default.steamLoginSecure) { Domain = target.Host });

        try
        {
            string response = await GetHttpAsync("https://login.steampowered.com/jwt/refresh?redir=https%3A%2F%2Fsteamcommunity.com");
            document.LoadHtml(response);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "CookieClient -> RefreshToken, for url = " + Settings.Default.myProfileURL);
        }

        return false;
    }

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest request = base.GetWebRequest(address);

        if (request is HttpWebRequest)
        {
            (request as HttpWebRequest).CookieContainer = _cookie;
        }

        return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request, System.IAsyncResult result)
    {
        try
        {
            WebResponse baseResponse = base.GetWebResponse(request);

            CookieCollection cookies = (baseResponse as HttpWebResponse).Cookies;

            // Check, if cookie should be deleted. This means that sessionID is now invalid and user has to log in again.
            // Maybe this shoud be done other way (authenticate exception), but because of shared settings and timers in frmMain...
            Cookie loginCookie = cookies["steamLoginSecure"];
            if (loginCookie != null && loginCookie.Value == "deleted")
            {
                ClearCookies();
            }

            _responseUri = baseResponse.ResponseUri;

            return baseResponse;
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "CookieClient -> WebResponse = " + request);
        }

        return null;
    }

    private static void ClearCookies()
    {
        Settings.Default.sessionid = string.Empty;
        Settings.Default.steamLogin = string.Empty;
        Settings.Default.steamLoginSecure = string.Empty;
        Settings.Default.steamparental = string.Empty;
        Settings.Default.steamMachineAuth = string.Empty;
        Settings.Default.steamRememberLogin = string.Empty;
        Settings.Default.myProfileURL = string.Empty;

        Settings.Default.Save();
    }

    private static CookieContainer GenerateCookies()
    {
        CookieContainer cookies = new();
        Uri target = new("https://steamcommunity.com");

        cookies.Add(new Cookie("sessionid", Settings.Default.sessionid) { Domain = target.Host });
        cookies.Add(new Cookie("steamLoginSecure", Settings.Default.steamLoginSecure) { Domain = target.Host });
        cookies.Add(new Cookie("steamparental", Settings.Default.steamparental) { Domain = target.Host });
        cookies.Add(new Cookie("steamRememberLogin", Settings.Default.steamRememberLogin) { Domain = target.Host });
        cookies.Add(new Cookie(GetSteamMachineAuthCookieName(), Settings.Default.steamMachineAuth) { Domain = target.Host });

        return cookies;
    }

    private static string GetSteamMachineAuthCookieName() => Settings.Default.steamLoginSecure != null && Settings.Default.steamLoginSecure.Length > 17
            ? string.Format("steamMachineAuth{0}", Settings.Default.steamLoginSecure[..17])
            : "steamMachineAuth";
}