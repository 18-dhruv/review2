namespace Review4.models;

public class Member
{
    public static int IdCounter = 0;
    public int Id { get; }
    public string Name { get; set; }
    public string Address { get; set; }
    public  int  PhoneNumber { get; }

   public Member(string name, string address, int number)
    {
        this.Name = name;
        this.Address = address;
        this.PhoneNumber = number;

        this.Id = ++IdCounter;
    }
}