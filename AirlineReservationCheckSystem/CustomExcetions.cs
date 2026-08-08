namespace AirlineReservationCheckSystem;

public class SeatsNotAvailabe:Exception
{
    public SeatsNotAvailabe(string message):base(message){}
}