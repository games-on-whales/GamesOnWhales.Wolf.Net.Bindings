using System.Diagnostics;

namespace GamesOnWhales.Wolf.Net.Bindings.Tests;

public static class Utils
{
    public static (string uid, string gid) GetUserIDs(string userName)
    {
        var uid = "";
        var gid = "";
        
        var uidProc = new Process 
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/id",
                Arguments = $"-u {userName}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        
        uidProc.Start();
        while (!uidProc.StandardOutput.EndOfStream)
        {
            var line = uidProc.StandardOutput.ReadLine();
            if(line is not null)
                uid = line;
        }
        
        var gidProc = new Process 
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/id",
                Arguments = $"-g {userName}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };
        
        gidProc.Start();
        while (!gidProc.StandardOutput.EndOfStream)
        {
            var line = gidProc.StandardOutput.ReadLine();
            if(line is not null)
                gid = line;
        }
        
        return (uid, gid);
    }
}