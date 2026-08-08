namespace AirlineReservationCheckSystem;

public class Booking
{
    public static int BookedSeats { get; set;}
    public Aircrafts a;
    public Booking(Aircrafts a)
    {
        BookedSeats = 0;
        this.a = a;
    }
    public List<Ticket> Book(int seats)
    {
        List<Ticket> t = new List<Ticket>();
        if (seats>a.seats-BookedSeats)
        {
            throw new SeatsNotAvailabe($"we have only {a.seats} left");
        }
        for(int i =0;i<seats;i++)
        {
            BookedSeats+=1;
            Console.WriteLine("passanger name");
            String name = Console.ReadLine().Trim();
            Ticket ticket = new Ticket(a,BookedSeats,name);
            passanger p=new  passanger(name, a.Destination, ticket);
            a.Passanger.Add(p);
            t.Add(ticket);
        }
        return t;
    }
}