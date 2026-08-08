namespace  AirlineReservationCheckSystem;

public class program
{
    public static void Main(string[]args)
    {
       
        Pilot p1 = new Pilot("p1", 21, 12);
        Pilot p2 = new Pilot("p3", 21, 12);
        FlightAttendant f = new FlightAttendant("f1", 21, 12);
        List<FlightAttendant> list = new List<FlightAttendant>();
        list.Add(f);
        Crew c = new Crew(p1,p2,list);
        Aircrafts a = new Aircrafts("boing 1",1,c);
        Booking b = new Booking(a);
        List<Ticket> t=b.Book(4);
        foreach (Ticket z in t)
        {
           Console.WriteLine(z.ToString());
        }        
    }
}
