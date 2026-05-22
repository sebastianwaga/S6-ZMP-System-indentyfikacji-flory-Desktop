namespace VirtualHerbarium.AdminPanel.Models;
public class HerbariumStatsResponse
{
    public string id { get; set; }
    public string name { get; set; }

    public string ownerId { get; set; }
    public string ownerUsername { get; set; }
    public string OwnerDisplay => ownerUsername ?? ownerId ?? "—";
}
