using P03_FootballBetting.Data;

namespace P03_FootballBetting
{
    public class P03_FootballBetting
    {
        static void Main(string[] args)
        {
            var db = new FootballBettingContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
