namespace OpenPOS.Infrastructure.Contexts
{
    public class FilterContext
    {
        public int Offset { get; set; }
        public int Limit { get; set; }
        
        public string SearchBy { get; set; }
        public string SearchTerm { get; set; }
        
        public string LimitBy { get; set; }
        public decimal FromPrice { get; set; }
        public decimal ToPrice { get; set; }
        
        public string OrderBy { get; set; }
        public bool IsDescending { get; set; }
    }
}