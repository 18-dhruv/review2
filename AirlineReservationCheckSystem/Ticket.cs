namespace AirlineReservationCheckSystem;

public class Ticket
{
    public Aircrafts aircraft { get; }
    public string passangerName { get; }
    public int SeatNo { get; }

    internal Ticket(Aircrafts a, int seatNo,string name )
    {
        this.aircraft = a;
        this.SeatNo = seatNo;
        this.passangerName = name;
    }

    public override string ToString()
    {
        return $"aircraft {this.aircraft.model}   seatno {this.SeatNo}  name  {passangerName}";
    }
}