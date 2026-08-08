namespace AirlineReservationCheckSystem;

public class Aircrafts
{
    public Crew crew { get; set; }
    public string model { get; }
    public int seats { get; }
    public List<passanger>Passanger { get; set; }
    public String Destination { get; set; }
    public Aircrafts(string model, int seats,Crew crew)
    {
        this.model = model;
        this.seats = seats;
        Passanger = new List<passanger>();
        this.crew = crew;
    }
    
    
}