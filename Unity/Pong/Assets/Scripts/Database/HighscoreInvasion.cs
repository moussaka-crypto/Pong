using SQLite4Unity3d;

namespace Database
{
    /// <summary>
    ///	Highscore Invasion class to interface with DB
    /// </summary>
    public class HighscoreInvasion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Score {get; set; }
        public string Name { get; set; }
        public int Difficulty {get; set; }
        /**
         * ToString converts HighscoreInvasion to csv
         */
        public override string ToString()
        {
            return string.Format("{1},{2},{3}", Score, Name, Difficulty);
        }
    }
}