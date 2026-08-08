namespace AirlineReservationCheckSystem;

public class Pilot:CrewMembers,CheckIn
{
    public string Name { get; }
    public int Age { get; }
    public int YearOfExperience { get;}

    public Pilot(string name, int age, int experience)
    {
        this.Name = name;
        this.Age = age;
        this.YearOfExperience = experience;
    }

    public void Responsibility()
    {
        Console.WriteLine("fly Aircraft");
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