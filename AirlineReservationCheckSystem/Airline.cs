namespace AirlineReservationCheckSystem;

public class Airline
{
    public string Name { get; }

    public List<Pilot> PilotsList {get;}
    public List<Aircrafts>AircraftsList { get;}

    public Airline(string name)
    {
        this.Name = name;
        PilotsList = new List<Pilot>();
        AircraftsList = new List<Aircrafts>();
    }
}