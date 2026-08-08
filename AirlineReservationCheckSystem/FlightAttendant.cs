namespace AirlineReservationCheckSystem;

public class FlightAttendant:CrewMembers
{
    public string Name { get; }
    public int Age { get; }
    public int YearOfExperience { get; }
    public string LicenceNo { get; }

    public FlightAttendant(string name, int age, int experience)
    {
        this.Name = name;
        this.Age = age;
        this.YearOfExperience = experience;
    }
    public void Responsibility()
    {
        Console.WriteLine("manage passangers");
    }
    public void BaggageHandeling()
    {
        Console.WriteLine("Baggage should be handeled on priority");
    }

    public void BoardingPriority()
    {
        Console.WriteLine("high");
    }
}