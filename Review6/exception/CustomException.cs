using System;

namespace Review5.exception;

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }

        public CustomException(string message, Exception inner) : base(message, inner) { }
    }

    public class MeterRollbackException : CustomException
    {
        public MeterRollbackException(string message) : base(message) { }
    }

    public class UnknownConnectionTypeException : CustomException
    {
        public UnknownConnectionTypeException(string message) : base(message) { }
    }

    public class InvalidReadingException : CustomException
    {
        public InvalidReadingException(string message) : base(message) { }
    }

    public class DuplicateMeterException : CustomException
    {
        public DuplicateMeterException(string message) : base(message) { }
    }

    public class EntityDontExist : Exception
    {
        public EntityDontExist(string message):base(message){}
    }
  public class EnergyException : Exception
  { 
      public EnergyException(string message) : base(message){}
  }
 