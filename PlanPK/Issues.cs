using PlanPK.Entities;

namespace PlanPK
{
    public class Issues : IIssues
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string? ParentId { get; set; }
        public double PlanHours {  get; set; }
        public double FaktHours { get; set; }
        public DateTime? TermDate { get; set; }
        public List<Issues> ChildrenIssue { get; set; }

    }
}
