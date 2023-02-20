using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Database;
using TMPro;

namespace Database
{
    /// <summary>
    /// Class that controls everything highscore related
    /// </summary>
    public class HighscoreManager : MonoBehaviour
    {
        public TMP_InputField inputScore;
        public TMP_InputField inputName;
        public TMP_InputField inputDifficulty;
        public TMP_Text[] names;
        public TMP_Text[] scores;
        public TMP_Text[] difficulties;
        public Slider btn_Gamemode;

        private int gameMode = 0;

        //Invasion? Todo
        private void Start()
        {
            getHighscore();
            btn_Gamemode.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        }

        private void Update()
        {
        }

        void ValueChangeCheck()
        {
            gameMode = (int)btn_Gamemode.value;
            if (gameMode == 0)
            {
                clearScore();
                getHighscore();
            }
            else if (gameMode == 1)
            {
                clearScore();
                getHighscoreInvasion();
            }
        }

        private void clearScore()
        {
            for (int i = 0; i < 10; i++)
            {
                names[i].text = "";
                scores[i].text = "";
                difficulties[i].text = "";
            }
        }

        /// <summary>
        /// Creates a new Highscore in the DB
        /// </summary>
        public void InsertHs()
        {
            int Score = Int32.Parse(inputScore.text);
            string Name = inputName.text;
            int Difficulty = Int32.Parse(inputDifficulty.text);
            NewHighscore(Score, Name, Difficulty);
            Debug.Log("Created Record for : "+Score+Name+Difficulty);
        }
        /// <summary>
        /// Returns an int of the Lowest classic mode Highscore
        /// </summary>
        public int FetchLowestHs()
        {
            int res = Database.HighscoreManager.GetLowestScore();
            Debug.Log ("done getting lowest hs");
            return res;
        }
        /// <summary>
        /// Returns an int of the Highest Highscore
        /// </summary>
        public int FetchHighestHs()
        {
            int res = Database.HighscoreManager.GetHighestScore();
            Debug.Log ("done getting lowest hs");
            return res;
        }
        /// <summary>
        /// Prints after a Highscore Record has been gotten
        /// </summary>
        public void FetchHs()
        {
            //string[] res = Database.HighscoreManager.getHighscore();
            Debug.Log ("done  getting hs");
        }
        /// <summary>
        /// Creates a new Highscore in the DB from parameters
        /// </summary>
        public void NewHighscore(int newScore, string newName, int newDifficulty)
        {
            var ds = new DataService("pong.db");
            ds.CreateHighscore(newScore, newName, newDifficulty);
        }
        /// <summary>
        /// Returns a string array of the Score, Name and Difficulty above the given new Score
        /// </summary>
        public static string[] GetTop(int newScore)
        {
            IEnumerable<Highscore> hs;
            var ds = new DataService("pong.db");
            hs = ds.GetNeighbourScoreTop(newScore);
            string tmp = "";
            foreach (var score in hs)
            {
                tmp += score.Id;
                tmp += ",";
                tmp += score.Score;
                tmp += ",";
                tmp += score.Name;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Returns a string array of the Score, Name and Difficulty below the given new Score
        /// </summary>
        public static string[] GetBottom(int newScore)
        {
            IEnumerable<Highscore> hs;
            var ds = new DataService("pong.db");
            hs = ds.GetNeighbourScoreBottom(newScore);
            string tmp = "";
            foreach (var score in hs)
            {
                tmp += score.Id;
                tmp += ",";
                tmp += score.Score;
                tmp += ",";
                tmp += score.Name;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Returns the lowest score in top ten
        /// </summary>
        public static int GetLowestScore()
        {
            var ds = new DataService("pong.db");
            var hs = ds.GetLowestHighscore();
            int tmp = 0;
            if (hs != null)
            {
                foreach (var score in hs)
                {
                    tmp=score.Score;
                }
                Debug.Log("Der niedrigste Score lautet: " + tmp);
                return tmp;
            }
            return 0;
        }
        
        /// <summary>
        /// Returns the highest score in top ten
        /// </summary>
        public static int GetHighestScore()
        {
            var ds = new DataService("pong.db");
            var hs = ds.GetHighestHighscore();
            int tmp = 0;
            foreach (var score in hs)
            {
                tmp=score.Score;
            }
            Debug.Log("Der höchste Score lautet:" + tmp);
            return tmp;
        }
        /// <summary>
        /// Returns a string array of the Score, Name and Difficulty for all top ten
        /// </summary> 
        public string[] getHighscore()
        {
            var ds = new DataService("pong.db");
            var hs = ds.GetHighscore();
            string tmp = hs.ToString();
            int i = 0;
            foreach (var score in hs)
            {
                tmp=(score.Score.ToString() + score.Name.ToString());  
                names[i].text = score.Name.ToString();
                scores[i].text = score.Score.ToString();
                switch (score.Difficulty)
                {
                    case 0:
                        difficulties[i].text = "Easy";
                        difficulties[i].color = Color.green;
                        break;
                    case 1:
                        difficulties[i].text = "Medium";
                        difficulties[i].color = Color.yellow;
                        break;
                    case 2:
                        difficulties[i].text = "Hard";
                        difficulties[i].color = Color.red;
                        break;
                }
                Debug.Log(tmp);
                i++;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Returns the amount of Scores above the given newScore
        /// </summary> 
        public static int RetTopCount(int newScore)
        {
            var ds = new DataService("pong.db");
            return ds.SubsetAboveCount(newScore);
        }
        /// <summary>
        /// Returns the amount of Scores below the given newScore
        /// </summary> 
        public static int RetBottomCount(int newScore)
        {
            var ds = new DataService("pong.db");
            return ds.SubsetBelowCount(newScore);
        }
        /// <summary>
        /// Returns the amount of Scores already entered
        /// </summary> 
        public static int RetCountRows()
        {
            var ds = new DataService("pong.db");
            return ds.countRows();
        }
        
        
        
        //Invasion mode
        
        
        /// <summary>
        /// Returns a string array of the Invasion Score, Name and Difficulty for all top ten
        /// </summary> 
        public string[] getHighscoreInvasion()
        {
            var ds = new DataService("pong.db");
            var hs = ds.GetHighscoreInvasion();
            string tmp = hs.ToString();
            int i = 0;
            foreach (var score in hs)
            {
                tmp=(score.Score.ToString() + score.Name.ToString());  
                names[i].text = score.Name.ToString();
                scores[i].text = score.Score.ToString();
                switch (score.Difficulty)
                {
                    case 0:
                        difficulties[i].text = "Easy";
                        difficulties[i].color = Color.green;
                        break;
                    case 1:
                        difficulties[i].text = "Medium";
                        difficulties[i].color = Color.yellow;
                        break;
                    case 2:
                        difficulties[i].text = "Hard";
                        difficulties[i].color = Color.red;
                        break;
                }
                Debug.Log(tmp);
                i++;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Creates a new Invasion Highscore in the DB from parameters
        /// </summary>
        public void NewHighscoreInvasion(int newScore, string newName, int newDifficulty)
        {
            var ds = new DataService("pong.db");
            ds.CreateHighscoreInvasion(newScore, newName, newDifficulty);
        }
        public static string[] GetTopInvasion(int newScore)
        {
            IEnumerable<HighscoreInvasion> hs;
            var ds = new DataService("pong.db");
            hs = ds.GetNeighbourScoreTopInvasion(newScore);
            string tmp = "";
            foreach (var score in hs)
            {
                tmp += score.Id;
                tmp += ",";
                tmp += score.Score;
                tmp += ",";
                tmp += score.Name;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Returns a string array of the Score, Name and Difficulty below the given new Score
        /// </summary>
        public static string[] GetBottomInvasion(int newScore)
        {
            IEnumerable<HighscoreInvasion> hs;
            var ds = new DataService("pong.db");
            hs = ds.GetNeighbourScoreBottomInvasion(newScore);
            string tmp = "";
            foreach (var score in hs)
            {
                tmp += score.Id;
                tmp += ",";
                tmp += score.Score;
                tmp += ",";
                tmp += score.Name;
            }
            return tmp.Split(',');
        }
        /// <summary>
        /// Returns the lowest invasion score in top ten
        /// </summary>
        public static int GetLowestScoreInvasion()
        {
            var ds = new DataService("pong.db");
            var hs = ds.GetLowestHighscoreInvasion();
            int tmp = 0;
            if (hs != null)
            {
                foreach (var score in hs)
                {
                    tmp=score.Score;
                }
                Debug.Log("Der niedrigste Score lautet: " + tmp);
                return tmp;
            }
            return 0;
        }
        /// <summary>
        /// Returns the amount  of Invasion Scores above the given newScore
        /// </summary> 
        public static int RetTopCountInvasion(int newScore)
        {
            var ds = new DataService("pong.db");
            return ds.SubsetAboveCountInvasion(newScore);
        }
        /// <summary>
        /// Returns the amount of Invasion Scores below the given newScore
        /// </summary> 
        public static int RetBottomCountInvasion(int newScore)
        {
            var ds = new DataService("pong.db");
            return ds.SubsetBelowCountInvasion(newScore);
        }
        /// <summary>
        /// Returns the amount of Invasion Scores already entered
        /// </summary> 
        public static int RetCountRowsInvasion()
        {
            var ds = new DataService("pong.db");
            return ds.countRowsInvasion();
        }
        /// <summary>
        /// Returns an int of the Lowest Invasion mode Highscore
        /// </summary>
        public int FetchLowestHsInvasion()
        {
            int res = Database.HighscoreManager.GetLowestScoreInvasion();
            Debug.Log ("done getting invasion lowest hs");
            return res;
        }
    }
}