namespace PlanPK.Entities
{
    public interface IIssues
    {
        string Id { get; set; }
        string Name { get; set; }
        string Number { get; set; }
        string? ParentId { get; set; }
        double PlanHours { get; set; }
        double FaktHours { get; set; }
        DateTime? TermDate { get; set; }
        List<Issues> ChildrenIssue { get; set; }
    }
}
