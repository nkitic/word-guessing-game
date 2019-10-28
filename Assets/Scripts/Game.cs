using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.Threading;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public Image image;
    public Canvas canvas;       
    public Text term;
    public AudioSource sound;

    private string[] terms;
    private int indexTerm;
    // private List<int> usedTerms = new List<int>();

    private int gameType;
    private string category;
    private string color;

    private string[,] letter = new string[2,30] { { "a", "b", "c", "č", "ć", "d", "đ", "dž", "e", "f", "g", "h", "i", "j", "k", "l", "lj", "m", "n", "nj", "o", "p", "r", "s", "š", "t", "u", "v", "z", "ž" },
                                                   {"0", "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0",  "0" } };
    private int indexLetter;

    private int numTerms;
    private float volume;

    private string connString;

    void Start()
    {
        connString = "URI=file:" + Application.dataPath + "/StreamingAssets/database.sqlite";
        
        numTerms = (int)PlayerPrefs.GetFloat("NumberTerms", numTerms);
        volume = PlayerPrefs.GetFloat("Volume")/2;
        gameType = PlayerPrefs.GetInt("GameType");
        category = PlayerPrefs.GetString("Category", category);

        terms = new string[numTerms];

        CreateImages();
        LoadTerm();
    }

    void CreateImages()
    {
        int i = 0;
        int x = 0;
        int y = 0;

        Image tempImage;
        Button tempButton;
        
        if (numTerms == 2 || numTerms == 4)
        {
            x = -200;
        }
        else
        {
            x = -400;
        }

        y = 0;
        
        if (numTerms > 3)
        {
            y = 100;
        }

        for (i = 0; i < numTerms; i++)
        {

            tempImage = Instantiate(image, new Vector2(x, y), Quaternion.identity) as Image;
            tempImage.transform.SetParent(canvas.transform, false);
            tempImage.name = "Image" + i;

            var copiedImage = tempImage;
            tempButton = tempImage.GetComponent<Button>();
            tempButton.onClick.AddListener(() => CheckTerm(copiedImage));
                        
            if (numTerms == 6 && i == 2)
            {
                x = -400;
                y = -100;
            }
            else if ((numTerms == 5 && i == 2) || (numTerms == 4 && i == 1))
            {
                x = -200;
                y = -100;
            }
            else
            {
                x += 400;
            }
        }
    }

    public void GetTerms()
    {
        string sqlQuery = "";
        int index = 0;
        bool changeToZero;

        using (IDbConnection dbConn = new SqliteConnection(connString))
        {
            dbConn.Open();

            using (IDbCommand dbCmd = dbConn.CreateCommand())
            {

                sqlQuery = "";

                if (gameType == 0)
                {
                    //sqlQuery = "SELECT naziv, id FROM Pojam WHERE id NOT LIKE " + usedTerms + " ORDER BY RANDOM() LIMIT " + numTerms;
                    if (category == "sve")
                    {
                        sqlQuery = "SELECT Pojam.naziv, Pojam.id, Kategorija.id FROM Pojam INNER JOIN Kategorija ON Pojam.kategorija = Kategorija.id ORDER BY RANDOM() LIMIT " + numTerms;
                    }
                    else
                    {
                        sqlQuery = "SELECT Pojam.naziv, Pojam.id, Pojam.kategorija, Kategorija.naziv, Kategorija.id FROM Pojam INNER JOIN Kategorija ON Pojam.kategorija = Kategorija.id WHERE Kategorija.naziv = '" + category + "' ORDER BY RANDOM() LIMIT " + numTerms;
                    }
                }
                else if (gameType == 1)
                {
                    changeToZero = true;

                    for (int i = 0; i < 30; i++)
                    {
                        if (letter[1, i] == "0")
                        {
                            changeToZero = false;
                        }
                    }

                    if (changeToZero)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            letter[1, i] = "0";
                        }
                    }

                    do
                    {
                        indexLetter = Random.Range(0, 30);
                    } while (letter[1, indexLetter] == "1");

                    sqlQuery = "SELECT naziv FROM Pojam WHERE naziv NOT LIKE '" + letter[0, indexLetter] + "%' ORDER BY RANDOM() LIMIT " + numTerms;

                    letter[1, indexLetter] = "1";
                }
                else if (gameType == 2)
                {
                    sqlQuery = "SELECT naziv FROM Boja ORDER BY RANDOM() LIMIT 1";
                    
                    dbCmd.CommandText = sqlQuery;

                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            color = reader.GetString(0);
                        }

                        reader.Close();
                    }

                    sqlQuery = "SELECT Pojam.naziv, Pojam.boja, Boja.id, Boja.naziv FROM Pojam INNER JOIN Boja ON Pojam.boja=Boja.id WHERE Boja.naziv NOT LIKE '" + color + "' ORDER BY RANDOM() LIMIT " + numTerms;
                }

                indexTerm = Random.Range(0, numTerms);

                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        terms[index] = reader.GetString(0);
                        index++;

                        //usedTerms.Add(reader.GetInt32(1));
                    }

                    index = 0;
                    reader.Close();
                }

                if (gameType == 1)
                {
                    if (letter[0, indexLetter] == "d")
                    {
                        sqlQuery = "SELECT naziv FROM Pojam WHERE naziv LIKE '" + letter[0, indexLetter] + "%' AND naziv NOT LIKE 'dž%' ORDER BY RANDOM() LIMIT 1";
                    }
                    else if (letter[0, indexLetter] == "l")
                    {
                        sqlQuery = "SELECT naziv FROM Pojam WHERE naziv LIKE '" + letter[0, indexLetter] + "%' AND naziv NOT LIKE 'lj%' ORDER BY RANDOM() LIMIT 1";
                    }
                    else if (letter[0, indexLetter] == "n")
                    {
                        sqlQuery = "SELECT naziv FROM Pojam WHERE naziv LIKE '" + letter[0, indexLetter] + "%' AND naziv NOT LIKE 'nj%' ORDER BY RANDOM() LIMIT 1";
                    }
                    else
                    {
                        sqlQuery = "SELECT naziv FROM Pojam WHERE naziv LIKE '" + letter[0, indexLetter] + "%' ORDER BY RANDOM() LIMIT 1";
                    }
                    dbCmd.CommandText = sqlQuery;

                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            terms[indexTerm] = reader.GetString(0);
                        }
                        reader.Close();
                    }
                }
                else if (gameType == 2)
                {
                    sqlQuery = "SELECT Pojam.naziv, Pojam.boja, Boja.id, Boja.naziv FROM Pojam INNER JOIN Boja ON Pojam.boja=Boja.id WHERE Boja.naziv LIKE '" + color + "' ORDER BY RANDOM() LIMIT 1";
                    
                    dbCmd.CommandText = sqlQuery;

                    using (IDataReader reader = dbCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            terms[indexTerm] = reader.GetString(0);
                        }
                        reader.Close();
                    }

                }
            }

            dbConn.Close();
        }
    }

    public void LoadTerm()
    {
        GetTerms();

        var allImages = canvas.GetComponentsInChildren<Image>();
        int i = 0;

        foreach (Image img in allImages)
        {
            if (img.name == ("Image" + i)) {
                img.sprite = Resources.Load<Sprite>("Images/" + terms[i]) as Sprite;
                i++;
            }
        }

        if (gameType == 0)
        {
            term.text = terms[indexTerm].ToUpper();
        }
        else if (gameType == 1)
        {
            term.text = letter[0, indexLetter].ToUpper() + "   " + letter[0, indexLetter];
        }
        else if (gameType == 2)
        {
            term.text = color.ToUpper();
        }

        PlaySound();
    }

    public void PlaySound()
    {
        if (gameType == 0)
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/" + terms[indexTerm]), volume);
        }
        else if (gameType == 1)
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/GameSounds/Letters/" + letter[0, indexLetter]), volume);
        }
        else if (gameType == 2)
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/GameSounds/Colors/" + color), volume);
        }
    }

    public void CheckTerm(Image img)
    {
        sound.Stop();

        if (gameType == 1)
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/" + img.sprite.name), volume);
            
            Thread.Sleep(1000);
        }

        if (img.sprite.name == terms[indexTerm])
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/GameSounds/točno"), volume);

            Thread.Sleep(700);
            
            LoadTerm();
        }
        else
        {
            sound.PlayOneShot((AudioClip)Resources.Load("Sounds/GameSounds/netočno"), volume);
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
