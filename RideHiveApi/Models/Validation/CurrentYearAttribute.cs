using System.ComponentModel.DataAnnotations;

namespace RideHiveApi.Models.Validation
{
    public class CurrentYearAttribute : RangeAttribute
    {
        public CurrentYearAttribute(int minimumYear)
            : base(minimumYear, DateTime.Now.Year)
        {
            ErrorMessage = $"Year must be between {minimumYear} and {DateTime.Now.Year}";  
        }
    }
}