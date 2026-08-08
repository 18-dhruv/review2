namespace AirlineReservationCheckSystem;

public class passanger
{
    public string name;
    public string destination;
    public Ticket ticket;

    public passanger(string name, string destination, Ticket ticket)
    {
        this.name = name;
        this.destination = destination;
        this.ticket = ticket;
    }
}