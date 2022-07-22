using System;
using UnityEngine;

[Serializable]
public class Flight
{
 public string startCode;
 public string destinationCode;
 public string startName;
 public string destinationName;
 public Vector2 startLocation;
 public Vector2 destinationLocation;
 public string startCountry;
 public string destinationCountry;
 public int duration;
 public int distance;
 public Flight(string startCode, string destinationCode, int duration, int distance)
 {
  this.startCode = startCode;
  this.destinationCode = destinationCode;
  this.duration = duration;
  this.distance = distance;
 }
}

