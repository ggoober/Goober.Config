using Goober.Web;

namespace Goober.Config.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProgramUtils.RunWebhost<Startup>(args);
        }
    }
}
