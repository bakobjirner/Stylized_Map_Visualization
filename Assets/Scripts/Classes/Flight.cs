using System;

[Serializable]
public class Flight
{
 public string airport1;
 public string airport2;
 public Airport start;
 public Airport destination;
 public int duration;
 public int distance;
 public Flight(string airport1, string airport2, int duration, int distance)
 {
  this.airport1 = airport1;
  this.airport2 = airport2;
  this.duration = duration;
  this.distance = distance;
 }
 
 public Flight(Airport start, Airport destination,int duration, int distance)
 {
  this.duration = duration;
  this.distance = distance;
  this.start = start;
  this.destination = destination;
 }

}

