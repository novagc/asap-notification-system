namespace AsapNotificationSystem.DataBase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProfileId { get; set; }
        public int ServiceId { get; set; }
        public bool SendNotification { get; set; }
        public BuildingNumber[] Number { get; set; } = {};
    }
}
