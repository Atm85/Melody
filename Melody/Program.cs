namespace Melody
{
    class Program
    {
        static void Main(string[] args)
        {
            new MelodyClient().InitializeAsync().GetAwaiter().GetResult();
        }
    }
}
