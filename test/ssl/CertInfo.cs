using System;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.IO;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// CertInfo.exe
/// --------------
/// Command line tool to get thumbprint from SSL certificate. 
/// Used to speed up/automate SSL self-signed certification 
/// using 'makecert.exe' and 'netsh http add sslcert'
/// as the 'netsh' command uses a thumbprint as input.
/// </summary>
class CertInfo
{
    //Reads a file. 
    internal static byte[] ReadFile(string fileName)
    {
        FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        int size = (int)f.Length;
        byte[] data = new byte[size];
        size = f.Read(data, 0, size);
        f.Close();
        return data;
    }
    //Main method begins here. 
    static void Main(string[] args)
    {
        //Test for correct number of arguments. 
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: CertInfo <filename>");
            return;
        }
        try
        {
            X509Certificate2 x509 = new X509Certificate2();
            //Create X509Certificate2 object from .cer file. 
            byte[] rawData = ReadFile(args[0]);

            x509.Import(rawData);

            //Print to console information contained in the certificate.
            Console.WriteLine(x509.Thumbprint);

            //Add the certificate to a X509Store.
            X509Store store = new X509Store();
            store.Open(OpenFlags.MaxAllowed);
            store.Add(x509);
            store.Close();
        }

        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Error: The directory specified could not be found.");
        }
        catch (IOException)
        {
            Console.WriteLine("Error: A file in the directory could not be accessed.");
        }
        catch (NullReferenceException)
        {
            Console.WriteLine("File must be a .cer file. Program does not have access to that type of file.");
        }
    }

}