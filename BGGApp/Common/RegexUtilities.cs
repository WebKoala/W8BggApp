﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

public class RegexUtilities
{
    bool invalid = false;

    public bool IsValidEmail(string strIn)
    {
        invalid = false;
        if (String.IsNullOrEmpty(strIn))
            return false;

        // NOT AVAILABLE (YET) in W8
        //// Use IdnMapping class to convert Unicode domain names. 
        //try
        //{
        //    strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
        //                          RegexOptions.None, TimeSpan.FromMilliseconds(200));
        //}
        //catch (RegexMatchTimeoutException)
        //{
        //    return false;
        //}

        if (invalid)
            return false;

        // Return true if strIn is in valid e-mail format. 
        try
        {
            return Regex.IsMatch(strIn,
                  @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                  RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    // NOT AVAILABLE (YET) in W8
    //private string DomainMapper(Match match)
    //{
        
    //    // IdnMapping class with default property values.
    //    IdnMapping idn = new IdnMapping();

    //    string domainName = match.Groups[2].Value;
    //    try
    //    {
    //        domainName = idn.GetAscii(domainName);
    //    }
    //    catch (ArgumentException)
    //    {
    //        invalid = true;
    //    }
    //    return match.Groups[1].Value + domainName;
    //}
}
