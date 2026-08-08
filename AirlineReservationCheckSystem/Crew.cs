namespace AirlineReservationCheckSystem;

public class Crew
{
    public Pilot pilot1{ get; set; }
    public Pilot pilot2 { get; set; }

    public List<FlightAttendant> FlightAttendants { get; set; }

    public Crew(Pilot p1, Pilot p2, List<FlightAttendant> list)
    {
        this.pilot1 = p1;
        this.pilot2 = p2;
        list = new List<FlightAttendant>();
    }
    
}