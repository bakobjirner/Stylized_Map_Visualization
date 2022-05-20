using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{

    public TextAsset countryJson;
    public CountryData countryData;

    // Start is called before the first frame update
    void Start()
    {
        countryData = new CountryData();
        countryData = JsonUtility.FromJson<CountryData>(countryJson.text);

        Feature[] countries = countryData.features;
        for(int i = 0; i < countries.Length; i++)
        {
            Debug.Log(countries[i].geometry.coordinates[0].points);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
