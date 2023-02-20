using System;
using SQLite4Unity3d;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System.Collections.Generic;
using System.Linq;

namespace Database
{
    public class DataService  {

	private SQLiteConnection _connection;

	public DataService(string DatabaseName){

#if UNITY_EDITOR
            var dbPath = string.Format(@"Assets/StreamingAssets/{0}", DatabaseName);
#else
        // check if file exists in Application.persistentDataPath
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

        if (!File.Exists(filepath))
        {
            Debug.Log("Database not in Persistent path");
            // if it doesn't ->
            // open StreamingAssets directory and load the db ->

#if UNITY_ANDROID 
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName);  // this is the path to your StreamingAssets in android
            while (!loadDb.isDone) { }  // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
            // then save to Application.persistentDataPath
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
                // then save to Application.persistentDataPath
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
		var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
		
#elif UNITY_STANDALONE_OSX
		var loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
		// then save to Application.persistentDataPath
		File.Copy(loadDb, filepath);
#else
	var loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName;  // this is the path to your StreamingAssets in iOS
	// then save to Application.persistentDataPath
	File.Copy(loadDb, filepath);

#endif

            Debug.Log("Database written");
        }

        var dbPath = filepath;
#endif
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
//        Debug.Log("Final PATH: " + dbPath);     

	}
	/// <summary>
	/// Returns an IEnumerable of the Highest Highscore
	/// </summary>
	public IEnumerable<Highscore> GetHighestHighscore()
	{
		try
		{
			var minScore = _connection.Table<Highscore>().Max(s => s.Score);
			return _connection.Table<Highscore>().Where(s => s.Score == minScore);
		}
		catch (InvalidOperationException e)
		{
			Debug.Log(" "+e);
		}
		return null;
	}
	/// <summary>
	/// Returns an IEnumerable of the Lowest Highscore
	/// </summary>
	public IEnumerable<Highscore> GetLowestHighscore()
	{
		try
		{
			var minScore = _connection.Table<Highscore>().Min(s => s.Score);
			return _connection.Table<Highscore>().Where(s => s.Score == minScore);

		}
		catch (InvalidOperationException e)
		{ 
			Debug.Log(e);
		}
		return null;
	}
	/// <summary>
	/// Returns an IEnumerable of the Skins Table
	/// </summary>
	public IEnumerable<Skins> getSkinsTable()
	{
		return _connection.Table<Skins>();
	}
	/// <summary>
	/// Creates a new Classic Mode Highscore in the DB
	/// </summary>
	public Highscore CreateHighscore(int newScore, string newName, int newDifficulty){
		var hs = new Highscore{
				Score = newScore,
				Name = newName,
				Difficulty = newDifficulty
		};
		_connection.Insert (hs);
		return hs;
	}
	/// <summary>
	/// Sets the skin ID as purchased in DB
	/// </summary>
	public void buySkin(int id)
	{
		try {
			var SkinToBeUpdated = _connection.Table<Skins>().Single(s => s.ID == id );
			SkinToBeUpdated.Purchased = 1;
			_connection.Update(SkinToBeUpdated);
		}
		catch (Exception e) {
			Console.WriteLine(e);
			throw;
		}
	}
	/// <summary>
	/// Returns IEnumerable of all Highscores
	/// </summary>
	public IEnumerable<Highscore> GetHighscore()
	{
		var list = _connection.Table<Highscore>().OrderByDescending(x => x.Score);
		Debug.Log(list.ToString());
		return list;
	} 
	
	/// <summary>
	/// Returns an IEnumerable of the Score above the given new Score
	/// </summary>
	public IEnumerable<Highscore> GetNeighbourScoreTop(int newScore)
	{
		var sub = _connection.Table<Highscore>().Where(s => s.Score > newScore).OrderBy(s => s.Score)
			.FirstOrDefault();
		return _connection.Table<Highscore>().Where(s => s.Score == sub.Score);
		;
	}
	/// <summary>
	/// Returns an IEnumerable of the Score below the given new Score
	/// </summary>
	public IEnumerable<Highscore> GetNeighbourScoreBottom(int newScore)
	{
		var sub = _connection.Table<Highscore>().Where(s => s.Score < newScore).OrderByDescending(s => s.Score)
			.FirstOrDefault();
		return _connection.Table<Highscore>().Where(s => s.Score == sub.Score);
	}

