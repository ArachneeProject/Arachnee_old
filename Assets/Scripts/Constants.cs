using UnityEngine;
using System.Collections;

public class Constants
{
    public const string PP_MainFolder = "MainFolder";
    public const string PP_NewFolderName = "NewFolderName";
    public const string PP_FolderHash = "FolderHash";
    public const string PP_PreviousLevel = "PreviousLevel";
    public const string PP_SeedVertexIdentifier = "SeedVertexIdentifier";
    public const string PP_MoviesToUpdate = "MoviesUpdate";

    public const string Res_DefaultImage = "default";
    public const string Res_LoadingImage = "loading";

    public const char EntryIdentifierSeparator = '-';

    public static string GetHash(string text)
    {        
        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return System.BitConverter.ToString(hash).Replace("-", string.Empty);            
        }
    }
}
