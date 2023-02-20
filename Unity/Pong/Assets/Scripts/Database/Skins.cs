using SQLite4Unity3d;

namespace Database
{
    /// <summary>
    ///	Skin class to interface with DB
    /// </summary>
    public class Skins
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int Type {get; set; }
        public int Price { get; set; }
        public int Purchased {get; set; }
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", ID, Type, Price, Purchased);
        }
    }
}