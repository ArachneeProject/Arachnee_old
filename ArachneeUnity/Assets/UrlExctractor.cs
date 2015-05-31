using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

public class UrlExctractor : MonoBehaviour 
{
    private string sourceCode = "";

    public void exctractSourceCode()
    {
        string urlAddress = "http://www.imdb.com/title/tt0088247/?ref_=fn_al_tt_1";
        
        HttpWebRequest request = null;
        HttpWebResponse response = null;
        StreamReader readStream = null;

        try
        {
            request = (HttpWebRequest)WebRequest.Create(urlAddress);
            response = (HttpWebResponse)request.GetResponse();
        
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                this.sourceCode = readStream.ReadToEnd();
                
                response.Close();
                readStream.Close();

                this.extractTitle();
            }
        }
        catch (System.Exception e)
        {
            if (response != null)
            {
                response.Close();
            }
            if(readStream != null)
            {
                readStream.Close();
            }            
            Debug.Log(e.Message);
        }
    }


    public void extractTitle()
    {
        string title = "lol";
        title = this.getBetween(this.sourceCode, "<span class=\"title-extra\" itemprop=\"name\">", "<i>(original title)</i>");
        Debug.Log(title);
    }

    public string getBetween(string strSource, string strStart, string strEnd)
    {
        int Start, End;
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }
        else
        {
            return "nope";
        }
    }
	
}
