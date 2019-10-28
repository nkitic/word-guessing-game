using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class GameMenu : MonoBehaviour
{

    public Text textVolume;
    public Text textNumTerms;
    public Dropdown dropdownCategory;

    private float volume = 1;
    private float numTerms = 2;

    private string connString;

    void Start()
    {
        textVolume = textVolume.GetComponent<Text>();
        textNumTerms = textNumTerms.GetComponent<Text>();
        dropdownCategory = dropdownCategory.GetComponent<Dropdown>();

        dropdownCategory.options.Clear();

        connString = "URI=file:" + Application.dataPath + "/StreamingAssets/database.sqlite";

        addDropdownOptions();

        PlayerPrefs.SetInt("GameType", 0);
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("NumberTerms", numTerms);
    }

    public void addDropdownOptions()
    {
        dropdownCategory.options.Add(new Dropdown.OptionData() { text = "sve" });

        using (IDbConnection dbConn = new SqliteConnection(connString))
        {
            dbConn.Open();

            using (IDbCommand dbCmd = dbConn.CreateCommand())
            {
                string sqlQuery = "";
                
                sqlQuery = "SELECT naziv FROM Kategorija";

                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dropdownCategory.options.Add(new Dropdown.OptionData() { text = reader.GetString(0) });
                    }
                    dbConn.Close();
                    reader.Close();
                }
            }
        }

        dropdownCategory.value = 1;
        dropdownCategory.value = 0;
    }

    public void ChangeCategory(Text category)
    {
        PlayerPrefs.SetString("Category", category.text);
    }

    public void ChangeGameType(int gameType)
    {
        PlayerPrefs.SetInt("GameType", gameType);

        if (gameType != 0)
        {
            dropdownCategory.gameObject.SetActive(false);
        }
        else
        {
            dropdownCategory.gameObject.SetActive(true);
        }
    }

    public void ChangeVolume(float sliderVolume)
    {
        volume = sliderVolume;

        textVolume.text = "Glasnoća : " + volume;

        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void ChangeNumTerms(float sliderNumTerms)
    {
        numTerms = sliderNumTerms;

        textNumTerms.text = "Broj slika : " + numTerms;

        PlayerPrefs.SetFloat("NumberTerms", numTerms);
    }


    public void ChangeToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
