namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Give a number or 'stop' to exit :\n");
            bool run = true;
            while (run)
            {
                var nb = Console.ReadLine();
                if (nb.Equals("stop"))
                {
                    run = false;    
                }
                else
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    string answer = await client.GetStringAsync("http://localhost:8080/exo3?param1=" + nb.ToString());
                    Console.WriteLine(answer);
                }
            }
        }
    }
}