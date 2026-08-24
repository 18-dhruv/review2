namespace Review4.Exceptions;

public class AlreadyExistInHashMap:Exception
{
    public AlreadyExistInHashMap(string message):base(message){}
}

public class AlreadyBorrowed : Exception
{
    public AlreadyBorrowed(string message):base(message){}
}

public class CanGetBook : Exception
{
    public CanGetBook(string message):base(message){}
}