public interface IOwnable {
    string OwnerName { get; set; }
    bool IsOwnedBy(string username);
}