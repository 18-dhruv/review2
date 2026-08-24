namespace Review4.models;

public class BorrowingHistory
{
    public Member Member { get; }
    public BorrowingHistory(Member member)
    {
        this.Member = member;
    }
}