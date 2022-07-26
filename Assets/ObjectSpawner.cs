using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BAMCIS.GeoJSON;
using Unity.Mathematics;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    public GameObject smallHouse;
    public GameObject mediumHouse;
    public GameObject bigHouse;
    public float radius = 1;
    public TextAsset cityJson;
    private FeatureCollection cities;
    public Texture2D heightmap;
    public float heightMultiplier = .1f;
    
    //threshold values to decide which category a city belongs to
    [Range(0,10000000)]
    public int minSmallCity = 50000;
    [Range(0,10000000)]
    public int minMediumCity = 500000;
    [Range(0,10000000)]
    public int minBigCity = 5000000;
    
    // Start is called before the first frame update
    void Start()
    {
       // getCityData();
        //placeAllHouses();
    }

    private void getCityData()
    {
        cities = FeatureCollection.FromJson(cityJson.text);
    }

    private void placeAllHouses()
    {
        List<Feature> cityList = cities.Features.ToList();
        int textureSizeX = heightmap.height;
        int textureSizeY = heightmap.width;
        for (int i = 0; i < cityList.Count; i++)
        {
            
            float lat = (float)cityList[i].Properties["LATITUDE"];
            float lon = (float)cityList[i].Properties["LONGITUDE"];
            int pixelLat = (int)(((lat / 180) + .5f) * textureSizeY)*-1;
            int pixelLon = (int)(((lat / 360) + .5f) * textureSizeX);
            float height = heightmap.GetPixel(pixelLon, pixelLat).r;
            int population = (int)cityList[i].Properties["POP_MAX"];
            if (population > minBigCity)
            {
                placeHouse(lat,lon,CitySize.BIG,height);  
            }else if (population > minMediumCity)
            {
                placeHouse(lat,lon,CitySize.MEDIUM,height);  
            }else if (population > minSmallCity)
            {
                placeHouse(lat,lon,CitySize.SMALL,height);  
            }
        }
    }
    private void placeHouse(float lat, float lon, CitySize size, float heigth)
    {
        GameObject myHouse;
        switch (size)
        {
          case  CitySize.BIG: myHouse = bigHouse;
              break;
          case CitySize.MEDIUM: myHouse = mediumHouse;
              break;
          case CitySize.SMALL: myHouse = smallHouse;
              break;
          default: return;
        }
        Vector3 pos = getPointOnSphere(lat, lon)*(1+heigth*heightMultiplier);
        Instantiate(myHouse, pos, Quaternion.LookRotation(pos),transform);
    }

    //based on sebastian lagues project https://www.youtube.com/watch?v=sLqXFF8mlEU&t=820s Minute: 3
    private Vector3 getPointOnSphere(float lat, float lon)
    {
        //use radians
        lat *= Mathf.PI / 180;
        lon *= Mathf.PI / 180;
        float y = Mathf.Sin(lat);
        float r = Mathf.Cos(lat);
        float x = Mathf.Sin(lon) * r;
        float z = -Mathf.Cos(lon) * r;
        return new Vector3(x, y, z).normalized;
    }
}