	/// <summary>
	///	returns the count of scores above the given score
	/// </summary>
	public int SubsetAboveCount(int newScore)
	{
		return _connection.Table<Highscore>().Count(s => s.Score >= newScore);
	}
	/// <summary>
	///	returns the count of scores below the given score
	/// </summary>
	public int SubsetBelowCount(int newScore)
	{
		return _connection.Table<Highscore>().Count(s => s.Score < newScore);
	}
	/// <summary>
	///	returns the count of scores
	/// </summary>
	public int countRows()
	{
		return _connection.Table<Highscore>().Count();
	}
	
	
	
	
	//Invasion mode methods:
	
	
	
	/// <summary>
	/// Creates a new Invasion Highscore in the DB
	/// </summary>
	public HighscoreInvasion CreateHighscoreInvasion(int newScore, string newName, int newDifficulty){
		var hs = new HighscoreInvasion{
			Score = newScore,
			Name = newName,
			Difficulty = newDifficulty
		};
		_connection.Insert(hs);
		return hs;
	}
	/// <summary>
	/// Returns the Highscore List for Invasion mode
	/// </summary>
	public IEnumerable<HighscoreInvasion> GetHighscoreInvasion()
	{
		Debug.Log("Getting Invasion Highscores!");
		var list = _connection.Table<HighscoreInvasion>().OrderByDescending(x => x.Score);
		Debug.Log(list.ToString());
		return list;
	}
	/// <summary>
	/// Returns the lowest highscore for Invasion mode
	/// </summary>
	public IEnumerable<HighscoreInvasion> GetLowestHighscoreInvasion()
	{
		try
		{
			var minScore = _connection.Table<HighscoreInvasion>().Min(s => s.Score);
			return _connection.Table<HighscoreInvasion>().Where(s => s.Score == minScore);
		}
		catch (InvalidOperationException e)
		{ 
			Debug.Log(e);
		}
		return null;
	}
	/// <summary>
	/// Returns an IEnumerable of the Invasion Score above the given new Score
	/// </summary>
	public IEnumerable<HighscoreInvasion> GetNeighbourScoreTopInvasion(int newScore)
	{
		var sub = _connection.Table<HighscoreInvasion>().Where(s => s.Score > newScore).OrderBy(s => s.Score)
			.FirstOrDefault();
		return _connection.Table<HighscoreInvasion>().Where(s => s.Score == sub.Score);
		;
	}
	/// <summary>
	/// Returns an IEnumerable of the Invasion Score below the given new Score
	/// </summary>
	public IEnumerable<HighscoreInvasion> GetNeighbourScoreBottomInvasion(int newScore)
	{
		var sub = _connection.Table<HighscoreInvasion>().Where(s => s.Score < newScore).OrderByDescending(s => s.Score)
			.FirstOrDefault();
		return _connection.Table<HighscoreInvasion>().Where(s => s.Score == sub.Score);
	}

	/// <summary>
	///	returns the count of Invasion scores above the given score
	/// </summary>
	public int SubsetAboveCountInvasion(int newScore)
	{
		return _connection.Table<HighscoreInvasion>().Count(s => s.Score >= newScore);
	}
	/// <summary>
	///	returns the count of Invasion scores below the given score
	/// </summary>
	public int SubsetBelowCountInvasion(int newScore)
	{
		return _connection.Table<HighscoreInvasion>().Count(s => s.Score < newScore);
	}
	/// <summary>
	///	returns the count of Invasion scores
	/// </summary>
	public int countRowsInvasion()
	{
		return _connection.Table<HighscoreInvasion>().Count();
	}
	
	
    }
}
