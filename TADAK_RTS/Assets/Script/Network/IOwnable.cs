public interface IOwnable {
    string OwnerName { get; }
    bool IsOwnedBy(string username);
}