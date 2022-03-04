using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace BasicServerHTTPlistener
{

    internal class Program
    {
        private static void Main(string[] args)
        {

            //if HttpListener is not supported by the Framework
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("A more recent Windows version is required to use the HttpListener class.");
                return;
            }


            // Create a listener.
            HttpListener listener = new HttpListener();

            // Add the prefixes.
            if (args.Length != 0)
            {
                foreach (string s in args)
                {
                    listener.Prefixes.Add(s);
                    // don't forget to authorize access to the TCP/IP addresses localhost:xxxx and localhost:yyyy 
                    // with netsh http add urlacl url=http://localhost:xxxx/ user="Tout le monde"
                    // and netsh http add urlacl url=http://localhost:yyyy/ user="Tout le monde"
                    // user="Tout le monde" is language dependent, use user=Everyone in english 

                }
            }
            else
            {
                Console.WriteLine("Syntax error: the call must contain at least one web server url as argument");
            }
            listener.Start();

            // get args 
            foreach (string s in args)
            {
                Console.WriteLine("Listening for connections on " + s);
            }

            // Trap Ctrl-C on console to exit 
            Console.CancelKeyPress += delegate {
                // call methods to close socket and exit
                listener.Stop();
                listener.Close();
                Environment.Exit(0);
            };


            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                // get url 
                Console.WriteLine($"Received request for {request.Url}");

                //get url protocol
                Console.WriteLine(request.Url.Scheme);
                //get user in url
                Console.WriteLine(request.Url.UserInfo);
                //get host in url
                Console.WriteLine(request.Url.Host);
                //get port in url
                Console.WriteLine(request.Url.Port);
                //get path in url 
                Console.WriteLine(request.Url.LocalPath);

                // parse path in url 
                String name = "";
                String exec = "";
                foreach (string str in request.Url.Segments)
                {
                    Console.WriteLine(str);
                    if (str.Equals("newAnswer"))
                    {
                        name = "newAnswer";
                    }
                    if (str.Equals("callExec"))
                    {
                        exec = "callExec";
                    };
                }

                //get params un url. After ? and between &

                String param1 = HttpUtility.ParseQueryString(request.Url.Query).Get("param1");
                String param2 = HttpUtility.ParseQueryString(request.Url.Query).Get("param2");

                Console.WriteLine(request.Url.Query);

                string result = "";

                if (request.Url.Segments[1].Equals("exo1/"))
                {
                    Console.WriteLine("EXO 1 MyMethods");

                    Type type = typeof(MyMethods);
                    MethodInfo method = type.GetMethod(request.Url.Segments[2]);

                    string prod = (string)method.Invoke(null, new object[] { param1, param2 });

                    result = "Exercice 1 : Resultat de " + request.Url.Segments[2] + " avec " + param1 + " et " + param2 + " => " + prod;

                    Console.WriteLine(result);
                }
                else if (request.Url.Segments[1].Equals("exo2"))
                {
                    Console.WriteLine("EXO 2 InvocExec");

                    Type type = typeof(InvocExec);
                    MethodInfo method = type.GetMethod("getExec");

                    string prod = (string)method.Invoke(null, new object[] { param1 });

                    result = "Exercice 2 : Resultat de getExec => " + prod;

                    Console.WriteLine(result);
                }
                else if (request.Url.Segments[1].Equals("exo3"))
                {
                    Console.WriteLine("EXO 3 MyMethods");

                    Type type = typeof(ServerToclient);
                    MethodInfo method = type.GetMethod("incr");

                    string prod = (string)method.Invoke(null, new object[] { param1 });

                    result = prod;

                    Console.WriteLine(result);
                }


                //parse params in url
                Console.WriteLine("param1 = " + param1);
                Console.WriteLine("param2 = " + param2);
                Console.WriteLine("param3 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param3"));
                Console.WriteLine("param4 = " + HttpUtility.ParseQueryString(request.Url.Query).Get("param4"));

                //
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                string responseString = result;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            // Httplistener neither stop ... But Ctrl-C do that ...
            // listener.Stop();
        }
    }
}

/*
 * Exercice 1 :
 */
internal class MyMethods
{
    /*
     * To use the methode add :
     * http://localhost:8080/exo1/add?param1=1&param2=2
     */
    public static string add(string a, string b)
    {
        try
        {
            int nb1 = Int32.Parse(a);
            int nb2 = Int32.Parse(b);
            return (nb1 + nb2).ToString();
        }
        catch (FormatException)
        {
            throw new FormatException();
        }
    }

    /*
     * To use the methode sub :
     * http://localhost:8080/exo1/sub?param1=1&param2=2
     */

    public static string sub(string a, string b)
    {
        try
        {
            int nb1 = Int32.Parse(a);
            int nb2 = Int32.Parse(b);
            return (nb1 - nb2).ToString();
        }
        catch (FormatException)
        {
            throw new FormatException();
        }
    }

    /*
     * To use the methode mult :
     * http://localhost:8080/exo1/mult?param1=1&param2=2
     */
    public static string mult(string a, string b)
    {
        try
        {
            int nb1 = Int32.Parse(a);
            int nb2 = Int32.Parse(b);
            return (nb1 * nb2).ToString();
        }
        catch (FormatException)
        {
            throw new FormatException();
        }
    }

    /*
     * To use the methode concat :
     * http://localhost:8080/exo1/concat?param1=1&param2=2
     */
    public static string concat(string a, string b)
    {
        return a + b;
    }
}

/*
 * Exercice 2 :
 */
internal class InvocExec
{
    /*
     * To use the methode getExec :
     * http://localhost:8080/exo2?param1=test
     */
    public static string getExec(string arg)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = @"../../../BasicWebServer/ExecExterne/ExecTest.exe";
        start.Arguments = arg; // Specify arguments.
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;

        string result = "Error no execution here";

        using (Process process = Process.Start(start))
        {
            //
            // Read in all the text from the process with the StreamReader.
            //
            using (StreamReader reader = process.StandardOutput)
            {
                result = reader.ReadToEnd();
                Console.WriteLine(result);
            }
        }
        return result;
    }
}

internal class ServerToclient
{
    /*
     * To use the methode incr :
     * http://localhost:8080/exo3?param1=5
     */
    public static string incr(string nb)
    {
        try
        {
            int nb1 = Int32.Parse(nb);
            return "incr OK val = " + (nb1+1).ToString();
        }
        catch (FormatException)
        {
            throw new FormatException();
        }
    }
}